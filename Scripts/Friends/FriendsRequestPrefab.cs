using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json.Linq;

public class FriendsRequestPrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI username;
    [SerializeField] private Button acceptFriendRequest;
    [SerializeField] private Button rejectFriendRequest;
    [SerializeField] private TextMeshProUGUI processingText;

    private string friendPlayfabId;
    public void SetFriendNameAndPlayfabId(string name, string playfabID)
    {
        username.text = name;
        friendPlayfabId = playfabID;
    }
    private void OnAcceptFriendRequestButtonClicked()
    {
        AnyButtonClicked();

        //Request to execute cloud script
        var executeCloudScriptRequest = new ExecuteCloudScriptRequest
        {
            FunctionName = "AcceptFriendRequest",
            FunctionParameter = new { FriendId = friendPlayfabId },
            GeneratePlayStreamEvent = true
        };

        //Sending request
        PlayFabClientAPI.ExecuteCloudScript
        (
            executeCloudScriptRequest,
            result =>
            {
                //Getting result and message from server
                var serializedResult = JObject.Parse
                (
                 PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).
                 SerializeObject(result.FunctionResult)
                );

                Destroy(gameObject);
            },
            error =>
            {
                print(error.ErrorMessage);
            }
        );
    }
    private void OnRejectFriendRequestButtonClicked()
    {
        AnyButtonClicked();

        //Request to execute cloud script
        var executeCloudScriptRequest = new ExecuteCloudScriptRequest
        {
            FunctionName = "RejectFriendRequest",
            FunctionParameter = new { FriendId = friendPlayfabId },
            GeneratePlayStreamEvent = true
        };

        //Sending request
        PlayFabClientAPI.ExecuteCloudScript
        (
            executeCloudScriptRequest,
            result =>
            {
                //Getting result and message from server
                var serializedResult = JObject.Parse
                (
                 PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).
                 SerializeObject(result.FunctionResult)
                );

                Destroy(gameObject);
            },
            error =>
            {
                print(error.ErrorMessage);
            }
        );
    }

    private void AnyButtonClicked()
    {
        acceptFriendRequest.gameObject.SetActive(false);
        rejectFriendRequest.gameObject.SetActive(false);
        processingText.gameObject.SetActive(true);
    }

    private void Start()
    {
        acceptFriendRequest.onClick.AddListener(OnAcceptFriendRequestButtonClicked);
        rejectFriendRequest.onClick.AddListener(OnRejectFriendRequestButtonClicked);
    }
}
