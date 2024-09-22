using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PhotonLobbyScript : MonoBehaviourPunCallbacks
{
    #region Singleton
    public static PhotonLobbyScript _instance;
    #endregion

    #region References to Objects in Multiplayer Menu
    [Header("References to Objects in Multiplayer Menu")]
    [SerializeField] private TextMeshProUGUI multiplayerMenuTitle;
    [SerializeField] private Button searchGameButton;
    [SerializeField] private Button quitButton;

    #endregion

    #region Room Control Functions
   
    #endregion

    #region Photon Functions for Connection
    private void ConnectingToPhotonServersAtStart()
    {
        searchGameButton.interactable = false;
        quitButton.interactable = false;

        multiplayerMenuTitle.text = "Connecting to Servers...";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        multiplayerMenuTitle.text = "Connected to Server Successfully";
        PhotonNetwork.AutomaticallySyncScene = true;

        searchGameButton.interactable = true;
        quitButton.interactable = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        print(cause);

        multiplayerMenuTitle.text = "Disconnected from Server";

        searchGameButton.interactable = false;
        quitButton.interactable = true;
    }


    #endregion

    #region Create or Join Room Function

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        print("Failed to join a Random Room");
        CreateRoom();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        print("Failed to create a Room");
        CreateRoom();
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        print("Room created successfully");
    }

    public void OnSearchForPlayerButtonClick()
    {
        searchGameButton.interactable = false;
        quitButton.interactable = false;

        multiplayerMenuTitle.text = "Searching for a game";

        PhotonNetwork.JoinRandomRoom();
    }
    private void CreateRoom()
    {
        int randomRoomNumber = UnityEngine.Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 2,
            EmptyRoomTtl = 0,
            PlayerTtl = 0
        };
        PhotonNetwork.CreateRoom(roomName: "Room" + randomRoomNumber, roomOptions);
    }

    #endregion

    #region Unity Functions
    private void Start()
    {
        _instance = this;
        ConnectingToPhotonServersAtStart();
    }
    #endregion
}
