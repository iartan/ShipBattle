using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A general pause menu implemented, without any great options available so far.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;

    public GameObject pauseMenuUI;
    
    void Start()
    {
        pauseMenuUI.SetActive(false);
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        // Unfreeze the game.
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        // Freeze the game.
        Time.timeScale = 0f;
        gameIsPaused = true;
    }
    
    public void LetsPauseIt()
    {
        if (gameIsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    void Update()
    {
        
    }
}
