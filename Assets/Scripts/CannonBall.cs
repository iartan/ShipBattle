using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public int scoreValue;
    public GameController gameController;

    // private bool keepFalling = false;

    public Vector3 originalPosition;
    public Vector3 groundPosition;
    public float distance;
    public float groundDistance;
    public Vector3 goal;
    
    // Start is called before the first frame update
    void Awake()
    {

    }
    
    void Start()
    {
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

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(5.0f);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerShip") || other.gameObject.CompareTag("Enemy"))
        {
            // print(this.name + " hit " + other.name); // Important lesson learned here: Those 2 lines schouldn't be inside those brackets but outside, right in the core of the method, because you dont see all objects which are collidet.
            // print(this.name + " hit " + other.transform.parent.name);

            if (this.name != other.name && this.name != other.transform.parent.name)    // For cannonballs to not hit the ship firing themself.
            {
                other.GetComponentInParent<Health>().ModifyHealth(-35);
                
                // gameController.AddScore(scoreValue); Removing the option to recieve points by only hiting an enemy.
                Destroy(this.gameObject);
                if (other.gameObject.CompareTag("Enemy"))
                {
                    other.GetComponentInParent<EnemyBot>().HealthOnTop();
                }
                else if (other.gameObject.CompareTag("PlayerShip"))
                {
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
        //groundPosition = new Vector3(transform.position.x, goal.y, transform.position.z);   // Für die Kanonenkugel einen Flugbogen simulieren, indem die Y-Achse verändert wird.
        //groundDistance = Vector3.Distance(groundPosition, goal);
        //if (groundDistance > (distance / 2) && !keepFalling)
        //{
        //    transform.position += Vector3.up * 5 * Time.deltaTime;
        //}
        //else
        //{
        //    keepFalling = true;
        //    transform.position += Vector3.up * -5 * Time.deltaTime;
        //}
        //transform.LookAt(goal, Vector3.left);
        //this.transform.position += transform.forward * 20f * Time.deltaTime;

        // Forcing a fixed Y position
        var pos = transform.position;   // Ship moves weird on the Y-axis, up and down. We force a 0.25 Y position here.
        pos.y = 0.45f;
        transform.position = pos;
    }
}