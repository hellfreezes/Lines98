using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node {
    public delegate void OnChange();
    public OnChange onChangeCallback;
    public bool walkable;
    public Vector2 worldPosition;
    public Ball ball;
    public GameObject slot;

    public Node(bool _walkable, Vector2 _worldPos)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        ball = null;
    }

    public void SetBall(Ball _ball)
    {
        ball = _ball;
        if (ball == null)
            return;
        walkable = false;
        Image icon = slot.transform.Find("SlotButton").transform.Find("Image").GetComponent<Image>();
        icon.sprite = ball.icon;
        icon.gameObject.SetActive(true);
        Grid.instance.FreeSlots--;
        if (onChangeCallback != null)
        {
            onChangeCallback.Invoke();
        }
    }

    public void RemoveBall()
    {
        walkable = true;
        ball = null;
        Image icon = slot.transform.Find("SlotButton").transform.Find("Image").GetComponent<Image>();
        icon.sprite = null;
        icon.gameObject.SetActive(false);
        Grid.instance.FreeSlots++;
        if (onChangeCallback != null)
        {
            onChangeCallback.Invoke();
        }
    }
}
