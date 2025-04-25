using System;
using CitizenFX.Core;

namespace VehicleControl.Server
{
    public class Server : BaseScript
    {
        [EventHandler("VehicleControl:Server:DoorIndex")]
        private void OnDoorIndex(int networkId, int doorIndex, bool open)
        {
            Entity vehicle = Entity.FromNetworkId(networkId);
            if (vehicle is null)
            {
                return;
            }

            vehicle.Owner.TriggerEvent("VehicleControl:Client:DoorIndex", networkId, doorIndex, open);
        }
    }
}
