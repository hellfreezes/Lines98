using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField]
    private int squareSize = 64;

    void Start () {
        int x = Screen.width;
        int y = Screen.height;
        
        Camera.main.orthographicSize = x / (((x / y) * 2) * squareSize);
        Debug.Log("Screen size is: " + x + "x" + y + "; Orth size: " + Camera.main.orthographicSize);
    }
}
