using UnityEngine;

public class Ball {
    public Sprite icon;
    public int code;
    public Vector2 coords;

    public Ball CopyBall()
    {
        Ball newBall = new Ball();
        newBall.icon = icon;
        newBall.code = code;
        newBall.coords = coords;
        return newBall;
    }
}
