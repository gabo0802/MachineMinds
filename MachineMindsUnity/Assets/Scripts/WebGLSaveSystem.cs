using UnityEngine;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Provides utility methods for handling save/load operations on the WebGL platform.
/// Uses PlayerPrefs as an alternative to file I/O operations.
/// </summary>
public static class WebGLSaveSystem
{
    /// <summary>
    /// Determines if the application is running on the WebGL platform.
    /// </summary>
    /// <returns>True if running on WebGL, otherwise false.</returns>
    public static bool IsWebGLPlatform()
    {
        return Application.platform == RuntimePlatform.WebGLPlayer;
    }


}
