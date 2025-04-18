using UnityEngine;

/// <summary>
/// Simple confirmation menu handler for destroying the menu on back.
/// </summary>
public class ConfirmationMenuScriptMenuScript : MonoBehaviour
{
    /// <summary>
    /// Called when the back button is pressed; closes this confirmation menu.
    /// </summary>
    public void onBackButtonPress()
    {
        Destroy(gameObject);
    }
}