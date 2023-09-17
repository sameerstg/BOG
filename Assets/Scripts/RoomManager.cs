using Cinemachine;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager _instance;
    PhotonView photonViewComponent;
    GameManager gm;
    public TextMeshProUGUI pingText;

    public CinemachineTargetGroup targetGroup;
    public List<PlayerDetails> players = new();

    public bool DidTimeout { private set; get; }
    static readonly RoomOptions s_RoomOptions = new RoomOptions
    {
        MaxPlayers = 4,
        EmptyRoomTtl = 5,
        PublishUserId = true,

    };

    void Awake()
    {
        _instance = this;
        targetGroup = GetComponentInChildren<CinemachineTargetGroup>();
        Assert.AreEqual(1, FindObjectsOfType<RoomManager>().Length);
        //photonViewComponent = GetComponent<PhotonView>();

    }
    private void Start()
    {
        PhotonNetwork.Disconnect();
        StartCoroutine(DoJoinOrCreateRoom("stg1"));
    }
    #region All Room Settings
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("Created Room");
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        PhotonNetwork.Instantiate("Player Holder", new Vector3(), Quaternion.identity, 0, new object[] { PhotonNetwork.LocalPlayer.UserId, "newusername" });
    }

    public void JoinOrCreateRoom(string preferredRoomName)
    {
        //Debug.LogError(preferredRoomName);
        StopAllCoroutines();
        const float timeoutSeconds = 15f;
        StartCoroutine(DoCheckTimeout(timeoutSeconds));
        StartCoroutine(DoJoinOrCreateRoom(preferredRoomName));
    }

    IEnumerator DoCheckTimeout(float timeout)
    {
        DidTimeout = false;
        while (!PhotonNetwork.InRoom && timeout >= 0)
        {
            yield return null;
            timeout -= Time.deltaTime;
        }

        if (timeout <= 0)
        {
            DidTimeout = true;
            StopAllCoroutines();
        }

    }



    static IEnumerator DoJoinOrCreateRoom(string preferredRoomName)
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connecting to server");
            PhotonNetwork.ConnectUsingSettings();
        }

        while (!PhotonNetwork.IsConnectedAndReady)
        {

            yield return null;
        }

        if (!PhotonNetwork.InLobby && PhotonNetwork.NetworkClientState != ClientState.JoiningLobby)
            Debug.Log($"Connecting to server ,state ={PhotonNetwork.NetworkClientState}");
        {
            PhotonNetwork.JoinLobby();

        }

        while (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer)
        {

            yield return null;
        }


        if (preferredRoomName != null)
        {
            Debug.Log("Joining or creating Room");

            bool isJoined = PhotonNetwork.JoinOrCreateRoom(preferredRoomName, s_RoomOptions, TypedLobby.Default);
        }
        else
        {
            Debug.Log("Joined Random Room");

            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Room Created on joining fail");
        PhotonNetwork.CreateRoom(null, s_RoomOptions, TypedLobby.Default);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Created on joining fail");
        PhotonNetwork.CreateRoom(null, s_RoomOptions, TypedLobby.Default);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        var leavingPlayer = players.Find(x => x.id == otherPlayer.UserId);
        if (leavingPlayer != null)
        {
            targetGroup.RemoveMember(leavingPlayer.player.transform);
            players.Remove(leavingPlayer);
        }

    }
    #endregion
    #region Synchronization Code
    IEnumerator SynchroniseGame()
    {
        while (PhotonNetwork.IsConnected)
        {
            yield return new WaitForSeconds(1);
            /*try
            {

            pingText.text = PhotonNetwork.GetPing().ToString();
            }
            catch (System.Exception)
            {

                
            }*/
        }
    }

    #endregion

}
[System.Serializable]
public class PlayerDetails
{
    public string id;
    public string name;
    public GameObject player;

    public PlayerDetails(string id, string name, GameObject player)
    {
        this.id = id;
        this.name = name;
        this.player = player;
    }
}
