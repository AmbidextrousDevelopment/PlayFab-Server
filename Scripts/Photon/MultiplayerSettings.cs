using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides a few values to all scripts that requires it
/// </summary>
public class MultiplayerSettings : MonoBehaviour
{
    public static MultiplayerSettings instance;

    [Header("Multiplayer Settings")]
    //When true, short delay is active
    public bool delayStart = true;

    public int maxPlayers = 2;

    public int multiPlayerScene = 3, mainMenuScene = 0; //could be 3 not 2


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
