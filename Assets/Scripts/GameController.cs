using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject scoreText;
    public GameObject enemyPrefab;
    public int score;
    private int num = 10;   // For naming the enemies.

    void Start()
    {
        score = 0;
        ScoreUpdate();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        ScoreUpdate();
    }

    void ScoreUpdate()
    {
        scoreText.GetComponent<Text>().text = "Score: " + score;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length < 5) // Wenn weniger als 5 Bots im Spiel sind weitere hinzufügen.
        {
            // print("Less than 3 enemies. Spawning a new one.");
            GameObject enemy = Instantiate(enemyPrefab, new Vector3(Random.Range(-50, 50), 0.25f, Random.Range(-50, 50)), Quaternion.identity);
            enemy.name = "SpawnedEnemy" + num;
            num++;
        }
    }
}
