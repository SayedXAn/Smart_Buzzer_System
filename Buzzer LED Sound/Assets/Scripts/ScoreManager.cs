using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public float[] scores = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public TMP_InputField[] playersIF;
    public TMP_Text[] playerScoreboard;
    public TMP_Text[] miniScoreboard;
    public string[] names;
    public AudioSource AS;

    private void Start()
    {
        if (Display.displays.Length > 1)
        {
            for (int i = 1; i < Display.displays.Length; i++)
            {
                Display.displays[i].Activate();
            }
        }

        for (int i = 0; i < playerScoreboard.Length; i++)
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

}
