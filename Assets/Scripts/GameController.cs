using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    [SerializeField]
    Ball[] balls;
    [SerializeField]
    GameObject gameDesk;
    [SerializeField]
    int ballsAtTurn = 3;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void UpdateGameDesk()
    {
        for (int x = 0; x < Grid.instance.GridSizeX; x++)
        {
            for (int y = 0; y < Grid.instance.GridSizeY; y++)
            {

            }
        }
    }

    public void GenerateBalls()
    {
        for (int i = 1; i<= ballsAtTurn; i++)
        {
            int newBall = Random.Range(0, balls.Length);
            Grid.instance.AddBall(balls[newBall]);
        }
    }
}
