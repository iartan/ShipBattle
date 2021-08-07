using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The score text above the ships needs to be rotated to face the camera, because they are not on the main UI.
/// </summary>
public class ScoreOnTop : MonoBehaviour
{
    public GameObject scoreText;
    public int score;

    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
    }
}
