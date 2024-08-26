
using System.Collections.Generic;
using UnityEngine;



public class CollectablesManager : Singleton<CollectablesManager>
{
    private GameManager gameManager => GameManager.Instance;

    public GameObject TorpedoCollectablePrefab;

    public Inventory PlayerInventory;

    public List<GameObject> Collectables;
    public List<GameObject> Chests;

    private readonly List<GameObject> stuffToRemove = new();



    void Start()
    {
        gameManager.OnDeath += OnDeath;

        if (TorpedoCollectablePrefab == null)
        {
            Debug.LogError("TorpedoCollectablePrefab is not set in the CollectablesManager");
        }
    }



    void LateUpdate()
    {
        foreach (GameObject thingToRemove in stuffToRemove)
        {
            bool removable = false;

            if (thingToRemove.TryGetComponent<Chest>(out _))
            {
                removable |= Chests.Remove(thingToRemove);
            }
            else if (thingToRemove.TryGetComponent<Collectable>(out _))
            {
                removable |= Collectables.Remove(thingToRemove);
            }

            if (removable)
            {
                Destroy(thingToRemove);
            }
        }

        stuffToRemove.Clear();
    }



    private void OnDestroy()
    {
        gameManager.OnDeath -= OnDeath;
    }



    public void CollectCollectable(GameObject collectable)
    {
        if (collectable.TryGetComponent<Inventory>(out Inventory inventory))
        {
            PlayerInventory.AddContents(inventory);
        }

        stuffToRemove.Add(collectable);
    }



    private void OnDeath(object sender, GameManager.OnDeathArgs deathArgs)
    {
        if (deathArgs.DeadThing.TryGetComponent<Inventory>(out Inventory inventory))
        {
            if (inventory.NumTorpedos > 0)
            {
                GameObject torpedoCollectable = Instantiate(TorpedoCollectablePrefab, deathArgs.DeadThing.transform.position, Quaternion.identity);
                torpedoCollectable.GetComponent<Inventory>().AddContents(inventory);

                Collectables.Add(torpedoCollectable);
            }

            if (deathArgs.DeadThing.TryGetComponent<Chest>(out _))
            {
                stuffToRemove.Add(deathArgs.DeadThing);
            }
        }
    }
}
