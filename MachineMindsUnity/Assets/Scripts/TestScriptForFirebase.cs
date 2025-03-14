using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class Test : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void AddDocument(string collectionPath, string value, string objectName, string callback, string fallback);

    public void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            StartCoroutine(TryFirebaseOperation());
        }
    }

    private IEnumerator TryFirebaseOperation()
    {
        // Wait for page to fully load
        yield return new WaitForSeconds(3.0f);

        Debug.Log("Attempting Firebase operation");
        AddDocument("test-collection", "{\"testField\": \"testValue\"}", gameObject.name, "OnRequestSuccess", "OnRequestFailed");
    }

    private void OnRequestSuccess(string data)
    {
        Debug.Log("Request Success: " + data);
    }

    private void OnRequestFailed(string data)
    {
        Debug.LogError("Request Failed: " + data);
    }
}


/*
AddDocument: function (collectionPath, value, objectName, callback, fallback) {
        var parsedPath = UTF8ToString(collectionPath);
        var parsedValue = UTF8ToString(value);
        var parsedObjectName = UTF8ToString(objectName);
        var parsedCallback = UTF8ToString(callback);
        var parsedFallback = UTF8ToString(fallback);

        try {

            firebase.firestore().collection(parsedPath).add(JSON.parse(parsedValue)).then(function(unused) {
                window.unityInstance.SendMessage(parsedObjectName, parsedCallback, "Success: document added in collection " + parsedPath);
            })
                .catch(function(error) {
                    window.unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
                });

        } catch (error) {
            window.unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },
*/