using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Impulser : MonoBehaviour
{
    public AudioClip _walk1Audio;
    public AudioClip _walk2Audio;
    public AudioClip _shotAudio;
    public CinemachineImpulseSource _impulse;
    public AudioSource _audioSource;

    public void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void GenerateImpulse()
    {
        _impulse.GenerateImpulseWithForce(5);
    }

    public void PlayWalk1Audio()
    {
        _audioSource.PlayOneShot(_walk1Audio);
    }
    public void PlayWalk2Audio()
    {
        _audioSource.PlayOneShot(_walk2Audio);
    }
    public void PlayShotAudio()
    {
        _audioSource.PlayOneShot(_shotAudio);
    }
}
