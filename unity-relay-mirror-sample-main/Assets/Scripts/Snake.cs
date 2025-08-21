using Mirror;
using UnityEngine;

public class Snake : NetworkBehaviour
{
    [SerializeField] private GameObject tailPrefab;
    [SerializeField] private MeshRenderer headRenderer;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float turnSpeed = 120f;
    [SerializeField] private float lerpSpeed = 5f;
    [SerializeField] private float tailOffset = 0.5f;

    private SyncList<GameObject> tails = new SyncList<GameObject>();

    [SyncVar(hook = nameof(OnDeathStateChanged))]
    private bool isDead = false;

    public override void OnStartClient()
    {
        base.OnStartClient();
        tails.Callback += OnTailUpdated;
    }

    public override void OnStartLocalPlayer()
    {
        headRenderer.material.color = new Color(0.8f, 1f, 0.8f);
    }

    void Update()
    {
        if (isLocalPlayer && !isDead)
            MoveHead();
    }

    void LateUpdate()
    {
        MoveTail();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer && !isDead)
            return;

        if (other.CompareTag("Coin"))
        {
            GetCoin();
        }
        
        if (other.CompareTag("Tail"))
        {
            Tail tail = other.GetComponent<Tail>();
            if(tail.ownerIdentity != netIdentity) // 자기 꼬리에는 죽지 않게
                Died();
        }
    }

    private void MoveHead()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        float h = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.forward * h * -turnSpeed * Time.deltaTime);
    }

    private void MoveTail()
    {
        Transform target = transform;

        foreach (var tail in tails)
        {
            if (tail != null)
            {
                tail.transform.position = Vector3.Lerp(tail.transform.position, target.position, lerpSpeed * Time.deltaTime);
                tail.transform.rotation = Quaternion.Lerp(tail.transform.rotation, target.rotation, lerpSpeed * Time.deltaTime);
                target = tail.transform;
            }
        }
    }

    void OnTailUpdated(SyncList<GameObject>.Operation op, int index, GameObject oldTail, GameObject newTail)
    {
        if (op == SyncList<GameObject>.Operation.OP_ADD && isLocalPlayer) // 추가돠었을 때
        {
            Transform target = transform;

            if (index > 0)
            {
                target = tails[index - 1].transform; // 꼬리쪽에 생성해라
            }

            newTail.transform.position = target.position - (target.up * tailOffset);
            newTail.transform.rotation = target.rotation;
        }
    }

    void OnDeathStateChanged(bool oldState, bool newState)
    {
        if (newState)
        {
            headRenderer.material.color = Color.gray;
        }
    }

    [Command]
    private void GetCoin()
    {
        GameManager.Instance.MoveCoin();
        AddTail();
    }

    [Server]
    private void AddTail()
    {
        Transform spawnTarget = transform;
        if (tails.Count > 0)
        {
            spawnTarget = tails[tails.Count - 1].transform;
        }

        Vector3 spawnPos = spawnTarget.position - (spawnTarget.up * tailOffset);
        Quaternion spawnRot = spawnTarget.rotation;

        GameObject newTail = Instantiate(tailPrefab, spawnPos, spawnRot);
        newTail.GetComponent<Tail>().ownerIdentity = netIdentity;

        NetworkServer.Spawn(newTail, connectionToClient);
        tails.Add(newTail);
    }

    [Command]
    private void Died()
    {
        isDead = true;
    }
}
