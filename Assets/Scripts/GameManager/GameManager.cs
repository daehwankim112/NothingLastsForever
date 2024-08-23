
using UnityEngine;
using static Explosions;

public class GameManager : Singleton<GameManager>
{
    public delegate void DeathEvent(GameObject deadThing, Alliance alliance);


    public enum Alliance
    {
        Player,
        Enemy
    }
}
