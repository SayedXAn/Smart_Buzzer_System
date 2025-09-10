using System.IO.Ports;
using System.Threading;
using System.Text;
using UnityEngine;
using TMPro;

public class SerialGameManager : MonoBehaviour
{
    [Header("Serial Settings")]
    public string portName = "COM6";
    public int baudRate = 115200;
    public int openDelayMs = 800; // short delay so ESP32 can boot

    [Header("UI")]
    public TMP_Text statusText;

    [Header("References")]
    public GameManager mngr; // optional

    private SerialPort serialPort;
    private Thread readThread;
    private volatile bool running = false;

    private string latestMessage = null;
    private readonly object lockObject = new object();

    private bool gameActive = false;

    void Start()
    {
        TryOpenPort();
    }

    public void TryOpenPort()
    {
        ClosePort();

        try
        {
            serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            serialPort.ReadTimeout = 200;
            serialPort.NewLine = "\n";
            serialPort.Encoding = Encoding.ASCII;
            serialPort.DtrEnable = false;
            serialPort.RtsEnable = false;
            serialPort.Open();

            // allow the board to finish booting
            Thread.Sleep(openDelayMs);

            // clear buffers and request status from receiver
            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
            // ask receiver for its state: 'q'
            serialPort.Write("q");

            running = true;
            readThread = new Thread(ReadSerial) { IsBackground = true };
            readThread.Start();

            Debug.Log($"Serial port {portName} opened at {baudRate}.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to open serial port {portName}: {ex.Message}");
            serialPort = null;
            running = false;
        }
    }

    void Update()
    {
        string message = null;
        lock (lockObject)
        {
            if (latestMessage != null)
            {
                message = latestMessage;
                latestMessage = null;
            }
        }

        if (!string.IsNullOrEmpty(message))
        {
            Debug.Log("Received ID: " + message);

            // N = remote / receiver reports game started
            if (message == "N")
            {
                StartGame(false); // started by receiver (remote)
                return;
            }

            // F = remote / receiver reports game stopped
            if (message == "F")
            {
                StopGame(false); // stopped by receiver (remote)
                return;
            }

            // any other single-char ID = winner buzzer
            if (gameActive)
            {
                Debug.Log("Winner Buzzer: " + message);
                if (statusText != null)
                    statusText.text = "Winner: " + message;

                // call GameManager hook if provided
                if (mngr != null)
                {
                    // implement a method on your GameManager if needed:
                    // mngr.OnBuzzerWin(message);
                }

                // lock until next start
                gameActive = false;
            }
            else
            {
                Debug.Log("Received buzzer but game not active: " + message);
            }
        }
    }

    private void ReadSerial()
    {
        while (running)
        {
            if (serialPort == null || !serialPort.IsOpen)
            {
                Thread.Sleep(200);
                continue;
            }

            try
            {
                string line = serialPort.ReadLine(); // blocks until '\n' or timeout
                if (!string.IsNullOrEmpty(line))
                {
                    string parsed = ExtractFirstPrintable(line);
                    if (!string.IsNullOrEmpty(parsed))
                    {
                        lock (lockObject) latestMessage = parsed;
                    }
                }
            }
            catch (System.TimeoutException) { }
            catch (System.Exception ex)
            {
                Debug.LogError("Serial read error: " + ex.Message);
                Thread.Sleep(200);
            }
        }
    }

    // Return the first printable letter/digit as uppercase single char string
    private string ExtractFirstPrintable(string s)
    {
        if (string.IsNullOrEmpty(s)) return null;
        foreach (char c in s)
        {
            if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                return c.ToString().ToUpperInvariant();
        }
        return null;
    }

    public void SendToReceiver(char command)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                serialPort.Write(command.ToString()); // single-char command
                Debug.Log("Sent command: " + command);

                if (mngr != null)
                {
                    if (command == 'n')
                    {
                        if (mngr.strokeImage != null && mngr.strokes != null && mngr.strokes.Length > 0)
                            mngr.strokeImage.sprite = mngr.strokes[0];
                    }
                    else if (command == 'f')
                    {
                        if (mngr.strokeImage != null && mngr.strokes != null && mngr.strokes.Length > 1)
                            mngr.strokeImage.sprite = mngr.strokes[1];
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to send command: " + ex.Message);
            }
        }
        else
        {
            Debug.LogWarning("Serial port not open. Cannot send command.");
        }
    }

    private void StartGame(bool fromUnity = true)
    {
        Debug.Log("Game Started!");
        if (statusText != null)
            statusText.text = "Game Started!";
        gameActive = true;

        if (fromUnity) SendToReceiver('n');
    }

    private void StopGame(bool fromUnity = true)
    {
        Debug.Log("Game Stopped!");
        if (statusText != null)
            statusText.text = "Game Stopped!";
        gameActive = false;

        if (fromUnity) SendToReceiver('f');
    }

    public void ClosePort()
    {
        running = false;
        try
        {
            if (readThread != null && readThread.IsAlive)
                readThread.Join(300);

            if (serialPort != null)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                    serialPort.Close();
                }
                serialPort.Dispose();
                serialPort = null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Error closing serial port: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        ClosePort();
    }
}
