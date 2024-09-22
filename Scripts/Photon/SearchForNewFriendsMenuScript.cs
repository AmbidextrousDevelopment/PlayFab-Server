using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json.Linq;
using System;

public class SearchForNewFriendsMenuScript : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] Button searchButton;
    [SerializeField] Button backButton;

    private string _friendUsername;

    private IEnumerator Routine_MessageText(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        messageText.gameObject.SetActive(false);
    }
    public void OnFriendUserNameInputFieldChanged(string value)
    {
        _friendUsername = value;
    }

    private void OnClickSearchForFriendButton()
    {
        //deactivating buttons
        searchButton.interactable = false;
        backButton.interactable = false;

        if (string.IsNullOrEmpty(_friendUsername))
        {
            StartCoroutine(Routine_MessageText("Please Enter a Username"));

            searchButton.interactable = true;
            backButton.interactable = true;
            return;
        }


        StartCoroutine(Routine_MessageText("Processing your Request"));

        //API Request to Search a Friend
        var searchRequest = new GetAccountInfoRequest
        {
            Username = _friendUsername
        };

        PlayFabClientAPI.GetAccountInfo
            (
            searchRequest,
            result =>
            {
                var playfabID = result.AccountInfo.PlayFabId;
                

                SendRequestToPlayerThatWasSuccessfullyFound(playfabID);
              
            },
            error =>
            {
                StartCoroutine(Routine_MessageText(error.ErrorMessage));
                searchButton.interactable = true;
                backButton.interactable = true;
            }
            );
    }

    private void SendRequestToPlayerThatWasSuccessfullyFound(string playersPlayfabId)
    {
        //Request to execute cloud script
        var executeCloudScriptRequest = new ExecuteCloudScriptRequest
        {
            FunctionName = "ProcessFriendRequest",
            FunctionParameter = new { FriendId = playersPlayfabId },
            GeneratePlayStreamEvent = true
        };

        //Sending request
        PlayFabClientAPI.ExecuteCloudScript
        (
            executeCloudScriptRequest,
            result =>
            {
                searchButton.interactable = true;
                backButton.interactable = true;

                try
                {
                    //Getting result and message from server
                    var serializedResult = JObject.Parse
                    (
                     PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).
                     SerializeObject(result.FunctionResult)
                    );

                    print(serializedResult["message"]);
                    StartCoroutine(Routine_MessageText(serializedResult["message"]?.ToString()));
                }
                catch (Exception e)
                {
                    var serializedResult = JObject.Parse
                    (
                     PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).
                     SerializeObject(result.Logs?[0].Data)
                    );

                    StartCoroutine(Routine_MessageText(serializedResult["apiError"]?["errorMessage"]?.ToString()));
                    print(e.Message);

                    throw;
                }


                print("kinda success");                

            },
            error =>
            {
                StartCoroutine(Routine_MessageText(error.ErrorMessage));
                searchButton.interactable = true;
                backButton.interactable = true;
            }
        );

    }

    private void Start()
    {
        searchButton.onClick.AddListener(OnClickSearchForFriendButton);
    }

}
