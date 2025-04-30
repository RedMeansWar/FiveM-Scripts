using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Client;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Common;

namespace Gsr.Client
{
    public class Client : ClientCommonScript
    {
        #region Variables
        internal bool _gsrFound;
        internal string _weaponType;
        #endregion

        #region Constructor
        public Client() => Weapons.LoadWeaponData();
        #endregion

        #region Commands
        [Command("checkgsr")]
        private void CheckGsrCommand()
        {
            uint currentWeaponHash = (uint)GetSelectedPedWeapon(ClientPed.Handle);
            string displayName = Weapons.GetWeaponDisplayNameFromHash(currentWeaponHash);

            Log.InfoOrError($"Current Weapon: {displayName}");
        }
        #endregion

        #region Methods
        
        #endregion

        #region Event Handlers
        [EventHandler("Gsr:Notes.Notes.Client:PerformTest")]
        private void OnDoTest(bool gsrFound)
        {

        }

        [EventHandler("Gsr:Notes.Notes.Client:ReturnTest")]
        private void OnReturnTest(bool gsrFound)
        {
            _gsrFound = gsrFound;

            if (gsrFound)
            {
                Notify.Alert("You have found gun shot residue for a {}");
            }
        }
        #endregion
    }
}
