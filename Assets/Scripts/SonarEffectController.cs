using UnityEngine;

public class SonarEffectController : MonoBehaviour
{
    public Material sonarMaterial;
    public float waveSpeed = 1.0f;
    private float currentWaveDistance = 0.0f;

    void Update()
    {
        currentWaveDistance += waveSpeed * Time.deltaTime;
        sonarMaterial.SetFloat("_WaveDistance", currentWaveDistance);
    }
}
