using System.IO.Ports;
using System.Threading;
using UnityEngine;
using TMPro;

public class SerialGameManager : MonoBehaviour
{
    [Header("Serial Settings")]
    public string portName = "COM6";
    public int baudRate = 115200;

    [Header("UI")]
    public TMP_Text statusText;        

    private SerialPort serialPort;
    private Thread readThread;
    private bool running = false;

    private string latestMessage = null;
    private readonly object lockObject = new object();

    void Start()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout = 50;
            serialPort.Open();

            running = true;
            readThread = new Thread(ReadSerial);
            readThread.Start();

            Debug.Log("Serial port opened.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to open serial port: " + ex.Message);
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
            Debug.Log("Winner ID: " + message);
            if (statusText != null)
                statusText.text = "Winner: " + message;
        }
    }

    private void ReadSerial()
    {
        while (running && serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string line = serialPort.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    lock (lockObject)
                    {
                        latestMessage = line.Trim(); 
                    }
                }
            }
            catch (System.TimeoutException)
            {
                // ignore, no data received
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Serial read error: " + ex.Message);
            }
        }
    }

    public void SendToReceiver(char command)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                serialPort.Write(command.ToString());
                Debug.Log("Sent command: " + command);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to send command: " + ex.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        running = false;

        if (readThread != null && readThread.IsAlive)
            readThread.Join();

        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }
}
