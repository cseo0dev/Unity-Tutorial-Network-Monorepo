using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Simple_GameManager : MonoBehaviour
{
    // 방에 입장하기 전에 작동되면 에러
    IEnumerator Start()
    {
        yield return null; // 동기화 과정에서의 타이밍 조절용?

        PhotonNetwork.Instantiate("Player", Vector3.up, Quaternion.identity);
    }
}
