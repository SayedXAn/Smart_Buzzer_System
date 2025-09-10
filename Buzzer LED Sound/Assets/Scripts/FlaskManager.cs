using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class FlaskGameManager : MonoBehaviour
{
    public TMP_Text statusText;
    string flaskUrl = "http://127.0.0.1:5000/game_state";

    void Start()
    {
        StartCoroutine(PollGameState());
    }

    IEnumerator PollGameState()
    {
        while (true)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(flaskUrl))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string json = www.downloadHandler.text;
                    GameState state = JsonUtility.FromJson<GameState>(json);
                    if (statusText != null)
                    {
                        if (state.winner != null) statusText.text = "Winner: " + state.winner;
                        else statusText.text = "Game " + (state.is_on ? "ON" : "OFF");
                    }
                }
            }
            yield return new WaitForSeconds(0.5f); // poll every 500ms
        }
    }

    [System.Serializable]
    public class GameState
    {
        public bool is_on;
        public string winner;
    }
}
