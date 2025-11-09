using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using System.Linq;

public class ScoreManager : MonoBehaviour
{
    public float[] scores = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public TMP_InputField[] playersIF;
    public TMP_Text[] playerNameboard;
    public TMP_Text[] playerScoreboard;
    public TMP_Text[] miniScoreboard;
    public string[] names;
    public AudioSource AS;
    public string fileName = "names.txt";
    private List<string> names_ = new List<string>();
    //private bool roundFound = false;
    public TMP_Text roundNumber;
    public TMP_InputField roundIF;
    public TMP_Text leaderboardText;
    public GameObject leaderboardPanel;
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

        for (int i = 0; i < names.Length; i++)
        {
            playerNameboard[i].text = names[i];
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
                    playerScoreboard[i].text = scores[i].ToString();
                    //playerNameboard[i].text = names[i];
                    miniScoreboard[i].text = scores[i].ToString();
                }
                else
                {
                    Debug.LogWarning($"Invalid score input for player {names[i]}: {input}");
                }
                playersIF[i].text = "";
            }
            if(!string.IsNullOrEmpty(roundIF.text.Trim()))
            {
                roundNumber.text = "ivDÛ: " + roundIF.text.Trim();
                roundIF.text = "";
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

            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                {
                    names_.Add(trimmed);
                    //if(!roundFound)
                    //{
                    //    roundFound = true;
                    //    roundNumber.text = "ivDÛ " + trimmed;
                    //}
                    //else
                    //{
                    //    names_.Add(trimmed);
                    //}                    
                    //This was very stupid approach
                }
                    
            }

            Debug.Log($"Loaded {names_.Count} names:");
            foreach (string n in names_)
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

    public void ShowLeaderboard()
    {
        //calculate top scorer
        float topScore = scores[0];
        for (int i = 1; i < scores.Length; i++)
        {
            if(scores[i] > topScore)
            {
                topScore = scores[i];
            }
        }
        string winnerNames = "";
        for (int i = 0; i < scores.Length; i++)
        {
            if(scores[i] == topScore)
            {
                winnerNames += names[i] + "\n";
            }
        }
        leaderboardText.text = winnerNames;
        leaderboardPanel.SetActive(true);
    }

}
