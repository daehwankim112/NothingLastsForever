
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerManager : Singleton<PlayerManager>, IDifficultySensor
{
    private GameManager gameManager => GameManager.Instance;
    private Settings settings => gameManager.Settings;

    public Inventory PlayerInventory;


    public float GetDifficulty()
    {
        int numTorpedos = PlayerInventory.NumTorpedos;
        int health = PlayerInventory.Health;

        return settings.PlayerTorpedoDifficultyValue * (settings.PlayerTorpedoThreshhold - numTorpedos) + settings.PlayerHealthDifficultyValue * (settings.PlayerHealthThreshhold - health);
    }
}
