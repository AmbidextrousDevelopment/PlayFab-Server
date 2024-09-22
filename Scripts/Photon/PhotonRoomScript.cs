using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonRoomScript : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    #region References to Objects, Scripts and Components
    [SerializeField] private TextMeshProUGUI multiplayerMenuTitle;
    [SerializeField] private PhotonView thisPhotonView;
    private PhotonNetworkPlayerScript photonNetworkPlayerScript;
    private Player[] _photonPlayers;
    #endregion

    #region Room Variables
    [Header("Room Variables")]
    private int _playersInRoom;
    private int _numberInRoom;
    private int _playersInGame;
    private float _startingTime = 2;
    private float _maxPlayers;
    private float _LessThanMaxPlayers;
    private bool _multiplayerSceneLoaded; //When true tells the game that multiplayer scene is loaded
    private int _currentScene; //current Scene Number
    private bool _readyToCount; //when true, tells game to start counting
    private bool _readyToStart;
    private float _timeToStartGame;
    #endregion

    #region Photon Override Functions

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        HandleJoiningOfAnotherPlayerInTheRoom();

    }
    public override void OnJoinedRoom()
    {
        HandleJoiningOfThisPlayerInTheRoom();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        TellLocalPlayerToDisplayTheDisconnectionCanvas();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        // Adding the OnSceneFinishedLoading Function to the SceneManager
        SceneManager.sceneLoaded += OnSceneFinishedLoading;

        // Add callback target from Photon
        PhotonNetwork.AddCallbackTarget(this);
    }
    public override void OnDisable()
    {
        base.OnDisable();
        // Remove the OnSceneFinishedLoading Function from the SceneManager
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;

        // Remove callback target from Photon
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    #endregion

    #region Room Control Functions

    private void TellLocalPlayerToDisplayTheDisconnectionCanvas()
    {
        photonNetworkPlayerScript.EnableDisconnectionCanvasWhenPlayerLeaves();
    }

    //a function that will help the master client to load multiplayer scene, when we have enough players
    //RPC that will create the player
    [PunRPC]
    public void RPC_CreatePlayer()
    {
        Debug.Log("Creating PhotonNetworkPlayer");

        GameObject spawnedPhotonNetworkPlayer =
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"),
            Vector3.zero,
            Quaternion.identity, 0);

        Debug.Log($"PhotonNetworkPlayer created with PhotonView ID: {spawnedPhotonNetworkPlayer.GetComponent<PhotonView>().ViewID}");

        photonNetworkPlayerScript =
            spawnedPhotonNetworkPlayer.GetComponent<PhotonNetworkPlayerScript>();

        // Setting player's number in room
        photonNetworkPlayerScript.SetPlayerNumber(numberToSet: _numberInRoom);

        // OldOne();
    }

    private void OldOne()
    {
        if (PhotonNetwork.PlayerList.Length <= _playersInRoom) //check if player isn't already loaded
        {
            // PhotonNetwork instantiate spawns on all clients
            Debug.Log("Creating PhotonNetworkPlayer");

            GameObject spawnedPhotonNetworkPlayer =
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"),
                Vector3.zero,
                Quaternion.identity, 0);

            Debug.Log($"PhotonNetworkPlayer created with PhotonView ID: {spawnedPhotonNetworkPlayer.GetComponent<PhotonView>().ViewID}");

            photonNetworkPlayerScript =
            spawnedPhotonNetworkPlayer.GetComponent<PhotonNetworkPlayerScript>();

            //Setting player's number in room
            photonNetworkPlayerScript.SetPlayerNumber(numberToSet: _numberInRoom);
            //photonNetworkPlayerScript.playerNumber = _numberInRoom;
        }
    }

    //Runs when the Scene is loaded and will be used to create a player when the scene is loaded
    [PunRPC]
    public void RPC_MultiplayerSceneLoaded()
    {
        _playersInGame++;
        if (_playersInGame == PhotonNetwork.PlayerList.Length)
        {
            thisPhotonView.RPC("RPC_CreatePlayer", RpcTarget.All);
        }
    }
    //function for the current user
    private void HandleJoiningOfThisPlayerInTheRoom()
    {
        Debug.Log("This Player joined Room");
        //Setting up the values of certain room variables
        _photonPlayers = PhotonNetwork.PlayerList;
        //_playersInRoom++;
        _playersInRoom = _photonPlayers.Length;
        _playersInGame = _photonPlayers.Length;

        //Setting up this player's number in room
        _numberInRoom = _playersInGame;

        if (MultiplayerSettings.instance.delayStart)
        {
            //if we have more than 1 player in the room
            if (_playersInRoom > 1)
            {
                _readyToCount = true;
            }

            //Handling number of max players
            if (_playersInRoom == MultiplayerSettings.instance.maxPlayers)
            {
                _readyToStart = true;
                //telling the player that game is about to start
                multiplayerMenuTitle.text = "Game is about to start";

                //If we are master client, we will start countdown
                if (!PhotonNetwork.IsMasterClient)
                {
                    return;
                }

                PhotonNetwork.CurrentRoom.IsOpen = false;
            }       
        }
        else
        {
            StartGame();
        }
    }
    //function of another player joining the room
    private void HandleJoiningOfAnotherPlayerInTheRoom()
    {
        print("Another Player joined Room");

        _photonPlayers = PhotonNetwork.PlayerList;
        //_playersInRoom++;
        _playersInRoom = _photonPlayers.Length;

        //If we have delay start
        if (MultiplayerSettings.instance.delayStart == true)
        {
            //if we have more than 1 player in the room
            if (_playersInRoom > 1)
            {
                _readyToCount = true;
            }

            //Handling number of max players
            if (_playersInRoom == MultiplayerSettings.instance.maxPlayers)
            {
                _readyToStart = true;
                //telling the player that game is about to start
                multiplayerMenuTitle.text = "Game is about to start";

                //If we are master client, we will start countdown
                if (!PhotonNetwork.IsMasterClient)
                {
                    return;
                }

                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
        /*else     //idk
        {
            StartGame();
        }*/
    }

    private void GameStartFunction() //starts when required players are reached
    {
        if (MultiplayerSettings.instance.delayStart == true)
        {
            //if we are in PhotonRoom
            if (PhotonNetwork.InRoom)
            {
                if (_readyToCount == false)
                {
                    multiplayerMenuTitle.text = "Searching for a game...";
                }
                if (_photonPlayers.Length == MultiplayerSettings.instance.maxPlayers)
                {
                    multiplayerMenuTitle.text = "Game is about to start...";
                }
                
            }

            //if one player is in the room we'll restart timer
            if (_playersInRoom == 1)
            {
                RestartTimer();
            }

            //if the game is not loaded we need to start countdown
            if (_multiplayerSceneLoaded == false)
            {
                //if we are ready to start, countdown will be at start of the game
                if (_readyToStart)
                {
                    _maxPlayers = Time.deltaTime;
                    _LessThanMaxPlayers = _maxPlayers;
                    _timeToStartGame = _maxPlayers;
                }

                //Runs when we are ready to countdown
                if (_readyToCount)
                {
                    _LessThanMaxPlayers -= Time.deltaTime;
                    _timeToStartGame = _LessThanMaxPlayers;
                    print("Time to start the game: " + _timeToStartGame);
                }

                if (_timeToStartGame <= 0)
                {
                    StartGame();
                }
            }
        }
    }
    private void StartGame()
    {
        _multiplayerSceneLoaded = true;

        //if we are not master client, we will not run code
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }

        //if we have delay start setting
        if (MultiplayerSettings.instance.delayStart == true)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        //Using photon LoadLevel to load Level Scene
        PhotonNetwork.LoadLevel(MultiplayerSettings.instance.multiPlayerScene);

    }
    private void RestartTimer() //Restarts timer when 1 player is in the room
    {
        _LessThanMaxPlayers = (int)_startingTime;
        _timeToStartGame = _startingTime;
        _maxPlayers = 2;
        _readyToCount = false;
        _readyToStart = false;
    }
    private void SetRoomControlVariableValues()
    {
        _readyToCount = false;
        _readyToStart = false;
        _LessThanMaxPlayers =(int)_startingTime;
        _maxPlayers = 2;
        _timeToStartGame = _startingTime;
        _multiplayerSceneLoaded = false;

    }
    [PunRPC]
    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        _currentScene = scene.buildIndex; //Getting current scene

        if (_currentScene == MultiplayerSettings.instance.multiPlayerScene) //if current scene is multiplayer scene
        {
            _multiplayerSceneLoaded = true;

            //creating player here
            if (MultiplayerSettings.instance.delayStart == true)
            {
                thisPhotonView.RPC("RPC_MultiplayerSceneLoaded", RpcTarget.All);
            }
            else
            {
                //Creating a player immediately
                RPC_CreatePlayer();
            }
            
        }

    }

    #endregion

    #region Unity Functions
    private void Awake()
    {
        SetupSingleton();
    }
    private void Start()
    {
        SetRoomControlVariableValues();
    }
    private void Update()
    {
        GameStartFunction();
    }
    #endregion

    #region Singleton
    public static PhotonRoomScript instance;
    private void SetupSingleton()
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
    #endregion
}
