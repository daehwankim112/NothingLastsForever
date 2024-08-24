using System.Collections;
using System.Collections.Generic;
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


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
