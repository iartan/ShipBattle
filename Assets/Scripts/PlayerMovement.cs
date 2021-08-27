using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

/// <summary>
/// Controls the movement of the player and the cheating buttons.
/// </summary>
public class PlayerMovement : MonoBehaviour
{   
    // Movement speed and vortex interaction.
    public float forwardSpeed = 2.5f;
    public float backwardSpeed = 0f;
    public float aroundSpeed = 20f;
    public float rotationSpeed = 0f;
    private bool shipRoutine = false;
    private bool beginDecrease = false;

    public int crateScoreValue = 10;
    public GameController gameController;
    public GameObject crate;
    public GameObject cannonBall;

    public GameObject healthText;

    // Arrow buttons movement.
    public bool buttonLeftBool = false;
    public bool buttonRightBool = false;
    public Button buttonLeft;

    public Transform explosionPrefab;

    void Start()
    {
        // Look for the GameController.
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        if (gameController == null)
        {
            Debug.Log("Can not find 'GameController' script");
        }
        // Refresh health on top.
        HealthOnTop();
    }

    public void HealthOnTop()
    {
        healthText.GetComponent<Text>().text = "HP: " + this.GetComponent<Health>().currentHealth + "  /  " + this.GetComponent<Health>().maxHealth;
    }

    // Those Decrease and Increase coroutines are a bad workaround for the realistic behaviour
    // when a ship enters a vortex to be slown down first and then slowly get faster.
    IEnumerator Decrease(float turningPoint, float end, float rateInc, float rateDec)
    {
        while (backwardSpeed < turningPoint && shipRoutine && !beginDecrease)
        {
            backwardSpeed += rateInc * Time.deltaTime;
            yield return null;
            shipRoutine = false;
        }
        if (backwardSpeed >= turningPoint)
        {
            beginDecrease = true;
        }
        while (backwardSpeed > end && shipRoutine && beginDecrease)
        {
            backwardSpeed -= rateDec * Time.deltaTime;
            yield return null;
            shipRoutine = false;
        }
        if (backwardSpeed < end)
        {
            backwardSpeed = end;
        }
    }
    
    IEnumerator Increase()
    {
        while (rotationSpeed < 100f && shipRoutine)
        {
            rotationSpeed += 20f * Time.deltaTime;
            yield return null;
            shipRoutine = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {   
        // Gain scorepoints when picking up a crate and destroy it.
        if (other.gameObject.CompareTag("Crates"))
        {
            gameController.AddScore(crateScoreValue);
            gameController.GetComponent<Raycasting>().ChangeShipModel();
            // this.GetComponent<LevelController>().ChangeLevel();
            Destroy(other.gameObject);
            this.GetComponent<Health>().ModifyHealth(10);   // Crates give 10 healthpoints.
            HealthOnTop();
        }
        else if (other.gameObject.CompareTag("Mine"))
        {
            Instantiate(explosionPrefab, other.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
            Destroy(other.gameObject);
            for (int i = 0; i < 3; i++) // Spawning crates after destruction.
            {
                var xz = UnityEngine.Random.insideUnitCircle * 0.5f;    // Random Vector2 position in a given radius.
                var newPosition = new Vector3(xz.x, 1, xz.y) + this.transform.position; // Converting Vector2 to Vector3 and adding 1 to the Y-Axis so the position is above ground level.
                Instantiate(crate, newPosition, UnityEngine.Random.rotation);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {   
        // Rotate around the vortex when inside.
        if (other.gameObject.CompareTag("VortexTag"))
        {
            this.transform.position -= transform.forward * backwardSpeed * Time.deltaTime; 
            shipRoutine = true;
            StartCoroutine(Decrease(1.5f, 0f, 2.5f, 1f));
            StartCoroutine(Increase());

            this.transform.RotateAround(other.transform.position, Vector3.down, aroundSpeed * Time.deltaTime);  // Lets this rotating around another transform. Vortex dragging the ship around.
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(other.transform.position - this.transform.position) * Quaternion.Euler(0f, 75f, 0f), rotationSpeed * Time.deltaTime);   // Copies the rotation on other object minus 75 on the Y-Axis, so it stand alwys sideways.
        }
    }

    // Instantiate crate at player position.
    public void DropCrate()
    {
        Vector3 dropPos = transform.position + (transform.up * 2.0f) + (transform.forward * 2.0f);
        Instantiate(crate, dropPos, UnityEngine.Random.rotation);
    }

    // The cheating button "Hurt" drops a cannonball on the player.
    public void DropCannonBall()
    {
        Vector3 dropPos = transform.position + (transform.up * 2.0f) + (transform.forward * 2.0f);
        Instantiate(cannonBall, dropPos, UnityEngine.Random.rotation);
    }

    // Control of the rotation when the arrow-buttons are pressed.
    public void OnPointerDown(Button btn)
    {
        buttonLeftBool = true;
        print("Button down.");
    }

    public void Test()
    {
        print("Button test");
    }

    public void OnMouseUp(Button btn)
    {
        buttonLeftBool = false;
        print("Button released.");
    }

    public void EnableLeftRotation()
    {
        buttonLeftBool = true;
    }
    public void DisableLeftRotation()
    {
        buttonLeftBool = false;
    }
    public void EnableRightRotation()
    {
        buttonRightBool = true;
    }
    public void DisableRightRotation()
    {
        buttonRightBool = false;
    }

    void Update()
    {
        this.transform.position += transform.forward * forwardSpeed * Time.deltaTime;  // Moves the player forward at given speed.
        var pos = transform.position;   // Ship moves weird on the Y-axis, up and down. We force a 0.25 Y position here.
        pos.y = 0.25f;
        transform.position = pos;

        if (buttonLeftBool)
        {
            this.transform.Rotate(0, -60 * Time.deltaTime, 0);
        }
        if (buttonRightBool)
        {
            this.transform.Rotate(0, 60 * Time.deltaTime, 0);
        }
    }
}