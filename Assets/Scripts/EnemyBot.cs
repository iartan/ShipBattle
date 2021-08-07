using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBot : MonoBehaviour
{
    public Vector3 randomPosition;
    
    public GameObject cannonBallPrefab;

    private bool startShooting = false;
    public float cannonForce = 20f;
    float distance;
    public float range = 6f;

    IEnumerator randomMove;
    IEnumerator shoot;

    public GameObject scoreText;
    public GameObject healthText;
    public int score = 0;
    public int crateScoreValue = 10;

    public List<Transform> cannonsLeft = new List<Transform>();    // Lists for putting all cannons in a left and right list.
    public List<Transform> cannonsRight = new List<Transform>();
    public List<Transform> currentCannons = new List<Transform>();

    // Vortex interaction.
    public bool insideVortex = false;
    private GameObject lastVortex;
    public GameObject vortexToPlace;
    private bool canCastVortex = false;
    IEnumerator castVortex;

    public float forwardSpeed = 2.5f;
    public float backwardSpeed = 0f;
    public float aroundSpeed = 20f;
    public float rotationSpeed = 0f;
    private bool shipRoutine = false;
    private bool beginDecrease = false;

    // Ship model changes.
    public GameObject shipLevel1;
    public GameObject shipLevel2;
    public GameObject shipLevel3;
    private GameObject currentShip;
    private int currentShipLevel = 1;

    public GameObject crate;
    public Transform explosionPrefab;

    float leftDist = 0;
    float rightDist = 0;

    void Start()
    {
        GetCannons();

        randomPosition = new Vector3(Random.Range(-50, 50), 0.25f, Random.Range(-50, 50));
        
        var viewCircle = new GameObject { name = "ViewRange" };   // Drawing a circle to indicate the view range
        viewCircle.transform.Translate(this.transform.position);
        viewCircle.transform.SetParent(this.transform);
        viewCircle.DrawCircle(6f, 0.05f);

        // Getting current and maxhealth and displaying it above the ships.
        HealthOnTop();
    }

    private IEnumerator Move(Vector3 destination, float speed)
    {
        // print(this.transform.name + " is going to position: " + randomPosition);
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

    public void HealthOnTop()
    {
        healthText.GetComponent<Text>().text = "HP: " + gameObject.GetComponent<Health>().currentHealth + "  /  " + gameObject.GetComponent<Health>().maxHealth;
    }

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

    public void ChangeShipModel()
    {
        if (score < 60 && score > 29)
        {
            if (currentShipLevel != 2)
            {
                currentShipLevel = 2;

                GameObject thisModel = Instantiate(shipLevel2, transform.position, transform.rotation) as GameObject;
                Destroy(currentShip);
                thisModel.transform.parent = transform;
                currentShip = thisModel;
                gameObject.GetComponent<Health>().maxHealth = 150;
                gameObject.GetComponent<Health>().currentHealth = 150;
                gameObject.GetComponent<Health>()
                          .ModifyHealth(0);
                HealthOnTop();
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

                print("Setting Enemyship to level 3: " + this.gameObject.name);
                GameObject thisModel = Instantiate(shipLevel3, transform.position, transform.rotation) as GameObject;
                Destroy(currentShip);
                thisModel.transform.parent = transform;
                currentShip = thisModel;
                gameObject.GetComponent<Health>().maxHealth = 200;
                gameObject.GetComponent<Health>().currentHealth = 200;
                gameObject.GetComponent<Health>()
                          .ModifyHealth(0);
                HealthOnTop();
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

    public float CheckShootingAngle(Collider other)
    {
        Vector3 targetDir = other.transform.position - this.transform.position;
        float angle = Vector3.SignedAngle(targetDir, this.transform.forward, transform.up);
        return angle;
    }

    void CreateVortex(Vector3 pos)
    {
        Instantiate(vortexToPlace, pos, Quaternion.identity);
    }

    void OnCollisionEnter(Collision other)  // Gain scorepoints on collision with the drop crates.
    {
        if (other.gameObject.CompareTag("Crates"))
        {
            AddScore(crateScoreValue);
            Destroy(other.gameObject);
            this.GetComponent<Health>().ModifyHealth(10);   // Crates give 5 healthpoints.
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
                var newPosition = new Vector3(xz.x, 1, xz.y) + this.transform.position; // Converting Vector2 to Vector3 and adding 1 to the Y-Axis so the position is above earth.
                Instantiate(crate, newPosition, UnityEngine.Random.rotation);
            }
        }
        else if (other.gameObject.CompareTag("VortexTag"))
        {
            Debug.Log("Entered a vortex.");
            lastVortex = other.gameObject;
            insideVortex = true;
        }

        ContactPoint cp = other.GetContact(0);
    }

    void OnCollisionStay(Collision other)
    {
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

    // If the bot meets an other ship, it evaluates if it should follow and attack or turn around and flee.
    void OnTriggerStay(Collider other)
    {   
        if (other.transform.CompareTag("PlayerShip") || other.transform.CompareTag("Enemy"))
        {
            bool shouldBotAttack = false;
            // Check if the other ship is stronger and decide if to attack.
            // Count the childs to find out the level. Very bad solution but It will do. The currentShipLevel variable is the current level.
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
                randomPosition = other.transform.position;
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
                

                //var go1 = new GameObject { name = "Circle" };   // Instantiating a circle on the water to indicate the flee to position.
                //go1.transform.Translate(randomPosition);
                //go1.DrawCircle(0.5f, 0.1f);
                //Destroy(go1.gameObject, 2f);

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
        else if (other.transform.CompareTag("Crates"))
        {
            randomPosition = new Vector3(other.transform.position.x, this.transform.position.y, other.transform.position.z);
        }
    }

    IEnumerator ShootOnSight(Collider other, float dist, float delay) 
    {
        while (startShooting)
        {
            leftDist = Vector3.Distance(cannonsLeft[0].transform.position, other.transform.position);
            rightDist = Vector3.Distance(cannonsRight[0].transform.position, other.transform.position);
            
            if (leftDist <= rightDist)
            {
                currentCannons = cannonsLeft;
            }
            else
            {
                currentCannons = cannonsRight;
            }

            foreach (Transform cannon in currentCannons)
            {
                Vector3 direction = other.transform.position - cannon.transform.position;

                GameObject cannonBall = Instantiate(cannonBallPrefab, cannon.transform.position, Quaternion.identity);  // Instantiating the cannonball further away in the front to not collide with the cannon itself.
                cannonBall.name = this.name;
                Rigidbody rb = cannonBall.GetComponent<Rigidbody>();
                cannonBall.tag = "EnemyCannonBall";
                // cannonBall.transform.SetParent(this.transform); // Removing cannonballs from beeing child of shooting ship.

                cannonBall.GetComponent<CannonBall>().goal = other.transform.position;
                
                rb.AddForce(direction.normalized * cannonForce, ForceMode.Impulse);
                // yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(delay);
            startShooting = false;
        }
    }

    private IEnumerator VortexTime(float waitTime, Vector3 pos)
    {
        while (canCastVortex)
        {
            CreateVortex(pos);
            yield return new WaitForSeconds(waitTime);
            canCastVortex = false;
        }
    }

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
