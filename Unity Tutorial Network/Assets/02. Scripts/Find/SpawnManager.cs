using System.Collections;
using Photon.Pun;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] npcPrefabs;
    [SerializeField] private int npcAmount = 10;
    [SerializeField] private Transform[] points;

    IEnumerator Start()
    {
        yield return null;

        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < npcAmount; i++)
            {
                int ranPoint = Random.Range(0, points.Length);
                
                int ranIndex = Random.Range(0, npcPrefabs.Length);
                Vector3 ranPos = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));

                var spawnPos = points[ranPoint].position + ranPos;
                
                GameObject npc = PhotonNetwork.Instantiate("Npc_" + ranIndex, spawnPos, Quaternion.identity);
                npc.transform.SetParent(transform);

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}