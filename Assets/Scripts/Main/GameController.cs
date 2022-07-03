using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [SerializeField] private Settings _settings;

    private void Awake()
    {
        if (!Instance) Instance = this;
        else
        {
            Destroy(gameObject);
            // ? Do I have to return, or will Destroy immediately stop this (I doubt it will).
            // Return to stop settings from re-loading.
            return;
        }

        // Load the settings
        _settings.Load();
    }

    public Settings GetSettings()
    {
        return _settings;
    }
}