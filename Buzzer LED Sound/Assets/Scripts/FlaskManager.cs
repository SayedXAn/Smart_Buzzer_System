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
    public TMP_Text adminWinnerText;
    public TMP_Text adminOnOff;
    public TMP_Text adminLog;
    public AudioSource AS;
    bool played = false;
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
            StartCoroutine(CountDownBeforeStart());
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(SendUnityCommand("unity_stop"));
            statusText.text = "";
            winnerText.text = "";
            statusText.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            statusText.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
    IEnumerator CountDownBeforeStart()
    {
        int countdown = 6;
        float t = 1;
        while (countdown > 0)
        {
            if(countdown == 6)
            {
                statusText.text = "";
                statusText.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                statusText.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                t = 2;
            }
            else
            {
                statusText.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                statusText.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                statusText.text = countdown.ToString();
                t = 1;
            }            
            yield return new WaitForSeconds(t);
            countdown--;
        }
        statusText.text = "";
        StartCoroutine(SendUnityCommand("unity_start"));
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
                    adminLog.text = www.error;
                }
                else
                {
                    string json = www.downloadHandler.text;
                    GameState state = JsonUtility.FromJson<GameState>(json);
                    adminLog.text = json;
                    if (state.isGameOn)
                    {
                        statusText.text = "Go!";
                        adminOnOff.text = "READY";
                        winnerText.text = "";
                        adminWinnerText.text = "---";
                        played = false;
                        adminLog.text += "played: false";
                    }
                    else
                    {
                        
                        adminOnOff.text = "Game Off";

                        if (!string.IsNullOrEmpty(state.winner))
                        {
                            //winnerText.text = "Winner: " + state.winner;
                            statusText.text = "";
                            statusText.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                            statusText.gameObject.transform.GetChild(1).gameObject.SetActive(false);
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
        string[] alphabets = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L"};
        for(int i = 0; i < alphabets.Length; i++)
        {
            if(alphabets[i] == id)
            {
                winnerText.text = scoreManager.names[i];
                adminWinnerText.text = scoreManager.names[i];
                if (!played)
                {
                    AS.Play();
                    played = true;
                    adminLog.text += "played: true";
                }
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
