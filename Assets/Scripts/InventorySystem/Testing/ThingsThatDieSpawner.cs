using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThingsThatDieSpawner : MonoBehaviour
{
    public GameObject ThingThatDiesPrefab;

    public float SpawnInterval = 1.0f;

    public int LeftToSpawn = 10;

    public float SpawnRadius = 100.0f;

    private float timeSinceLastSpawn = 0.0f;

    private MRUKAnchor floorAnchor = null;
    private Vector2 floorSize;
    private List<Vector2> floorPlaneBoundry;
    private MRUKRoom mrukRoom;

    private MRUK mruk => MRUK.Instance;


    void Start()
    {

    }


    void Update()
    {
        if (floorAnchor == null) return;

        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= SpawnInterval && LeftToSpawn > 0)
        {
            timeSinceLastSpawn = 0.0f;
            LeftToSpawn--;

            //floorAnchor.pl

            //floorAnchor.GetClosestSurfacePosition();

            GameObject newTtd = Instantiate(ThingThatDiesPrefab, GetRandomFloorLocation(), Quaternion.identity);

            //newTtd.GetComponent<ThingThatDies>().DeathTimer = Random.Range(1, 20);

            newTtd.GetComponent<Inventory>().NumTorpedos = Random.Range(1, 5);
        }
    }



    private Vector3 GetRandomFloorLocation()
    {
        Vector2 randomPoint = new(1000.0f, 1000.0f);

        int attempts = 0;
        do
        {
            attempts++;
            randomPoint = new(Random.Range(-floorSize.x / 2.0f, floorSize.x / 2.0f), Random.Range(-floorSize.y / 2.0f, floorSize.y / 2.0f));


            floorAnchor.GetClosestSurfacePosition(randomPoint, out Vector3 closestPosition);
        } while (attempts < 100 && !floorAnchor.IsPositionInBoundary(randomPoint));

        Vector3 worldPoint = new Vector3(randomPoint.x, randomPoint.y, 0.0f);

        worldPoint = floorAnchor.transform.TransformPoint(worldPoint);

        Debug.Log($"Attempts: {attempts}");

        if (attempts >= 100)
        {
            return floorAnchor.GetAnchorCenter();
        }

        return worldPoint;
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
}
