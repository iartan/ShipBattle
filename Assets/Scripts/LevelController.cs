using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The management of the leveling system for the ships and the score.
/// </summary>
public class LevelController : MonoBehaviour
{
    // Initial variables.
    int score;
    // Make the level variable public to get it from other scripts.
    public int level;
    int levelStep = 35;    // TODO - Find better variable name for this.
    float scaleChange = 5.0f;
    private int cratePoints = 10;

    public GameObject hull;
    public GameObject cannonPrefab;
    public GameObject cannonSystem;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        level = 1;
    }

    // Method to change the level of the ship depending on the score.
    public void ChangeLevel()
    {
        // Debug.Log("Score: " + score);
        // Debug.Log(this.transform.name);
        // Check depending on the score if the ship is ready to level.
        if (score >= level * levelStep)
        {
            level++;
            // On each level the ship gets a nex max health and some 100 health points.
            this.GetComponent<Health>().maxHealth = level * 50 + 50;
            // this.GetComponent<Health>().currentHealth = level * 100;
            this.GetComponent<Health>().ModifyHealth(100);
            
            // Debug.Log("Time to level.");
            hull.transform.localScale += new Vector3(scaleChange, scaleChange * 2.0f, scaleChange);

            // Move all cannons which are inside Cannons a bit to the front to make place for the new cannons.
            foreach (Transform child in cannonSystem.transform)
            {
                Vector3 offset = new Vector3(0f, 0f, 0.25f);
                child.transform.localPosition += offset;
            }

            // Add a new cannon on both sides.
            GameObject cannon = Instantiate(cannonPrefab, new Vector3(this.transform.localPosition.x - 0.4f, this.transform.localPosition.y + 1.0f, this.transform.localPosition.z), this.transform.rotation);
            cannon.transform.SetParent(cannonSystem.transform);
            cannon.name = "LeftCannon" + level;
            cannon.tag = "LeftCannon";
            cannon.transform.localPosition = new Vector3(-0.3f, 1.0f, -0.25f * (level - 1));
            cannon.transform.localEulerAngles = new Vector3(0, 0, 0);
            GameObject secondCannon = Instantiate(cannonPrefab, new Vector3(this.transform.position.x + 0.4f, this.transform.position.y + 0.25f, this.transform.position.z), Quaternion.identity);
            secondCannon.transform.SetParent(cannonSystem.transform);
            secondCannon.name = "RightCannon" + level;
            secondCannon.tag = "RightCannon";
            secondCannon.transform.localPosition = new Vector3(0.3f, 0.25f, -0.25f * (level - 1));
            secondCannon.transform.localEulerAngles = new Vector3(0, 180.0f, 0);
        }
        

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

    void OnTriggerEnter(Collider other)
    {
        // Call ChangeLevel() when picking up a crate.
        if (other.gameObject.CompareTag("Crates"))
        {
            score += cratePoints;
            ChangeLevel();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
