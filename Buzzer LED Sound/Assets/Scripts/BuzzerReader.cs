using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class BuzzerReader : MonoBehaviour
{
    [SerializeField] private TMP_Text buzzerText;
    [SerializeField] private TMP_Text[] buzzerTexts;
    bool paisi = false;
    public ArduinoWriter arduinoWriter;
    public AudioSource audioSource;
    public AudioClip clip;
    public InputField[] buttonValuesIF;
    public Dictionary<int, string[]> buttonValues = new Dictionary<int, string[]>();
    public GameObject settingsCanvas;

    private List<string> buzzerOrder = new List<string>();

    void Start()
    {
        //buzzerText.text = "Ready!";
        //arduinoWriter = FindObjectOfType<ArduinoWriter>();

        for (int i = 0; i < buzzerTexts.Length; i++)
        {
            //buzzerTexts[i].text = "Ready!";
        }
    }
    /*void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            settingsCanvas.SetActive(true);
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene("BuzzerLight");
        }


        if (Input.GetKeyDown(KeyCode.Alpha1) && !paisi)
        {
            buzzerText.text = "A";
            audioSource.clip = clip;
            audioSource.Play();
            arduinoWriter.WriteSerialData("1");
            paisi = true;
            StartCoroutine(AutoReset(10f));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) && !paisi)
        {
            buzzerText.text = "A";
            audioSource.clip = clip;
            audioSource.Play();
            arduinoWriter.WriteSerialData("1");
            paisi = true;
            StartCoroutine(AutoReset(10f));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && !paisi)
        {
            buzzerText.text = "B";
            audioSource.clip = clip;
            audioSource.Play();
            arduinoWriter.WriteSerialData("2");
            paisi = true;
            StartCoroutine(AutoReset(10f));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8) && !paisi)
        {
            buzzerText.text = "B";
            audioSource.clip = clip;
            audioSource.Play();
            arduinoWriter.WriteSerialData("2");
            paisi = true;
            StartCoroutine(AutoReset(10f));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7) && !paisi)
        {
            buzzerText.text = "C";
            audioSource.clip = clip;
            audioSource.Play();
            arduinoWriter.WriteSerialData("3");
            paisi = true;
            StartCoroutine(AutoReset(10f));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6) && !paisi)
        {
            buzzerText.text = "D";
            audioSource.clip = clip;
            audioSource.Play();
            arduinoWriter.WriteSerialData("4");
            paisi = true;
            StartCoroutine(AutoReset(10f));
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            buzzerText.text = "Ready!";
            arduinoWriter.ResetSerialData();
            paisi = false;
        }
    }*/

    void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            settingsCanvas.SetActive(true);
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        //if (Input.GetKey(KeyCode.R))
        //{
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //}

        // Buzzer key presses

        //if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha5))
        //    RegisterBuzzer("A", "1");

        //else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha8))
        //    RegisterBuzzer("B", "2");

        //else if (Input.GetKeyDown(KeyCode.Alpha7))
        //    RegisterBuzzer("C", "3");

        //else if (Input.GetKeyDown(KeyCode.Alpha6))
        //    RegisterBuzzer("D", "4");

        //else if (Input.GetKeyDown(KeyCode.Backspace))
        //{
        //    ResetBuzzer();
        //}
    }

    //IEnumerator AutoReset(float t)
    //{
    //    /*yield return new WaitForSeconds(t);
    //    buzzerText.text = "Ready!";
    //    arduinoWriter.ResetSerialData();
    //    paisi = false;*/

    //    yield return new WaitForSeconds(t);
    //    ResetBuzzer();
    //}

    private void ResetBuzzer()
    {
        for (int i = 0; i < buzzerTexts.Length; i++)
        {
            buzzerTexts[i].text = "Ready!";
        }
        buzzerOrder.Clear();
        //arduinoWriter.ResetSerialData();
        paisi = false;
    }

    public void SetButtonValues()
    {
        foreach(InputField inp in buttonValuesIF)
        {
            if(inp.text != null)
            {
                string[] splitted = inp.text.Split(' ');
                buttonValues[int.Parse(splitted[0])] = new string[] { splitted[1], splitted[2]};
            }
        }
    }

    private void RegisterBuzzer(string label, string serialData)
    {
        if (buzzerOrder.Count >= 4 || buzzerOrder.Contains(label)) return;

        buzzerOrder.Add(label);
        buzzerTexts[buzzerOrder.Count - 1].text = label;


        if (buzzerOrder.Count == 1) // Only the first buzzer sets paisi and starts timer
        {
            audioSource.clip = clip;
            audioSource.Play();
            paisi = true;
            //StartCoroutine(AutoReset(10f));
        }
        //arduinoWriter.WriteSerialData(serialData);
    }

}
