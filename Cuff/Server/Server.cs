using System;
using CitizenFX.Core;

namespace Cuff.Server
{
    public class Server : BaseScript
    {
        [EventHandler("Cuff:Notes.Server:CuffClosestPlayer")]
        private void OnCuffClosestPlayer([FromSource] Player player, int target, bool isFront, bool isZiptie)
        {
            Player targetPlayer = Players[target];
            targetPlayer?.TriggerEvent("Cuff:Notes.Notes.Client:GetCuffedPlayer", player.Handle, isFront, isZiptie);
        }

        [EventHandler("Cuff:Notes.Server:PlayAnimation")]
        private void OnPlayCuffAnimation(int cuffer, bool uncuff)
        {
            Player cufferPlayer = Players[cuffer];
            cufferPlayer?.TriggerEvent("Cuff:Notes.Notes.Client:PlayCuffAnimation", uncuff);
        }
    }
}
