using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When collision entered, see if it is a targettable enemy. If so, turn towards it and move towards it.
/// </summary>
public class ProjectileController : MonoBehaviour
{
    // [SerializeField] private MeshCollider cone;
    [SerializeField] private Transform projectile;
    [SerializeField] private float speed = 1.0f;
    private List<Transform> potentialTarget;

    private void Start()
    {
        Destroy(projectile.gameObject, 6f);
        potentialTarget = new List<Transform>();
    }

    void Update()
    {
        projectile.transform.position += projectile.transform.forward * 0.007f * speed;
        if (potentialTarget.Count > 0)
        {
            Transform target = potentialTarget[0];
            Vector3 headingToward = (target.position - projectile.transform.position).normalized;
            projectile.transform.rotation = Quaternion.RotateTowards(projectile.transform.rotation, Quaternion.LookRotation(headingToward), 1);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log($"Collision with {collider.gameObject.name}");
        if (collider.gameObject.tag == "Enemy")
        {
            Debug.Log("Hit enemy");
            GameObject enemyInSight = collider.gameObject;
            if (enemyInSight != null)
            {
                Vector3 headingToward = (collider.transform.position - projectile.transform.position).normalized;
                projectile.transform.rotation = Quaternion.RotateTowards(projectile.transform.rotation, Quaternion.LookRotation(headingToward), 1);
                potentialTarget.Add(enemyInSight.transform);
            }
        }
    }




    private void OnTriggerExit(Collider collider)
    {
        Debug.Log("Exit enemy");
        potentialTarget.Remove(collider.transform);
    }
}
