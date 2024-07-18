using System.Collections;
using UnityEngine;

public class SonarEffectController : MonoBehaviour
{
    public Material sonarMaterial;
    public float waveSpeed = 1f;
    public Camera mainCamera;
    [SerializeField] HandPoseDetectionAndPing handPoseDetectionAndPing;
    private bool pinging = false;
    [SerializeField] AudioClip audio;
    AudioSource audioSource;

    // public bool 
    private float currentWaveDistance = 999f;

    void Start()
    {
        mainCamera.depthTextureMode |= DepthTextureMode.Depth;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (handPoseDetectionAndPing.pingGestureActivated && !pinging)
        {
            StartCoroutine(SonarPing());
        }
        if (pinging)
        {
            currentWaveDistance += waveSpeed * Time.deltaTime;
            sonarMaterial.SetFloat("_WaveDistance", currentWaveDistance);
            if (currentWaveDistance > 2.5f)
            {
                currentWaveDistance = 999f;
            }
        }
        else
        {
            currentWaveDistance = 999f;
        }
    }

    private void OnDestroy()
    {
        pinging = false;
        StopAllCoroutines();
    }

    IEnumerator SonarPing()
    {
        pinging = true;
        currentWaveDistance = 0.0f;
        audioSource.PlayOneShot(audio);
        yield return new WaitForSeconds(2.5f);
        pinging = false;
        handPoseDetectionAndPing.pingGestureActivated = false;
        currentWaveDistance = 999.0f;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, sonarMaterial);
    }
}
