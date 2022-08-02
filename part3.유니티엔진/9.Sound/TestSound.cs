using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSound : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public AudioClip audioClip;
    private void OnTriggerEnter(Collider other)
    {

        //AudioSource audio = GetComponent<AudioSource>();
        //audio.PlayOneShot(audioClip);

        Managers.Sound.Play(audioClip);
    }
}
