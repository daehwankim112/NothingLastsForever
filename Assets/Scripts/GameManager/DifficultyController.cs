
using System.Linq;
using System.Collections.Generic;

using UnityEngine;



public class DifficultyController : MonoBehaviour
{
    private GameManager gameManager => GameManager.Instance;
    private GameManagerTuneParameter settings => GameManager.Instance.Settings;
    // private Settings settings => GameManager.Instance.Settings;


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


    [SerializeField]
    private float nextControlOutput;


    [SerializeField]
    private float lastControlOutput;


    [SerializeField]
    private float currentError;



    void Start()
    {
        ActionTimer = settings.GameStartGracePeriod;
        difficultySlope = settings.DifficultySlope;
        actionThreshold = settings.ActionThreshold;
        targetDifficulty = settings.StartDifficulty;

        //difficultySensors = difficultySensorComponents.Select(component => component as IDifficultySensor).ToList();

        difficultySensors = new() { PlayerManager.Instance, CollectablesManager.Instance, BoidManager.Instance, EnemySubmarinesManager.Instance };
    }



    void Update()
    {
        targetDifficulty += Time.deltaTime * difficultySlope;

        ActionTimer -= Time.deltaTime;

        if (ActionTimer < 0.0f)
        {
            ActionTimer = settings.ActionTickPeriod;

            DoActionTick();
        }
    }



    private float GetDifficulty() => difficultySensors.Count > 0 ? difficultySensors.Sum(sensor => sensor.GetDifficulty()) : 0.0f;



    private float DifficultyError()
    {
        currentDifficulty = GetDifficulty();

        return targetDifficulty - currentDifficulty;
    }



    private float CalculateControlSignal()
    {
        currentError = DifficultyError();
        return Mathf.Clamp(settings.DifficulyProportionWeight * currentError, -settings.ActionMaxDifficulty, settings.ActionMaxDifficulty);
    }



    private void DoActionTick()
    {
        currentDifficulty = GetDifficulty();
        nextControlOutput = CalculateControlSignal();

        if (Mathf.Abs(currentError) < 0.01 * actionThreshold * currentDifficulty) return;

        if (Random.value < settings.TickWaveChance)
        {
            lastControlOutput = nextControlOutput;
            gameManager.Wave(nextControlOutput);
        }
    }
}
