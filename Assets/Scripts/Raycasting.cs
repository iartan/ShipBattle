using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// This controls the Model changes and the shooting of the player itself and the clicks/touches on the screen.
/// </summary>
public class Raycasting : MonoBehaviour
{
    public GameObject vortexToPlace;
    public GameObject cannonBallPrefab;
    public Transform player;
    public Transform firePoint;

    Ray ray;
    RaycastHit hit;
    public LayerMask layerMask;
    // Counts how long the player touches the screen.
    float clickTimer = 0.0f;

    public float cannonForce = 3.0f;
    public float delay = 3.0f;
    private bool canShoot = true;
    Vector3 direction;

    public GameObject mine;

    // Arrays in which the positions for the cannonballs to spawn are saved here.
    float[] oddCannonPos = { 0f, 1.0f, -1.0f, 2.0f, -2.0f };
    float[] evenCannonPos = { 0.5f, -0.5f, 1.5f, -1.5f };

    // Variable for the cannonball spawn position from the arrays.
    float spawnZ = 0;   

    // Slider for the shooting timer.
    public Slider slider;

    // Score saving.
    public GameController gameController;

    // Delay the player shooting.
    IEnumerator shootDelay()
    {
        slider.value = 0.0f;
        yield return new WaitForSeconds(delay);    
        canShoot = true;
    }

    void Start()
    {
        GetCannons();

        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        if (gameController == null)
        {
            Debug.Log("Can not find 'GameController' script");
        }
    }

    // TODO - Do not need this anymore because I changed the shooting system.
    // Get all cannons by searching the childs and save then in lists.
    public void GetCannons()
    {
        //foreach (Transform child in player.transform)
        //{
        //    if (child.gameObject.CompareTag("Cannons"))
        //    {
        //        currentShip = child.gameObject;
        //        currentShip.transform.parent = player.transform;
        //        foreach (Transform childOfChild in child.transform)
        //        {
        //            if (childOfChild.gameObject.CompareTag("LeftCannon"))
        //            {
        //                cannonsLeft.Add(childOfChild.transform);
        //            }
        //            else if (childOfChild.gameObject.CompareTag("RightCannon"))
        //            {
        //                cannonsRight.Add(childOfChild.transform);
        //            }
        //        }
        //    }
        //}
    }

    // Replace the player ship prefab depending on the score.
    public void ChangeShipModel()   // TODO - Remove, deprecated.
    {   
        //int score = gameController.GetComponent<GameController>().score;
        //if (score < 60 && score > 29)
        //{
        //    if (currentShipLevel != 2)
        //    {
        //        currentShipLevel = 2;
        //        print("2 Level 2 for Player ship.");

        //        // Replace old prefab by the new one.
        //        GameObject thisModel = Instantiate(shipLevel2, player.transform.position, player.transform.rotation) as GameObject;
        //        Destroy(currentShip);
        //        thisModel.transform.parent = player.transform;
        //        currentShip = thisModel;
        //        // Top the health to a nex maximum.
        //        player.GetComponent<Health>().maxHealth = 150;
        //        player.GetComponent<Health>().currentHealth = 150;
        //        player.GetComponent<Health>().ModifyHealth(0);
        //        player.GetComponent<PlayerMovement>().HealthOnTop();
        //        // The cannons-lists needs to be reverted.
        //        cannonsLeft.Clear();
        //        cannonsRight.Clear();
        //        foreach (Transform child in currentShip.transform)
        //        {
        //            if (child.gameObject.CompareTag("LeftCannon"))
        //            {
        //                cannonsLeft.Add(child.transform);
        //            }
        //            else if (child.gameObject.CompareTag("RightCannon"))
        //            {
        //                cannonsRight.Add(child.transform);
        //            }
        //        }
        //    }
        //}
        //else if (score < 90 && score > 59)
        //{
        //    if (currentShipLevel != 3)
        //    {
        //        currentShipLevel = 3;

        //        // Replace the old ship model by a new prefab.
        //        GameObject thisModel = Instantiate(shipLevel3, player.transform.position, player.transform.rotation) as GameObject;
        //        Destroy(currentShip);
        //        thisModel.transform.parent = player.transform;
        //        currentShip = thisModel;
        //        // Top the health to a nex maximum
        //        player.GetComponent<Health>().maxHealth = 200;
        //        player.GetComponent<Health>().currentHealth = 200;
        //        player.GetComponent<Health>()
        //                  .ModifyHealth(0);
        //        player.GetComponent<PlayerMovement>().HealthOnTop();
        //        // The cannons-lists needs to be reverted.
        //        cannonsLeft.Clear();
        //        cannonsRight.Clear();
        //        foreach (Transform child in currentShip.transform)
        //        {
        //            if (child.gameObject.CompareTag("LeftCannon"))
        //            {
        //                cannonsLeft.Add(child.transform);
        //            }
        //            else if (child.gameObject.CompareTag("RightCannon"))
        //            {
        //                cannonsRight.Add(child.transform);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        print("Player ship already is level 3");
        //    }
        //}
    }

