using UnityEngine;
using UnityEngine.UI;
using Utp;

public class ConnectManager : MonoBehaviour
{
    public RelayNetworkManager networkManager;

    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    void Start()
    {
        hostButton.onClick.AddListener(() =>
        {
            networkManager.StartRelayHost(2, null);
        });

        clientButton.onClick.AddListener(() =>
        {
            networkManager.JoinRelayServer();
        });
    }
}
