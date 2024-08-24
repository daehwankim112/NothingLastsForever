using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySubmarineController : MonoBehaviour
{
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


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
