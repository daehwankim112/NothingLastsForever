
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerManager : Singleton<PlayerManager>, IDifficultySensor
{
    private GameManager gameManager => GameManager.Instance;
    private Settings settings => gameManager.Settings;

    [SerializeField]
    private Inventory playerInventory;
    public Inventory PlayerInventory => playerInventory;

    [SerializeField]
    private GameObject player;
    public GameObject Player => player;



    void Start()
    {
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory is not set in the PlayerManager");
        }

        if (player == null)
        {
            Debug.LogError("Player is not set in the PlayerManager");
        }
    }



    public float GetDifficulty()
    {
        int numTorpedos = PlayerInventory.NumTorpedos;
        int health = PlayerInventory.Health;

        return settings.PlayerTorpedoDifficultyValue * (settings.PlayerTorpedoThreshhold - numTorpedos) + settings.PlayerHealthDifficultyValue * (settings.PlayerHealthThreshhold - health);
    }
}
