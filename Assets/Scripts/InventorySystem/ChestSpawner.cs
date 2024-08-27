
using Meta.XR.MRUtilityKit;

using System.Collections.Generic;

using UnityEngine;



public class ChestSpawner : MonoBehaviour
{
    private GameManager gameManager => GameManager.Instance;
    private Settings settings => gameManager.Settings;

    private CollectablesManager collectablesManager => CollectablesManager.Instance;


    public GameObject ChestPrefab;

    public float SpawnInterval = 1.0f;

    public int LeftToSpawn = 10;

    public float ChestSize = 0.5f;

    private float timeSinceLastSpawn = 0.0f;

    private MRUKAnchor floorAnchor = null;
    private Vector2 floorSize;
    private List<Vector2> floorPlaneBoundry;
    private MRUKRoom mrukRoom;

    private MRUK mruk => MRUK.Instance;


    void Start()
    {
        gameManager.OnWave += OnWave;
    }


    void Update()
    {
        //if (floorAnchor == null) return;

        //timeSinceLastSpawn += Time.deltaTime;

        //if (timeSinceLastSpawn >= SpawnInterval && LeftToSpawn > 0)
        //{
        //    timeSinceLastSpawn = 0.0f;
        //    LeftToSpawn--;

        //    GameObject newChest = Instantiate(ChestPrefab, GetRandomSpawnLocation(), Quaternion.AngleAxis(360.0f * Random.value, Vector3.up));

        //    newChest.GetComponent<Inventory>().NumTorpedos = Random.Range(1, 5);

        //    collectablesManager.AddChest(newChest);
        //}
    }



    private Vector3 GetRandomSpawnLocation()
    {
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



    private GameObject SpawnChest(int numTorpedos = 0, int health = 0)
    {
        GameObject newChest = Instantiate(ChestPrefab, GetRandomSpawnLocation(), Quaternion.AngleAxis(360.0f * Random.value, Vector3.up));

        newChest.GetComponent<Inventory>().NumTorpedos = numTorpedos;
        newChest.GetComponent<Inventory>().Health = health;

        collectablesManager.AddChest(newChest);

        return newChest;
    }



    private List<GameObject> SpawnChests(int numChests, int numTorpedos, int health)
    {
        List<GameObject> newChests = new(numChests);

        for (int i = 0; i < numChests && i + collectablesManager.Chests.Count < settings.ChestMax; i++)
        {
            newChests.Add(SpawnChest());
        }

        return newChests;
    }



    private void DistributeTorpedoesAndHealth(int health, int torpedoes, List<GameObject> chests)
    {
        int numChests = chests.Count;

        if (numChests == 0) return;

        // Ensure that each chest has at least one item
        foreach (GameObject chest in chests)
        {
            AddRandomToChest(chest);
        }

        // Distribute the remaining items randomly
        while (health > 0 || torpedoes > 0)
        {
            int randomChestIndex = Random.Range(0, numChests);
            AddRandomToChest(chests[randomChestIndex]);
        }

        void AddRandomToChest(GameObject chest)
        {
            if (health > 0 && torpedoes > 0)
            {
                if (Random.value < 0.5f)
                {
                    chest.GetComponent<Inventory>().NumTorpedos++;
                    torpedoes--;
                }
                else
                {
                    chest.GetComponent<Inventory>().Health++;
                    health--;
                }
            }
            else if (health > 0)
            {
                chest.GetComponent<Inventory>().Health++;
                health--;
            }
            else if (torpedoes > 0)
            {
                chest.GetComponent<Inventory>().NumTorpedos++;
                torpedoes--;
            }
        }
    }



    public void MrukRoomCreatedEvent()
    {
        mrukRoom = mruk.GetCurrentRoom();

        if (!mrukRoom.HasAllLabels(MRUKAnchor.SceneLabels.FLOOR)) return;

        floorAnchor = mrukRoom.FloorAnchor;

        floorSize = floorAnchor.PlaneRect.Value.size;

        floorPlaneBoundry = floorAnchor.PlaneBoundary2D;
    }



    public void MrukRoomRemovedEvent()
    {
        floorAnchor = null;
    }



    private void OnWave(object sender, GameManager.OnWaveArgs waveArgs)
    {
        if (collectablesManager.Chests.Count >= settings.ChestMax) return;

        float difficultyQuota = settings.ChestMaxWaveContribution * waveArgs.DifficultyDelta;
        float maxSingleItemChestDifficultyValue = settings.ChestDifficultyValue - Mathf.Min(settings.ChestTorpedoDifficultyValue, settings.ChestHealthDifficultyValue);
        int maxNumChestsToSpawn = Mathf.Clamp(Mathf.RoundToInt(difficultyQuota / maxSingleItemChestDifficultyValue), 1, settings.ChestMaxSpawnPerWave);
        int numChestsToSpawn = Random.Range(0, maxNumChestsToSpawn + 1);

        float baseChestDifficultyValue = settings.ChestDifficultyValue * numChestsToSpawn;
        difficultyQuota -= baseChestDifficultyValue;
        float healthDifficultyRatio = Random.Range(0.0f, 1.0f);

        int totalHealth = Mathf.Max(Mathf.CeilToInt(healthDifficultyRatio * numChestsToSpawn), Mathf.FloorToInt(difficultyQuota * healthDifficultyRatio / settings.ChestHealthDifficultyValue));
        int totalTorpedoes = Mathf.Max(Mathf.CeilToInt((1.0f - healthDifficultyRatio) * numChestsToSpawn), Mathf.FloorToInt(difficultyQuota * (1.0f - healthDifficultyRatio) / settings.ChestTorpedoDifficultyValue));

        List<GameObject> newChests = SpawnChests(numChestsToSpawn, totalTorpedoes, totalHealth);
        DistributeTorpedoesAndHealth(totalHealth, totalTorpedoes, newChests);

        Debug.Log($"Wave {waveArgs.DifficultyDelta} spawned {newChests.Count} chests with {totalTorpedoes} torpedoes and {totalHealth} health. Total collectables difficulty: {collectablesManager.GetDifficulty()}");
    }
}
