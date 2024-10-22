
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class CollectablesManager : Singleton<CollectablesManager>, IDifficultySensor
{
    private GameManager gameManager => GameManager.Instance;
    private GameManagerTuneParameter settings => gameManager.Settings;
    // private Settings settings => gameManager.Settings;

    private Inventory playerInventory => PlayerManager.Instance.PlayerInventory;

    [SerializeField]
    private readonly List<GameObject> collectables = new();
    public int NumCollectables => collectables.Count;

    [SerializeField]
    private readonly List<GameObject> chests = new();
    public int NumChests => chests.Count;

    private readonly Queue<GameObject> stuffToRemove = new();

    public GameObject TorpedoCollectablePrefab;


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
        while (stuffToRemove.TryDequeue(out GameObject thingToRemove))
        {
            bool removable = false;

            if (thingToRemove == null) continue;

            if (thingToRemove.TryGetComponent<Chest>(out _))
            {
                removable |= chests.Remove(thingToRemove);
            }
            else if (thingToRemove.TryGetComponent<Collectable>(out _))
            {
                removable |= collectables.Remove(thingToRemove);
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
        if (GameManager.InstanceExists)
        {
            gameManager.OnDeath -= OnDeath;
        }
    }



    public void CollectCollectable(GameObject collectable)
    {
        if (collectable.TryGetComponent<Inventory>(out Inventory inventory))
        {
            playerInventory.MergeContents(inventory);
        }

        stuffToRemove.Enqueue(collectable);
    }



    public void AddChest(GameObject chest)
    {
        if (NumChests < settings.ChestMax && chest != null)
        {
            chests.Add(chest);
        }
    }

    public void AddCollectable(GameObject collectable)
    {
        if (NumCollectables < settings.ChestMax && collectable != null)
        {
            collectables.Add(collectable);
        }
    }



    public float GetDifficulty()
    {
        /*float chestDifficulty = chests.Sum(chest => settings.ChestDifficultyValue
                                    - (settings.ChestTorpedoDifficultyValue * chest.GetComponent<Inventory>().NumTorpedos)
                                    - (settings.ChestHealthDifficultyValue * chest.GetComponent<Inventory>().Health));*/

        float collectableDifficulty = collectables.Sum(collectable => settings.CollectableDifficultyValue
                                    - (settings.CollectableTorpedoDifficultyValue)
                                    - (settings.CollectableHealthDifficultyValue));

        // return chestDifficulty + collectableDifficulty;
        return collectableDifficulty;
    }



    private void OnDeath(object sender, GameManager.OnDeathArgs deathArgs)
    {
        if (deathArgs.DeadThing.TryGetComponent<Inventory>(out Inventory inventory))
        {
            if (inventory.NumTorpedos > 0)
            {
                GameObject torpedoCollectable = Instantiate(TorpedoCollectablePrefab, deathArgs.DeadThing.transform.position, Quaternion.identity);
                torpedoCollectable.GetComponent<Inventory>().MergeContents(inventory);

                AddCollectable(torpedoCollectable);
            }

            if (deathArgs.DeadThing.TryGetComponent<Chest>(out _))
            {
                stuffToRemove.Enqueue(deathArgs.DeadThing);
            }
        }
    }
}
