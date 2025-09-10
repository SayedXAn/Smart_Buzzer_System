using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class FlaskGameManager : MonoBehaviour
{
    public TMP_Text statusText;
    public string serverUrl = "http://127.0.0.1:5000";

    private bool gameOn = false;

    void Start()
    {
        StartCoroutine(PollGameState());
    }

    IEnumerator PollGameState()
    {
        while (true)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(serverUrl + "/state"))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error polling state: " + www.error);
                }
                else
                {
                    // Parse JSON
                    string json = www.downloadHandler.text;
                    var data = JsonUtility.FromJson<GameState>(json);

                    gameOn = data.isGameOn;

                    if (gameOn)
                        statusText.text = "Game Started";
                    else
                        statusText.text = "Game Stopped";

                    // Check winners
                    if (data.winners != null && data.winners.Length > 0)
                    {
                        string winner = data.winners[0];
                        statusText.text = "Winner: " + winner;
                        Debug.Log("Winner: " + winner);
                    }
                }
            }

            yield return new WaitForSeconds(0.1f); // Poll every 100ms
        }
    }

    [System.Serializable]
    public class GameState
    {
        public bool isGameOn;
        public string[] winners;
    }
}
