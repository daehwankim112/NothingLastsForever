
[System.Serializable]
public struct Settings
{
    /// <summary>
    /// Difficulty settings
    /// </summary>
    # region Difficulty settings
    public float DifficultySlope;
    public float StartDifficulty;

    public float ActionThreshold;

    public float MinActionTimer;
    public float MaxActionTimer;

    public float GameStartGracePeriod;

    public bool AdaptiveDifficulty;
    public float AdaptiveDifficultyIdleTimeIncreaseLimit;
    public float AdaptiveDifficultySeconds;
    #endregion



    #region
    public float BoidWeight;
    public float BoidMaxSpawnRate;
    public float BoidMaxWaveContribution;


    public float ChestDifficultyValue;
    public float ChestTorpedoDifficultyValue;
    public float ChestHealthDifficultyValue;
    public int   ChestMaxSpawnPerWave;
    public float ChestMaxWaveContribution;


    public float CollectableDifficultyValue;
    public float CollectableTorpedoDifficultyValue;
    public float CollectableHealthDifficultyValue;
    #endregion
}