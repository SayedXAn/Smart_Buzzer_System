using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SerialGameManager serialGameManager;
    void Start()
    {
        
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            GetComponent<SerialGameManager>().SendToReceiver('n');
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            GetComponent<SerialGameManager>().SendToReceiver('f');
        }
    }
}
