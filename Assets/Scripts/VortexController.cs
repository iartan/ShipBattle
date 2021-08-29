using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keep the vortex contantly rotating and destroys them after a few seconds.
/// </summary>
public class VortexController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {   
        // Self destruction after some seconds.
        Destroy(gameObject, 7.0f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Crates") || other.gameObject.CompareTag("Mine"))
        {
            other.transform.RotateAround(this.transform.position, Vector3.up, -100 * Time.deltaTime);
        }
    }

    private void OnCollisionEnter (Collision other)
    {
        Debug.Log("Collison with " + other.transform.name);
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Crates") || other.gameObject.CompareTag("Mine"))
        {
            other.transform.RotateAround(this.transform.position, Vector3.up, -100 * Time.deltaTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0.0f, -120.0f * Time.deltaTime, 0.0f, Space.Self);
    }
}
