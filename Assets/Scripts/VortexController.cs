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

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0.0f, -120.0f * Time.deltaTime, 0.0f, Space.Self);
    }
}
