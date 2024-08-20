using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarEffectController : MonoBehaviour
{
    [SerializeField] private List<Material> sonarMaterial;
    [SerializeField] private float waveSpeed = 3f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private HandPoseDetectionAndPing handPoseDetectionAndPing;
    [SerializeField] private AudioClip audio;
    [SerializeField] private float pingingCooldown = 2.5f;
    [SerializeField] private bool debug = false;

    private bool pinging = false;
    private AudioSource audioSource;
    private float currentWaveDistance = 999f;

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
        /*if ((handPoseDetectionAndPing.pingGestureActivated 
            || (OVRInput.GetDown(OVRInput.Button.One))) 
            && !pinging)
        {
            StartCoroutine(SonarPing());
        }*/
        if ((handPoseDetectionAndPing.pingGestureActivated 
            || (OVRInput.GetDown(OVRInput.RawButton.A) || OVRInput.GetDown(OVRInput.RawButton.X))) 
            && !pinging && !debug)
        {
            StartCoroutine(SonarPing());
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
                sonarMaterial.SetFloat("_MaxWaveDistance", pingingCooldown);
            }
            if (currentWaveDistance > pingingCooldown)
            {
                currentWaveDistance = 999f;
            }
        }
        else
        {
            currentWaveDistance = 999f;
        }
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.RTouch);
            OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.LTouch);
            Debug.Log("One button pressed");
        }
        if (OVRInput.GetDown(OVRInput.RawButton.A))
        {
            OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.RTouch);
            Debug.Log("A button pressed");
        }
        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.LTouch);
            Debug.Log("X button pressed");
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
        audioSource.PlayOneShot(audio);
        yield return new WaitForSeconds(pingingCooldown + 0.1f);
        pinging = false;
        handPoseDetectionAndPing.pingGestureActivated = false;
        currentWaveDistance = 999f;
    }
}
