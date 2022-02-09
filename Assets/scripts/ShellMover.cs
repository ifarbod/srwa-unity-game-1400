using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellMover : MonoBehaviour
{
    private int speed = 3;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }
}
