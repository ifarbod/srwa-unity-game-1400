using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankFire : MonoBehaviour
{
    public GameObject shot;
    public Transform shotSpawn;
    private float fireRate = 3.73f;
    private float nextFire = 0.0f;

    private void Update()
    {
        if (Globals.isPlaying && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
            GameObject.FindGameObjectWithTag("onfiresound").GetComponent<AudioSource>().Play();
        }
    }
}
