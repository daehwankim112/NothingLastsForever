
using System.Linq;
using System.Collections.Generic;

using UnityEngine;



public class DifficultyController : MonoBehaviour
{
    private Settings settings => GameManager.Instance.Settings;


    [SerializeField]
    private List<Component> difficultySensorComponents;
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

        difficultySensors = difficultySensorComponents.Select(component => component as IDifficultySensor).ToList();

        GameManager.Instance.OnWave += Wave;
    }



    void Update()
    {
        ActionTimer -= Time.deltaTime;

        targetDifficulty += Time.deltaTime * difficultySlope;
    }



    private float GetDifficulty() => difficultySensors.Count > 0 ? difficultySensors.Sum(sensor => sensor.GetDifficulty()) : 0.0f;



    private float DifficultyError()
    {
        currentDifficulty = GetDifficulty();

        return targetDifficulty - GetDifficulty();
    }



    public float CalculateControlSignal()
    {
        if (ActionTimer > 0.0f) return 0.0f;

        float currentError = DifficultyError();

        return Mathf.Abs(currentError) > actionThreshold ? currentError : 0.0f;
    }



    public void Wave(object obj, GameManager.OnWaveArgs args)
    {
        ActionTimer = Random.Range(settings.MinActionTimer, settings.MaxActionTimer);
    }
}
