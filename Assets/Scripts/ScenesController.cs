using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesController : MonoBehaviour {

    private static ScenesController instance;

    private int score;

    public static ScenesController Instance
    {
        get
        {
            return instance;
        }
    }

    void Start()
    {
        if (instance != null)
        {
            Debug.LogError("На сцене не должно быть два таких объекта");
        }
        instance = this;
    }

    //Функции контроля сцены
    public void NewGame()
    {
        ScoreController.Instance.ResetScore();
        SceneManager.LoadScene(0);
    }

    public void GameOver()
    {
        SceneManager.LoadScene(2);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void GlobalScores()
    {
        SceneManager.LoadScene(2);
    }
}
