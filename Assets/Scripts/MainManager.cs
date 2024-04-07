using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;

    public GameObject setNameCanvas;
    public GameObject bestScoreText;
    public GameObject restartButton;
    public Text bestScoreTextDefault;
    public TMP_InputField nameInput;

    private static int BEST_SCORE = 0;
    private static string BEST_NAME = "Alex";

    private bool m_Started = false;
    private int m_Points;

    private bool m_GameOver = false;


    // Start is called before the first frame update
    void Start()
    {
        JsonInfo jsonInfo = JsonInfo.ReadJson();
        BEST_NAME = jsonInfo.name;
        BEST_SCORE = jsonInfo.bestScore;
        bestScoreTextDefault.text = $"Best Score : {BEST_NAME} : {BEST_SCORE}";

        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        restartButton.GetComponent<Button>().onClick.AddListener(ButtonListener);
        nameInput.onEndEdit.AddListener(InputFieldListener);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = UnityEngine.Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;

        if (m_Points > BEST_SCORE)
        {
            setNameCanvas.SetActive(true);

            BEST_SCORE = m_Points;
            bestScoreText.GetComponent<Text>().text = $"You set new best score! {m_Points}";
            bestScoreText.SetActive(true);
        }
        else
        {
            GameOverText.SetActive(true);
            restartButton.SetActive(true);
        }
    }

    private void ButtonListener()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void InputFieldListener(string name)
    {
        BEST_NAME = name;
        bestScoreTextDefault.text = $"Best Score : {name} : {BEST_SCORE}";
        JsonInfo.SaveJson(new JsonInfo(name, BEST_SCORE));
    }

}

[Serializable]
class JsonInfo
{

    public string name;
    public int bestScore;
    public JsonInfo(string name, int bestScore)
    {
        this.name = name;
        this.bestScore = bestScore;
    }

    private static readonly string FILE_PATH = Application.dataPath + "/BestScoreInfo.json";
    public static void SaveJson(JsonInfo ji)
    {
        File.WriteAllText(FILE_PATH, JsonUtility.ToJson(ji));
    }

    public static JsonInfo ReadJson()
    {
        if (File.Exists(FILE_PATH))
        {
            return JsonUtility.FromJson<JsonInfo>(File.ReadAllText(FILE_PATH));
        }
        else
        {
            return new JsonInfo("Alex", 0);
        }
    }

}