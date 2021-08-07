using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Code for the main menu. For now only the "Play Offline" and "Quit Game" buttons are doing something.
/// </summary>
public class MainMenu : MonoBehaviour
{
    public void PlayOffline()
    {
        // Load the offline scene, which is the first scene in the index/list of scenes.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game.");
        Application.Quit();
    }
}
