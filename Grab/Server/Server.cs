using System;
using CitizenFX.Core;

namespace Grab.Server
{
    public class Server : BaseScript
    {
        [EventHandler("Grab:Notes.Server:GrabClosestPlayer")]
        private void OnGrabClosestPlayer([FromSource] Player player, int target)
        {
            Player targetPlayer = Players[target];
            targetPlayer?.TriggerEvent("Grab:Notes.Notes.Client:GetGrabbed", player.Handle);
        }

        [EventHandler("Grab:Notes.Server:Notify")]
        private void OnNotify(int netId, string message)
        {
            Player grabberPlayer = Players[netId];
            grabberPlayer?.TriggerEvent("Grab:Notes.Notes.Client:Notify", grabberPlayer.Handle, message);
        }
    }
}
