using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Originally this scripts purpose was to let the cannonball fly an arc for realistic reasons.
/// Thats the reason why the cannonballs have their own scripts and not only do fly by the force given when they got instantiated.
/// Because of the game design decisions, the cannonballs now flies in a straight line, like in the earlier versions, so the existance of this whole script is now questionable - TODO.
/// </summary>
public class CannonBall : MonoBehaviour
{
    public int scoreValue;
    public GameController gameController;

    public Vector3 originalPosition;
    public Vector3 groundPosition;
    public float distance;
    public float groundDistance;
    public Vector3 goal;
 
    void Start()
    {
        // Get the GameController in the scene.
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        if (gameController == null)
        {
            Debug.Log("Can not find 'GameController' script");
        }
        
        originalPosition = this.transform.position;
        distance = Vector3.Distance(this.transform.position, goal);

        StartCoroutine(SelfDestruct());
    }

    // Cannonball gets destroyed after some seconds. This is called in the start.
    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(5.0f);
        Destroy(gameObject);
    }

    // If the cannonball hits other ships, it gets destroyed and the others recieve given damage.
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerShip") || other.gameObject.CompareTag("Enemy"))
        {
            if (this.name != other.name && this.name != other.transform.parent.name)    // For cannonballs to not hit the ship by which they are fired.
            {
                other.GetComponentInParent<Health>().ModifyHealth(-35);
                
                Destroy(this.gameObject);
                if (other.gameObject.CompareTag("Enemy"))
                {
                    // Call health-text refresh on other.
                    other.GetComponentInParent<EnemyBot>().HealthOnTop();
                }
                else if (other.gameObject.CompareTag("PlayerShip"))
                {
                    // Call health-text refresh on other.
                    other.GetComponentInParent<PlayerMovement>().HealthOnTop();
                }
            }
        }
        else if (other.transform.CompareTag("Ground"))
        {
            // Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Forcing a fixed Y position
        var pos = transform.position;   
        pos.y = 0.45f;
        transform.position = pos;
    }
}