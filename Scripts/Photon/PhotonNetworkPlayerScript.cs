using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

//A simple script that will spawn player avatar on network
public class PhotonNetworkPlayerScript : MonoBehaviour, IPunObservable
{
    #region Photon Network Player Canvas
    [Header("Photon Network Player Canvas")]
    [SerializeField] private GameObject disconnectionCanvas;

    //Runs when player is disconnected, called by PhotonRoomScript
    public void EnableDisconnectionCanvasWhenPlayerLeaves()
    {
        print("Player left the room or disconnected");
        disconnectionCanvas.SetActive(true);
        StartCoroutine(methodName: nameof(Routine_ReturnPlayerToMainMenuWhenPlayerLeaves));
    }

    private IEnumerator Routine_ReturnPlayerToMainMenuWhenPlayerLeaves()
    {
        yield return new WaitForSeconds(seconds: 3.0f);

        //PhotonNetwork.LoadLevel(1);
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
    #endregion

    #region Photon Network Player Number
    // Reference to number in room
    public int playerNumber;

    //Sets the number of the Player
    public void SetPlayerNumber(int numberToSet)
    {
        playerNumber = numberToSet;

        thisPhotonView.RPC("RPC_SetPlayerNumberAcrossTheNetwork", RpcTarget.OthersBuffered, playerNumber);
    }

    //Sets number of player on foreign players
    [PunRPC]
    private void RPC_SetPlayerNumberAcrossTheNetwork(int numberToSet)
    {
        playerNumber = numberToSet;
    }
    #endregion

    #region Avatar Spawning
    [Header("Photon Network Player Variables")]

    public GameObject myAvatar;

    [SerializeField] private PhotonView thisPhotonView;

    //Spawns Player avatar across network

    private void SpawnPlayerAvatar()
    {
        // Only runs this function for local player
        if (!thisPhotonView.IsMine) return;

        // Ensure there's no existing avatar before creating a new one
        /*if (myAvatar != null)
        {
            PhotonNetwork.Destroy(myAvatar);
        }*/

        myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"), 
            Vector2.zero, Quaternion.identity, group:0);
    }
    #endregion
    private void Start()
    {
        SpawnPlayerAvatar();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //print("just for test");
    }
}
