using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class Find_GameManager : Singleton<Find_GameManager>
{
    [SerializeField] private GameObject observerCamera;

    [SerializeField] private TextMeshProUGUI scoreUI;
    [SerializeField] private TextMeshProUGUI winnerUI;
    [SerializeField] private Transform[] spawnPoints;

    private int score;
    [SerializeField] private int winnerScore = 2;
    
    IEnumerator Start()
    {
        float randomTime = Random.Range(0f, 1f);
        yield return new WaitForSeconds(randomTime);
        
        int ranPoint = Random.Range(0, spawnPoints.Length);
        int ranIndex = Random.Range(0, 5); // 0, 1, 2, 3, 4
        
        var randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        var spawnPos = spawnPoints[ranPoint].position + randomPos;
        
        PhotonNetwork.Instantiate("Player_" + ranIndex, spawnPos, Quaternion.identity);

        scoreUI.text = $"현재 스코어는 0 입니다.";
    }

    public void SetObserver()
    {
        observerCamera.SetActive(true);
    }

    public bool SetScore()
    {
        score++;
        scoreUI.text = $"현재 스코어는 {score} 입니다.";

        if (score >= winnerScore)
            return true;

        return false;
    }
    
    public void EndGame(string nickName)
    {
        Fade.onFadeAction(3f, Color.white, true, () =>
        {
            winnerUI.text = $"{nickName}이(가) 승자입니다!!";
            winnerUI.gameObject.SetActive(true);
        });
    }
}