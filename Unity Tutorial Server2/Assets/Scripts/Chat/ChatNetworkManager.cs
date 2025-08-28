using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatNetworkManager : MonoBehaviour
{
    public static ChatNetworkManager Instance { get; private set; }

    private Socket client_socket;
    private byte[] receive_buffer = new byte[1024];

    private List<byte> incomplete_packetBuffer = new List<byte>();

    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button connectButton;
    [SerializeField] private Button sendButton;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        connectButton.onClick.AddListener(Connect);
        sendButton.onClick.AddListener(Send);
    }

    public void Connect()
    {
        try
        {
            client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 7979);

            Debug.Log("Connecting to server...");
            client_socket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), null);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void ConnectCallback(IAsyncResult AR)
    {
        try
        {
            client_socket.EndConnect(AR);
            Debug.Log("Connected Successfully!");

            // 서버가 클라이언트에게 보내기
            client_socket.BeginReceive(receive_buffer, 0, receive_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    // 서버가 클라이언트에게 보내기
    private void ReceiveCallback(IAsyncResult AR)
    {
        try
        {
            int bytesRead = client_socket.EndReceive(AR);

            if (bytesRead > 0)
            {
                for (int i = 0; i < bytesRead; i++)
                {
                    incomplete_packetBuffer.Add(receive_buffer[i]);
                }

                ProcessReceivedData();

                client_socket.BeginReceive(receive_buffer, 0, receive_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
            }
            else
            {
                Debug.Log("Server disConnected.");
                DisConnect();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Receive failed: {e.Message}");
            DisConnect();
        }
    }

    // 서버가 클라이언트에게 보내기
    private void ProcessReceivedData()
    {
        while (true)
        {
            if (incomplete_packetBuffer.Count < Defines.HEADERSIZE)
            {
                return;
            }

            short bodySize = BitConverter.ToInt16(incomplete_packetBuffer.ToArray(), 0);

            if (incomplete_packetBuffer.Count < Defines.HEADERSIZE + bodySize)
            {
                return;
            }

            byte[] completedMessage = new byte[bodySize];
            incomplete_packetBuffer.CopyTo(Defines.HEADERSIZE, completedMessage, 0, bodySize);

            string received_text = Encoding.UTF8.GetString(completedMessage);
            Debug.Log($"[Echo from Server] {received_text}");

            incomplete_packetBuffer.RemoveRange(0, Defines.HEADERSIZE + bodySize);
        }
    }

    public void Send()
    {
        if (client_socket == null || !client_socket.Connected)
        {
            Debug.Log("Not Connected to server");
            return;
        }

        string message = inputField.text;

        Packet packet = new Packet();
        packet.Push(message);
        packet.RecordSize();

        byte[] dataToSend = new byte[packet.position];
        Array.Copy(packet.buffer, 0, dataToSend, 0, packet.position);

        client_socket.BeginSend(dataToSend, 0, dataToSend.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
    }

    private void SendCallback(IAsyncResult AR)
    {
        try
        {
            int bytesSent = client_socket.EndSend(AR);

            Debug.Log($"Sent {bytesSent} bytes to server");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void DisConnect()
    {
        if (client_socket != null && client_socket.Connected)
        {
            client_socket.Shutdown(SocketShutdown.Both);
            client_socket.Close();
        }

        client_socket = null;
    }

    void OnApplicationQuit()
    {
        DisConnect();
    }
}