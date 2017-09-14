using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public class GlobalHighScoreManager : MonoBehaviour {

    [SerializeField]
    string postScoreURL = "http://localhost/game/postscore.php";
    [SerializeField]
    string getScoreURL = "http://localhost/game/getscore.php";
    [SerializeField]
    string deleteScoreURL = "http://localhost/game/deletescore.php";
    [SerializeField]
    private GameObject scorePrefab;
    [SerializeField]
    private GameObject scoreParent;
    [SerializeField]
    private int topRanks;
    [SerializeField]
    private int saveScores;
    [SerializeField]
    private InputField enterName;
    [SerializeField]
    private GameObject nameDialog;

    private int scoreToAdd;
    private string nameToAdd;

    private delegate void OnGetScore();
    //private OnGetScore cbOnGetScore;
    private string[] items;
    private string connectionString;
    private List<HighScore> highScore = new List<HighScore>();



    // Use this for initialization
    void Start()
    {
        //connectionString = "URI=file:" + Application.dataPath + "/HighScoreDB.sqlite";

        //DeleteExtraScore();
        //DeleteScore(2);
        //GetScore();
        //InsertScore("Misha", 113);
        //InsertScore("hellfreezes24", 15);
        //InsertScore("Mina", 18);
        //InsertScore("Tima", 93);
        //InsertScore("Lena", 33);
        //InsertScore("Nastya", 25);
        //InsertScore("Petuh99", 61);
        //InsertScore("iPhone", 54);
        //InsertScore("Zelenuha", 49);
        //InsertScore("HouseMD", 77);
        //InsertScore("Rin", 81);
        //InsertScore("Putin", 53);
        //InsertScore("Stalin", 153);
        IsScoreIsHighScore();
        ShowScore();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //      {
        //          nameDialog.SetActive(!nameDialog.activeSelf);
        //      }
    }

    public void EnterName()
    {
        if (enterName.text != string.Empty)
        {
            int score = ScoreController.Instance.Score;
            InsertScore(enterName.text, score);
            enterName.text = string.Empty;
            ShowScore();
            nameDialog.SetActive(false);
            ScoreController.Instance.ResetScore();
        }
    }

    private void IsScoreIsHighScore()
    {
        StartCoroutine(GetScore(ShowEnterNameDialog));
    }

    private void ShowEnterNameDialog()
    {
        int hsCount = highScore.Count;
        
        if (highScore.Count > 0)
        {
            HighScore lowestScore = highScore[highScore.Count - 1];
            //Debug.Log("lowestScore:"+ lowestScore+"; hC:"+highScore.Count+"; sS:"+saveScores+"; score:"+ ScoreController.Instance.Score+"; lowestScore:"+ lowestScore.Score);
            //Debug.Log("First:" + (lowestScore != null) + "; Second:" + (highScore.Count >= saveScores) + "; Third:" + (ScoreController.Instance.Score > lowestScore.Score));
            if (lowestScore != null && saveScores > 0 && highScore.Count < saveScores && ScoreController.Instance.Score > lowestScore.Score)
            {
                //Debug.Log("Cond");
                nameDialog.SetActive(true);
            }
        }
    }

    private void InsertScore(string name, int newScore)
    {
        StartCoroutine(GetScore(TryPostScore));
        nameToAdd = name;
        scoreToAdd = newScore;
    }

    private void TryPostScore()
    {
        if (nameToAdd != string.Empty && scoreToAdd > 0) 
            StartCoroutine(PostScoreToBase(nameToAdd, scoreToAdd));
    }

    IEnumerator PostScoreToBase(string name, int newScore)
    {
        int hsCount = highScore.Count;

        if (highScore.Count > 0)
        {
            HighScore lowestScore = highScore[highScore.Count - 1];
            if (lowestScore != null && saveScores > 0 && highScore.Count >= saveScores && newScore > lowestScore.Score)
            {
                DeleteScore(lowestScore.ID);
                hsCount--;
            }
        }

        if (hsCount < saveScores)
        {
            WWWForm form = new WWWForm();
            form.AddField("usernamePost", name);
            form.AddField("scorePost", newScore);

            WWW www = new WWW(postScoreURL, form);
            yield return www;

            nameToAdd = string.Empty;
            scoreToAdd = 0;

            if (!string.IsNullOrEmpty(www.error))
            {   //Ошибка подключения. Прервать процесс!
                Debug.LogError("Ошибка при подключении: " + www.error);
                yield break;
            }
        }
    }

    IEnumerator GetScore(OnGetScore cbAction)
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

            cbAction.Invoke();
        }
    }

    private void FillScoreTable()
    {
        foreach (GameObject score in GameObject.FindGameObjectsWithTag("Score"))
        {
            Destroy(score);
        }

        for (int i = 0; i < topRanks; i++)
        {
            if (i <= highScore.Count - 1)
            {
                GameObject tmpObj = Instantiate(scorePrefab);
                HighScore tmpScore = highScore[i];
                tmpObj.GetComponent<HighScoreScript>().SetScore(tmpScore.Name, tmpScore.Score.ToString(), "#" + (i + 1).ToString());
                tmpObj.transform.SetParent(scoreParent.transform);
                tmpObj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            }
        }
    }


    IEnumerator DeleteScore(int id)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);

        WWW www = new WWW(deleteScoreURL, form);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {   //Ошибка подключения. Прервать процесс!
            Debug.LogError("Ошибка при подключении: " + www.error);
            yield break;
        }
    }

    private void ShowScore()
    {
        StartCoroutine(GetScore(FillScoreTable));
    }

    private void DeleteExtraScore()
    {
        StartCoroutine(GetScore(InitDeleteExtra));
    }

    private void InitDeleteExtra()
    {
        if (saveScores <= highScore.Count)
        {
            int deleteCount = highScore.Count - saveScores;

            for (int i = 0; i < deleteCount; i++)
            {
                StartCoroutine(DeleteScore(highScore[i].ID));
            }
        }
    }

    private string GetDataValue(string data, string index)
    {
        string value = data.Substring(data.IndexOf(index) + index.Length);
        if (value.Contains("|"))
            value = value.Remove(value.IndexOf("|"));
        return value;
    }
}
