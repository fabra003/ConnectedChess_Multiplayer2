using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class NetworkGameManager : MonoBehaviour
{
    public static NetworkGameManager Instance { get; private set; } // ✅ Singleton accessor

    [Header("Player Prefab")]
    public GameObject playerPrefab;

    private List<ulong> connectedClients = new List<ulong>();

    public PlayerNetwork.PlayerSide CurrentTurnSide { get; private set; } = PlayerNetwork.PlayerSide.White; // ✅ Turn tracking

    private void Awake()
    {
        // ✅ Initialize the singleton instance
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public void StartHost()
    {
        RegisterPrefabs();
        NetworkManager.Singleton.StartHost();
        Debug.Log("Host started");
    }

    public void StartClient()
    {
        RegisterPrefabs();
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

        connectedClients.Clear(); // optional: clear list to allow reconnect
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

        if (NetworkManager.Singleton.IsServer)
        {
            connectedClients.Add(clientId);

            GameObject playerObj = Instantiate(playerPrefab);
            var netObj = playerObj.GetComponent<NetworkObject>();
            var playerNetwork = playerObj.GetComponent<PlayerNetwork>();

            // Assign player side based on join order
            if (connectedClients.Count == 1)
            {
                playerNetwork.assignedSide.Value = PlayerNetwork.PlayerSide.White;
                CurrentTurnSide = PlayerNetwork.PlayerSide.White; // ✅ Set initial turn
            }
            else
            {
                playerNetwork.assignedSide.Value = PlayerNetwork.PlayerSide.Black;
            }

            netObj.SpawnAsPlayerObject(clientId);

            Debug.Log($"Assigned {playerNetwork.assignedSide.Value} to client {clientId}");
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} has disconnected.");

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.LogWarning("You were disconnected from the session.");
        }

        connectedClients.Remove(clientId);
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

    private void RegisterPrefabs()
    {
        NetworkManager.Singleton.AddNetworkPrefab(playerPrefab);
    }
}
