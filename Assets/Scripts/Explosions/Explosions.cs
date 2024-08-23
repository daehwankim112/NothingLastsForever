using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TorpedoManager;

public static class Explosions
{
    public delegate void ExplosionEvent(Vector3 position, float power, ExplosionAlliance explosionAlliance);


    public enum ExplosionAlliance
    {
        Player,
        Enemy
    }
}
