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

            var ui = FindObjectOfType<TurnUIController>();
            ui.SetPlayerSide(LocalPlayerSide.ToString()); // ✅ Always show which side the player is

            // ✅ Show current turn info, even if it's not your turn
            var currentTurn = GameManager.Instance.SideToMove.ToString();
            ui.SetTurnInfo($"{currentTurn}");
        }
    }



    private void OnAssignedSideChanged(PlayerSide previous, PlayerSide current)
    {
        if (IsOwner)
        {
            HandlePlayerSideAssigned(current);
        }
    }

    private void HandlePlayerSideAssigned(PlayerSide side)
    {
        LocalPlayerSide = side;
        Debug.Log("My side is: " + LocalPlayerSide);

        var ui = FindObjectOfType<TurnUIController>();
        if (ui != null)
        {
            ui.SetPlayerSide(LocalPlayerSide.ToString());

            // Also show current turn if this player starts
            if (NetworkGameManager.Instance.CurrentTurnSide == LocalPlayerSide)
            {
                ui.SetTurnInfo(LocalPlayerSide.ToString());
            }
        }
    }
}
