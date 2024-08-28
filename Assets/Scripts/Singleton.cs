
using UnityEngine;



public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    Debug.LogError($"An instance of {typeof(T)} is needed in the scene, but it isn't there.");
                }
                else
                {
                    DontDestroyOnLoad(instance.gameObject);
                }
            }

            return instance;
        }
    }


    /// <summary>
    /// Check if an instance of this singleton exists. Does not search for an instance if one does not exist.
    /// </summary>
    public static bool InstanceExists => instance != null;
}
