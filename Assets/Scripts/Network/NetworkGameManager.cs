using Unity.Netcode;
using UnityEngine;

public class NetworkGameManager : MonoBehaviour
{
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("Host started");
    }

    public void StartClient()
    {
        bool result = NetworkManager.Singleton.StartClient();
        if (!result)
        {
            Debug.LogError("Client failed to connect. Is the host running?");
        }
        else
        {
            Debug.Log("Client started");
        }
    }

    public void LeaveSession()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("Host stopped and left session.");
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("Client left session.");
        }
    }

    public void RejoinSession()
    {
        bool result = NetworkManager.Singleton.StartClient();
        if (!result)
        {
            Debug.LogError("Rejoin failed. Is the host still running?");
        }
        else
        {
            Debug.Log("Rejoining as client...");
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected: {clientId}");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} has disconnected.");

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.LogWarning("You were disconnected from the session.");
        }
    }

    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }
}
