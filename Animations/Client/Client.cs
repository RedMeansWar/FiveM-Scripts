using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Client;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Common;
using Common.Client.Models;
using static CitizenFX.Core.Native.API;

namespace Animations.Client
{
    public class Client : ClientCommonScript
    {
        #region Variables
        internal bool _handsUp, _handsOnHead, _handsUpKnees, _usingCamera, _selfieCamera, _animLoaded;
        internal int _radioKey; // config variables
        #endregion

        #region Constructor
        public Client() => ReadConfigFile();
        #endregion
        
        #region Commands
        [Command("handsup")]
        private void HandsUpCommand()
        {
            _handsUp = !_handsUp;

            if (_handsUp || _handsOnHead)
            {
                ClientPed.Task.ClearAnimation("random@arrests@busted", "idle_c");
                ClientPed.Task.ClearAnimation("random@mugging3", "handsup_standing_base");
                _handsOnHead = false;
                _handsUpKnees = false;
                Tick -= DisableControlsTick;
            }

            if (_handsUp)
            {
                ClientPed.Task.PlayAnimation("random@mugging3", "handsup_standing_base", 2.5f, -1, AnimationFlags.StayInEndFrame | AnimationFlags.UpperBodyOnly | AnimationFlags.AllowRotation);
                Tick += DisableControlsTick;
            }
            else
            {
                ClientPed.Task.ClearAnimation("random@mugging3", "handsup_standing_base");
                Tick -= DisableControlsTick;
            }
        }
        
        [Command("handsonhead")]
        private void HandsOnHead()
        {
            _handsOnHead = !_handsOnHead;

            if (_handsOnHead || _handsUp)
            {
                ClientPed.Task.ClearAnimation("random@mugging3", "handsup_standing_base");
                ClientPed.Task.ClearAnimation("random@getawaydriver", "idle_a");
                _handsUp = false;
                _handsUpKnees = false;
                Tick -= DisableControlsTick;
            }

            if (_handsOnHead)
            {
                ClientPed.Task.PlayAnimation("random@arrests@busted", "idle_c", 2.5f, -1, AnimationFlags.StayInEndFrame | AnimationFlags.UpperBodyOnly | AnimationFlags.AllowRotation);
                Tick += DisableControlsTick;
            }
            else
            {
                ClientPed.Task.ClearAnimation("random@arrests@busted", "idle_c");
                Tick -= DisableControlsTick;
            }
        }

        [Command("handsupknees")]
        private void HandsUpKneesCommand()
        {
            _handsUpKnees = !_handsUpKnees;

            if (_handsUp || _handsOnHead)
            {
                ClientPed.Task.ClearAnimation("random@arrests@busted", "idle_c");
                ClientPed.Task.ClearAnimation("random@mugging3", "handsup_standing_base");
                _handsOnHead = false;
                _handsUp = false;
                Tick -= DisableControlsTick;
            }

            if (_handsUpKnees && !ClientPed.IsGettingIntoAVehicle && !ClientPed.IsInVehicle())
            {
                ClientPed.Task.PlayAnimation("random@mugging3", "handsup_standing_base", 2.5f, -1, AnimationFlags.StayInEndFrame | AnimationFlags.UpperBodyOnly | AnimationFlags.AllowRotation);
                ClientPed.Task.PlayAnimation("random@getawaydriver", "idle_a", 2.5f, -1, AnimationFlags.StayInEndFrame);
                Tick += DisableControlsTick;
            }
            else
            {
                ClientPed.Task.ClearAnimation("random@mugging3", "handsup_standing_base");
                ClientPed.Task.ClearAnimation("random@getawaydriver", "idle_a");
                Tick -= DisableControlsTick;
            }
        }

        [Command("hu")]
        private void HuCommand() => HandsUpCommand();
        
        [Command("huk")]
        private void HukCommand() => HandsUpKneesCommand();
        
        [Command("hoh")]
        private void HohCommand() => HandsUpKneesCommand();
        
