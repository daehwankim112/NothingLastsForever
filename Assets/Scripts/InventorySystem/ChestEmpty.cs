
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

using UnityEngine;



public class ChestEmpty : MonoBehaviour
{
    private GameManager gameManager => GameManager.Instance;


    void Start()
    {
        GetCollected();
    }

    [ContextMenu("Get Collected")]
    private void GetCollected()
    {
        gameManager.Death(gameObject, GameManager.Alliance.Enemy);
    }
}
