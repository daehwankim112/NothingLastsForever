using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarEffectController : MonoBehaviour
{
    [SerializeField] private List<Material> sonarMaterial;
    [SerializeField] private float waveSpeed = 3f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private HandPoseTrigger handPoseTrigger;
    [SerializeField] private AudioClip audio;
    [SerializeField] private float pingingCooldown = 2.5f;
    [SerializeField] private bool debug = false;
    [SerializeField] private EnemySubmarinesManager enemySubmarinesManager;

    private bool pinging = false;
    private AudioSource audioSource;
    private float currentWaveDistance = 999f;
    private bool debugFromInspector = false;

    void Start()
    {
        mainCamera.depthTextureMode |= DepthTextureMode.Depth;
        audioSource = GetComponent<AudioSource>();
        if (debug)
        {
            StartCoroutine(SonarPing());
        }
    }

    void Update()
    {
        OVRInput.Update();

        // Check if the ping gesture is activated or the A or X button is pressed. RawButton.A and RawButton.X are mapped as Button.One
        // https://developer.oculus.com/documentation/unity/unity-ovrinput/
        /*if (OVRInput.GetDown(OVRInput.Button.One))
        {
            OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.RTouch);
            OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.LTouch);
            Debug.Log("One button pressed");
        }
        if (OVRInput.GetUp(OVRInput.Button.One))
        {
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
        }
        if (OVRInput.Get(OVRInput.RawButton.A))
        {
            OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.RTouch);
            Debug.Log("A button pressed");
        }
        else
        {
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        }
        if (OVRInput.Get(OVRInput.RawButton.X))
        {
            OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.LTouch);
            Debug.Log("X button pressed");
        }
        else
        {
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
        }*/


        if (((handPoseTrigger.pingGestureActivated 
            || (OVRInput.Get(OVRInput.RawButton.A) || OVRInput.Get(OVRInput.RawButton.X))) 
            && !pinging && !debug) || debugFromInspector)
        {
            Debug.Log("Ping gesture activated");
            currentWaveDistance = 0.0f;
            StartCoroutine(SonarPing());
            if (debugFromInspector)
            {
                debugFromInspector = false;
            }
        }
        if (debug && !pinging)
        {
            currentWaveDistance = 0.0f;
            StartCoroutine(SonarPing());
        }
        if (pinging)
        {
            currentWaveDistance += Time.deltaTime;
            foreach (Material sonarMaterial in sonarMaterial)
            {
                sonarMaterial.SetFloat("_WaveDistance", waveSpeed * currentWaveDistance);
                sonarMaterial.SetFloat("_Threshold", 1);
                sonarMaterial.SetFloat("_MaxWaveDistance", waveSpeed * pingingCooldown);
                enemySubmarinesManager.sonarPingDistanceFromPlayer = waveSpeed * currentWaveDistance;
            }
            if (currentWaveDistance > waveSpeed * pingingCooldown)
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

    [ContextMenu("SonarPingDebugFromInspector")]
    private void SonarPingDebugFromInspector()
    {
        debugFromInspector = true;
    }

    IEnumerator SonarPing()
    {
        pinging = true;
        audioSource.PlayOneShot(audio);
        yield return new WaitForSeconds(pingingCooldown + 0.1f);
        pinging = false;
        handPoseTrigger.pingGestureActivated = false;
        currentWaveDistance = 999f;
    }
}
