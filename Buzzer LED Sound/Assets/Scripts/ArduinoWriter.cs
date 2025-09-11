using UnityEngine;
using TMPro;
//using System.IO.Ports;
using System.Threading;
using System.Collections.Concurrent;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class ArduinoWriter : MonoBehaviour
{
    //public int readTimeout = 10;
    //public string portName = "COM6";
    //public int baudRate = 115200;

    //private SerialPort serialPort;
    //private Thread serialThread;
    //private bool threadRunning = false;
    //private ConcurrentQueue<string> serialDataQueue = new ConcurrentQueue<string>();

    //public TMP_InputField portIF;
    //public TMP_Text screenText;
    //public float debounceTime = 1f;
    //public bool hasWritePermission = true;
    //public bool called = false;
    //private bool gameState = false;


    //void Start()
    //{
    //    if (PlayerPrefs.HasKey("port"))
    //    {
    //        portName = PlayerPrefs.GetString("port");
    //    }

    //    StartCoroutine(TryConnect());

    //    string[] ports = SerialPort.GetPortNames();
    //    Debug.Log("Available Ports:");
    //    foreach (string p in ports)
    //    {
    //        Debug.Log(p);
    //    }
    //}

    //IEnumerator TryConnect()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    try
    //    {
    //        serialPort = new SerialPort(portName, baudRate);
    //        serialPort.ReadTimeout = readTimeout;
    //        serialPort.Open();

    //        threadRunning = true;
    //        serialThread = new Thread(ReadFromSerial);
    //        serialThread.Start();
    //        StopAllCoroutines();
    //        Debug.Log($"Serial port {portName} opened successfully.");
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.LogError("Failed to open serial port: " + ex.Message);
    //        StartCoroutine(TryConnect());
    //    }
    //}
    //private void ReadFromSerial()
    //{
    //    while (threadRunning && serialPort != null && serialPort.IsOpen)
    //    {
    //        try
    //        {
    //            string data = serialPort.ReadLine();
    //            serialDataQueue.Enqueue(data); // Pass data to main thread
    //        }
    //        catch (TimeoutException)
    //        {
    //            // Ignore timeout
    //        }
    //        catch (Exception ex)
    //        {
    //            serialDataQueue.Enqueue("ERROR:" + ex.Message); // Handle in Update()
    //        }

    //        Thread.Sleep(10);
    //    }
    //}


    //public void SetText(string data)
    //{
    //    called = true;
    //    if (data.Length == 1)
    //    {
    //        screenText.text = "team " + data;
    //    }
    //    else
    //    {
    //        screenText.text = data[data.Length-1].ToString();
    //    }
    //    Debug.Log("Dhukse ekhane? " + data);

    //}
    //public void ResetText()
    //{
    //    called = false;
    //    screenText.text = "";
    //}


    //private void Update()
    //{
    //    while (serialDataQueue.TryDequeue(out string line))
    //    {
    //        if (line.StartsWith("ERROR:"))
    //        {
    //            Debug.LogError(line);
    //            continue;
    //        }

    //        Debug.Log("Received from ESP32: " + line);

    //        if (!called)
    //        {
    //            SetText(line);
    //        }
    //    }
    //    if(Input.GetKeyDown(KeyCode.Backspace))
    //    {
    //        ResetText();
    //    }

    //    if (Input.GetKeyDown(KeyCode.Alpha1))
    //    {
    //        gameState = true;
    //        ResetText();
    //        TurnGameOnOff('n');
    //    }
    //    if (Input.GetKeyDown(KeyCode.Alpha2))
    //    {
    //        gameState = false;
    //        ResetText();
    //        TurnGameOnOff('f');
    //    }
    //}

    //public void TurnGameOnOff(char chr)
    //{
    //    if (!hasWritePermission || serialPort == null || !serialPort.IsOpen)
    //        return;

    //    try
    //    {          

    //        serialPort.Write(new char[] { chr }, 0, 1);

    //        // Optional: Update on-screen status
    //        screenText.text = gameState ? "Game ON" : "Game OFF";

    //        StartCoroutine(StopSerialWriteForSeconds(2));
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.LogError("Serial write error: " + ex.Message);
    //        gameState = !gameState;
    //    }
    //}



    ////public void WriteSerialData(string data)
    ////{
    ////    if (!hasWritePermission || serialPort == null || !serialPort.IsOpen)
    ////        return;

    ////    try
    ////    {
    ////        serialPort.WriteLine(data);
    ////        Debug.Log($"Sent to ESP32: {data}");
    ////        StartCoroutine(StopSerialWriteForSeconds(2));
    ////    }
    ////    catch (Exception ex)
    ////    {
    ////        Debug.LogError("Serial write error: " + ex.Message);
    ////    }
    ////}

    //IEnumerator StopSerialWriteForSeconds(int seconds)
    //{
    //    hasWritePermission = false;
    //    yield return new WaitForSeconds(seconds);
    //    hasWritePermission = true;
    //}

    ////public void ResetSerialData()
    ////{
    ////    if (serialPort != null && serialPort.IsOpen)
    ////    {
    ////        serialPort.WriteLine("a");
    ////        Debug.Log("Sent reset command");
    ////    }
    ////}

    //void OnApplicationQuit()
    //{
    //    threadRunning = false;

    //    if (serialThread != null && serialThread.IsAlive)
    //    {
    //        serialThread.Join(); // Gracefully stop thread
    //    }

    //    if (serialPort != null && serialPort.IsOpen)
    //    {
    //        serialPort.Close();
    //        Debug.Log("Serial port closed.");
    //    }
    //}

    //public void OnSave()
    //{
    //    PlayerPrefs.SetString("port", portIF.text);
    //    SceneManager.LoadScene("BuzzerLight");
    //}
}
