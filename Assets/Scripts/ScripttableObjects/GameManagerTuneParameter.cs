using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameManagerTuneParameter", order = 1)]
public class GameManagerTuneParameter : ScriptableObject
{
    public float DifficultySlope = 0.5f;
    public float StartDifficulty = 100f;

    public float DifficulyProportionWeight = 0.5f;
    public float ActionMaxDifficulty = 1000f;

    public float ActionThreshold = 20f;

    public float ActionTickPeriod = 1f;
    public float TickWaveChance = 0.5f;

    public float GameStartGracePeriod = 5f;

    public bool AdaptiveDifficulty = false;
    public float AdaptiveDifficultyIdleTimeIncreaseLimit = 0f;
    public float AdaptiveDifficultySeconds = 0f;
    
    public float PlayerDamageMultiplyer = 1f;
    public float BoidsAroundPlayerThreshhold = 10f;
    public float BoidsAroundPlayerDamage = 0.2f;
    public float BoidsAroundPlayerRadius = 0.5f;

    public float BoidWeight = 1f;
    public float BoidMaxSpawnRate = 1000f;
    public float BoidMaxWaveContribution = 0.5f;
    public bool  BoidsRemovable = false;
    public int   BoidMax = 300;

    public float ChestDifficultyValue = 0.5f;
    public float ChestTorpedoDifficultyValue = 2f;
    public float ChestHealthDifficultyValue = 1f;
    public int   ChestMaxSpawnPerWave = 2;
    public float ChestMaxWaveContribution = 1f;
    public int ChestMax = 10;
    public int ChestMaxTorpedoes = 10;
    public int ChestMaxHealth = 50;

    public float CollectableDifficultyValue = 1f;
    public float CollectableTorpedoDifficultyValue = 2f;
    public float CollectableHealthDifficultyValue = 1f;

    public float PlayerHealthDifficultyValue = 10f;
    public float PlayerTorpedoDifficultyValue = 5f;
    public float PlayerHealthThreshhold = 40f;
    public float PlayerTorpedoThreshhold = 6f;

    public float SubDifficultyValue = 10f;
}
