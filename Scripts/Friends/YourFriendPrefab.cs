using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab.ClientModels;
using PlayFab;
using Newtonsoft.Json.Linq;

public class YourFriendPrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI username;
    [SerializeField] private Button deleteFriendButton;
    [SerializeField] private TextMeshProUGUI processingText;
    [SerializeField] private TextMeshProUGUI statusTextReference;

    private string friendPlayfabId;
    ///<summary>
    ///A function that will set name of the username and status of the friend. If friend has confirmed
    /// then status will be "confirmed". Otherwise will be "pending"
    ///</summary>
    public void SetFriendAndPlayfabIdAndStatusName(string name, string playfabId, string status)
    {
        username.text = name;
        friendPlayfabId = playfabId;
        statusTextReference.text = status;
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
        deleteFriendButton.gameObject.SetActive(false);
        processingText.gameObject.SetActive(true);
    }

    private void Start()
    {
        deleteFriendButton.onClick.AddListener(OnRejectFriendRequestButtonClicked);
    }
}
