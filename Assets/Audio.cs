using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource BGMSource;
    [SerializeField] AudioSource SFXSource;
    

    [Header("Audio Clips")]
    public AudioClip background;
    public AudioClip collection;
    public AudioClip loss;


    private void Start()
    {
        BGMSource.clip = background;
        BGMSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
