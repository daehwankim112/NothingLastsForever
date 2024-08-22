using Meta.XR.MRUtilityKit;
using Meta.XR.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySubmarineSpawner : MonoBehaviour
{
    [SerializeField] private Transform enemySubmarinePrefab;
    [SerializeField] private int numberOfSubmarines = 5;
    [SerializeField] private int maxIterations = 1000;
    [SerializeField] private MRUKAnchor.SceneLabels labels = ~(MRUKAnchor.SceneLabels)0;
    [SerializeField] private float surfaceClearanceDistance = 0.3f;

    void Start()
    {
        if (MRUK.Instance)
        {
            MRUK.Instance.RegisterSceneLoadedCallback(() =>
            {
                for (int i = 0; i < numberOfSubmarines; i++)
                {
                    SpawnSubmarines(MRUK.Instance.GetCurrentRoom());
                }
            });
        }
    }
    public void SpawnSubmarines(MRUKRoom room)
    {
        Debug.Log("Spawning a submarine");
        if (room.HasAllLabels(MRUKAnchor.SceneLabels.DOOR_FRAME) || room.HasAllLabels(MRUKAnchor.SceneLabels.WINDOW_FRAME))
        {
            Debug.Log($"Room has door frame: {room.HasAllLabels(MRUKAnchor.SceneLabels.DOOR_FRAME)}. room has window frame: {room.HasAllLabels(MRUKAnchor.SceneLabels.WINDOW_FRAME)}");
            Debug.Log(labels.ToString());
            for (int j = 0; j < maxIterations; j++)
            {
                Vector3 spawnPosition = Vector3.zero;
                Vector3 spawnNormal = Vector3.zero;
                MRUK.SurfaceType surfaceType = 0;
                surfaceType |= MRUK.SurfaceType.FACING_UP;
                surfaceType |= MRUK.SurfaceType.VERTICAL;
                surfaceType |= MRUK.SurfaceType.FACING_DOWN;
                if (room.GenerateRandomPositionOnSurface(surfaceType, 0.1f, LabelFilter.Included(labels), out var pos, out var normal))
                {
                    spawnPosition = pos - normal * surfaceClearanceDistance;
                    Debug.Log($"Spawn position: {spawnPosition}");
                    Debug.Log($"Spawn normal: {normal}");
                    spawnNormal = normal;
                    var center = spawnPosition;
                    if (room.IsPositionInRoom(center))
                    {
                        continue;
                    }
                    if (room.IsPositionInSceneVolume(center))
                    {
                        continue;
                    }
                    if (room.Raycast(new Ray(pos, normal), surfaceClearanceDistance, out _))
                    {
                        continue;
                    }
                }

                Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, spawnNormal);
                if (enemySubmarinePrefab.gameObject.scene.path == null && spawnPosition != Vector3.zero)
                {
                    Instantiate(enemySubmarinePrefab, spawnPosition, spawnRotation, transform);
                    Debug.Log($"Spawn position: {spawnPosition}");
                }
                else
                {
                    enemySubmarinePrefab.transform.position = spawnPosition;
                    enemySubmarinePrefab.transform.rotation = spawnRotation;
                    return;
                }
                break;
            }
        }
        else
        {
            Debug.Log($"Following labels do not exist in your room {labels.ToString()}");
        }
    }
}
