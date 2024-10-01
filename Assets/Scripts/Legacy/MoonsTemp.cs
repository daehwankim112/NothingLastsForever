using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonsTemp : MonoBehaviour
{
    [SerializeField] private GameObject GravitySphere;
    [SerializeField] private Transform moonPrefab;
    [SerializeField] private int numberOfMoon;

    private List<Transform> moons;
    void Start()
    {
        InitiateMoons();
    }

    void Update()
    {
        UpdateMoons();
    }

    /// <summary>
    /// Initiate moons at random location on the sphere
    /// </summary>
    void InitiateMoons()
    {
        moons = new List<Transform>();
        for (int i = 0; i < numberOfMoon; i++)
        {
            Vector3 randomLocationOnSphere = Random.onUnitSphere * 2f;
            Quaternion randomRotation = Random.rotation;
            moons.Add(Instantiate(moonPrefab, randomLocationOnSphere, randomRotation));
            Color randomColor = Random.ColorHSV();
            moons[i].GetComponent<MeshRenderer>().material.color = randomColor;
            moons[i].GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", randomColor);
        }
    }
    /// <summary>
    /// Update moons position and apply forces
    /// </summary>
    void UpdateMoons()
    {
        Matrix4x4 rotationYby90 = new Matrix4x4(new Vector4(0f, 0f, 1f, 0f),
                                                    new Vector4(0f, 1f, 0f, 0f),
                                                    new Vector4(-1f, 0f, 0f, 0f),
                                                    new Vector4(0f, 0f, 0f, 1f));
        for (int i = 0; i < moons.Count; i++)
        {
            float magnitute = (moons[i].position - GravitySphere.transform.position).magnitude;
            Vector3 temp = Vector3.ProjectOnPlane(moons[i].position - GravitySphere.transform.position, new Vector3(0, 1, 0));
            // cubes[i].GetComponent<Rigidbody>().AddForce(rotationYby90.MultiplyVector(rotationZby90.MultiplyVector(new Vector4(temp.x, temp.y, temp.z, 1))) * 36 / magnitute);
            // Apple rotation force
            moons[i].GetComponent<Rigidbody>().AddForce(rotationYby90.MultiplyVector(new Vector4(temp.x, temp.y, temp.z, 1)) * 5 / magnitute);
            // Apply gravity towards the center of the sphere
            moons[i].GetComponent<Rigidbody>().AddForce(GravitySphere.transform.position - moons[i].position);
        }
    }
}
