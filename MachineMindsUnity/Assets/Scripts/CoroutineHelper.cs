using UnityEngine;
using System.Collections;

/// <summary>
/// Provides a static interface to start and stop coroutines
/// from non-MonoBehaviour classes by maintaining a hidden singleton.
/// </summary>
public class CoroutineHelper : MonoBehaviour
{
    private static CoroutineHelper instance;

    /// <summary>
    /// Ensures a single persistent GameObject and Component exist
    /// to run coroutines when no other MonoBehaviour is available.
    /// </summary>
    private static CoroutineHelper Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("CoroutineHelper");
                instance = go.AddComponent<CoroutineHelper>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    /// <summary>
    /// Starts a coroutine on the singleton instance.
    /// </summary>
    /// <param name="coroutine">IEnumerator representing the coroutine to run.</param>
    public static Coroutine StartCoroutineStatic(IEnumerator coroutine)
    {
        return Instance.StartCoroutine(coroutine);
    }

    /// <summary>
    /// Stops a previously started coroutine on the singleton instance.
    /// </summary>
    /// <param name="coroutine">Coroutine reference to stop.</param>
    public static void StopCoroutineStatic(Coroutine coroutine)
    {
        if (coroutine != null)
            Instance.StopCoroutine(coroutine);
    }
}