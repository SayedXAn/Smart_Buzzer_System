using System.Collections;
using TMPro;
using UnityEngine;

public class TimerCountDown : MonoBehaviour
{
    public int timer = 21;
    public TMP_Text timerText;
    bool timerRunning = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T) && !timerRunning)
        {
            StartCoroutine(CountDown());
        }
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(1f);
        timer--;
        timerText.text = "Time Remaining\n" + timer.ToString() + " Second/s";
        if(timer == 0)
        {
            StopCoroutine(CountDown());
            timerRunning = false;
            timer = 21;
        }
        else
        {
            StartCoroutine(CountDown());
        }
    }
}
