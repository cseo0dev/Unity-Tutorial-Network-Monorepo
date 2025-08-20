using System.Collections;
using Photon.Pun;
using StarterAssets;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Find_PlayerController : MonoBehaviourPun
{
    private Animator anim;
    private Camera mainCamera;

    [SerializeField] private Transform playerRoot;
    [SerializeField] private TextMeshPro nickNameUI;

    [SerializeField] private GameObject punchBox;
    [SerializeField] private GameObject kickBox;


    private bool isAttack = false;
    private bool isDead = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            nickNameUI.text = PhotonNetwork.NickName;
            nickNameUI.color = Color.green;

            var followCamera = FindFirstObjectByType<CinemachineCamera>();
            followCamera.Target.TrackingTarget = playerRoot;
        }
        else
        {
            nickNameUI.text = photonView.Owner.NickName;
            nickNameUI.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

            GetComponent<PlayerInput>().enabled = false;
        }
    }

    void OnPunch()
    {
        if (!isAttack && !isDead)
            photonView.RPC(nameof(RPC_Punch), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_Punch()
    {
        StartCoroutine(PunchRoutine());
    }

    IEnumerator PunchRoutine()
    {
        isAttack = true;
        anim.SetTrigger("Punch");
        yield return new WaitForSeconds(0.5f);
        punchBox.SetActive(true);

        yield return new WaitForSeconds(0.3f);
        punchBox.SetActive(false);
        isAttack = false;
    }

    void OnKick()
    {
        if (!isAttack || !isDead)
            photonView.RPC(nameof(RPC_Kick), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_Kick()
    {
        StartCoroutine(KickRoutine());
    }

    IEnumerator KickRoutine()
    {
        isAttack = true;
        anim.SetTrigger("Kick");
        yield return new WaitForSeconds(0.6f);
        kickBox.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        kickBox.SetActive(false);
        isAttack = false;
    }

    public void GetHit()
    {
        photonView.RPC(nameof(Dead), RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void Dead()
    {
        isDead = true;
        anim.SetTrigger("Death");
        GetComponent<CharacterController>().enabled = false;
        GetComponent<Collider>().enabled = false;
        GetComponent<ThirdPersonController>().enabled = false;

        if (photonView.IsMine)
        {
            mainCamera.cullingMask |= (1 << 9);
            Find_GameManager.Instance.SetObserver();
        }
    }

    [PunRPC]
    public void Winner(string nickName)
    {
        Find_GameManager.Instance.EndGame(nickName);
    }
}