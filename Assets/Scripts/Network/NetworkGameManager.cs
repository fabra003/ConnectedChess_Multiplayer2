using Unity.Netcode;
using UnityEngine;

public class NetworkGameManager : MonoBehaviour
{
    void Start()
    {
        if (NetworkManager.Singleton.IsHost)
            Debug.Log("This is the HOST.");
        if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
            Debug.Log("This is the CLIENT.");
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }
}
