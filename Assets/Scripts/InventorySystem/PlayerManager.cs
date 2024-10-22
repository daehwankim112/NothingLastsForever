
using System;

using UnityEngine;



public class PlayerManager : Singleton<PlayerManager>, IDifficultySensor
{
    public class OnPlayerDamageArgs : EventArgs
    {
        public float Damage;
    }
    public event EventHandler<OnPlayerDamageArgs> OnPlayerDamage;



    private GameManager gameManager => GameManager.Instance;
    private GameManagerTuneParameter settings => gameManager.Settings;
    // private Settings settings => gameManager.Settings;

    [SerializeField]
    private Inventory playerInventory;
    public Inventory PlayerInventory => playerInventory;

    [SerializeField]
    private GameObject player;
    public GameObject Player => player;

    public int NumBoidsAroundPlayer = 0;



    void Start()
    {
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory is not set in the PlayerManager");

            if (TryGetComponent<Inventory>(out playerInventory))
            {
                Debug.LogWarning("PlayerInventory was automatically set in the PlayerManager, please set in the editor to ensure the correct inventory is selected");
            }
            else
            {
                Debug.LogWarning("PlayerInventory could not be found on the PlayerManager please set it in the editor");
            }
        }

        if (player == null)
        {
            Debug.LogError("Player is not set in the PlayerManager");
        }

        gameManager.OnExplosion += OnExplosion;
    }



    void LateUpdate()
    {
        if (NumBoidsAroundPlayer > settings.BoidsAroundPlayerThreshhold)
        {
            playerInventory.TakeHealth(settings.BoidsAroundPlayerDamage * NumBoidsAroundPlayer * Time.deltaTime, out bool playerDead);

            if (playerDead)
            {
                gameManager.Death(player, GameManager.Alliance.Player);
                gameManager.PlayerDead();
            }
        }
    }



    void OnDestroy()
    {
        if (GameManager.InstanceExists)
        {
            gameManager.OnExplosion -= OnExplosion;
        }
    }



    public float GetDifficulty()
    {
        int numTorpedos = PlayerInventory.NumTorpedos;
        float health = PlayerInventory.Health;

        float torpedoDifficulty = settings.PlayerTorpedoDifficultyValue * Mathf.Max(settings.PlayerTorpedoThreshhold - numTorpedos, 0.0f);
        float healthDifficulty = settings.PlayerHealthDifficultyValue * Mathf.Max(settings.PlayerHealthThreshhold - health, 0.0f);

        return torpedoDifficulty + healthDifficulty;
    }



    private void OnExplosion(object sender, GameManager.OnExplosionArgs e)
    {
        if (e.ExplosionAlliance == GameManager.Alliance.Player) return;

        float damage = settings.PlayerDamageMultiplyer * e.Power / Vector3.SqrMagnitude(e.Position - player.transform.position);

        if (damage <= 0 || damage > 10.0f) return;

        OnPlayerDamage?.Invoke(null, new OnPlayerDamageArgs { Damage = damage });

        playerInventory.TakeHealth(damage, out bool playerDead);

        if (playerDead)
        {
            gameManager.Death(player, GameManager.Alliance.Player);
            gameManager.PlayerDead();
        }
    }
}
