using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Whole Health script copied from Jason Weimanns Healthbar Video.
/// The whole script is a bit of a mess and needs to be reviewed - TODO.
/// </summary>
public class Health : MonoBehaviour
{
    [SerializeField]
    public int maxHealth = 100;

    public int currentHealth;
    public GameObject crate;

    public event Action<float> OnHealthPctChanged = delegate { };

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public void ModifyHealth(int amount)
    {
        if (this.transform.root.gameObject.CompareTag("Player"))
        {
            // print("1 CurrentHealth is: " + currentHealth + ", and MaxHealth is: " + maxHealth);
        }

        currentHealth += amount;

        if (currentHealth > maxHealth)    // To prevent health to get higher than the maximum health.
        {
            // print("2 CurrentHealth is: " + currentHealth + ", and MaxHealth is: " + maxHealth);
            currentHealth = maxHealth;
        }
        
        float currentHealthPct = (float)currentHealth / (float)maxHealth;
        OnHealthPctChanged(currentHealthPct);

        if (this.transform.root.gameObject.CompareTag("Player"))    // Calling HealthOnTop to refresh Healthtext.
        {
            this.GetComponentInParent<PlayerMovement>().HealthOnTop();
            // print("Calling HealthonTop for Player");
        }
        else
        {
            this.GetComponentInParent<EnemyBot>().HealthOnTop();
            // print("Calling HealthonTop for Enemy");
        }
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            for (int i = 0; i < 3; i++) // Spawning crates after destruction.
            {
                var xz = UnityEngine.Random.insideUnitCircle * 0.5f;    // Random Vector2 position in a given radius.
                var newPosition = new Vector3(xz.x, 1, xz.y) + this.transform.position; // Converting Vector2 to Vector3 and adding 1 to the Y-Axis so the position is above earth.
                Instantiate(crate, newPosition, UnityEngine.Random.rotation);
            }
            this.transform.gameObject.SetActive(false); // TODO Should it be destroyed or only set to false?
            // Destroy(this.gameObject);
        }
    }
}
