using UnityEngine;

public class SonarEffectController : MonoBehaviour
{
    public Material sonarMaterial;
    public float waveSpeed = 1.0f;
    public Camera mainCamera;
    private float currentWaveDistance = 0.0f;

    void Start()
    {
        mainCamera.depthTextureMode |= DepthTextureMode.Depth;
    }

    void Update()
    {
        currentWaveDistance += waveSpeed * Time.deltaTime;
        sonarMaterial.SetFloat("_WaveDistance", currentWaveDistance);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, sonarMaterial);
    }
}
