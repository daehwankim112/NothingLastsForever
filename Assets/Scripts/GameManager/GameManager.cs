
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
    #region Echo Event
    public class OnEchoArgs : EventArgs
    {
    }
    public event EventHandler<OnEchoArgs> OnEcho;

    public void Echo()
    {
        OnEcho?.Invoke(null, new OnEchoArgs());
    }
    #endregion


    /// <summary>
    /// Wave Event
    /// </summary>
    #region Wave Event
    public class OnWaveArgs : EventArgs
    {
        public float DifficultyDelta;
    }
    public event EventHandler<OnWaveArgs> OnWave;

    private void Wave(float difficultyDelta)
    {
        OnWave?.Invoke(null, new OnWaveArgs { DifficultyDelta = difficultyDelta });
    }
    #endregion


    /// <summary>
    /// MRUK Event
    /// </summary>
    #region MRUK Event
    public class OnMrukCreatedArgs : EventArgs
    {
    }
    public event EventHandler<OnMrukCreatedArgs> OnMruk;

    public void MrukCreatedEvent()
    {
        mrukRoomCreated = true;
        OnMruk?.Invoke(null, new OnMrukCreatedArgs());
    }
    #endregion



    /// <summary>
    /// Game Playing Event
    /// </summary>
    #region Play Event
    public class OnPlayArgs : EventArgs
    {
    }
    public event EventHandler<OnPlayArgs> OnPlay;

    public void Play()
    {
        currentGameState = GameState.Playing;

        OnPlay?.Invoke(null, new OnPlayArgs());
    }
    #endregion



    /// <summary>
    /// Game Pause Event
    /// </summary>
    #region Pause Event
    public class OnPauseArgs : EventArgs
    {
    }
    public event EventHandler<OnPauseArgs> OnPause;

    public void Pause()
    {
        currentGameState = GameState.Paused;

        OnPause?.Invoke(null, new OnPauseArgs());
    }
    #endregion



    /// <summary>
    /// Game Over Event
    /// </summary>
    #region Game Over Event
    public class OnGameOverArgs : EventArgs
    {
    }
    public event EventHandler<OnGameOverArgs> OnGameOver;

    public void EndGame()
    {
        currentGameState = GameState.GameOver;

        OnGameOver?.Invoke(null, new OnGameOverArgs());
    }

    public void PlayerDead()
    {
        EndGame();
    }
    #endregion



    /// <summary>
    /// Game Start Event
    /// </summary>
    #region Game Start Event
    public class OnStartArgs : EventArgs
    {
    }
    public event EventHandler<OnStartArgs> OnStart;

    public void StartGame()
    {
        currentGameState = GameState.Playing;

        OnStart?.Invoke(null, new OnStartArgs());
    }
    #endregion



    public enum Alliance
    {
        Player,
        Enemy
    }



    public enum GameState
    {
        Playing,
        Paused,
        GameOver
    }



    [SerializeField]
    private GameState currentGameState;
    public GameState CurrentGameState { get => currentGameState; }

    [SerializeField]
    private DifficultyController difficultyController;

    [SerializeField]
    private Settings settings;
    public Settings Settings { get => settings; }

    bool mrukRoomCreated = false;



    void Start()
    {
        if (difficultyController == null)
        {
            Debug.LogError("Difficulty Controller is not set in GameManager");
            difficultyController = gameObject.AddComponent<DifficultyController>();
        }

        StartGame();
    }



    void Update()
    {
        if (currentGameState == GameState.Playing)
        {
            float controlSignal = difficultyController.CalculateControlSignal();

            if (controlSignal != 0.0f)
            {
                Wave(controlSignal);
            }
        }

        if (!mrukRoomCreated && Time.time > 10.0f)
        {
            Debug.LogWarning("MRUK Room not created within 10 seconds of game start. Is the event connected to GameManager?");
        }
    }



    public void LoadSettingsFromFile(string path)
    {
        settings = JsonUtility.FromJson<Settings>(path);
    }



    public void SaveSettingsToFile(string path)
    {
        string json = JsonUtility.ToJson(settings);

        System.IO.File.WriteAllText(path, json);
    }
}