    // Drop a mine at player position when button is pressed.
    public void DropMine()
    {
        Vector3 dropPos = player.transform.position;
        Instantiate(mine, dropPos, UnityEngine.Random.rotation);
    }

    // Reset ship model when the reset button is pressed. TODO - since the leveling system got changed this is not needed anymore.
    public void ResetShipModel()
    {   
        //// Swap current prefab with the level 1 prefab.
        //GameObject thisModel = Instantiate(shipLevel1, player.transform.position, player.transform.rotation) as GameObject;
        //Destroy(currentShip);
        //thisModel.transform.parent = player.transform;
        //currentShip = thisModel;
        //player.GetComponent<Health>().maxHealth = 100;
        //player.GetComponent<Health>().ModifyHealth(0);
        //player.GetComponent<PlayerMovement>().HealthOnTop();
        //cannonsLeft.Clear();
        //cannonsRight.Clear();
        //foreach (Transform child in currentShip.transform)
        //{
        //    if (child.gameObject.CompareTag("LeftCannon"))
        //    {
        //        cannonsLeft.Add(child.transform);
        //    }
        //    else if (child.gameObject.CompareTag("RightCannon"))
        //    {
        //        cannonsRight.Add(child.transform);
        //    }
        //}
    }

    // Check if touch/click is on the UI to stop the raycast.
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    // TODO - Remove the whole shooting angel stuff, it is not needed any more because of the new game design decisions.
    public float CheckShootingAngle(Vector3 other)
    {
        Vector3 targetDir = other - player.transform.position;
        float angle = Vector3.SignedAngle(targetDir, player.transform.forward, transform.up);
        return angle;
    }

    // Respawn the player when the respawn button is pressed.
    // TODO - The score also needs to be set to 0.
    public void RespawnPlayer()
    {
        player.transform.position = new Vector3(Random.Range(-50, 50), 0.25f, Random.Range(-50, 50));
        player.transform.gameObject.SetActive(true);
        ResetShipModel();
        player.GetComponent<Health>().maxHealth = 100;

    }

