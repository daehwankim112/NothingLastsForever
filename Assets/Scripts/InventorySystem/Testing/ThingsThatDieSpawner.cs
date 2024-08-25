using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class ThingsThatDieSpawner : MonoBehaviour
{
    public GameObject ThingThatDiesPrefab;

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
