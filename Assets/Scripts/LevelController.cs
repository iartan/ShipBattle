using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The management of the leveling system for the ships and the score.
/// As a workaround to separate the cannons on different decks, I am giving them tags like "Deck1", "Deck2" etc. but It is not easy
/// to create tags from the script so I manually created 10 deck tags in the Unity Editor and that should do for a while.
/// </summary>
public class LevelController : MonoBehaviour
{
    // Initial variables.
    int score;
    // Make the level variable public to get access from other scripts.
    public int level;
    private int decks = 1;
    int levelStep = 35;    // TODO - Find better variable name for this.
    int deckStep = 5;
    public int currentDeck = 1;  // Helper variable to decide on which deck to spawn the cannon.
    int cannonPos = 1;
    float scaleChange = 3.0f;
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
        // Check if a new deck is needed.
        if (level >= decks * deckStep)
        {
            Debug.Log("New deck needed.");
            decks++;
        }

        // If a new deck appeared, a new variable is need to keep track.
        if (currentDeck < decks)
        {
            Debug.Log("Deck swaping needed.");
            currentDeck++;
            cannonPos = 1;
        }
        // Check depending on the score if the ship is ready to level.
        if (score >= level * levelStep)
        {
            level++;
            cannonPos++;
            // On each level the ship gets a new max health and some 100 health points.
            this.GetComponent<Health>().maxHealth = level * 50 + 50;
            // this.GetComponent<Health>().currentHealth = level * 100;
            this.GetComponent<Health>().ModifyHealth(100);

            // Scale the ship up each 3 levels.
            if (true)
            {
                hull.transform.localScale += new Vector3(scaleChange, scaleChange * 1.5f, scaleChange);
                // Move the ship in the up direction, because scale happens on boths sides the ships sinks too much in the water.
                hull.transform.position += new Vector3(0f, scaleChange * 1.5f / 100.0f, 0f);
            }

            // Move all cannons which are inside Cannons a bit to the front to make place for the new cannons.
            foreach (Transform child in cannonSystem.transform)
            {   
                if (child.gameObject.CompareTag("Deck" + currentDeck))
                {
                    Vector3 offset = new Vector3(0f, 0f, 0.25f);
                    child.transform.localPosition += offset;
                }

                // When the ship grows bigger, the cannons are more and more hidden inside the hull, therefore we need to pull them out on the local X-axis each new level.
                if (child.gameObject.name.StartsWith("Right"))
                {
                    Vector3 offsetX = new Vector3(0.05f, 0f, 0f);
                    child.transform.localPosition += offsetX;
                }
                else
                {
                    Vector3 offsetX = new Vector3(-0.05f, 0f, 0f);
                    child.transform.localPosition += offsetX;
                }
            }

            // Add a new cannon on both sides.
            GameObject cannon = Instantiate(cannonPrefab, new Vector3(this.transform.localPosition.x - 0.4f, this.transform.localPosition.y + 1.0f, this.transform.localPosition.z), this.transform.rotation);
            cannon.transform.SetParent(cannonSystem.transform);
            cannon.name = "LeftCannon" + level;
            cannon.tag = "Deck" + currentDeck;
            cannon.transform.localPosition = new Vector3(-0.3f - (level * 0.05f), 0.5f * currentDeck - 0.25f, -0.25f * (cannonPos - 1));
            cannon.transform.localEulerAngles = new Vector3(0, 0, 0);
            GameObject secondCannon = Instantiate(cannonPrefab, new Vector3(this.transform.position.x + 0.4f, this.transform.position.y + 0.25f, this.transform.position.z), Quaternion.identity);
            secondCannon.transform.SetParent(cannonSystem.transform);
            secondCannon.name = "RightCannon" + level;
            secondCannon.tag = "Deck" + currentDeck;
            secondCannon.transform.localPosition = new Vector3(0.3f + (level * 0.05f), 0.5f * currentDeck - 0.25f, -0.25f * (cannonPos - 1));
            secondCannon.transform.localEulerAngles = new Vector3(0, 180.0f, 0);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        // Call ChangeLevel() when picking up a crate.
        if (other.gameObject.CompareTag("Crates"))
        {
            score += cratePoints;
            ChangeLevel();
            Destroy(other.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // There is a mess with all the trigger and collider stuff going on, the bots handle those different and the player ship different,
        // thats the reason I need to create this helper method here for the player to be able to pick up crates.
        if (this.CompareTag("Player") && other.gameObject.CompareTag("Crates"))
        {
            score += cratePoints;
            ChangeLevel();
            Destroy(other.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
