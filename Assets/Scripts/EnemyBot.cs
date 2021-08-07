using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script handles the movement and actions (shoot, patrol, flee) of the enemy-bots.
/// </summary>
public class EnemyBot : MonoBehaviour
{     
    public GameObject cannonBallPrefab;
    public GameObject crate;
    public Transform explosionPrefab;
    public GameObject scoreText;
    public GameObject healthText;
    
    IEnumerator randomMove;
    public Vector3 randomPosition;  // Important variable, its always the position where the bot is heading to. 
    float distance;

    IEnumerator shoot;
    public float cannonForce = 20f;
    private bool startShooting = false;
    public float range = 6f;
    
    // Variables for selecting shooting side.
    float leftDist = 0;
    float rightDist = 0;

    // The bot score and the score-value for picking up a crate.
    public int score = 0;
    public int crateScoreValue = 10;
    
    // Lists for putting all cannons in a left and right list.
    public List<Transform> cannonsLeft = new List<Transform>();    
    public List<Transform> cannonsRight = new List<Transform>();
    public List<Transform> currentCannons = new List<Transform>();

    // Vortex interaction.
    IEnumerator castVortex;
    private GameObject lastVortex;
    public GameObject vortexToPlace;
    public bool insideVortex = false;
    private bool canCastVortex = false;
    private bool shipRoutine = false;
    private bool beginDecrease = false;
    public float forwardSpeed = 2.5f;
    public float backwardSpeed = 0f;
    public float aroundSpeed = 20f;
    public float rotationSpeed = 0f;
    
    // Ship model changes.
    public GameObject shipLevel1;
    public GameObject shipLevel2;
    public GameObject shipLevel3;
    private GameObject currentShip;
    private int currentShipLevel = 1;

    void Start()
    {   
        // Add all cannons to cannons-list on startup.
        GetCannons();

        // Generate new position to start patrol.
        randomPosition = new Vector3(Random.Range(-50, 50), 0.25f, Random.Range(-50, 50));
        
        var viewCircle = new GameObject { name = "ViewRange" };   // Draw a circle on the ground to indicate the view range of the bots.
        viewCircle.transform.Translate(this.transform.position);
        viewCircle.transform.SetParent(this.transform);
        viewCircle.DrawCircle(6f, 0.05f);

        // Get current and maxhealth and display it above the bot.
        HealthOnTop();
    }

