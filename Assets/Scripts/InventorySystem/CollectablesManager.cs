
using UnityEngine;



public class CollectablesManager : Singleton<CollectablesManager>
{
    private GameManager gameManager => GameManager.Instance;



    void Start()
    {
        gameManager.OnDeath += OnDeath;
    }



    private void OnDestroy()
    {
        gameManager.OnDeath -= OnDeath;
    }



    private void OnDeath(object sender, GameManager.OnDeathArgs deathArgs)
    {
        if (!deathArgs.DeadThing.TryGetComponent<Inventory>(out Inventory inventory))
        {
            Debug.Log($"CollectablesManager: {deathArgs.DeadThing.name} died, dropping {inventory.NumTorpedos} torpedos");
        }
    }
}
