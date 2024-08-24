
using UnityEngine;



public class CollectablesManager : Singleton<CollectablesManager>
{
    private GameManager gameManager => GameManager.Instance;


    public GameObject TorpedoCollectablePrefab;



    void Start()
    {
        gameManager.OnDeath += OnDeath;

        if (TorpedoCollectablePrefab == null)
        {
            Debug.LogError("TorpedoCollectablePrefab is not set in the CollectablesManager");
        }
    }



    private void OnDestroy()
    {
        gameManager.OnDeath -= OnDeath;
    }



    private void OnDeath(object sender, GameManager.OnDeathArgs deathArgs)
    {
        if (deathArgs.DeadThing.TryGetComponent<Inventory>(out Inventory inventory))
        {
            if (inventory.NumTorpedos > 0)
            {
                GameObject torpedoCollectable = Instantiate(TorpedoCollectablePrefab, deathArgs.DeadThing.transform.position, Quaternion.identity);
                torpedoCollectable.GetComponent<Inventory>().NumTorpedos = inventory.NumTorpedos;
            }
        }
    }
}
