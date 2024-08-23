
using System;

using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// Death Event
    /// </summary>
    #region Death Event
    public class OnDeathArgs : EventArgs
    {
        public GameObject DeadThing;
        public Alliance DeadThingAlliance;
    }
    public static event EventHandler<OnDeathArgs> OnDeath;

    public static void Death(GameObject deadThing, Alliance alliance)
    {
        OnDeath?.Invoke(null, new OnDeathArgs { DeadThing = deadThing, DeadThingAlliance = alliance });
    }
    #endregion


    /// <summary>
    /// Explosion Event
    /// </summary>
    #region Explosion Event
    public class OnExplosionArgs : EventArgs
    {
        public Vector3 Position;
        public float Power;
        public Alliance ExplosionAlliance;
    }
    public static event EventHandler<OnExplosionArgs> OnExplosion;

    public static void Explosion(Vector3 position, float power, Alliance alliance)
    {
        OnExplosion?.Invoke(null, new OnExplosionArgs { Position = position, Power = power, ExplosionAlliance = alliance });
    }
    #endregion



    public enum Alliance
    {
        Player,
        Enemy
    }
}
