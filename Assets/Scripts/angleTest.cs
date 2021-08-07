using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class angleTest : MonoBehaviour
{
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 targetDir = target.transform.position - this.transform.position;
            float angle = Vector3.SignedAngle(targetDir, this.transform.forward, transform.up);

            print("The angle is: " + angle);
        }
    }
}
