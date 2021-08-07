using UnityEngine;

/// <summary>
/// The camera follows the player on a given distance, it doesn't copy the rotation though.
/// The variables can be changed in the editor.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public Transform player;

    public float offset;
    public float x;
    public float y;
    public float z;

    // Update is called once per frame
    void LateUpdate()
    {
        if (player != null)
        {
            this.transform.position = new Vector3(player.transform.position.x, y, player.transform.position.z - offset);
        }
    }
}
