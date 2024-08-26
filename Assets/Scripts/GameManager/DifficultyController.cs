
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public class DifficultyController : MonoBehaviour
{
    [SerializeField]
    private List<IDifficultySensor> difficultySensors;


    [SerializeField]
    public float targetDifficulty;


    [SerializeField]
    private float currentDifficulty;


    [SerializeField]
    private float actionThreshold;


    [SerializeField]
    public float ActionTimer;



    private float GetDifficulty() => difficultySensors.Sum(sensor => sensor.GetDifficulty());



    private float DifficultyError()
    {
        return GetDifficulty() - targetDifficulty;
    }



    public float CalculateControlSignal()
    {
        float currentError = DifficultyError();

        return ActionTimer < 0.0f && Mathf.Abs(currentError) > actionThreshold ? currentError : 0.0f;
    }



    private void Update()
    {
        ActionTimer -= Time.deltaTime;
    }
}
