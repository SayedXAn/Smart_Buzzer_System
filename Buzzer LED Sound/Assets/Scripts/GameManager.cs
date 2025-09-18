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
}
