using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Client;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Common;

namespace VehicleControl.Client
{
    internal class ClientCruiseControl : ClientCommonScript
    {
        #region Variables
        internal bool _radarCruise, _cruising;
        internal float _targetSpeed = -1f;

        internal readonly IReadOnlyList<VehicleClass> _ignoreClasses = new List<VehicleClass>
        {
            VehicleClass.Cycles, VehicleClass.Motorcycles, VehicleClass.Planes, VehicleClass.Helicopters, VehicleClass.Boats, VehicleClass.Trains
        };

        internal readonly IReadOnlyList<int> _tireIndex = new List<int>
        {
            0, 1, 2, 3, 4, 5, 45, 47
        };
        #endregion

        #region Constructor
        public ClientCruiseControl() => RegisterKeyMapping("+cruisecontrol", "Toggle cruise control.", "keyboard", "f7");
        #endregion

        #region Commands
        [Command("+cruisecontrol")]
        private void CruiseControlCommand()
        {
            _cruising = !_cruising;
            if (!_cruising && ClientCurrentVehicle is not null)
            {
                CancelCruise();
            }
        }
        #endregion

        #region Methods
        private void CancelCruise()
        {
            SetVehicleMaxSpeed(ClientCurrentVehicle.Handle, 500f);
            _targetSpeed = -1f;
            _cruising = false;
        }

        private bool HasAnyTiresBurst() => _tireIndex.Any(t => IsVehicleTyreBurst(ClientCurrentVehicle.Handle, t, false));
        #endregion

        #region Ticks
        [Tick]
        private async Task CruiseControlTick()
        {
            Vehicle vehicle = ClientCurrentVehicle;
            if (vehicle is null && _cruising)
            {
                _cruising = false;
                _targetSpeed = -1f;

                await Delay(1000);
                return;
            }

            while (_cruising)
            {
                if (_ignoreClasses.Contains(vehicle.ClassType))
                {
                    CancelCruise();
                    return;
                }

                if (_targetSpeed == -1)
                {
                    _targetSpeed = GetEntitySpeed(vehicle.Handle);
                    SetVehicleMaxSpeed(vehicle.Handle, _targetSpeed);
                }

                if (_targetSpeed > 1f || !_radarCruise)
                {
                    Controls.SetControlNormal(Control.VehicleAccelerate, 0.9f);
                }

                if (vehicle.Driver is null || vehicle.Driver != ClientPed || vehicle.IsInWater || vehicle.IsInBurnout || !vehicle.IsEngineRunning || vehicle.IsInAir || vehicle.HasCollided || _targetSpeed.ConvertToMph() < 25f || _targetSpeed.ConvertToMph() > 100f || HasAnyTiresBurst() || Controls.IsControlJustPressed(Control.VehicleHandbrake))
                {
                    CancelCruise();
                    return;
                }

                if (Controls.GetControlValue(Control.VehicleAccelerate) > 250f)
                {
                    float currentSpeed = _targetSpeed.ConvertToMph();
                    float newSpeed = (float)Math.Ceiling(++currentSpeed);

                    _targetSpeed = newSpeed.ConvertToMph();
                    SetVehicleMaxSpeed(vehicle.Handle, _targetSpeed);
                }

                if (Controls.IsControlPressed(Control.VehicleBrake))
                {
                    int delay = 3;
                    
                    while (Controls.IsControlHeld(Control.VehicleBrake))
                    {
                        delay--;

                        if (delay == 0)
                        {
                            CancelCruise();
                            return;
                        }

                        await Delay(100);
                    }

                    float current = _targetSpeed.ConvertToMph();
                    float newSpeed = (float)Math.Ceiling(current);

                    _targetSpeed = newSpeed.ConvertFromMph();
                    SetVehicleMaxSpeed(vehicle.Handle, _targetSpeed);
                }

                RaycastResult rr = World.RaycastCapsule(vehicle.Position, vehicle.GetPositionOffset(new(0f, 15f, 0f)), 2f, IntersectOptions.Everything, vehicle);
                if (rr.DitHitEntity)
                {
                    Vehicle forwardVehicle = (Vehicle)rr.HitEntity;

                    if (forwardVehicle.Driver.Handle == 0)
                    {
                        _radarCruise = false;
                       
                        if (vehicle.Speed < _targetSpeed)
                        {
                            SetVehicleMaxSpeed(vehicle.Handle, _targetSpeed);
                        }

                        return;
                    }

                    _radarCruise = true;

                    if (vehicle.Speed > GetEntitySpeed(forwardVehicle.Handle))
                    {
                        float currentSpeed = vehicle.Speed;
                        float newSpeed = (float)Math.Ceiling(currentSpeed - 3);

                        SetVehicleMaxSpeed(vehicle.Handle, newSpeed.ConvertFromMph());
                        vehicle.AreBrakeLightsOn = true;

                        await Delay(50);
                    }
                }
                else
                {
                    _radarCruise = false;
                    if (vehicle.Speed < _targetSpeed)
                    {
                        SetVehicleMaxSpeed(vehicle.Handle, _targetSpeed);
                    }
                }

                await Delay(0);
            }

            await Delay(100);
        }

        [Tick]
        private async Task CruiseHudTick()
        {
            if (!Hud.IsHudVisible || ClientCurrentVehicle is null)
            {
                await Delay(1000);
                return;
            }

            string cruise = "~r~Cruise";
            
            if (_cruising)
            {
                cruise = $"~g{(_radarCruise ? "DCC" : "Cruise")}~";
                
                if (_targetSpeed - ClientCurrentVehicle.Speed > 1f)
                {
                    cruise += $"~s~: {Math.Ceiling(_targetSpeed.ConvertToMph())}mph";
                }
            }

            Hud.DrawText2d(0.01f, 0.07f, 0.45f, cruise, 255, 255, 255, 200);
        }
        #endregion
    }
}
