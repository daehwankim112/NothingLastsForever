
using Meta.XR.MRUtilityKit;

using System.Collections.Generic;

using UnityEngine;



public class ChestSpawner : Singleton<ChestSpawner>
{
    [SerializeField] private bool isEmptyChest;
    private GameManager gameManager => GameManager.Instance;
    private GameManagerTuneParameter settings => gameManager.Settings;
    // private Settings settings => gameManager.Settings;
    private CollectablesManager collectablesManager => CollectablesManager.Instance;
    private int numChests => collectablesManager.NumChests;


    public GameObject ChestPrefab;

    public float ChestSize = 0.5f;

    private MRUKAnchor floorAnchor = null;
    private MRUKAnchor ceilingAnchor = null;
    private Vector2 floorSize;
    private Vector2 ceilingSize;
    private List<Vector2> floorPlaneBoundry;
    private List<Vector2> ceilingPlaneBoundry;
    private MRUKRoom mrukRoom;

    private MRUK mruk => MRUK.Instance;


    void Start()
    {
        gameManager.OnWave += OnWave;
        gameManager.OnMruk += MrukRoomCreatedEvent;

        if (ChestPrefab == null)
        {
            Debug.LogError("Chest prefab is not set in ChestSpawner script. Disabling script.");
            enabled = false;
        }
    }


    private void OnDestroy()
    {
        if (GameManager.InstanceExists)
        {
            gameManager.OnWave -= OnWave;
            gameManager.OnMruk -= MrukRoomCreatedEvent;
        }
    }



    private Vector3 GetRandomSpawnLocation()
    {
        if (isEmptyChest)
        {
            if (ceilingAnchor == null)
            {
                Debug.LogError("No ceiling anchor found in ChestSpawner script! Have you connected the MRUK events");
                return Vector3.zero;
            }
            
            Vector3 randomPoint = new Vector3();

            for (int attempts = 0; attempts < 20; attempts++)
            {
                mrukRoom.GenerateRandomPositionOnSurface(MRUK.SurfaceType.FACING_DOWN, ChestSize, LabelFilter.Included(MRUKAnchor.SceneLabels.CEILING), out randomPoint, out Vector3 normal);

                randomPoint += normal * 0.2f;
                Debug.Log("chest spawned at a random Point: " + randomPoint);

                if (mrukRoom.IsPositionInRoom(randomPoint) && !mrukRoom.IsPositionInSceneVolume(randomPoint))
                {
                    return randomPoint;
                }
            }
            
            return ceilingAnchor.GetAnchorCenter();

        }
        else
        {
            if (floorAnchor == null)
            {
                Debug.LogError("No floor anchor in chest spawner! Have you connected the MRUK events?");
                return Vector3.zero;
            }

            Vector3 randomPoint = new(1000.0f, 1000.0f, 1000.0f);

            for (int attempts = 0; attempts < 20; attempts++)
            {
                mrukRoom.GenerateRandomPositionOnSurface(MRUK.SurfaceType.FACING_UP, ChestSize, LabelFilter.Excluded(MRUKAnchor.SceneLabels.CEILING), out randomPoint, out Vector3 normal);

                randomPoint += normal * 0.2f;

                if (mrukRoom.IsPositionInRoom(randomPoint) && !mrukRoom.IsPositionInSceneVolume(randomPoint))
                {
                    return randomPoint;
                }
            }
            return floorAnchor.GetAnchorCenter();
        }
    }



    private GameObject SpawnChest(int numTorpedos = 0, int health = 0)
    {
        GameObject newChest;
        if (isEmptyChest)
        {
            newChest = Instantiate(ChestPrefab, GetRandomSpawnLocation(), Quaternion.AngleAxis(360.0f * Random.value, Vector3.up));
            newChest.GetComponent<Inventory>().Initialize(numTorpedos, health, 1, settings.ChestMaxHealth);
        }
        else
        {
            newChest = Instantiate(ChestPrefab, GetRandomSpawnLocation(), Quaternion.AngleAxis(360.0f * Random.value, Vector3.up));
            newChest.GetComponent<Inventory>().Initialize(numTorpedos, health, settings.ChestMaxTorpedoes, settings.ChestMaxHealth);
            collectablesManager.AddChest(newChest);
        }


        return newChest;
    }



