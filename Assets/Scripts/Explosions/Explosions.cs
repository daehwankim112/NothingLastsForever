
using UnityEngine;

public class Explosions : Singleton<Explosions>
{
    public delegate void ExplosionEvent(Vector3 position, float power, GameManager.Alliance explosionAlliance);
}
