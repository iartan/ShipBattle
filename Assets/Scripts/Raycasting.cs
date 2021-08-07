using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Raycasting : MonoBehaviour
{
    public GameObject vortexToPlace;
    public GameObject cannonBallPrefab;
    public Transform player;
    public Transform firePoint;

    Ray ray;
    RaycastHit hit;
    public LayerMask layerMask;
    float clickTimer = 0.0f;

    public float cannonForce = 3.0f;
    public float delay = 3.0f;
    private bool canShoot = true;
    Vector3 direction;

    // Ship level changes.
    public GameObject shipLevel1;
    public GameObject shipLevel2;
    public GameObject shipLevel3;
    private GameObject currentShip;
    private int currentShipLevel = 1;
    public GameObject mine;

    public List<Transform> cannonsLeft = new List<Transform>();    // Lists for putting all cannons in a left and right list.
    public List<Transform> cannonsRight = new List<Transform>();
    public List<Transform> currentCannons = new List<Transform>();

    float leftDist = 0;
    float rightDist = 0;

    public Slider slider;

    // Score saving.
    public GameController gameController;

    IEnumerator shootDelay()
    {
        slider.value = 0.0f;
        yield return new WaitForSeconds(delay);    // Delaying player shooting.
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

    public void GetCannons()
    {
        foreach (Transform child in player.transform)
        {
            if (child.gameObject.CompareTag("PlayerShip"))
            {
                currentShip = child.gameObject;
                currentShip.transform.parent = player.transform;
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
        int score = gameController.GetComponent<GameController>().score;
        if (score < 60 && score > 29)
        {
            if (currentShipLevel != 2)
            {
                currentShipLevel = 2;
                print("2 Level 2 for Player ship.");

                GameObject thisModel = Instantiate(shipLevel2, player.transform.position, player.transform.rotation) as GameObject;
                Destroy(currentShip);
                thisModel.transform.parent = player.transform;
                currentShip = thisModel;
                player.GetComponent<Health>().maxHealth = 150;
                player.GetComponent<Health>().currentHealth = 150;
                player.GetComponent<Health>()
                          .ModifyHealth(0);
                player.GetComponent<PlayerMovement>().HealthOnTop();
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

                GameObject thisModel = Instantiate(shipLevel3, player.transform.position, player.transform.rotation) as GameObject;
                Destroy(currentShip);
                thisModel.transform.parent = player.transform;
                currentShip = thisModel;
                player.GetComponent<Health>().maxHealth = 200;
                player.GetComponent<Health>().currentHealth = 200;
                player.GetComponent<Health>()
                          .ModifyHealth(0);
                player.GetComponent<PlayerMovement>().HealthOnTop();
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
            else
            {
                print("Player ship already is level 3");
            }
        }
    }

    public void DropMine()
    {
        Vector3 dropPos = player.transform.position;
        Instantiate(mine, dropPos, UnityEngine.Random.rotation);
    }

    public void ResetShipModel()
    {
        GameObject thisModel = Instantiate(shipLevel1, player.transform.position, player.transform.rotation) as GameObject;
        Destroy(currentShip);
        thisModel.transform.parent = player.transform;
        currentShip = thisModel;
        player.GetComponent<Health>().maxHealth = 100;
        player.GetComponent<Health>()
                  .ModifyHealth(0);
        player.GetComponent<PlayerMovement>().HealthOnTop();
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

    private bool IsPointerOverUIObject()    // Checking if touching on UI for stoping raycast.
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public float CheckShootingAngle(Vector3 other)
    {
        Vector3 targetDir = other - player.transform.position;
        float angle = Vector3.SignedAngle(targetDir, player.transform.forward, transform.up);
        return angle;
    }

    public void RespawnPlayer()
    {
        player.transform.position = new Vector3(Random.Range(-50, 50), 0.25f, Random.Range(-50, 50));
        player.transform.gameObject.SetActive(true);
        ResetShipModel();
        player.GetComponent<Health>().maxHealth = 100;

    }

    void Update()
    {
        slider.value += Time.deltaTime;

        if (!IsPointerOverUIObject()) // To prevent raycasting from going through UI elements. && EventSystem.current.currentSelectedGameObject != null
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100f, layerMask))
            {
                if (Input.GetMouseButton(0))
                {
                    clickTimer += Time.deltaTime;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    Debug.Log(clickTimer);
                    
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

                    if (hit.transform.gameObject.layer == 9)
                    {
                        if (hit.transform.gameObject.name == "PlayerVortex")
                        {
                            Destroy(hit.transform.gameObject);
                        }
                    }

                    else if (hit.transform.gameObject.layer == 8 && canShoot && rightAngle && clickTimer < 0.2f || hit.transform.gameObject.layer == 4 && canShoot && rightAngle && clickTimer < 0.2f)
                    {
                        clickTimer = 0;
                        leftDist = Vector3.Distance(cannonsLeft[0].transform.position, hit.point);
                        rightDist = Vector3.Distance(cannonsRight[0].transform.position, hit.point);

                        if (leftDist <= rightDist)
                        {
                            currentCannons = cannonsLeft;
                        }
                        else
                        {
                            currentCannons = cannonsRight;
                        }

                        var go1 = new GameObject { name = "Circle" };   // Instantiating a circle on the water to indicate shooting position.
                        go1.transform.Translate(hit.point);
                        go1.DrawCircle(0.5f, 0.1f);
                        Destroy(go1.gameObject, 2f);

                        foreach (Transform cannon in currentCannons)
                        {
                            var xz = UnityEngine.Random.insideUnitCircle * 0.5f;    // Random Vector2 position in a given radius.
                            var randomizedPosition = new Vector3(xz.x, cannon.transform.position.y, xz.y) + hit.point; // Converting Vector2 to Vector3 and adding 1 to the Y-Axis so the position is above earth.
                            Vector3 direction = randomizedPosition - cannon.transform.position;

                            GameObject cannonBall = Instantiate(cannonBallPrefab, cannon.transform.position, Quaternion.identity); // Instantiating the cannonball further ahead to not automatic touch the cannon itself and cause collision.
                            Rigidbody rb = cannonBall.GetComponent<Rigidbody>();
                            cannonBall.name = "Player";
                            cannonBall.gameObject.tag = "PlayerCannonBall";
                            // cannonBall.transform.SetParent(player);  // Removing cannonballs from beeing child of shooting ship.

                            cannonBall.GetComponent<CannonBall>().goal = randomizedPosition;
                            rb.AddForce((direction.normalized) * cannonForce, ForceMode.Impulse);
                        }

                        canShoot = false;   // Bool for shootDelay.
                        StartCoroutine(shootDelay());
                    }
                    else if (hit.transform.gameObject.layer == 4 && clickTimer >= 0.2f || hit.transform.gameObject.layer == 8 && clickTimer >= 0.2f)
                    {
                        clickTimer = 0;
                        GameObject track = Instantiate(vortexToPlace, new Vector3(hit.point.x, 0.0f, hit.point.z), Quaternion.identity);
                        track.gameObject.name = "PlayerVortex";
                        // print(angle);
                    }
                    else if (hit.transform.gameObject.layer == 7)
                    {

                    }
                    clickTimer = 0;
                }

                
            }
        }
    }
}
