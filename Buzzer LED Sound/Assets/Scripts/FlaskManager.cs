using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FlaskManager : MonoBehaviour
{
    public TMP_Text statusText;
    public TMP_Text winnerText;

    private string flaskURL = "http://127.0.0.1:5000/state";

    void Start()
    {
        StartCoroutine(PollGameState());
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
                        statusText.text = "Game Started";
                        winnerText.text = "";
                    }
                    else
                    {
                        statusText.text = "Game Stopped";

                        if (!string.IsNullOrEmpty(state.winner))
                        {
                            winnerText.text = "Winner: " + state.winner;
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
}

[System.Serializable]
public class GameState
{
    public bool isGameOn;
    public string winner;  // single winner, not array
}
