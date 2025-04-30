using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace DeleteVehicle.Server
{
    public class Server : BaseScript
    {
        [EventHandler("DeleteVehicle:Server:DeleteVehicle")]
        private void OnDeleteVehicle(int netId)
        {
            Entity vehicle = Entity.FromNetworkId(netId);
            if (vehicle is null)
            {
                return;
            }
            
            DeleteEntity(netId);
        }
    }
}