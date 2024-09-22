//Sets up Player Win/Loss Data for the first time
handlers.setupWinLossData = function()
{
    //Setting up Win/Loss Dictionary
    var winLossDictionary = {
      "Wins" : 0,
      "Losses" : 0
    };
    //Setting JSON to upload
    var playerDataToUpdate = JSON.stringify(winLossDictionary);
    
    //Setting User Data
    var updateDataRequest = server.UpdateUserData({
        PlayFabId: currentPlayerId,
        Data:{
            "PlayerData": playerDataToUpdate
        }
    });
    
    return {success: true, message: "Player Win/Loss Data is ready on server"};
}

//Function to Update Level 1 Player time
handlers.updateLevelOnePlayerTimeStat = function(args, context)
{
    //Storing the time in seconds
    var timeInSeconds = args.timeInSeconds;
    
    //Updating Level 1 Record Time Start
    var updateWinRecord = server.UpdatePlayerStatistics({
       PlayFabId: currentPlayerId,
       Statistics: [{
           StatisticName: "Level 1 Record Times",
           value: timeInSeconds
       }]
    });
    
    return {success: true, message: "Level 1 time record was updated"};
}
//Function to Update Level 2 Player time
handlers.updateLevelTwoPlayerTimeStat = function(args, context)
{
    //Storing the time in seconds
    var timeInSeconds = args.timeInSeconds;
    
    //Updating Level 1 Record Time Start
    var updateWinRecord = server.UpdatePlayerStatistics({
       PlayFabId: currentPlayerId,
       Statistics: [{
           StatisticName: "Level 2 Record Times",
           value: timeInSeconds
       }]
    });
    
    return {success: true, message: "Level 2 time record was updated"};
}

//Adds one win to Player Data
handlers.addOneWinToPlayerData = function()
{
    //Getting Player Data from server
    var playerData = server.GetUserData({
        PlayFabId: currentPlayerId,
        Key: ["PlayerData"],
    });
    
    //Converting Data and adding One Win
    var playerDataAsJson = JSON.parse(playerData.Data.PlayerData.Value);
    playerDataAsJson.Wins = playerDataAsJson.Wins + 1;
    
    //Converting changed Data to a string to prepare for upload
    var playerDataToUpdate = JSON.stringify(playerDataAsJson);
    
    //Updating the win record
    var updateWinRecord = server.UpdatePlayerStatistics({
       PlayFabId: currentPlayerId,
       Statistics: [{
           StatisticName: "Win Record",
           value: playerDataAsJson.Wins
       }]
    });
    
     //Setting User Data
    var updateDataRequest = server.UpdateUserData({
        PlayFabId: currentPlayerId,
        Data:{
            "PlayerData": playerDataToUpdate
        }
    });
    
    return {success: true, message: "One win was added to Player's Data"};
}

//Adds one loss to Player Data
handlers.addOneLossToPlayerData = function()
{
    //Getting Player Data from server
    var playerData = server.GetUserData({
        PlayFabId: currentPlayerId,
        Key: ["PlayerData"],
    });
    
    //Converting Data and adding One Loss
    var playerDataAsJson = JSON.parse(playerData.Data.PlayerData.Value);
    playerDataAsJson.Losses = playerDataAsJson.Losses + 1;
    
    //Converting changed Data to a string to prepare for upload
    var playerDataToUpdate = JSON.stringify(playerDataAsJson);
    
     //Setting User Data
    var updateDataRequest = server.UpdateUserData({
        PlayFabId: currentPlayerId,
        Data:{
            "PlayerData": playerDataToUpdate
        }
    });
    
    return {success: true, message: "One loss was added to Player's Data"};
}

//Called when user wants to add friend
handlers.ProcessFriendRequest = function(args, context)
{
    //Adding the friend first
    server.AddFriend({
        PlayFabId: currentPlayerId,
        FriendPlayFabId: args.FriendId
    });
    
    //Setting the tag for requestee
    server.SetFriendTags({
        PlayFabId: currentPlayerId,
        FriendPlayFabId: args.FriendId,
        Tags: ["requestee"],
    });
    
    //Adding the current user as friend of requested Player
    server.AddFriend({
        PlayFabId: args.FriendId,
        FriendPlayFabId: currentPlayerId
    });
    
    //Setting the tag for the Requester
    server.SetFriendTags({
        PlayFabId: args.FriendId,
        FriendPlayFabId: currentPlayerId,
        Tags: ["requester"]
    });
    
     return {success: true, message: "Friend Request was sent"};
}

//Function to accept friend Request
handlers.AcceptFriendRequest = function(args, context)
{
    //Confirming friend for player that hit accept friend button
    server.SetFriendTags({
        PlayFabId: currentPlayerId,
        FriendPlayFabId: args.FriendId,
        Tags:["confirmed"],
    });
    
    //Accepting friend request for other player as well
    server.SetFriendTags({
        PlayFabId: args.FriendId,
        FriendPlayFabId: currentPlayerId,
        Tags:["confirmed"],
    });
    return {success: true, message: "Friend Request Accepted"};
}

//Function to reject friend Request
handlers.RejectFriendRequest = function(args, context)
{
    //Removing friend for player that pushed button first
    server.RemoveFriend({
        PlayFabId: currentPlayerId,
        FriendPlayFabId: args.FriendId,
    });
    
    //Removing friend for other player
    server.RemoveFriend({
        PlayFabId: args.FriendId,
        FriendPlayFabId: currentPlayerId,
    });
    return {success: true, message: "Friend Request Rejected"};
}