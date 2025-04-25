using System;
using System.Threading.Tasks;
using Common.Client;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using static VehicleControl.Client.ClientCommands;

namespace VehicleControl.Client
{
    public class Client : ClientCommonScript
    {
        #region Variables
        internal float _steeringAngle;
        #endregion

        #region Ticks
        [Tick]
        private async Task VehicleControllerTick()
        {
            Vehicle vehicle = ClientCurrentVehicle;
            if (vehicle is null)
            {
                await Delay(500);
                return;
            }

            if (Controls.IsControlJustPressed(Control.VehicleExit) && ClientPed.IsAlive && vehicle.ClassType != VehicleClass.Helicopters && vehicle.ClassType != VehicleClass.Planes)
            {
                await Delay(150);
                if (Controls.IsControlPressed(Control.VehicleExit) && ClientPed.IsAlive)
                {
                    vehicle.IsEngineRunning = true;
                }
            }

            if (noShuffle)
            {
                if (vehicle.GetPedOnSeat(VehicleSeat.Passenger) == ClientPed && GetIsTaskActive(ClientPed.Handle, 165))
                {
                    if (!vehicle.IsSeatFree(VehicleSeat.Driver) && !vehicle.Driver.IsPlayer)
                    {
                        await Delay(2000);
                    }
                    else if (vehicle.IsSeatFree(VehicleSeat.Driver))
                    {
                        ClientPed.SetConfigFlag(PedConfigFlag.PreventAutoShuffleToDriversSeat, true);
                        ClientPed.Task.ClearAllImmediately();
                        ClientPed.SetIntoVehicle(vehicle, VehicleSeat.Passenger);
                    }
                }
            }
            else if (vehicle.IsSeatFree(VehicleSeat.Driver))
            {
                ClientPed.ResetConfigFlag(PedConfigFlag.PreventAutoShuffleToDriversSeat);
                ClientPed.Task.ClearAllImmediately();
                ClientPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
                noShuffle = true;
            }

            if (vehicle.Driver != ClientPed || vehicle.Model.IsBicycle)
            {
                return;
            }

            if (vehicle.SteeringAngle > 20f)
            {
                _steeringAngle = 40f;
            }
            else if (vehicle.SteeringAngle < -20f)
            {
                _steeringAngle = -40f;
            }

            if (ClientPed.IsOnFoot || vehicle.IsStopped)
            {
                vehicle.SteeringAngle = _steeringAngle;
            }
        }

        [Tick]
        private async Task VehicleHudTick()
        {
            Vehicle vehicle = ClientCurrentVehicle;
            if (vehicle is null || !Hud.IsHudVisible)
            {
                await Delay(1000);
                return;
            }

            Hud.DrawRectangle(0.1405f, 0.06f, -0.045f, 0.03f, 0, 0, 0, 100);
            Hud.DrawText2d(0.82f, -0.114f, 0.5f, $"{Math.Ceiling(vehicle.Speed.ConvertToMph())}", Alignment.Right);
            Hud.DrawText2d(0.84f, -0.114f, 0.5f, "mph");

            if (vehicle.Model.IsPlane || vehicle.Model.IsHelicopter)
            {
                Hud.DrawRectangle(0.1405f, 0.06f, -0.045f, 0.03f, 0, 0, 0, 100);
                Hud.DrawText2d(0.82f, -0.114f, 0.5f, $"{Math.Ceiling(vehicle.HeightAboveGround.ConvertToFeet())}", Alignment.Right);
                Hud.DrawText2d(0.84f, -0.114f, 0.5f, "feet");
            }

            Hud.DrawText2d(0.515f, 0.05f, 0.559f, vehicle.Mods.LicensePlate, Alignment.Center);
            Hud.DrawText2d(0.84f, 0.067f, 0.45f, vehicle.IsEngineRunning ? "~g~ENG" : "~r~ENG", 255, 255, 255, 200, Alignment.Right);
            Hud.DrawText2d(0.165f, 0.045f, 0.45f, vehicle.IsInBurnout ? "~r~DSC" : "DSC", 255, 255, 255, 200);

            float engineHealth = vehicle.EngineHealth;
            float bodyHealth = vehicle.BodyHealth;

            Hud.DrawText2d(1f, 0.045f, 0.45f, bodyHealth < 310 ? "~r~AC" : bodyHealth < 900 ? "~y~AC" : "AC", 255, 255, 255, 200, Alignment.Right);
            Hud.DrawText2d(0.865f, 0.045f, 0.45f, engineHealth < 110 ? "~r~Fluid" : engineHealth < 315 ? "~r~Fluid" : engineHealth < 900 ? "~y~Fluid" : "Fluid", 255, 255, 255, 200, Alignment.Right);
            Hud.DrawText2d(0.01f, 0.045f, 0.45f, engineHealth < 110 ? "~r~Oil" : engineHealth < 315 ? "~y~Oil" : "Oil", 255, 255, 255, 200);
        }
        #endregion
    }
}
