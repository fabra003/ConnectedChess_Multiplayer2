using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    public enum PlayerSide { None, White, Black }
    public static PlayerSide LocalPlayerSide = PlayerSide.None;

    public NetworkVariable<PlayerSide> assignedSide = new NetworkVariable<PlayerSide>(PlayerSide.None, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalPlayerSide = assignedSide.Value;
            Debug.Log("My side is: " + LocalPlayerSide);

            // Show the turn info right away if it's this player's turn
            if (NetworkGameManager.Instance.CurrentTurnSide == LocalPlayerSide)
            {
                FindObjectOfType<TurnUIController>().SetPlayerSide(LocalPlayerSide.ToString());
                FindObjectOfType<TurnUIController>().SetTurnInfo(LocalPlayerSide.ToString());
            }

        }
    }
}
