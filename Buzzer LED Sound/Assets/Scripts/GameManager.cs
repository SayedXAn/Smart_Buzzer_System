using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public SerialGameManager serialGameManager;
    public Sprite[] strokes;
    public Image strokeImage;

    void Start()
    {
        strokeImage.sprite = strokes[1];
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            GetComponent<SerialGameManager>().SendToReceiver('n');
            strokeImage.sprite = strokes[0];
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            GetComponent<SerialGameManager>().SendToReceiver('f');
            strokeImage.sprite = strokes[1];
        }
    }
}
