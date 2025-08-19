using System.Collections;
using Photon.Pun;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Fight_PlayerController : MonoBehaviourPun
{
    private Animator anim;

    [SerializeField] private TextMeshPro nickName;
    [SerializeField] private Transform playerRoot;

    [SerializeField] private GameObject punchBox;
    [SerializeField] private GameObject kickBox;

    private bool isAttack = false; //PlayerInput을 사용하기 전에 필요했음
    private PlayerInput playerInput;

    [SerializeField] private Image hpBar;
    private float currentHp = 100f;
    private float maxHp = 100f;

    public bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();

        currentHp = maxHp;

        if (photonView.IsMine)
        {
            var followCamera = FindFirstObjectByType<CinemachineCamera>();
            followCamera.Target.TrackingTarget = playerRoot;

            nickName.text = PhotonNetwork.NickName;
            nickName.color = Color.green;
        }
        else
        {
            nickName.text = photonView.Owner.NickName;
            nickName.color = Color.red;

            playerInput.enabled = false;
        }
    }

    void OnPunch()
    {
        if (!isAttack && !isDead)
            photonView.RPC("Attack", RpcTarget.All, "Punch", 0.5f, 0.3f, 0);
    }

    void OnKick()
    {
        if (!isAttack && !isDead)
            photonView.RPC("Attack", RpcTarget.All, "Kick", 0.6f, 0.2f, 1);
    }

    #region 코루틴 통합 전
    //IEnumerator PunchRoutine()
    //{
    //    isAttack = true;
    //    anim.SetTrigger("Punch");
    //    yield return new WaitForSeconds(0.5f);
    //    punchBox.SetActive(true);

    //    yield return new WaitForSeconds(0.3f);
    //    punchBox.SetActive(false);
    //    isAttack = false;
    //}

    //IEnumerator KickRoutine()
    //{
    //    isAttack = true;

    //    anim.SetTrigger("Kick");
    //    yield return new WaitForSeconds(0.6f);
    //    kickBox.SetActive(true);

    //    yield return new WaitForSeconds(0.2f);
    //    kickBox.SetActive(false);
    //    isAttack = false;
    //}
    #endregion

    [PunRPC]
    private void Attack(string parameter, float playTime, float endTime, int hitBoxIndex)
    {
        StartCoroutine(AttackRoutine(parameter, playTime, endTime, hitBoxIndex));
    }

    IEnumerator AttackRoutine(string parameter, float playTime, float endTime, int hitBoxIndex)
    {
        GameObject hitBox = hitBoxIndex == 0 ? punchBox : kickBox;

        isAttack = true;
        anim.SetTrigger(parameter);

        yield return new WaitForSeconds(playTime);
        hitBox.SetActive(true);

        yield return new WaitForSeconds(endTime);
        hitBox.SetActive(false);
        isAttack = false;
    }

    [PunRPC]
    private void TriggerEvent(int viewID, float damage)
    {
        PhotonView targetPv = PhotonView.Find(viewID);

        if (targetPv != null)
            targetPv.GetComponent<Fight_PlayerController>().GetDamage(damage);
    }

    public void GetDamage(float damage)
    {
        currentHp -= damage;

        hpBar.fillAmount = currentHp / maxHp;

        if (currentHp <= 0f)
        {
            if (photonView.IsMine)
            {
                isDead = true;
                anim.SetTrigger("Death");
                GetComponent<CharacterController>().enabled = false;
                Fight_GameManager.Instance.EndGame();
            }
        }
    }
}
