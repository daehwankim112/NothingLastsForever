
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

    /// <summary>
    /// Difficulty Weights
    /// </summary>
    #region
    public float BoidWeight;
    #endregion
}
