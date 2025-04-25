using System;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace DeleteVehicle.Server
{
    public class Server
    {
        [EventHandler("DeleteVehicle:Server:DeleteVehicle")]
        private void OnDeleteVehicle(int networkId)
        {
            Entity vehicle = Entity.FromNetworkId(networkId);
            if (vehicle is null)
            {
                return;
            }

            DeleteEntity(networkId);
        }
    }
}
