using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySubmarineController : MonoBehaviour
{
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public enum SubmarineState
    {
        GETINROOM,
        ROTATEAROUNDCENTRE,
        SONARPING,
        FIRETORPEDO,
        APPROACHPLAYER,
        CHASEPLAYER,
        EXPLODES
    }
    [SerializeField] private AudioClip SubmarineSonarSound;
    [SerializeField] private AudioClip TorpedoFiringSound;
    [SerializeField] private AudioClip SubmarineExplosionSound;
    private SubmarineState submarineState;
    private bool isSubmarineChasingPlayer = false;
    private float timeSinceLastSonarPing = 0;
    private float timeSinceLastTorpedoFired = 0;
    private bool keepTrackOfSubmarineSonarTime = false;
    private bool keepTrackOfSubmarineTorpedoTime = false;

    public void SetState(SubmarineState state)
    {
        submarineState = state;
    }

    public SubmarineState GetState()
    {
        return submarineState;
    }

    public void PlaySonarSound()
    {
        audioSource.PlayOneShot(SubmarineSonarSound);
    }

    public void PlayTorpedoFiringSound()
    {
        audioSource.PlayOneShot(TorpedoFiringSound);
    }

    public void PlayExplosionSound()
    {
        audioSource.PlayOneShot(SubmarineExplosionSound);
    }

    public bool GetSubmarineSonarIsTrackingTime()
    {
        return keepTrackOfSubmarineSonarTime;
    }

    public void SetSubmarineSonarTrackingTime(bool value)
    {
        keepTrackOfSubmarineSonarTime = value;
    }

    public bool GetSubmarineTorpedoIsTrackingTime()
    {
        return keepTrackOfSubmarineTorpedoTime;
    }

    public void SetSubmarineTorpedoTrackingTime(bool value)
    {
        keepTrackOfSubmarineTorpedoTime = value;
    }

    public void SonarPinged()
    {
        timeSinceLastSonarPing = 0;
    }

    public void FiredTorpedoSound()
    {
        timeSinceLastTorpedoFired = 0;
    }

    public float GetTimeSinceLastSonarPing()
    {
        return timeSinceLastSonarPing;
    }

    public float GetTimeSinceLastTorpedoFired()
    {
        return timeSinceLastTorpedoFired;
    }



    void Start()
    {
        
    }

    void Update()
    {
        if (keepTrackOfSubmarineSonarTime)
        {
            timeSinceLastSonarPing += Time.deltaTime;
        }
        else
        {
            timeSinceLastSonarPing = 0;
        }
        if (keepTrackOfSubmarineTorpedoTime)
        {
            timeSinceLastTorpedoFired += Time.deltaTime;
        }
        else
        {
            timeSinceLastTorpedoFired = 0;
        }
    }
}
