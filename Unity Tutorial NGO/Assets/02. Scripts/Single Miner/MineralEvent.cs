using UnityEngine;

public class MineralEvent : MonoBehaviour
{
    private MinerScoreManager scoreManager;

    void Start()
    {
        scoreManager = FindFirstObjectByType<MinerScoreManager>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            Debug.Log("���� ȹ��");
            scoreManager.AddScore();
            gameObject.SetActive(false);
        }
    }
}
