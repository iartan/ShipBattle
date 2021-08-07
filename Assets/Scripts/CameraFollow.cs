using UnityEngine;

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
