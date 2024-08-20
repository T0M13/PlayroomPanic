using UnityEngine;
using Unity.Netcode;

public class SimpleNetworkManager : MonoBehaviour
{
    // Start the Host
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    // Start the Client
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    // Start the Server
    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }

    // Stop the Host, Client, or Server
    public void Stop()
    {
        NetworkManager.Singleton.Shutdown();
    }

    private void OnEnable()
    {
        Debug.Log("SimpleNetworkManager enabled");
    }

    private void OnDisable()
    {
        Debug.Log("SimpleNetworkManager disabled");
    }
}
