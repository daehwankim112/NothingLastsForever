
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
    public event EventHandler<OnDeathArgs> OnDeath;

    public void Death(GameObject deadThing, Alliance alliance)
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
    public event EventHandler<OnExplosionArgs> OnExplosion;

    public void Explosion(Vector3 position, float power, Alliance alliance)
    {
        OnExplosion?.Invoke(null, new OnExplosionArgs { Position = position, Power = power, ExplosionAlliance = alliance });
    }
    #endregion

    /// <summary>
    /// Player Echo Event
    /// </summary>
    #region Explosion Event
    public class OnEchoArgs : EventArgs
    {
    }
    public event EventHandler<OnEchoArgs> OnEcho;

    public void Echo()
    {
        OnEcho?.Invoke(null, new OnEchoArgs());
    }
    #endregion



    public enum Alliance
    {
        Player,
        Enemy
    }
}
