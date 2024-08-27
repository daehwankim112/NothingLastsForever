
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class CollectablesManager : Singleton<CollectablesManager>, IDifficultySensor
{
    private GameManager gameManager => GameManager.Instance;
    private Settings settings => gameManager.Settings;

    public GameObject TorpedoCollectablePrefab;

    private Inventory playerInventory => PlayerManager.Instance.PlayerInventory;

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



    void OnDestroy()
    {
        gameManager.OnDeath -= OnDeath;
    }



    public void CollectCollectable(GameObject collectable)
    {
        if (collectable.TryGetComponent<Inventory>(out Inventory inventory))
        {
            playerInventory.AddContents(inventory);
        }

        stuffToRemove.Add(collectable);
    }



    public void AddChest(GameObject chest)
    {
        Chests.Add(chest);
    }



    public float GetDifficulty()
    {
        float chestDifficulty = Chests.Sum(chest => settings.ChestDifficultyValue
                                    - (settings.ChestTorpedoDifficultyValue * chest.GetComponent<Inventory>().NumTorpedos)
                                    - (settings.ChestHealthDifficultyValue * chest.GetComponent<Inventory>().Health));

        float collectableDifficulty = Collectables.Sum(collectable => settings.CollectableDifficultyValue
                                    - (settings.CollectableTorpedoDifficultyValue * collectable.GetComponent<Inventory>().NumTorpedos)
                                    - (settings.CollectableHealthDifficultyValue * collectable.GetComponent<Inventory>().Health));

        return chestDifficulty + collectableDifficulty;
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
