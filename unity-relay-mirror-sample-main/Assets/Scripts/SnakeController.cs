using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SnakeController : NetworkBehaviour
{
    [SerializeField] private GameObject tailPrefab;
    
    // SyncVar : 대상이 변경되면 동기화해주는 기능
    [SyncVar]
    private Transform coin;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float turnSpeed = 120f;
    [SerializeField] private float lerpSpeed = 5f;

    // SyncList : 추가 / 삽입 / 삭제할 때 동기화해주는 기능
    private SyncList<Transform> tails = new SyncList<Transform>();

    [Server] // Photon에서 IsMasterClient와 동일한 기능
    public override void OnStartServer()
    {
        coin = GameObject.FindGameObjectWithTag("Coin").transform;
    }

    void Update()
    {
        if (isLocalPlayer)
            MoveHead();
    }

    void LateUpdate()
    {
        MoveTail();
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            // 코인 획득 
            AddTail();

            // 코인 이동
            MoveCoin();
        }
    }

    private void MoveHead()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        float h = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.forward * h * -turnSpeed * Time.deltaTime);
    }

    [Server] // 서버에서만 호출되는 함수
    private void MoveCoin()
    {
        if (coin == null)
            return;

        float ranX = Random.Range(-13f, 13f);
        float ranY = Random.Range(-10f, 10f);

        coin.position = new Vector3(ranX, ranY, 0);
    }

    [Server]
    private void AddTail()
    {
        GameObject newTail = Instantiate(tailPrefab);
        newTail.transform.position = transform.position;

        NetworkServer.Spawn(newTail, connectionToClient); // 연결된 모든 클라이언트에게 newTail 생성 알림

        tails.Add(newTail.transform);
    }

    private void MoveTail() // 꼬리가 따라다니는 기능
    {
        Transform target = transform;

        foreach (var tail in tails)
        {
            if (tail == null)
                continue;

            tail.position = Vector3.Lerp(tail.position, target.position, lerpSpeed * Time.deltaTime);
            tail.rotation = Quaternion.Lerp(tail.rotation, target.rotation, lerpSpeed * Time.deltaTime);

            target = tail;
        }
    }
}
