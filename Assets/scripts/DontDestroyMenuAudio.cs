using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyMenuAudio : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
}
