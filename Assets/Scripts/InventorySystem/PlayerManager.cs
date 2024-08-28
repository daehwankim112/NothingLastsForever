
using System;

using UnityEngine;



public class PlayerManager : Singleton<PlayerManager>, IDifficultySensor
{
    public class OnPlayerDamageArgs : EventArgs
    {
        public int Damage;
    }
    public event EventHandler<OnPlayerDamageArgs> OnPlayerDamage;



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



    public float GetDifficulty()
    {
        int numTorpedos = PlayerInventory.NumTorpedos;
        int health = PlayerInventory.Health;

        return settings.PlayerTorpedoDifficultyValue * (settings.PlayerTorpedoThreshhold - numTorpedos) + settings.PlayerHealthDifficultyValue * (settings.PlayerHealthThreshhold - health);
    }



    private void OnExplosion(object sender, GameManager.OnExplosionArgs e)
    {
        if (e.ExplosionAlliance == GameManager.Alliance.Player) return;

        int damage = Mathf.FloorToInt(settings.PlayerDamageMultiplyer * e.Power / Vector3.SqrMagnitude(e.Position - player.transform.position));

        if (damage <= 0) return;

        OnPlayerDamage?.Invoke(null, new OnPlayerDamageArgs { Damage = damage });

        playerInventory.TakeHealth(damage, out bool playerDead);

        if (playerDead)
        {
            gameManager.Death(player, GameManager.Alliance.Player);
            gameManager.PlayerDead();
        }
    }
}
