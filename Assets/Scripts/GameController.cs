using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the scenes when the buttons are pressed and manages the player score.
/// Also manages the bot amount and respawn bots in a random position when there are less than 5 in the game.
/// </summary>
public class GameController : MonoBehaviour
{
    public GameObject scoreText;
    public GameObject enemyPrefab;
    public int score;
    private int num = 10;   // Number for naming of the enemies.

    void Start()
    {
        score = 0;
        ScoreUpdate();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Those methods update the player score on top of the screen.
    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        ScoreUpdate();
    }

    void ScoreUpdate()
    {
        scoreText.GetComponent<Text>().text = "Score: " + score;
    }

    // Quit the game when the exit button is pressed.
    public void ExitGame()
    {
        Application.Quit();
    }

    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length < 5) // If there are less than 5 bots in the game, spawn a new one in a random position.
        {
            GameObject enemy = Instantiate(enemyPrefab, new Vector3(Random.Range(-50, 50), 0.25f, Random.Range(-50, 50)), Quaternion.identity);
            enemy.name = "SpawnedEnemy" + num;
            num++;
        }
    }
}
