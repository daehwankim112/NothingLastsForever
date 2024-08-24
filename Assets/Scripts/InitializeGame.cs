using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeGame : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private void Start()
    {
        mainCamera.depthTextureMode = DepthTextureMode.Depth;
    }

    private void CheckRoomInitialization()
    {

    }
}
