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

    // Drag seamines around on trigger stay.
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Mine"))
        {
            other.transform.RotateAround(this.transform.position, Vector3.up, -100 * Time.deltaTime);
        }
    }

    // Drag crates around on collision stay.
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Crates"))
        {
            other.transform.RotateAround(this.transform.position, Vector3.up, -100 * Time.deltaTime);
        }
    }

    // Update is called once per frame
    void Update()
    {   
        // Rotate the Vortex around self.
        this.transform.Rotate(0.0f, -120.0f * Time.deltaTime, 0.0f, Space.Self);
        // Workaround orce a 0.0f position at y-axis here, because bots place it always a bit over 0.0f.
        var pos = transform.position;
        pos.y = 0.0f;
        transform.position = pos;
    }
}
