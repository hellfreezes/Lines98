using System;
using System.Data;
using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreManager : MonoBehaviour {
    [SerializeField]
    string postScoreURL = "http://localhost/game/postscore.php";
    [SerializeField]
    string getScoreURL = "http://localhost/game/getscore.php";
    [SerializeField]
    GameObject scorePrefab;
    [SerializeField]
    GameObject scorePanel;
    [SerializeField]
    int numTopRanks = 10;

    [SerializeField]
    private int topRanks;
    [SerializeField]
    private int saveScores;
    [SerializeField]
    private InputField enterName;
    [SerializeField]
    private GameObject nameDialog;

    private static HighScoreManager instance;

    private string[] items;

    private List<HighScore> highScore = new List<HighScore>();

    public static HighScoreManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Start()
    {
        if (instance != null)
        {
            Debug.LogWarning("Два HighScoreManager на сцене!!!");
        }
        instance = this;
        //StartCoroutine(PostOnlineScore("Katya", 33));
        //StartCoroutine(PostOnlineScore("Anya", 13));
        //StartCoroutine(PostOnlineScore("Petya", 41));
        //StartCoroutine(PostOnlineScore("Angel", 19));
        //StartCoroutine(PostOnlineScore("Dunkan", 10));
        //StartCoroutine(PostOnlineScore("Ilon", 91));
        //StartCoroutine(PostOnlineScore("Zeus", 101));
        //StartCoroutine(PostOnlineScore("Inna", 5));
        //StartCoroutine(PostOnlineScore("Pinka", 17));
        //StartCoroutine(PostOnlineScore("Orka", 37));
        //StartCoroutine(PostOnlineScore("Kira", 24));
        //StartCoroutine(PostOnlineScore("Joe", 67));
        StartCoroutine(LoadOnlineScore());
        
    }

    IEnumerator LoadOnlineScore()
    {
        highScore.Clear();
        WWW itemsData = new WWW(getScoreURL);

 
        yield return itemsData;
        string itemsDataString = itemsData.text;
        if (!string.IsNullOrEmpty(itemsData.error))
        {   //Ошибка подключения. Прервать процесс!
            Debug.LogError("Ошибка при подключении: " + itemsData.error);
            yield break;
        }
        else
        {   //Соединились успешно. Делаем чтонибудь
            items = itemsDataString.Split(';');

            items[items.Length - 1] = null;

            highScore.Clear();
            int id = 0;
            int score = 0;
            string name = "";
            DateTime date = DateTime.Now;

            foreach (string item in items)
            {
                if (item != null)
                {
                    int.TryParse(GetDataValue(item, "ID:"), out id);
                    int.TryParse(GetDataValue(item, "Score:"), out score);
                    name = GetDataValue(item, "Name:");
                    DateTime.TryParse(GetDataValue(item, "Date:"), out date);

                    highScore.Add(new HighScore(id, score, name, date));
                }
            }

            highScore.Sort(); //сортировка тут!
        }
        ShowOnlineScores();
        //Debug.Log(GetDataValue(items[0], "Score:"));
    }

    IEnumerator PostOnlineScore(string userName, int score)
    {
        WWWForm form = new WWWForm();
        form.AddField("usernamePost", userName);
        form.AddField("scorePost", score);

        WWW www = new WWW(postScoreURL, form);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {   //Ошибка подключения. Прервать процесс!
            Debug.LogError("Ошибка при подключении: " + www.error);
            yield break;
        }
    }

    string GetDataValue(string data, string index)
    {
        string value = data.Substring(data.IndexOf(index) + index.Length);
        if (value.Contains("|"))
            value = value.Remove(value.IndexOf("|"));
        return value;
    }

    public void EnterName()
    {
        if (enterName.text != string.Empty)
        {
            int score = ScoreController.Instance.Score;
            PostOnlineScore(enterName.text, score);
            enterName.text = string.Empty;
            ShowOnlineScores();
            nameDialog.SetActive(false);
            ScoreController.Instance.ResetScore();
        }
    }

    private bool IsScoreIsHighScore(int score)
    {
        LoadOnlineScore();
        int hsCount = highScore.Count;

        if (highScore.Count > 0)
        {
            HighScore lowestScore = highScore[highScore.Count - 1];
            if (lowestScore != null && saveScores > 0 && highScore.Count >= saveScores && score > lowestScore.Score)
            {
                return true;
            }
        }
        return false;
    }

    void ShowOnlineScores()
    {
        if (highScore.Count > 0)
        {
            for (int i = 0; i < numTopRanks; i++)
            {
                if (i > highScore.Count - 1)
                {
                    break;
                }
                GameObject tmpObject = Instantiate(scorePrefab);
                tmpObject.transform.SetParent(scorePanel.transform);
                HighScore tmpScore = highScore[i];
                tmpObject.GetComponent<HighScoreScript>().SetScore(tmpScore.Name, tmpScore.Score.ToString(), "#" + (i + 1).ToString());
            }
        }
    }
}
