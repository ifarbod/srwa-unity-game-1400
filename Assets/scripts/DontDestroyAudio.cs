using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyAudio : MonoBehaviour
{
    public AudioClip[] musics;

    private int RandomInt(int min, int max)
    {
        var delta = max - min;
        return System.Convert.ToInt32(min + (Random.value * delta));
    }

    private void Awake()
    {
        int random = RandomInt(1, 6);
        GetComponent<AudioSource>().clip = musics[random];
        GetComponent<AudioSource>().Play();
        DontDestroyOnLoad(transform.gameObject);
    }
}
