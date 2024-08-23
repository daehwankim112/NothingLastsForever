
using UnityEngine;
using static TorpedoManager;

public static class Explosions
{
    public delegate void ExplosionEvent(Vector3 position, float power, GameManager.Alliance explosionAlliance);
}
