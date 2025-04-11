using UnityEngine;
using System.Collections;

public class CoroutineHelper : MonoBehaviour
{
    private static CoroutineHelper instance;
    
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

    public static Coroutine StartCoroutineStatic(IEnumerator coroutine)
    {
        return Instance.StartCoroutine(coroutine);
    }

    public static void StopCoroutineStatic(Coroutine coroutine)
    {
        if (coroutine != null)
            Instance.StopCoroutine(coroutine);
    }
}