using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon.StructWrapping;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Connection Status Panel")]
    public Text ConnectionStatusText;

    [Header("Login UI Panel")]
    public InputField playerNameInput;
    public GameObject LoginUIPanels;

    [Header("Game Options Panel")]
    public GameObject gameOptionsPanel;

    [Header("Create Room Panel")]
    public GameObject createRoomPanel;
    public InputField roomNameInputField;
    public InputField playerCountInputField;

    [Header("Join Random Room Panel")]
    public GameObject joinRandomRoomPanel;

    [Header("Show Room List Panel")]
    public GameObject showRoomListPanel;

    [Header("inside Room Panel")]
    public GameObject insideRoomPanel;
    public Text roomInfoText;
    public GameObject playerListItemPrefab;
    public GameObject playerListViewParent;
    public GameObject startGameButton;

    [Header("Room List Panel")]
    public GameObject roomListPanel;
    public GameObject roomItemPrefab;
    public GameObject roomListParent;

    private Dictionary<string, RoomInfo> cacheedRoomList;
    private Dictionary<string, GameObject> roomListGameObjects;
    private Dictionary<int, GameObject> playerListGameObjects;

    #region Unity Functions
    
    void Start()
    {
        cacheedRoomList = new Dictionary<string, RoomInfo>();
        roomListGameObjects = new Dictionary<string, GameObject>();
        ActivatePanel(LoginUIPanels);

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Update()
    {
        ConnectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;
    }

    #endregion

    #region UI CallBacks

    public void OnLoginButtonClick()
    {
        string playerName = playerNameInput.text;

        if(string.IsNullOrEmpty(playerName)){
            Debug.Log("Player name is invalid");
        }
        else{
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void OnCreateRoomButtonClick()
    {
        string roomName = roomNameInputField.text;

        if(string.IsNullOrEmpty(roomName))
            roomName = "Room " + Random.Range(1000, 10000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)int.Parse(playerCountInputField.text);

        PhotonNetwork.CreateRoom(roomName, roomOptions); 
    }

    public void OnCancelButtonClick()
    {
        ActivatePanel(gameOptionsPanel);
    }
    
    public void OnShowRoomListButtonClick()
    {
        if(!PhotonNetwork.InLobby){
            PhotonNetwork.JoinLobby();
        }

        ActivatePanel(showRoomListPanel);
    }

    public void OnBackButtonClick()
    {
        if(PhotonNetwork.InLobby){
            PhotonNetwork.LeaveLobby();
        }

        ActivatePanel(gameOptionsPanel);
    }

    public void OnLeaveGameButtonClick()
    {
        PhotonNetwork.LeaveRoom();
    }
    
    public void OnJoinRandomRoomClick()
    {
        ActivatePanel(joinRandomRoomPanel);
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnStartGameButtonClick()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }

    #endregion

    #region PunCallBacks

    public override void OnConnected()
    {
        Debug.Log("connected to the internet!");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " Has connected to Photon server");
        ActivatePanel(gameOptionsPanel);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " created!");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has joined " + PhotonNetwork.CurrentRoom.Name);
        ActivatePanel(insideRoomPanel);

        roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount 
            + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        if(playerListGameObjects == null){
            playerListGameObjects = new Dictionary<int, GameObject>();
        }

        foreach (Player player in PhotonNetwork.PlayerList){
            GameObject playerItem = Instantiate(playerListItemPrefab);
            playerItem.transform.SetParent(playerListViewParent.transform);
            playerItem.transform.localScale = Vector3.one;

            playerItem.transform.Find("PlayerNameText").GetComponent<Text>().text = player.NickName;
            playerItem.transform.Find("PlayerIndicator").gameObject.SetActive(player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);

            playerListGameObjects.Add(player.ActorNumber, playerItem);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListGameObjects();

        startGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);

        foreach(RoomInfo info in roomList){
            Debug.Log(info.Name);

            if(!info.IsOpen || !info.IsVisible || info.RemovedFromList){
                if(cacheedRoomList.ContainsKey(info.Name)){
                    cacheedRoomList.Remove(info.Name);
                }
            }
            else{
                //update existing rooms info
                if(cacheedRoomList.ContainsKey(info.Name)){
                    cacheedRoomList[info.Name] = info;
                }
                else{
                    cacheedRoomList.Add(info.Name, info);
                }
            }
        }

        foreach(RoomInfo info in cacheedRoomList.Values){
            GameObject listItem = Instantiate(roomItemPrefab);
            listItem.transform.SetParent(roomListParent.transform);
            listItem.transform.localScale = Vector3.one;

            listItem.transform.Find("RoomNameText").GetComponent<Text>().text = info.Name;
            listItem.transform.Find("RoomPlayersText").GetComponent<Text>().text = "Player Count: " + info.PlayerCount 
                + " / " + info.MaxPlayers;
            
            listItem.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(() => OnJoinRoomClicked(info.Name));

            roomListGameObjects.Add(info.Name, listItem);
        }
    }

    public override void OnLeftLobby()
    {
        ClearRoomListGameObjects();
        cacheedRoomList.Clear();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount 
            + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        GameObject playerItem = Instantiate(playerListItemPrefab);
        playerItem.transform.SetParent(playerListViewParent.transform);
        playerItem.transform.localScale = Vector3.one;

        playerItem.transform.Find("PlayerNameText").GetComponent<Text>().text = newPlayer.NickName;
        playerItem.transform.Find("PlayerIndicator").gameObject.SetActive(newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);

        playerListGameObjects.Add(newPlayer.ActorNumber, playerItem);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        startGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);

        roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount 
            + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        Destroy(playerListGameObjects[otherPlayer.ActorNumber]);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);
    }

    public override void OnLeftRoom()
    {
        foreach(var gameObject in playerListGameObjects.Values){
            Destroy(gameObject);
        }
        playerListGameObjects.Clear();
        playerListGameObjects = null;
        ActivatePanel(gameOptionsPanel);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning(message);
        string roomName = "Room " + Random.Range(1000, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    #endregion

    #region Private Methods

    private void OnJoinRoomClicked(string roomName)
    {
        if(PhotonNetwork.InLobby){
            PhotonNetwork.LeaveLobby();
        }

        PhotonNetwork.JoinRoom(roomName);
    }

    private void ClearRoomListGameObjects()
    {
        foreach(var item in roomListGameObjects.Values){
            Destroy(item);
        }

        roomListGameObjects.Clear();
    }

    #endregion

    #region Public Methods

    public void ActivatePanel(GameObject panelToBeActivated)
    {
        LoginUIPanels.SetActive(panelToBeActivated.Equals(LoginUIPanels));
        gameOptionsPanel.SetActive(panelToBeActivated.Equals(gameOptionsPanel));
        createRoomPanel.SetActive(panelToBeActivated.Equals(createRoomPanel));
        joinRandomRoomPanel.SetActive(panelToBeActivated.Equals(joinRandomRoomPanel));
        showRoomListPanel.SetActive(panelToBeActivated.Equals(showRoomListPanel));
        insideRoomPanel.SetActive(panelToBeActivated.Equals(insideRoomPanel));
        roomListPanel.SetActive(panelToBeActivated.Equals(roomListPanel));
    }

    #endregion
}
