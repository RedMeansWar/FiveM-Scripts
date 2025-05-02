using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Client;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Gsr.Client
{
    public class Client : ClientCommonScript
    {
        #region Variables
        internal int _lastShot;
        internal bool _shotRecently;
        
        internal readonly List<WeaponHash> _whitelistedWeapons =
        [
            WeaponHash.FireExtinguisher, WeaponHash.Snowball, WeaponHash.PetrolCan, WeaponHash.Ball, WeaponHash.StunGun,
            WeaponHash.Molotov, WeaponHash.Flare
        ];
        #endregion
        
        #region Commands
        [Command("cleangsr")]
        private void CleanGsrCommand()
        {
            if (!_shotRecently)
            {
                Notify.Alert("You haven't shot recently.");
                return;
            }

            Notify.Alert("You've wiped your hands of blood.", true);
        }
        
        [Command("wipegsr")]
        private void WipeGsrCommand() => CleanGsrCommand();

        [Command("gsrtest")]
        private void GsrTestCommand()
        {
            Player closestPlayer = GetClosestPlayer();
            if (closestPlayer is null)
            {
                Notify.Alert("You must be closesr to the player you wish to test.", true);
                return;
            }
            
            TriggerServerEvent("Gsr:Server:SubmitTest", closestPlayer.ServerId);
        }
        #endregion

        #region Methods
        private void CleanGsr()
        {
            ClientPed.ClearBloodDamage();
            ClearPedEnvDirt(ClientPed.Handle);
            ClientPed.ResetVisibleDamage();
            _shotRecently = false;
        }
        #endregion
        
        #region Event Handlers
        [EventHandler("Gsr:Client:PerformTest")]
        private void OnPerformTest(string testerId) => TriggerServerEvent("Gsr:Server:ReturnTest", _lastShot, testerId);

        [EventHandler("Gsr:Client:Notify")]
        private void OnNotify(bool shotRecently) => Hud.DisplayNotification(shotRecently ? "Sample from swab comes back ~g~~h~positive~h~~s~." : "Sample from swab comes back ~o~~h~negative~h~~s~.");
        #endregion

        #region Ticks
        [Tick]
        private async Task GsrTick()
        {
            if (ClientPed.IsShooting && !_whitelistedWeapons.Contains(ClientPed.Weapons.Current.Hash))
            {
                _lastShot = Game.GameTime;
                _shotRecently = true;
            }

            if (_shotRecently && Game.GameTime - _lastShot > 900000) // 15 minutes
            {
                _shotRecently = false;
            }

            if (ClientPed.IsInWater && _shotRecently)
            {
                CleanGsr();
                Notify.Alert("You've ~b~~h~washed off~h~~s~ the evidence of gunshot residue on yourself.", true);
            }
        }
        #endregion
    }
}
