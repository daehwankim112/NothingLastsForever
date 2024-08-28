using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SonarEffectController : Singleton<SonarEffectController>
{
    [HideInInspector] public UnityEvent onSonarPing;
    [SerializeField] private List<Material> sonarMaterial;
    [SerializeField] private float waveSpeed = 3f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private AudioClip audio;
    [SerializeField] private float pingingCooldown = 2.5f;
    [SerializeField] private bool debug = false;
    [SerializeField] private EnemySubmarinesManager enemySubmarinesManager => EnemySubmarinesManager.Instance;

    private bool pinging = false;
    private AudioSource audioSource;
    private float currentWaveDistance = 999f;
    private bool debugFromInspector = false;
    private IEnumerator sonarPingCoroutine;

    private void Start()
    {
        mainCamera.depthTextureMode |= DepthTextureMode.Depth;
        audioSource = GetComponent<AudioSource>();
        onSonarPing.AddListener(PingGenstureActivated);
        sonarPingCoroutine = SonarPing();
        currentWaveDistance = 999f;
        if (debug)
        {
            StartCoroutine(sonarPingCoroutine);
        }
    }

    private void Update()
    {
        // OVRInput.Update();

        /*if (((handPoseTrigger.pingGestureActivated 
            || (OVRInput.Get(OVRInput.RawButton.A) || OVRInput.Get(OVRInput.RawButton.X))) 
            && !pinging && !debug) || debugFromInspector)
        {
            Debug.Log("Ping gesture activated");
            currentWaveDistance = 0.0f;
            pinging = true;
            StartCoroutine(SonarPing());
            if (debugFromInspector)
            {
                debugFromInspector = false;
            }
        }*/
        /*if (debug && !pinging)
        {
            currentWaveDistance = 0.0f;
            pinging = true;
            StartCoroutine(SonarPing());
        }*/
        Debug.Log("currentWaveDistance: " + currentWaveDistance);
        if (pinging)
        {
            currentWaveDistance += Time.deltaTime;
            foreach (Material sonarMaterial in sonarMaterial)
            {
                sonarMaterial.SetFloat("_WaveDistance", waveSpeed * currentWaveDistance);
                sonarMaterial.SetFloat("_Threshold", 1);
                sonarMaterial.SetFloat("_MaxWaveDistance", waveSpeed * pingingCooldown);
            }
            if (currentWaveDistance > waveSpeed * pingingCooldown)
            {
                currentWaveDistance = 999f;
                pinging = false;
            }
        }
        else
        {
            currentWaveDistance = 999f;
            pinging = false;
        }
        enemySubmarinesManager.sonarPingDistanceFromPlayer = waveSpeed * currentWaveDistance;
    }

    private void PingGenstureActivated()
    {
        Debug.Log("Ping gesture activated");
        audioSource.Stop();
        StopCoroutine(sonarPingCoroutine);
        pinging = true;
        currentWaveDistance = 0.0f;
        audioSource.PlayOneShot(audio);
        StartCoroutine(sonarPingCoroutine);
    }

    private void OnDestroy()
    {
        pinging = false;
        StopCoroutine(sonarPingCoroutine);
    }

    [ContextMenu("SonarPingDebugFromInspector")]
    private void SonarPingDebugFromInspector()
    {
        debugFromInspector = true;
    }

    IEnumerator SonarPing()
    {
        yield return new WaitForSeconds(pingingCooldown + 0.1f);
        currentWaveDistance = 999f;
        pinging = false;
    }
}
