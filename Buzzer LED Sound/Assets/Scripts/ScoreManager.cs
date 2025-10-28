using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;

public class ScoreManager : MonoBehaviour
{
    public float[] scores = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public TMP_InputField[] playersIF;
    public TMP_Text[] playerScoreboard;
    public TMP_Text[] miniScoreboard;
    public string[] names;
    public AudioSource AS;
    public string fileName = "names.txt";
    private List<string> names_ = new List<string>();

    private void Start()
    {
        if (Display.displays.Length > 1)
        {
            for (int i = 1; i < Display.displays.Length; i++)
            {
                Display.displays[i].Activate();
            }
        }

        FetchNames();

        //for (int i = 0; i < playerScoreboard.Length; i++)
        for (int i = 0; i < names.Length; i++)
        {
            playerScoreboard[i].text = names[i] + ": 0.0";
            miniScoreboard[i].text = scores[i].ToString();
            TextMeshProUGUI placeholderText = playersIF[i].placeholder as TextMeshProUGUI;
            placeholderText.text = names[i];
        }

    }


    public void UpdateScore()
    {
        for (int i = 0; i < playersIF.Length; i++)
        {
            string input = playersIF[i].text.Trim();
            if (!string.IsNullOrEmpty(input))
            {
                float value;
                if (float.TryParse(input, out value))
                {
                    scores[i] += value;
                    playerScoreboard[i].text = names[i] + ": " + scores[i].ToString();
                    miniScoreboard[i].text = scores[i].ToString();
                }
                else
                {
                    Debug.LogWarning($"Invalid score input for player {names[i]}: {input}");
                }
                playersIF[i].text = "";
            }
        }
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AS.Play();
        }
    }

    

    private void FetchNames()
    {
        string filePath = Path.Combine(Application.dataPath, fileName);

        if (File.Exists(filePath))
        {
            // Read all lines
            string[] lines = File.ReadAllLines(filePath);

            // Trim and store valid names
            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                    names_.Add(trimmed);
            }

            Debug.Log($"Loaded {names_.Count} names:");
            foreach (string n in names)
                Debug.Log(n);
        }
        else
        {
            Debug.LogError("File not found at: " + filePath);
        }

        names = new string[names_.Count];
        for(int i = 0; i < names.Length; i++)
        {
            names[i] = names_[i];
        }
    }

    public List<string> GetNames()
    {
        return names_;
    }

}
