
[System.Serializable]
public struct Settings
{
    /// <summary>
    /// Difficulty settings
    /// </summary>
    # region Difficulty settings
    public float DifficultySlope;
    public float StartDifficulty;

    public float DifficulyProportionWeight;
    public float ActionMaxDifficulty;

    public float ActionThreshold;

    public float ActionTickPeriod;
    public float TickWaveChance;

    public float GameStartGracePeriod;

    public bool AdaptiveDifficulty;
    public float AdaptiveDifficultyIdleTimeIncreaseLimit;
    public float AdaptiveDifficultySeconds;
    #endregion


    public float PlayerDamageMultiplyer;
    public float BoidsAroundPlayerThreshhold;
    public float BoidsAroundPlayerDamage;
    public float BoidsAroundPlayerRadius;


    #region
    public float BoidWeight;
    public float BoidMaxSpawnRate;
    public float BoidMaxWaveContribution;
    public bool  BoidsRemovable;
    public int   BoidMax;


    public float ChestDifficultyValue;
    public float ChestTorpedoDifficultyValue;
    public float ChestHealthDifficultyValue;
    public int   ChestMaxSpawnPerWave;
    public float ChestMaxWaveContribution;
    public int ChestMax;
    public int ChestMaxTorpedoes;
    public int ChestMaxHealth;


    public float CollectableDifficultyValue;
    public float CollectableTorpedoDifficultyValue;
    public float CollectableHealthDifficultyValue;


    public float PlayerHealthDifficultyValue;
    public float PlayerTorpedoDifficultyValue;
    public float PlayerHealthThreshhold;
    public float PlayerTorpedoThreshhold;
    #endregion
}
