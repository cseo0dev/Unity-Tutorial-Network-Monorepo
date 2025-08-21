using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SnakeController : NetworkBehaviour
{
    [SerializeField] private GameObject tailPrefab;
    
    // SyncVar : ����� ����Ǹ� ����ȭ���ִ� ���
    [SyncVar]
    private Transform coin;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float turnSpeed = 120f;
    [SerializeField] private float lerpSpeed = 5f;

    // SyncList : �߰� / ���� / ������ �� ����ȭ���ִ� ���
    private SyncList<Transform> tails = new SyncList<Transform>();

    [Server] // Photon���� IsMasterClient�� ������ ���
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
            // ���� ȹ�� 
            AddTail();

            // ���� �̵�
            MoveCoin();
        }
    }

    private void MoveHead()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        float h = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.forward * h * -turnSpeed * Time.deltaTime);
    }

    [Server] // ���������� ȣ��Ǵ� �Լ�
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

        NetworkServer.Spawn(newTail, connectionToClient); // ����� ��� Ŭ���̾�Ʈ���� newTail ���� �˸�

        tails.Add(newTail.transform);
    }

    private void MoveTail() // ������ ����ٴϴ� ���
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
