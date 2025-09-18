using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;

public class FlaskManager : MonoBehaviour
{
    public ScoreManager scoreManager;
    public TMP_Text statusText;
    public TMP_Text winnerText;

    private string flaskURL = "http://127.0.0.1:5000/state";

    void Start()
    {
        StartCoroutine(PollGameState());

        DOTween.Init();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            StartCoroutine(SendUnityCommand("unity_start"));
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(SendUnityCommand("unity_stop"));
        }
    }
    IEnumerator SendUnityCommand(string endpoint)
    {
        string furl = "http://127.0.0.1:5000";
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(furl + "/" + endpoint, ""))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Unity command sent: " + endpoint);
            }
            else
            {
                Debug.LogError("Error sending Unity command: " + www.error);
            }
        }
    }

    IEnumerator PollGameState()
    {
        while (true)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(flaskURL))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Flask request failed: " + www.error);
                }
                else
                {
                    string json = www.downloadHandler.text;
                    GameState state = JsonUtility.FromJson<GameState>(json);

                    if (state.isGameOn)
                    {
                        statusText.text = "Ready";
                        winnerText.text = "";
                    }
                    else
                    {
                        statusText.text = "";

                        if (!string.IsNullOrEmpty(state.winner))
                        {

                            //winnerText.text = "Winner: " + state.winner;
                            AssignNamesToState(state.winner);
                        }
                        else
                        {
                            winnerText.text = "";
                        }
                    }
                }
            }

            yield return new WaitForSeconds(1f); // poll every second
        }
    }
    public void AssignNamesToState(string id)
    {
        string[] alphabets = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
        for(int i = 0; i < alphabets.Length; i++)
        {
            if(alphabets[i] == id)
            {
                winnerText.text = scoreManager.names[i];
                break;
            }
        }
    }
}



[System.Serializable]
public class GameState
{
    public bool isGameOn;
    public string winner;
}
