
using System.Linq;
using System.Collections.Generic;

using UnityEngine;



public class DifficultyController : MonoBehaviour
{
    private Settings settings => GameManager.Instance.Settings;

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


    [SerializeField]
    private float difficultySlope;



    void Start()
    {
        ActionTimer = settings.GameStartGracePeriod;
        difficultySlope = settings.DifficultySlope;
        actionThreshold = settings.ActionThreshold;
        targetDifficulty = settings.StartDifficulty;
    }



    void Update()
    {
        ActionTimer -= Time.deltaTime;

        targetDifficulty += Time.deltaTime * difficultySlope;
    }



    private float GetDifficulty() => difficultySensors.Sum(sensor => sensor.GetDifficulty());



    private float DifficultyError()
    {
        return GetDifficulty() - targetDifficulty;
    }



    public float CalculateControlSignal()
    {
        if (ActionTimer > 0.0f) return 0.0f;

        float currentError = DifficultyError();

        return Mathf.Abs(currentError) > actionThreshold ? currentError : 0.0f;
    }



    public void Wave(float _)
    {
        ActionTimer = Random.Range(settings.MinActionTimer, settings.MaxActionTimer);
    }
}
