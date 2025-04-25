using System;
using Common;
using Common.Client;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace VehicleControl.Client
{
    internal class ClientCommands : ClientCommonScript
    {
        #region Variables
        public static bool noShuffle = true;
        #endregion

        #region Commands
        [Command("engine")]
        private void EngineCommand() => ToggleVehicleEngine(ClientCurrentVehicle);

        [Command("eng")]
        private void EngCommand() => ToggleVehicleEngine(ClientCurrentVehicle);

        [Command("shuffle")]
        private void ShuffleCommand() => TriggerEvent("VehicleControl:Client:ShuffleSeats");

        [Command("shuff")]
        private void ShuffCommand() => TriggerEvent("VehicleControl:Client:ShuffleSeats");

        [Command("anchor")]
        private void AnchorCommand()
        {
            if (!ClientPed.IsInBoat)
            {
                Notify.Error("You must be coning a boat.", true);
                return;
            }

            Vehicle boat = ClientCurrentVehicle;
            if (boat.Speed >= 5f)
            {
                Notify.Error("You're going to fast to anchor the boat.", true);
                return;
            }

            if (IsBoatAnchoredAndFrozen(boat.Handle))
            {
                SetBoatAnchor(boat.Handle, false);
                Notify.Success("Un-anchored boat.", true);
            }
            else 
            {
                SetBoatAnchor(boat.Handle, true);
                SetBoatFrozenWhenAnchored(boat.Handle, true);
                Notify.Success("Anchored boat.", true);
            }
        }

        [Command("door")]
        private void DoorCommand(string[] args)
        {
            Vehicle vehicle = ClientCurrentVehicle ?? GetClosestVehicle(1f);
            if (vehicle is null)
            {
                Notify.Error("You must be in or near a vehicle.", true);
                return;
            }

            if (vehicle is not null && ClientPed.SeatIndex != VehicleSeat.Driver)
            {
                Notify.Error("You must be the driver.", true);
                return;
            }

            if (!int.TryParse(args[0], out int doorIndex) || doorIndex < 0 || doorIndex > 4)
            {
                Notify.Error("Invalid door", true);
                return;
            }

            if (doorIndex > 0)
            {
                doorIndex--;
            }

            if (vehicle.Doors[(VehicleDoorIndex)doorIndex].IsBroken)
            {
                if (NetworkGetEntityOwner(vehicle.Handle) == ClientPlayer.Handle)
                {
                    vehicle.Doors[(VehicleDoorIndex)doorIndex].Close();
                }
                else
                {
                    TriggerServerEvent("VehicleControl:Server:doorAction", vehicle.NetworkId, doorIndex, true);
                }

                Notify.Success("Door opened.");
            }
        }

        [Command("flip")]
        private void FlipCommand()
        {
            Vehicle vehicle = ClientCurrentVehicle ?? GetClosestVehicle(1f);
            if (vehicle is not null)
            {
                if (ClientPed.IsInVehicle() && ClientPed.SeatIndex != VehicleSeat.Driver)
                {
                    Notify.Error("You must be the driver.", true);
                    return;
                }

                if (SetVehicleOnGroundProperly(vehicle.Handle))
                {
                    Notify.Success("Flipped vehicle.", true);
                    return;
                }

                Notify.Error("Failed to flip vehicle, try again.", true);
            }
            else
            {
                Notify.Error("You must be in or near a vehicle", true);
            }
        }

        [Command("trunk")]
        private void TrunkCommand()
        {
            Vehicle vehicle = ClientCurrentVehicle ?? GetClosestVehicle(1f);
            if (vehicle is null)
            {
                Notify.Error("You must be in or near a vehicle.", true);
                return;
            }

            if (ClientCurrentVehicle is not null && ClientPed.SeatIndex != VehicleSeat.Driver)
            {
                Notify.Error("You must be the driver.", true);
                return;
            }

            if (vehicle.LockStatus != VehicleLockStatus.Unlocked)
            {
                Notify.Error("You must unlock the car.", true);
                return;
            }

            if (vehicle.Doors[VehicleDoorIndex.Hood].IsBroken)
            {
                Notify.Error("The trunk isn't intact.", true);
                return;
            }

            if (vehicle.Doors[VehicleDoorIndex.Trunk].IsOpen)
            {
                if (NetworkGetEntityOwner(vehicle.Handle) == ClientPlayer.Handle)
                {
                    vehicle.Doors[VehicleDoorIndex.Trunk].Close();
                }
                else
                {
                    TriggerServerEvent("VehicleControl:Server:doorAction", vehicle.NetworkId, (int)VehicleDoorIndex.Trunk, false);
                }

                Notify.Success("Trunk closed.", true);
            }
            else
            {
                if (NetworkGetEntityOwner(vehicle.Handle) == ClientPlayer.Handle)
                {
                    vehicle.Doors[VehicleDoorIndex.Trunk].Open();
                }
                else
                {
                    TriggerServerEvent("VehicleControl:Server:doorAction", vehicle.NetworkId, (int)VehicleDoorIndex.Trunk, false);
                }

                Notify.Success("Trunk opened.", true);
            }
        }

        [Command("hood")]
        private void HoodCommand()
        {
            Vehicle vehicle = ClientCurrentVehicle ?? GetClosestVehicle(1f);
            if (vehicle is null)
            {
                Notify.Error("You must be in or near a vehicle.", true);
                return;
            }

            if (vehicle is not null && ClientPed.SeatIndex != VehicleSeat.Driver)
            {
                Notify.Error("You must be the driver.", true);
                return;
            }

            if (vehicle.LockStatus != VehicleLockStatus.Unlocked)
            {
                Notify.Error("You must unlock the car.", true);
                return;
            }

            if (vehicle.Doors[VehicleDoorIndex.Hood].IsBroken)
            {
                Notify.Error("The hood isn't intact.", true);
                return;
            }

            if (vehicle.Doors[VehicleDoorIndex.Hood].IsOpen)
            {
                if (NetworkGetEntityOwner(vehicle.Handle) == ClientPlayer.Handle)
                {
                    vehicle.Doors[VehicleDoorIndex.Hood].Close();
                }
                else
                {
                    TriggerServerEvent("VehicleControl:Server:doorAction", vehicle.NetworkId, (int)VehicleDoorIndex.Hood, false);
                }

                Notify.Success("Hood closed.");
            }
            else
            {
                if (NetworkGetEntityOwner(vehicle.Handle) == ClientPlayer.Handle)
                {
                    vehicle.Doors[VehicleDoorIndex.Hood].Open();
                }
                else
                {
                    TriggerServerEvent("VehicleControl:Server:doorAction", vehicle.NetworkId, (int)VehicleDoorIndex.Hood, true);
                }

                Notify.Success("Hood opened.");
            }
        }
        #endregion

        #region Methods
        private void ToggleVehicleEngine(Vehicle vehicle)
        {
            if (vehicle is null)
            {
                Notify.Error("You must be in a vehicle to toggle its engine.");
                return;
            }

            ClientPed.SetConfigFlag(429, true);
            vehicle.IsEngineRunning = !vehicle.IsEngineRunning;
            // vehicle.IsEngineRunning = false; // use this method if the one above does not work.
        }
        #endregion

        #region Event Handlers
        [EventHandler("VehicleControl:Client:ShuffleSeats")]
        private void OnShuffleSeats()
        {
            Vehicle vehicle = ClientCurrentVehicle;
            if (vehicle is null)
            {
                return;
            }

            if (vehicle.Driver == ClientPed)
            {
                ClientPed.SetIntoVehicle(vehicle, VehicleSeat.Passenger);
            }
            else
            {
                noShuffle = true;
            }
        }

        [EventHandler("VehicleControl:Client:DoorAction")]
        private void OnDoorAction(int networkId, int doorIndex, bool open)
        {
            Vehicle vehicle = (Vehicle)Entity.FromNetworkId(networkId);

            if (vehicle is null)
            {
                Log.InfoOrError($"Got Network ID '{networkId}' from doorAction event and wasn't able to convert to vehicle, bailing.", "VehicleControl");
                return;
            }

            if (open)
            {
                vehicle.Doors[(VehicleDoorIndex)doorIndex].Open();
            }
            else
            {
                vehicle.Doors[(VehicleDoorIndex)doorIndex].Close();
            }
        }
        #endregion
    }
}
