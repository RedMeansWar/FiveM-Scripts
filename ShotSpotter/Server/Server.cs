using System;
using CitizenFX.Core;

namespace ShotSpotter.Server
{
    public class Server : BaseScript
    {
        #region Variables
        internal Random _random = new();
        #endregion

        #region Event Handlers
        [EventHandler("ShotSpotter:Server:ShowNotification")]
        private async void OnShowNotification([FromSource] Player player, Vector3 plyPos, string postal, string zoneName, string caliber)
        {
            await Delay(_random.Next(15000, 35000)); // will display notification at a random time between 15 and 35 seconds.
            TriggerClientEvent("ShotSpotter:Client:ShowNotification");
        }
        #endregion
    }
}
