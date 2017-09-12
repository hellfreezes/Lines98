using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public class LocalHighScoreManager : MonoBehaviour {
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


    private string connectionString;
    private List<HighScore> highScore = new List<HighScore>();
    
    

	// Use this for initialization
	void Start () {
        connectionString = "URI=file:" + Application.dataPath + "/HighScoreDB.sqlite";

        DeleteExtraScore();
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
        if (IsScoreIsHighScore(ScoreController.Instance.Score))
        {
            nameDialog.SetActive(true);
        }
        ShowScore();
    }
	
	// Update is called once per frame
	void Update () {
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

    private bool IsScoreIsHighScore(int score)
    {
        GetScore();
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

    private void InsertScore(string name, int newScore)
    {
        GetScore();
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
            using (IDbConnection dbConnection = new SqliteConnection(connectionString))
            {
                dbConnection.Open();
                using (IDbCommand dbCmd = dbConnection.CreateCommand())
                {
                    string sqlQuery = String.Format("INSERT INTO HighScores(Name,Score) VALUES(\"{0}\",\"{1}\")", name, newScore);

                    dbCmd.CommandText = sqlQuery;
                    dbCmd.ExecuteScalar();
                    dbConnection.Close();
                }
            }
        }
    }

    private void GetScore()
    {
        highScore.Clear();

        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "SELECT * FROM HighScores";

                dbCmd.CommandText = sqlQuery;

                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        highScore.Add(new HighScore(reader.GetInt32(0), reader.GetInt32(2), reader.GetString(1), reader.GetDateTime(3)));
                    }
                    highScore.Sort();
                    dbConnection.Close();
                    reader.Close();
                }
            }
        }
    }

    private void DeleteScore(int id)
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = String.Format("DELETE FROM HighScores WHERE PlayerID=\"{0}\"", id);

                dbCmd.CommandText = sqlQuery;
                dbCmd.ExecuteScalar();
                dbConnection.Close();
            }
        }
    }

    private void ShowScore()
    {
        GetScore();

        foreach(GameObject score in GameObject.FindGameObjectsWithTag("Score"))
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

    private void DeleteExtraScore()
    {
        GetScore();

        if (saveScores <= highScore.Count)
        {
            int deleteCount = highScore.Count - saveScores;

            using (IDbConnection dbConnection = new SqliteConnection(connectionString))
            {
                dbConnection.Open();
                using (IDbCommand dbCmd = dbConnection.CreateCommand())
                {
                    for (int i = 0; i < deleteCount; i++)
                    {
                        string sqlQuery = String.Format("DELETE FROM HighScores WHERE PlayerID=\"{0}\"", highScore[i].ID);

                        dbCmd.CommandText = sqlQuery;
                        dbCmd.ExecuteScalar();
                    }
                    dbConnection.Close();
                }
            }
        }
    }
}
