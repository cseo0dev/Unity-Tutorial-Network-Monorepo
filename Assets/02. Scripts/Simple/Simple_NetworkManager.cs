using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// MonoBehaviourPunCallbacks -> 매니저일 때 사용
// MonoBehaviourPun -> 못들음!
public class Simple_NetworkManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1";

    void Awake()
    {
        Screen.SetResolution(1920, 1080, false); // 해상도 설정, false = Full Screen 사용 여부
        PhotonNetwork.SendRate = 60; // 내 컴퓨터 게임 정보에 대한 전송률
        PhotonNetwork.SerializationRate = 30; // Photon View 관측 중인 대상에 대한 전송률
        PhotonNetwork.GameVersion = gameVersion;
    }

    void Start()
    {
        Connect();
    }

    private void Connect()
    {
        PhotonNetwork.ConnectUsingSettings(); // App ID 기반으로 접속
        Debug.Log("서버 접속");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 20}, null);
        Debug.Log("서버 접속 완료");
    }

    public override void OnJoinedRoom()
    {
        // 네트워크 상에 생성 (/Assets/Resource 폴더에 있는 "Player" 이름의 오브젝트 생성)
        PhotonNetwork.Instantiate("Player", Vector3.up, Quaternion.identity);

        Debug.Log("캐릭터 생성");
    }
}
