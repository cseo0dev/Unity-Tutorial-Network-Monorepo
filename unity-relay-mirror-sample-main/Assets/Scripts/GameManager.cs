using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SerializeField] private Transform coin;

    [SyncVar(hook = nameof(OnCoinPositionChanged))]
    public Vector3 coinPosition;

    void Awake()
    {
        Instance = this;
    }

    public override void OnStartServer()
    {
        MoveCoin();
    }

    public void MoveCoin()
    {
        float ranX = Random.Range(-20f, 20f);
        float ranY = Random.Range(-10f, 10f);

        coinPosition = new Vector3(ranX, ranY, 0);
    }

    private void OnCoinPositionChanged(Vector3 prevPos, Vector3 newPos)
    {
        coin.position = newPos;
    }
}
