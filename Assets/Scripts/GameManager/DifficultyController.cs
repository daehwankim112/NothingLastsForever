
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

        //difficultySensors = difficultySensorComponents.Select(component => component as IDifficultySensor).ToList();

        difficultySensors = new() { PlayerManager.Instance, CollectablesManager.Instance, BoidManager.Instance };

        GameManager.Instance.OnWave += Wave;
    }



    void Update()
    {
        ActionTimer -= Time.deltaTime;

        targetDifficulty += Time.deltaTime * difficultySlope;
        currentDifficulty = GetDifficulty();
    }



    private float GetDifficulty() => difficultySensors.Count > 0 ? difficultySensors.Sum(sensor => sensor.GetDifficulty()) : 0.0f;



    private float DifficultyError()
    {
        currentDifficulty = GetDifficulty();

        if (targetDifficulty < 0.0f) targetDifficulty = 0.0f;

        return targetDifficulty - currentDifficulty;
    }



    public float CalculateControlSignal()
    {
        if (ActionTimer > 0.0f) return 0.0f;
        ActionTimer = Random.Range(settings.MinActionTimer, settings.MaxActionTimer);

        float currentError = DifficultyError();
        return Mathf.Abs(currentError) > 0.01 * actionThreshold * currentDifficulty ? currentError : 0.0f;
    }



    public void Wave(object obj, GameManager.OnWaveArgs args)
    {
    }
}
