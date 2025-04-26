using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Client;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System.CodeDom;
using Common;

namespace DevTools.Client
{
    public class Client : ClientCommonScript
    {
        #region Variables
        internal bool _idInfoOn, _coordsOn;
        internal string _xText, _yText, _zText, _hText, _modelText, _modelVehicleText;
        #endregion

        #region Commands
        [Command("idgun")]
        private void IdGunCommand()
        {
            if (!ClientPed.Weapons.HasWeapon(WeaponHash.HeavyPistol))
            {
                ClientPed.Weapons.Give(WeaponHash.HeavyPistol, 9999, true, true);
                _idInfoOn = true;
            }

            Tick += TestTick;
        }

        private async Task TestTick()
        {
            Log.InfoOrError($"Got Entity: {GetEntityFromAiming().Handle}, {GetEntityFromAiming().Model}, {GetEntityFromAiming().IsPersistent}");
            await Delay(50);
            Tick -= TestTick;
        }

        [Command("coords")]
        private void CoordsCommand()
        {
            if (_idInfoOn)
            {
                Hud.SendChatMessage("You can't have the ID gun information on the same time as the player coordinants!", "SYSTEM", 255, 0, 0);
                return;
            }

            _coordsOn = true;
            Tick += StartCoordsTick;
        }
        #endregion

        #region Methods
        public Entity GetEntityFromAiming()
        {
            int player = ClientPlayer.Handle;
            int targetEntityHandle = 0;
            bool aiming = false;

            aiming = GetEntityPlayerIsFreeAimingAt(player, ref targetEntityHandle);
            if (aiming && targetEntityHandle != 0)
            {
                return Entity.FromHandle(targetEntityHandle);
            }

            return null;
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task IdGunTick()
        {
            int delay = 250;

            if (_idInfoOn)
            {
                delay = 5;
                if (IsPlayerFreeAiming(ClientPlayer.Handle))
                {
                    Vector3 pos = ClientPed.Position;
                }
            }
        }

        private async Task StartCoordsTick()
        {
            Vector3 pos = ClientPed.Position;

            _xText = $"X: {pos.X}";
            _yText = $"Y: {pos.Y}";
            _zText = $"Z: {pos.Z}";
            _hText = $"H: {ClientPed.Heading}";

            await Delay(0);

            Hud.DrawText2d(0f, 0f, 1f, "");
            Hud.DrawText2d(0f, 0f, 1f, "");
            Hud.DrawText2d(0f, 0f, 1f, "");
            Hud.DrawText2d(0f, 0f, 1f, "");
        }
        #endregion
    }
}
