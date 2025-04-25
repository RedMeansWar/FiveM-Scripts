using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Models;
using Common.Client;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace ShotSpotter.Client
{
    public class Client : ClientCommonScript
    {
        #region Variables
        internal bool _shotSpotterNotification, _shotSpotterSound;
        internal Character _currentCharacter;

        internal readonly List<Vector3> _ignoredLocations = new()
        {
            new(13.35f, -1097.08f, 29.83f),
            new(821.51f, -2163.73f, 29.66f)
        };
        
        internal readonly List<WeaponGroup> _whitelistedClasses = new()
        {
            WeaponGroup.Pistol, WeaponGroup.SMG, WeaponGroup.AssaultRifle, WeaponGroup.MG, WeaponGroup.Shotgun, WeaponGroup.Sniper
        };
        #endregion

        #region Constructor
        public Client()
        {
            int val = GetResourceKvpInt("shotSpotterNotifications");
            int val1 = GetResourceKvpInt("shotSpotterSound");

            if (val == 0 || val1 == 0)
            {
                SetResourceKvpInt("shotSpotterNotifications", 2);
                SetResourceKvpInt("shotSpotterSound", 2);
            }
            else
            {
                _shotSpotterNotification = GetResourceKvpInt("shotSpotterNotifications") == 2;
                _shotSpotterSound = GetResourceKvpInt("shotSpotterSound") == 2;
            }
        }
        #endregion

        #region Methods
        private bool InIgnoredLocation()
        {
            foreach (Vector3 pos in _ignoredLocations)
            {
                if (ClientPed.DistanceTo(pos) < 20f)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Event Handlers
        [EventHandler("Framework:Client:SelectedCharacter")]
        private void OnSelectedCharacter(string json) => _currentCharacter = Json.Parse<Character>(json);

        [EventHandler("ShotSpotter:Client:ShowNotification")]
        private async void OnShowNotification(Vector3 playerPos, string postal, string zoneName, string caliber)
        {
            if (!_shotSpotterNotification || _currentCharacter is not null && _currentCharacter.Department == "Civ" || _currentCharacter is not null && _currentCharacter.Department == "LSFD")
            {
                return;
            }

            Blip blip = World.CreateBlip(playerPos);
            blip.Sprite = (BlipSprite)161;

            SetBlipDisplay(blip.Handle, 3);

            blip.Color = (BlipColor)1;
            blip.Name = "ShotSpotter Alert";

            if (_shotSpotterSound)
            {
                int soundId = Audio.PlaySoundFrontend("TIMER_STOP", "HUD_MINI_GAME_SOUNDSET");
                while (!Audio.HasSoundFinished(soundId))
                {
                    await Delay(0);
                }

                Audio.ReleaseSound(soundId);
            }

            Hud.DisplayNotification($"~o~~h~ShotSpotter~h~~s~: {(caliber is not null ? caliber + " arms fire detected" : "Gunfire detected")} near {postal}, {zoneName}.");

            await Delay(30000); // the time before the blip is delete | 30 seconds

            blip.Delete();
        }

        [EventHandler("ShotSpotter:Client:ToggleSound")]
        private void OnToggleSound(int type, bool state)
        {
            if (type == 0)
            {
                _shotSpotterNotification = state;
                SetResourceKvpInt("shotSpotterNotifications", _shotSpotterNotification ? 2 : 1);
            }
            else if (type == 1)
            {
                _shotSpotterSound = state;
                SetResourceKvpInt("shotSpotterSound", _shotSpotterSound ? 2 : 1);
            }
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task ShotSpotterTick()
        {
            if (!ClientPed.IsAlive || _currentCharacter is not null && _currentCharacter.Department != "Civ")
            {
                await Delay(1000);
                return;
            }

            if (!IsPedArmed(ClientPed.Handle, 4))
            {
                await Delay(500);
                return;
            }

            if (_whitelistedClasses.Contains(ClientPed.Weapons.Current.Group) && ClientPed.IsShooting)
            {
                if (ClientPed.Position.Y > Globals.BC_MIN_Y || InIgnoredLocation())
                {
                    await Delay(3000);
                    return;
                }

                Vector3 playerPos = ClientPed.Position;
                string postal = Exports["nearestpostal"].getClosestPostal(ClientPed.Position);
                string zoneName = GetLabelText(GetNameOfZone(playerPos.X, playerPos.Y, playerPos.Z));
                string caliber = ClientPed.Weapons.Current.Group == WeaponGroup.Pistol ? "Small" : "Large";

                TriggerServerEvent("ShotSpotter:Server:ShowNotification");
            }
        }
        #endregion
    }
}
