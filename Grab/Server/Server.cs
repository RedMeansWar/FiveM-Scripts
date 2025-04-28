using System;
using CitizenFX.Core;

namespace Grab.Server
{
    public class Server : BaseScript
    {
        [EventHandler("Grab:Server:GrabClosestPlayer")]
        private void OnGrabClosestPlayer([FromSource] Player player, int target)
        {
            Player targetPlayer = Players[target];
            targetPlayer?.TriggerEvent("Grab:Client:GetGrabbed", player.Handle);
        }

        [EventHandler("Grab:Server:Notify")]
        private void OnNotify(int netId, string message)
        {
            Player grabberPlayer = Players[netId];
            grabberPlayer?.TriggerEvent("Grab:Client:Notify", grabberPlayer.Handle, message);
        }
    }
}