    void Update()
    {
        // Shooting timer slider.
        slider.value += Time.deltaTime;

        if (!IsPointerOverUIObject()) // To prevent raycasting from going through UI elements. && EventSystem.current.currentSelectedGameObject != null
        {   
            // Shoot a ray from the camera to the click/touch position.
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100f, layerMask))
            {
                if (Input.GetMouseButton(0))
                {
                    // Start counting for how long the screen is pressed to either get a normal click or a long touch.
                    clickTimer += Time.deltaTime;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    // Debug.Log(clickTimer);
                    
                    bool rightAngle = true; // Setting rightAngle manually to true, to be able to shoot from all angles.
                    float angle = CheckShootingAngle(hit.point);

                    if ((angle > 45f && angle < 135f) || (angle < -45f && angle > -135f))
                    {
                        rightAngle = true;
                        // print("Angle is right at " + angle);
                    }
                    else
                    {
                        // print("Cant shoot at this angle.");  TODO revise the whole rightAngle stuff.
                    }

                    // If the ray hits a vortex (layer 9) and the vortex belongs to the player itself, destroy it.
                    if (hit.transform.gameObject.layer == 9)
                    {
                        if (hit.transform.gameObject.name == "PlayerVortex")
                        {
                            Destroy(hit.transform.gameObject);
                        }
                    }

                    // If the player can shoot and the click/touch duration is less than 0.2 seconds, then shoot from all cannons.
                    else if (hit.transform.gameObject.layer == 8 && canShoot && rightAngle && clickTimer < 0.2f || hit.transform.gameObject.layer == 4 && canShoot && rightAngle && clickTimer < 0.2f)
                    {
                        // Get the player level.
                        int level = player.GetComponent<LevelController>().level;
                        int currentDeck = player.GetComponent<LevelController>().currentDeck;

                        // Reset the clickTimer.
                        clickTimer = 0;
                        //leftDist = Vector3.Distance(cannonsLeft[0].transform.position, hit.point);
                        //rightDist = Vector3.Distance(cannonsRight[0].transform.position, hit.point);

                        //if (leftDist <= rightDist)
                        //{
                        //    currentCannons = cannonsLeft;
                        //}
                        //else
                        //{
                        //    currentCannons = cannonsRight;
                        //}

                        var go1 = new GameObject { name = "Circle" };   // Instantiating a circle on the water to indicate shooting position.
                        go1.transform.Translate(hit.point);
                        go1.DrawCircle(0.5f, 0.1f);
                        Destroy(go1.gameObject, 2f);

                        // Shoot the amount of times matching the level.
                        for (int i = 0; i < level; i++)
                        {   
                            // The hitpoint of the raycast in the ground-layer is the shoot position.
                            Vector3 shotPos = hit.point;

                            spawnZ = oddCannonPos[i - ((i / 5) * 5)];

                            //if (i < 5)
                            //{
                            //    if (level % 2 != 0)
                            //    {
                            //        spawnZ = oddCannonPos[i - ((i / 5) * 5)];
                            //    }
                            //    else
                            //    {
                            //        spawnZ = evenCannonPos[i - ((i / 4) * 4)];
                            //    }
                            //}
                            //else if (i >= 5)
                            //{

                            //}
                            

                            // Spawn cannonball in a row in the direction where the shoot position faces the player.
                            // I came to this formula ((0.5f * (level - (i * 2)) -0.5f)) to let the cannons shoot where each cannonball is cented and aligned in the desired line and keeping
                            // the distance one to the other.

                            // shootDirection = Direction in which the cannonballs are flying.
                            // Setting both Y-axis manually to 0 for the direction to be a completely horizontal line.
                            Vector3 shootDirection = (new Vector3(shotPos.x, 0f, shotPos.z) - new Vector3(player.transform.position.x, 0f, player.transform.position.z)).normalized;

                            // spawnDirection = Direction in which the cannonballs spawn on the ship, 
                            Vector3 spawnDirection = Quaternion.Euler(0f, -90.0f, 0f) * shootDirection;
                            
                            // Hardcasting cannonball spawn positions because its too complicated for now.
                            Vector3 cannonSpawnPos = new Vector3(player.transform.position.x, (0.625f * (i / 5)) + 0.25f, player.transform.position.z) + spawnDirection * spawnZ; // - ((i) / 5) * 0.5f);  + ((i / 5) * 5)

                            // Vector3 cannonSpawnPos = new Vector3(player.transform.position.x, (0.5f * (i / 5)) + 0.25f, player.transform.position.z) + spawnDirection * (0.5f * (level - (i * 3)) - 0.5f); // - ((i) / 5) * 0.5f);  + ((i / 5) * 5)
                            // Vector3 cannonSpawnPos = new Vector3(player.transform.position.x, (0.5f * (i / 5)) + 0.25f, player.transform.position.z) + spawnDirection * (0.5f * (level - (((level - 1) / 5 * 5)) - ((i - i / 5 * 5) * 2) - 0.5f)); // - ((i) / 5) * 0.5f);  + ((i / 5) * 5)


                            // Vector3 direction = shotPos - cannonSpawnPos;

                            // Shooting.
                            GameObject cannonBall = Instantiate(cannonBallPrefab, cannonSpawnPos, Quaternion.identity); // Instantiating the cannonball further ahead to not automatic touch the cannon itself and cause collision.
                            // Get access to the rigidbody of the cannonball to manipulate it's states.
                            Rigidbody rb = cannonBall.GetComponent<Rigidbody>();
                            cannonBall.name = "Player";
                            cannonBall.gameObject.tag = "PlayerCannonBall";

                            cannonBall.GetComponent<CannonBall>().goal = shotPos;
                            rb.AddForce((shootDirection) * cannonForce, ForceMode.Impulse);

                            // Debug.Log("Level " + (i + 1) + " / 5 = " + (i + 1) / 5);
                            // Debug.Log(level + " " + (i * 2));
                            // Debug.Log((level - i / 5 * 5) + " " + ((i - i / 5 * 5) * 2));
                            // Debug.Log("Level is: " + (level - ((level - 1) / 5) * 5) + " and i is: " + (i - i / 5 * 5) * 2);
                            // Debug.Log("Level is: " + (level - ((level - 1) / 5) * 5) + " and i is: " + (i - i / 5 * 5) * 2);

                            // Debug.Log((0.5f * (level - i / 5 * 5 - ((i - i / 5 * 5) * 2)) - 0.5f));
                            // Debug.Log((0.5f * (i / 5)) + 0.25f);
                            // Debug.Log(level / currentDeck);
                            //Debug.Log(level);
                            //Debug.Log((i / 5) * 5);
                            //Debug.Log(level - i / 5 * 5);
                            //Debug.Log(level - i / 5 * 5);
                            // Debug.Log((0.5f * (level - (i * 2)) - 0.5f));
                            // Debug.Log((level * 0.5f) - ((i + 1) * 0.5f));
                            // Debug.Log(level - level * 0.75f); // (i * 0.5));
                            // Debug.Log(level / 2.0f - 1.5);
                            // Debug.Log(-1 + i * 0.5f);
                            // Debug.Log(0.5f * (level - (((level - 1) / 5 * 5)) - ((i - i / 5 * 5) * 2) - 0.5f));
                            // Debug.Log((0.5f * (level - (i * 2)) - 0.5f));
                            // Debug.Log("Level is: " + (level - (level - 1) / 5 * 5) + " And i is:" + (i * 2));
                            Debug.Log((level - 1));

                        }

                        // TODO - This is the old shooting method, currently changing it.
                        //foreach (Transform cannon in currentCannons)
                        //{
                        //    var xz = UnityEngine.Random.insideUnitCircle * 0.5f;    // Random Vector2 position in a given radius.
                        //    var randomizedPosition = new Vector3(xz.x, cannon.transform.position.y, xz.y) + hit.point; // Converting Vector2 to Vector3 and adding 1 to the Y-Axis so the position is above earth.
                        //    Vector3 direction = randomizedPosition - cannon.transform.position;

                        //    GameObject cannonBall = Instantiate(cannonBallPrefab, cannon.transform.position, Quaternion.identity); // Instantiating the cannonball further ahead to not automatic touch the cannon itself and cause collision.
                        //    // Get access to the rigidbody of the cannonball to manipulate it's states.
                        //    Rigidbody rb = cannonBall.GetComponent<Rigidbody>();
                        //    cannonBall.name = "Player";
                        //    cannonBall.gameObject.tag = "PlayerCannonBall";

                        //    cannonBall.GetComponent<CannonBall>().goal = randomizedPosition;
                        //    rb.AddForce((direction.normalized) * cannonForce, ForceMode.Impulse);
                        //}

                        canShoot = false;   // Bool for shootDelay.
                        StartCoroutine(shootDelay());
                    }
                    // If clickTimer is greater than 0.2f then is is a long press, so instantiate a vortex.
                    else if (hit.transform.gameObject.layer == 4 && clickTimer >= 0.2f || hit.transform.gameObject.layer == 8 && clickTimer >= 0.2f)
                    {
                        clickTimer = 0;
                        // Save the created vortex in a GameObject variable to give it a tag.
                        GameObject track = Instantiate(vortexToPlace, new Vector3(hit.point.x, 0.0f, hit.point.z), Quaternion.identity);
                        track.gameObject.name = "PlayerVortex";
                    }
                    else if (hit.transform.gameObject.layer == 7)
                    {
                        // Do nothing for now.
                    }
                    clickTimer = 0;
                }
            }
        }
    }
}
