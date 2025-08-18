using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1";

    [SerializeField] private TMP_InputField nickNameField;
    [SerializeField] private Button connectButton;

    void Awake()
    {
        Screen.SetResolution(1920, 1080, false); // 해상도 설정, false = Full Screen 사용 여부
        PhotonNetwork.SendRate = 60; // 내 컴퓨터 게임 정보에 대한 전송률
        PhotonNetwork.SerializationRate = 30; // Photon View 관측 중인 대상에 대한 전송률
        PhotonNetwork.GameVersion = gameVersion;
    }

    void Start()
    {
        connectButton.onClick.AddListener(Connect);
    }

    private void Connect()
    {
        PhotonNetwork.NickName = nickNameField.text;

        PhotonNetwork.ConnectUsingSettings(); // App ID 기반으로 접속
        Debug.Log("서버 접속");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 20 }, null);
        Debug.Log("서버 접속 완료");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(1);
        Debug.Log("방 입장");
    }
}