    // A coroutine to move to the next random position when a certain destination is reached.
    // TODO - This method seems a bit odd, why is here no waiting time? I don't remember... maybe this can be transformed to a normal method.
    private IEnumerator Move(Vector3 destination, float speed)
    {
        while (Vector3.Distance(this.transform.position, destination) > 0.1f)
        {
            if (!insideVortex)
            {
                this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(randomPosition - this.transform.position), speed);
            }
            // this.transform.LookAt(randomPosition * Time.deltaTime);
            yield return null;
        }
        randomPosition = new Vector3(Random.Range(-50, 50), 0.25f, Random.Range(-50, 50));
    }

    // Show health-text on top of the bot.
    public void HealthOnTop()
    {
        healthText.GetComponent<Text>().text = "HP: " + gameObject.GetComponent<Health>().currentHealth + "  /  " + gameObject.GetComponent<Health>().maxHealth;
    }

    // Search the own children for cannons and add them to the cannons-lists, either right or left depending on their tags.
    public void GetCannons()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.CompareTag("Enemy"))
            {
                currentShip = child.gameObject;
                currentShip.transform.parent = transform;
                foreach (Transform childOfChild in child.transform)
                {
                    if (childOfChild.gameObject.CompareTag("LeftCannon"))
                    {
                        cannonsLeft.Add(childOfChild.transform);
                    }
                    else if (childOfChild.gameObject.CompareTag("RightCannon"))
                    {
                        cannonsRight.Add(childOfChild.transform);
                    }
                }
            }
        }
    }

    // Change the ship model depending on the score. Swap the current prefab with a higher-level prefab and top health to a the new maximum health.
    // 0-29 = Model 1, 30 - 59 = Model 2, 60+ = Model 3. 
    public void ChangeShipModel()
    {
        if (score < 60 && score > 29)
        {
            if (currentShipLevel != 2)
            {
                currentShipLevel = 2;

                // Place the new model and get rid of the old one.
                GameObject thisModel = Instantiate(shipLevel2, transform.position, transform.rotation) as GameObject;
                Destroy(currentShip);
                thisModel.transform.parent = transform;
                currentShip = thisModel;
                // Set health to 200 by chanching the values on the health script attached on the bot.
                gameObject.GetComponent<Health>().maxHealth = 150;
                gameObject.GetComponent<Health>().currentHealth = 150;
                gameObject.GetComponent<Health>()
                          .ModifyHealth(0);
                // Refresh healh on top.
                HealthOnTop();
                // Because model changed, the cannons-lists need to be reverted.
                cannonsLeft.Clear();
                cannonsRight.Clear();
                foreach (Transform child in currentShip.transform)
                {
                    if (child.gameObject.CompareTag("LeftCannon"))
                    {
                        cannonsLeft.Add(child.transform);
                    }
                    else if (child.gameObject.CompareTag("RightCannon"))
                    {
                        cannonsRight.Add(child.transform);
                    }
                }
            }
        }
        else if (score < 90 && score > 59)
        {
            if (currentShipLevel != 3)
            {
                currentShipLevel = 3;

                // Place the new model and get rid of the old one.
                GameObject thisModel = Instantiate(shipLevel3, transform.position, transform.rotation) as GameObject;
                Destroy(currentShip);
                thisModel.transform.parent = transform;
                currentShip = thisModel;
                // Set health to 200 by chanching the values on the health script attached on the bot.
                gameObject.GetComponent<Health>().maxHealth = 200;
                gameObject.GetComponent<Health>().currentHealth = 200;
                gameObject.GetComponent<Health>().ModifyHealth(0);
                HealthOnTop();
                // Because model changed, the cannons-lists need to be reverted.
                cannonsLeft.Clear();
                cannonsRight.Clear();
                foreach (Transform child in currentShip.transform)
                {
                    if (child.gameObject.CompareTag("LeftCannon"))
                    {
                        cannonsLeft.Add(child.transform);
                    }
                    else if (child.gameObject.CompareTag("RightCannon"))
                    {
                        cannonsRight.Add(child.transform);
                    }
                }
            }
        }
    }

    // Method to update the score.
    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        ScoreUpdate();
        ChangeShipModel();
    }

    void ScoreUpdate()
    {
        scoreText.GetComponent<Text>().text = "Score: " + score;
    }

    // TODO - Not longer needed. Former method to check the angle for deciding if a bot is able to shoot, but now shooting angle is not a thing.
    public float CheckShootingAngle(Collider other)
    {
        Vector3 targetDir = other.transform.position - this.transform.position;
        float angle = Vector3.SignedAngle(targetDir, this.transform.forward, transform.up);
        return angle;
    }

    // Place a vortex at position.
    void CreateVortex(Vector3 pos)
    {
        Instantiate(vortexToPlace, pos, Quaternion.identity);
    }

    void OnCollisionEnter(Collision other)  // Gain scorepoints on collision with the dropped crates.
    {
        if (other.gameObject.CompareTag("Crates"))
        {
            AddScore(crateScoreValue);
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
        else if (other.gameObject.CompareTag("VortexTag"))
        {
            Debug.Log("Entered a vortex.");
            lastVortex = other.gameObject;
            insideVortex = true;
        }

        // TODO - Remove this, contact points when a collision happens are not longer needed.
        ContactPoint cp = other.GetContact(0);
    }

    void OnCollisionStay(Collision other)
    {   
        // If a bot is inside a vortex, let is be dragged around till the vortex disappers.
        if (other.gameObject.CompareTag("VortexTag"))
        {
            lastVortex = other.gameObject;
            insideVortex = true;
            this.transform.position -= transform.forward * backwardSpeed * Time.deltaTime;
            shipRoutine = true;
            StartCoroutine(Decrease(1.5f, 0f, 2.5f, 1f));
            StartCoroutine(Increase());

            this.transform.RotateAround(other.transform.position, Vector3.down, aroundSpeed * Time.deltaTime);  // Lets this rotating around another transform. Vortex dragging the ship around.
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(other.transform.position - this.transform.position) * Quaternion.Euler(0f, 75f, 0f), rotationSpeed * Time.deltaTime);   // Copies the rotation on other object minus 75 on the Y-Axis, so it stand alwys sideways.
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("VortexTag"))
        {
            Debug.Log("Leaving the vortex.");
            insideVortex = false;
        }
    }

    // If the bot meets an other ship, it evaluates depending on the level if it should follow and attack or turn around and flee.
    void OnTriggerStay(Collider other)
    {   
        if (other.transform.CompareTag("PlayerShip") || other.transform.CompareTag("Enemy"))
        {
            bool shouldBotAttack = false;
            // Check if the other ship is stronger and decide if to attack.
            // Count the childs to find out the level. Very bad solution but tt will do for now. The currentShipLevel variable is the current level.
            if (currentShipLevel * 2 == other.transform.childCount)
            {
                // print("This ship has " + other.transform.childCount + " cannons and I have " + currentShipLevel * 2 + " I'll attack.");
                if (true) // %70 percent attack chance (1 - 0.3 is 0.7)
                {
                    shouldBotAttack = true;
                }
            }
            else if (currentShipLevel * 2 < other.transform.childCount)
            {
                // print("This ship has " + other.transform.childCount + " cannons and I have " + currentShipLevel * 2 + " - I'll run.");
                if (true) // %20 percent attack chance (1 - 0.8 is 0.2)
                {
                    shouldBotAttack = false;
                }
            }
            else
            {
                // print("This ship has " + other.transform.childCount + " children and I have " + currentShipLevel * 2 + " - RUN!");
                shouldBotAttack = true;
            }

            if (shouldBotAttack)
            {   
                // If attacking is true, then follow the bot and start the shooting coroutine.
                randomPosition = other.transform.position;
                distance = (Vector3.Distance(this.transform.position, other.transform.position));
                if (distance < range)
                {
                    if (!startShooting)
                    {
                        float angle = CheckShootingAngle(other);
                        if (angle > 45f && angle < 135f || true)    // TODO - Setting this statement manually to true, for bots to be able to shoot from all angles. Because I dropped the angle-idea-thing.
                        {
                            startShooting = true;
                            shoot = ShootOnSight(other, 15f, 3f);
                            StartCoroutine(shoot);
                        }
                        else if (angle < -45f && angle > -135f)
                        {
                            startShooting = true;
                            shoot = ShootOnSight(other, 15f, 3f);
                            StartCoroutine(shoot);
                        }
                    }
                }
            }
            else
            {
                // Get a position in the opposite direction from the enemy to run away. Save that position as randomPosition.
                Vector3 direction = other.transform.position - this.transform.position;
                randomPosition = transform.position - direction * 2.0f;

                // Cast vortex at enemies position.
                if (!canCastVortex)
                {
                    canCastVortex = true;
                    // To cast the vortex a bit ahead of the enemy, subtract a float in the direction.
                    castVortex = VortexTime(15.0f, other.transform.position - direction * 0.25f);
                    StartCoroutine(castVortex);
                }

                // Calculate the distance between self and the enemy to check it shooting is possible.
                distance = (Vector3.Distance(this.transform.position, other.transform.position));
                if (distance < range)
                {
                    if (!startShooting)
                    {
                        float angle = CheckShootingAngle(other);
                        if (angle > 45f && angle < 135f || true)    // DISCLAIMER Setting this statement manually to true, for bots to be able to shoot from all angles.
                        {
                            startShooting = true;
                            shoot = ShootOnSight(other, 15f, 3f);
                            StartCoroutine(shoot);
                        }
                        else if (angle < -45f && angle > -135f)
                        {
                            startShooting = true;
                            shoot = ShootOnSight(other, 15f, 3f);
                            StartCoroutine(shoot);
                        }
                    }
                }
            }
        }
        else if (other.transform.CompareTag("Crates"))  // Pick up the crates.
        {
            randomPosition = new Vector3(other.transform.position.x, this.transform.position.y, other.transform.position.z);
        }
    }

    // Coroutine for shooting with a delay.
    IEnumerator ShootOnSight(Collider other, float dist, float delay) 
    {
        while (startShooting)
        {   
            // The distance between the first left or right cannon to the enemy decides from which side the cannons are shooting. Bad solution but it will do.
            leftDist = Vector3.Distance(cannonsLeft[0].transform.position, other.transform.position);
            rightDist = Vector3.Distance(cannonsRight[0].transform.position, other.transform.position);
            
            // Add the cannons from the closest side to currentCannons list for shooting.
            if (leftDist <= rightDist)
            {
                currentCannons = cannonsLeft;
            }
            else
            {
                currentCannons = cannonsRight;
            }

            // Shoot from all cannons in the currentCannons list.
            foreach (Transform cannon in currentCannons)
            {
                Vector3 direction = other.transform.position - cannon.transform.position;

                GameObject cannonBall = Instantiate(cannonBallPrefab, cannon.transform.position, Quaternion.identity);  // Instantiating the cannonball further away in the front to not collide with the cannon itself.
                cannonBall.name = this.name;
                Rigidbody rb = cannonBall.GetComponent<Rigidbody>();
                cannonBall.tag = "EnemyCannonBall";
                // cannonBall.transform.SetParent(this.transform); // Removing cannonballs from beeing child of shooting ship.

                // The instantiated cannonball has his own script to fly to the position, so the shooting position needs to be sent to the "CannonBall" script.
                cannonBall.GetComponent<CannonBall>().goal = other.transform.position;
                
                rb.AddForce(direction.normalized * cannonForce, ForceMode.Impulse);
                // yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(delay);
            startShooting = false;
        }
    }

    // Coroutine for placing a vortex once in some seconds.
    private IEnumerator VortexTime(float waitTime, Vector3 pos)
    {
        while (canCastVortex)
        {
            CreateVortex(pos);
            yield return new WaitForSeconds(waitTime);
            canCastVortex = false;
        }
    }

    // Those Decrease and Increase coroutines are a bad workaround for the realistic behaviour when a ship enters a vortex to be slown down first and then slowly get faster.
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

    private void Update()
    {   
        // Brute-check is the lastVortex entered still exists, because OnTriggerExit/OnColliderExit doesn't get triggered if the "other" object is destroyed.
        if (insideVortex)
        {
            if (lastVortex == null)
            {
                Debug.Log("Vortex destroyed.");
                insideVortex = false;
            }
        }
    }

    void FixedUpdate()
    {
        // Move and rotate toward a position, the second parameter is the rotation speed aka turning speed of the bot.
        if (!insideVortex)
        {
            randomMove = Move(randomPosition, 0.0015f);
            StartCoroutine(randomMove);
        }
        this.transform.position += transform.forward * 2.5f * Time.deltaTime;   // This makes the enemy to move forward.

        var pos = transform.position;   // Ship moves weird on the Y-axis, up and down. We force a 0.25 Y position here.
        pos.y = 0.25f;
        transform.position = pos;

        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}
