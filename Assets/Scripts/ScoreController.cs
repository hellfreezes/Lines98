using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour {

    private static ScoreController instance;

    private int score;

    public static ScoreController Instance
    {
        get
        {
            return instance;
        }
    }

    public int Score
    {
        get
        {
            return score;
        }
    }

    // Use this for initialization
    void Start () {
		if (instance != null)
        {
            Debug.Log("На сцене не должно быть два таких объекта. Удаляю");
            Destroy(gameObject);
        }
        instance = this;

        DontDestroyOnLoad(this);
	}
	
	public void ResetScore()
    {
        score = 0;
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
    }
}
