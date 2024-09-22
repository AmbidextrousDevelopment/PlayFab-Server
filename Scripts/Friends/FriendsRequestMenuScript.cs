using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FriendsRequestMenuScript : MonoBehaviour
{
    [SerializeField] private FriendsRequestPrefab friendsRequestPrefab;
    [SerializeField] private GameObject friendRequestContent;
    [SerializeField] private TextMeshProUGUI friendRequestMenuTitleText;

    private void DestroyAllChildrenInScrollViewContent()
    {
        if (friendRequestContent.transform.childCount == 0) return;
        for (int i = 0; i < friendRequestContent.transform.childCount; i++)
        {
            Destroy(friendRequestContent.transform.GetChild(i).gameObject);
        }
    }

    private void GetFriendsAndLoadThemOnScrollView()
    {
        var getFriendRequest = new GetFriendsListRequest();

        PlayFabClientAPI.GetFriendsList
        (
            getFriendRequest,
            result => 
            {
                //Loading all of the friends
                foreach (FriendInfo friendInfo in result.Friends)
                {
                    //Only loading friends that have requester tag, as these are the friends, that send request
                    if (friendInfo.Tags[0] == "requester")
                    {
                        var friendRequestPrefab = Instantiate(this.friendsRequestPrefab,
                            friendRequestContent.transform).gameObject;

                        //Getting friendRequestPrefab Script
                        var friendRequestPrefabScript = friendRequestPrefab.GetComponent<FriendsRequestPrefab>();

                        //Setting friendRequest Prefab Script Variables
                        friendRequestPrefabScript.SetFriendNameAndPlayfabId(friendInfo.Username, friendInfo.FriendPlayFabId);

                    }
                }
            },
            error =>
            {
                friendRequestMenuTitleText.text = error.ErrorMessage;
            }
        );
    }

    private void OnEnable()
    {
        friendRequestMenuTitleText.text = "Friends Request";
        DestroyAllChildrenInScrollViewContent();
        GetFriendsAndLoadThemOnScrollView();
    }
}