    private List<GameObject> SpawnChests(int numChests)
    {
        List<GameObject> newChests = new(numChests);

        for (int i = 0; i < numChests && i + collectablesManager.NumChests < settings.ChestMax; i++)
        {
            newChests.Add(SpawnChest());
        }

        return newChests;
    }



    private void DistributeTorpedoesAndHealth(float difficulty, List<GameObject> chests)
    {
        int numChests = chests.Count;

        if (numChests == 0) return;

        // Ensure that each chest has at least one item
        foreach (GameObject chest in chests)
        {
            AddRandomToChest(chest);
        }

        // Distribute the remaining items randomly
        while (difficulty > float.Epsilon)
        {
            int randomChestIndex = Random.Range(0, numChests);
            AddRandomToChest(chests[randomChestIndex]);
            //Debug.Log($"Difficulty left: {difficulty}");
        }

        void AddRandomToChest(GameObject chest)
        {
            if (difficulty > settings.ChestTorpedoDifficultyValue && difficulty > settings.ChestHealthDifficultyValue)
            {
                if (Random.value < 0.7f)
                {
                    chest.GetComponent<Inventory>().AddTorpedoes(1);
                    difficulty -= settings.ChestTorpedoDifficultyValue;
                }
                else
                {
                    chest.GetComponent<Inventory>().AddHealth(1.0f);
                    difficulty -= settings.ChestHealthDifficultyValue;
                }
            }
            else if (difficulty > settings.ChestTorpedoDifficultyValue)
            {
                chest.GetComponent<Inventory>().AddTorpedoes(1);
                difficulty -= settings.ChestTorpedoDifficultyValue;
            }
            else
            {
                chest.GetComponent<Inventory>().AddHealth(difficulty / settings.ChestHealthDifficultyValue);

                //Debug.Log($"Added health to chest: {difficulty / settings.ChestHealthDifficultyValue}, with remaining difficulty {difficulty} and health value {settings.ChestHealthDifficultyValue}");
                difficulty = -1.0f;
            }
        }
    }



    private void MrukRoomCreatedEvent(object sender, GameManager.OnMrukCreatedArgs args)
    {
        mrukRoom = mruk.GetCurrentRoom();

        if (!mrukRoom.HasAllLabels(MRUKAnchor.SceneLabels.FLOOR | MRUKAnchor.SceneLabels.CEILING)) return;

        floorAnchor = mrukRoom.FloorAnchor;
        ceilingAnchor = mrukRoom.CeilingAnchor;

        floorSize = floorAnchor.PlaneRect.Value.size;
        ceilingSize = ceilingAnchor.PlaneRect.Value.size;

        floorPlaneBoundry = floorAnchor.PlaneBoundary2D;
        ceilingPlaneBoundry = ceilingAnchor.PlaneBoundary2D;
    }



    public void MrukRoomRemovedEvent()
    {
        floorAnchor = null;
        ceilingAnchor = null;
    }



    private void OnWave(object sender, GameManager.OnWaveArgs waveArgs)
    {
        //Debug.Log($"Wave Difficulty: {waveArgs.DifficultyDelta}, num chests: {numChests}");

        if (numChests >= settings.ChestMax) return;
        if (waveArgs.DifficultyDelta >= 0.0f) return;

        float difficultyQuota = -waveArgs.DifficultyDelta * settings.ChestMaxWaveContribution;
        int numChestsToSpawn = Mathf.Max(Mathf.Min(settings.ChestMaxSpawnPerWave, Mathf.FloorToInt(difficultyQuota / (settings.ChestDifficultyValue + 2 * settings.ChestTorpedoDifficultyValue))), 1);
        if (numChestsToSpawn + numChests > settings.ChestMax) numChestsToSpawn = settings.ChestMax - numChests;

        //Debug.Log($"Attempt spawning {numChestsToSpawn} chests with quota {difficultyQuota}");

        if (numChestsToSpawn <= 0) return;

        float remainingDifficulty = difficultyQuota - (numChestsToSpawn * settings.ChestDifficultyValue);

        List<GameObject> newChests = SpawnChests(numChestsToSpawn);
        DistributeTorpedoesAndHealth(remainingDifficulty, newChests);

        //Debug.Log($"Spawned {numChestsToSpawn} chests with {remainingDifficulty} difficulty remaining");
    }
}
