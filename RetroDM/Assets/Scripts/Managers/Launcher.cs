using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks {
    public static Launcher Instance;
    private GameObject playerListItemPrefab;
    private GameObject startGameButton;
    private RoomOptions roomOptions;
    private List<RoomInfo> roomList;
    private Transform playerListContent;
    [SerializeField] private TMP_Text errorMessage;
    [SerializeField] private TMP_Text errorCode;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private GameObject roomListItemPrefab;
    [SerializeField] private GameObject SettingsMenu;
    [SerializeField] private Transform roomListContent;
    [SerializeField] private TMP_InputField roomNameInputField;

    private void Awake() => Instance = this;

    private void Start() {
        if (!PhotonNetwork.IsConnected) {
            PhotonNetwork.ConnectUsingSettings();
            MenuManager.Instance.OpenMenu("LoadingMenu");
        }

        if(!SettingsMenu.activeSelf) {
            SettingsMenu.SetActive(true);
            Settings.Instance.LoadSettings();
            SettingsMenu.SetActive(false);
        }
    }

    public override void OnConnectedToMaster() {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby() {
        MenuManager.Instance.OpenMenu("MainMenu");
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
        soundManager.Play("MenuMusic");
    }

    //public override void OnDisconnected(DisconnectCause cause) {
    //    switch (cause) {
    //        case DisconnectCause.DisconnectByClientLogic:
    //            break;
    //    }
    //}

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        foreach (Transform transform in roomListContent) {
            Destroy(transform.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++) {
            if (roomList[i].RemovedFromList) { continue; }
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().Setup(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(newPlayer);
    }

    public void StartGame() {
        PhotonNetwork.LoadLevel(1);
    }

    public void CreateRoom() {
        if (string.IsNullOrEmpty(roomNameInputField.text)) { return; }
        roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions);
        MenuManager.Instance.OpenMenu("LoadingMenu");
    }

    public void JoinRoom(RoomInfo info) {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("LoadingMenu");
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.JoinLobby();
        SceneManager.LoadScene("MainMenu");
        MenuManager.Instance.OpenMenu("LoadingMenu");
    }

    public override void OnJoinedRoom() {
        PhotonNetwork.LoadLevel(1);
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++) {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnLeftRoom() {
        MenuManager.Instance.OpenMenu("MainMenu");
    }

    public override void OnMasterClientSwitched(Player newMasterClient) {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        ShowError(returnCode, message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {
        ShowError(returnCode, message);
    }

    private void ShowError(short returnCode, string message) {
        MenuManager.Instance.OpenMenu("ErrorMenu");
        errorCode.text = "Code: " + returnCode.ToString();
        errorMessage.text = message;
    }
}