        [Command("dropweapon")]
        private void DropWeaponCommand() => TriggerEvent("Animations:Client:DropWeapon");

        [Command("camera")]
        private void CameraCommand()
        {
            if (!ClientPed.IsOnFoot)
            {
                return;
            }
            
            _usingCamera = !_usingCamera;
            if (_usingCamera)
            {
                CreateMobilePhone(0);
                CellCamActivate(true, true);

                Hud.IsHudVisible = false;
                Hud.IsRadarVisible = false;
                Tick += CameraControlsTick;
            }
            else
            {
                DestroyMobilePhone();
                CellCamActivate(false, false);
                _selfieCamera = false;
                
                Hud.IsHudVisible = true;
                Hud.IsRadarVisible = true;
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("Animations:Client:DropWeapon")]
        private void OnDropWeapon()
        {
            if (ClientPed.Exists())
            {
                SetPedDropsInventoryWeapon(ClientPed.Handle, (uint)GetSelectedPedWeapon(ClientPed.Handle), 1, 1, 1, -1);
                ClientPed.Weapons.Give(WeaponHash.Unarmed, -1, true, true);
            }
        }
        #endregion

        #region Methods
        private void PlayRadioAnim(bool playAnim)
        {
            if (playAnim)
            {
                Tick += RadioAnimationTick;
            }
            else
            {
                Tick -= RadioAnimationTick;
            }
        }

        private bool CanPedPlayRadioAnimation(Ped ped) =>  ped.IsDead || ped.IsSwimming || ped.IsSwimmingUnderWater || ped.IsTryingToEnterALockedVehicle || !ped.Exists();

        private void ReadConfigFile()
        {
            string data = LoadResourceFile(GetCurrentResourceName(), "config.ini");
            _radioKey = Config.GetValue(data, "Animations", "RadioAnimationKey", 19);

            if (!Config.KeyExists(data, "Animations", "RadioAnimationKey"))
            {
                Log.Error("No radio animation key found in config file!", "ANIMATIONS");
            }
        }
        #endregion
        
        #region Ticks
        private async Task DisableControlsTick()
        {
            List<Control> controlsToDisable =
            [
                Control.Aim, Control.Attack, Control.Attack2, Control.Cover, Control.Jump, Control.MeleeAttack1,
                Control.MeleeAttack2, Control.MeleeAttackAlternate, Control.MeleeAttackHeavy, Control.MeleeAttackLight,
                Control.MeleeBlock, Control.Reload
            ];
            
            controlsToDisable.ForEach(c => Controls.DisableControlThisFrame(c));
        }

        private async Task CameraControlsTick()
        {
            if (Controls.IsControlJustPressed(Control.PhoneCancel))
            {
                Hud.IsHudVisible = true;
                Hud.IsRadarVisible = true;
                DestroyMobilePhone();
                CellCamActivate(false, false);
                _usingCamera = false;
                _selfieCamera = false;
                Tick -= CameraControlsTick;
            }

            if (Controls.IsControlJustPressed(Control.NextCamera))
            {
                Function.Call((Hash)2635073306796480568L, _selfieCamera);
            }
        }

        [Tick]
        private async Task RadioAnimationTick()
        {
            if (!_animLoaded)
            {
                RequestAnimDict("random@arrests");
                if (HasAnimDictLoaded("random@arrests"))
                {
                    await Delay(100);
                }
                
                _animLoaded = true;
            }

            if (Controls.IsControlJustPressed((Control)_radioKey) && ClientPed is not null && CanPedPlayRadioAnimation(ClientPed))
            {
                await ClientPed.Task.PlayAnimation("", "", 0.8f, 0.8f, -1, AnimationFlags.AllowRotation | AnimationFlags.Loop | AnimationFlags.StayInEndFrame | AnimationFlags.UpperBodyOnly, 0.8f);

                while (true)
                {
                    ClientPed.Task.ClearSecondary();
                    break;
                }
            }
        }
        #endregion
    }
}