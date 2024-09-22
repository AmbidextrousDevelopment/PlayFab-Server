using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Destroys Photon Instance gameObjects
public class PhotonDestroyer : MonoBehaviour
{
    private IEnumerator Routine_DisconnectAndDestroyPhotonRelatedObjects()
    {
        while (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            yield return new WaitForSeconds(seconds: 0.02f); //instead will be infinite loop
        }

        //running until disconnected
        while (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            yield return new WaitForSeconds(seconds: 0.02f); //instead will be infinite loop
        }

        if (PhotonRoomScript.instance)
        {
            Destroy(PhotonRoomScript.instance.gameObject);
        }
        if (MultiplayerSettings.instance)
        {
            Destroy(MultiplayerSettings.instance.gameObject);
        }
    }
    void Start()
    {
        StartCoroutine(Routine_DisconnectAndDestroyPhotonRelatedObjects());
    }
  
}
