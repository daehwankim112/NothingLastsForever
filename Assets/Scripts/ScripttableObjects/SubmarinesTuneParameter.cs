using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SubmarinesTuneParameter", order = 1)]
public class SubmarinesTuneParameter : ScriptableObject
{
    public float enemySubmarineMaxSpeed = 0.003f;
    public float towardTheTargetWeight = 0.7f;
    public float rotateAroundTheTargetWeight = 12f;
    public float avoidCollisionWeight = 2f;
    public float collisionTestDistance = 1f;
    public float rotationEuqalibriumDistance = 1f;
    public float testDistance = 0.1f;
    public float testRadius = 0.2f;
    public float torpedoFireCooldown = 2f;
    public int numberOfTest = 10;
    public Transform torpedoPrefab;
}
