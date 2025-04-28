using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Client;
using SharpConfig;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Client
{
    public class Client : ClientCommonScript
    {
        #region Variables
        internal bool _isCuffed, _isFrontCuffed;
        internal bool _usingCS; // config booleans
        internal Prop _cuffsProp;
        #endregion

        #region Constructor
        public Client() => ReadConfigFile();
        #endregion

        #region Commands
        [Command("cuff")]
        private void CuffCommand() => CuffHandler();

        [Command("frontcuff")]
        private void FrontCuffCommand() => CuffHandler(true);

        [Command("ziptie")]
        private void ZiptieCommand() => CuffHandler(false, true);

        [Command("frontziptie")]
        private void FrontZiptieCommand() => CuffHandler(true, true);

        [Command("cuffme")]
        private void CuffMeCommand() => CuffMeHandler();

        [Command("frontcuffme")]
        private void FrontCuffMeCommand() => CuffMeHandler(true);

        [Command("ziptieme")]
        private void ZipetieMeCommand() => CuffMeHandler(isZiptie: true);

        [Command("frontziptieme")]
        private void FrontZiptieMeCommand() => CuffMeHandler(true, true);
        #endregion

        #region Methods
        private void CuffHandler(bool isFront = false, bool isZiptie = false)
        {
            if (ClientPed.CannotDoAction())
            {
                Notify.Error("You can't do this right now.", true);
                return;
            }

            Player closestPlayer = GetClosestPlayer();
            if (closestPlayer is null)
            {
                Notify.Error("You need to be closer to the person you wish to cuff.", true);
                return;
            }

            TriggerServerEvent("Cuff:Server:CuffClosestPlayer", closestPlayer.ServerId, isFront, isZiptie);
        }

        private async void CuffMeHandler(bool isFront = false, bool isZiptie = false)
        {
            _isCuffed = !_isCuffed;
            _isFrontCuffed = isFront;

            if (_isCuffed)
            {
                if (!isZiptie && _usingCS) TriggerServerEvent("Server:SoundToRadius", ClientPed.NetworkId, 5f, "cuff", 0.2f);

                ClientPed.Task.ClearAll();
                ClientPed.Task.PlayAnimation(_isFrontCuffed ? "anim@move_m@prisoner_cuffed" : "mp_arresting", "idle", 8f, -1, AnimationFlags.StayInEndFrame | AnimationFlags.AllowRotation | AnimationFlags.UpperBodyOnly);

                _cuffsProp = await World.CreateProp(new($"{(isZiptie ? "hei_prop_zip_tie_positioned" : "p_cs_cuffs_02_s")}"), Vector3.Zero, false, false);

                if (_isFrontCuffed)
                {
                    Vector3 pos, rot;

                    if (isZiptie)
                    {
                        pos = new(-0.012f, 0f, 0.08f);
                        rot = new(340f, 95f, 120f);
                    }
                    else
                    {
                        pos = new(-.058f, .005f, .09f);
                        rot = new(290f, 95f, 120f);
                    }

                    AttachEntityToEntity(_cuffsProp.Handle, ClientPed.Handle, GetPedBoneIndex(ClientPed.Handle, 60309), pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z, true, false, false, false, 0, true);
                }
                else
                {
                    Vector3 pos,rot;

                    if (isZiptie)
                    {
                        pos = new(-0.020f, 0.035f, 0.06f);
                        rot = new(0.04f, 155f, 80f);
                    }
                    else
                    {
                        pos = new(-.055f, .06f, .04f);
                        rot = new(265f, 155f, 80f);
                    }

                    AttachEntityToEntity(_cuffsProp.Handle, ClientPed.Handle, GetPedBoneIndex(ClientPed.Handle, 60309), pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z, true, false, false, false, 0, true);
                }

                SetPedDropsWeapon(ClientPed.Handle);
                SetPedCanPlayGestureAnims(ClientPed.Handle, false);

                Tick += CuffTick;
            }
            else
            {
                if (!isZiptie && _usingCS) TriggerServerEvent("Server:SoundToRadius", ClientPed.NetworkId, 5f, "uncuff", 0.2f);

                ClientPed.Task.ClearAnimation("mp_arresting", "idle");
                ClientPed.Task.ClearAnimation("anim@move_m@prisoner_cuffed", "idle");

                _cuffsProp?.Delete();
                _cuffsProp = null;

                SetPedCanPlayGestureAnims(ClientPed.Handle, true);
                Tick -= CuffTick;
            }
        }

        private void ReadConfigFile()
        {
            string data = LoadResourceFile(GetCurrentResourceName(), "config.ini");

            // this is only for people using CustomSounds by LondonStudios.
            _usingCS = Config.GetValue(data, "Cuff", "UsingCustomSounds", false);
            if (!Config.KeyExists(data, "Cuff", "UsingCustomSounds"))
            {
                Log.InfoOrError($"ERROR: 'config.ini' not configured properly or is missing. Please check if the config is there or has any data inside of it. ", "CUFF");
            }
        }

        private async void PlayCuffAnimation(int cuffer, bool isZiptie)
        {
            TriggerServerEvent("Cuff:Server:PlayAnimation", cuffer, !_isCuffed);

            if (_isCuffed)
            {
                if (!isZiptie && _usingCS) TriggerServerEvent("Server:SoundToRadius", ClientPed.NetworkId, 5f, "cuff", 0.2f);

                ClientPed.Task.ClearAll();
                ClientPed.Task.PlayAnimation(_isFrontCuffed ? "anim@move_m@prisoner_cuffed" : "mp_arresting", "idle", 8f, -1, AnimationFlags.StayInEndFrame | AnimationFlags.AllowRotation | AnimationFlags.UpperBodyOnly);

                await Delay(1000);
                _cuffsProp = await World.CreateProp(new($"{(isZiptie ? "hei_prop_zip_tie_positioned" : "p_cs_cuffs_02_s")}"), Vector3.Zero, false, false);

                Vector3 pos, rot;

                if (_isFrontCuffed)
                {
                    pos = isZiptie ? new(-0.012f, 0f, 0.08f) : new(-.058f, .005f, .09f);
                    rot = isZiptie ? new(340f, 95f, 120f) : new(290f, 95f, 120f);
                }
                else
                {
                    pos = isZiptie ? new(-0.020f, 0.035f, 0.06f) : new(-.055f, .06f, .04f);
                    rot = isZiptie ? new(0.04f, 155f, 80f) : new(265f, 155f, 80f);
                }

                AttachEntityToEntity(_cuffsProp.Handle, ClientPed.Handle, GetPedBoneIndex(ClientPed.Handle, 60309), pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z, true, false, false, false, 0, true);

                SetPedDropsWeapon(ClientPed.Handle);
                SetPedCanPlayGestureAnims(ClientPed.Handle, false);

                Tick += CuffTick;
            }
            else
            {
                await Delay(3000);

                if (!isZiptie && _usingCS) TriggerServerEvent("Server:SoundToRadius", ClientPed.NetworkId, 5f, "uncuff", 0.2f);

                ClientPed.Task.ClearAnimation("mp_arresting", "idle");
                ClientPed.Task.ClearAnimation("anim@move_m@prisoner_cuffed", "idle");

                _cuffsProp?.Delete();
                _cuffsProp = null;

                SetPedCanPlayGestureAnims(ClientPed.Handle, true);

                Tick -= CuffTick;
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("Cuff:Client:PlayAnimation")]
        private void OnPlayAnimation(bool uncuff) => ClientPed.Task.PlayAnimation(uncuff ? "mp_arresting" : "rcmpaparazzo_3", uncuff ? "a_uncuff" : "poppy_arrest_cop", 4f, 4f, 3000, AnimationFlags.UpperBodyOnly, 0.595f);

        [EventHandler("Cuff:Client:GetCuffedPlayer")]
        private void OnGetCuffedPlayer(int cuffer, bool isFront, bool isZiptie)
        {
            _isCuffed = !_isCuffed;
            _isFrontCuffed = isFront;
            PlayCuffAnimation(cuffer, isZiptie);
        }
        #endregion

        #region Ticks
        private async Task CuffTick()
        {
            List<Control> controls = new()
            {
                Control.Attack, Control.Attack2, Control.MeleeAttack1, Control.MeleeAttack2, Control.MeleeAttackAlternate, Control.MeleeAttackHeavy, Control.MeleeAttackLight,
                Control.Aim, Control.SelectWeapon, Control.VehicleMoveLeftOnly, Control.VehicleMoveLeftRight, Control.MoveRightOnly, Control.VehicleHandbrake,
                Control.VehicleSubTurnLeftRight, Control.VehicleSubTurnLeftOnly, Control.VehicleSubTurnRightOnly, Control.VehicleSubTurnHardLeft, Control.VehicleSubTurnHardRight
            };

            controls.ForEach(c => Controls.DisableControlThisFrame(c));
            if (!IsEntityPlayingAnim(ClientPed.Handle, _isFrontCuffed ? "anim@move_m@prisoner_cuffed" : "mp_arresting", "idle", 3))
            {
                ClientPed.Task.PlayAnimation(_isFrontCuffed ? "anim@move_m@prisoner_cuffed" : "mp_arresting", "idle", 8f, -1, AnimationFlags.StayInEndFrame | AnimationFlags.AllowRotation | AnimationFlags.UpperBodyOnly);
            }

            ClientPed.Weapons.Select(WeaponHash.Unarmed);
            SetPedStealthMovement(ClientPed.Handle, false, "DEFAULT_ACTION");
        }
        #endregion
    }
}
