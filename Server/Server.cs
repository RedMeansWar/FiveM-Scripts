using System;
using CitizenFX.Core;

namespace Server
{
    public class Servr : BaseScript
    {
        [EventHandler("Cuff:Server:CuffClosestPlayer")]
        private void OnCuffClosestPlayer([FromSource] Player player, int netId, bool isFront, bool isZiptie)
        {
            Player target = Players[netId];
            target?.TriggerEvent("Cuff:Client:GetCuffedPlayer", player.Handle, isFront, isZiptie);
        }

        [EventHandler("Cuff:Server:PlayAnimation")]
        private void OnPlayAnimation(int cuffer, bool uncuff)
        {
            Player cufferPlayer = Players[cuffer];
            cufferPlayer?.TriggerEvent("Cuff:Client:PlayAnimation");
        }
    }
}
