
using UnityEngine;

public class ThingThatDies : MonoBehaviour
{
    public float DeathTimer;


    void Update()
    {
        DeathTimer -= Time.deltaTime;

        if (DeathTimer <= 0)
        {
            GameManager.Instance.Death(gameObject, GameManager.Alliance.Player);

            Destroy(gameObject);
        }
    }
}
