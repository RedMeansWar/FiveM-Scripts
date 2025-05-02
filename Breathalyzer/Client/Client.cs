using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Client;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Breathalyzer.Client
{
    public class Client : ClientCommonScript
    {
        #region Variables
        internal string _bac = "0.00";
        #endregion

        #region Constructor
        public Client()
        {
            RegisterNUICallback("startTest", StartTest);
            RegisterNUICallback("resetTest", ResetTest);
            RegisterNUICallback("closeNui", CloseNui);
        }
        #endregion

        #region Commands
        [Command("bac")]
        private void BacCommand() => DisplayNui();

        [Command("breathalyzer")]
        private void BreathalyzerCommand() => DisplayNui();

        [Command("setbac")]
        private void SetBacCommand(string[] args) => SetBacLevel(args);

        [Command("resetbac")]
        private void ResetBacCommand()
        {
            _bac = "0.00";
            Hud.SendChatMessage("You're BAC has been reset back to 0.00", "[Breathalyzer]");
        }
        #endregion

        #region NUI Methods
        private async void StartTest(IDictionary<string, object> data, CallbackDelegate result)
        {
            Player targetPlayer = GetClosestPlayer();
            if (targetPlayer is null)
            {
                Notify.Error("You need to be closer to the player you wish to breathalyze.");
                return;
            }

            TaskPlayAnim(ClientPed.Handle, "weapons@first_person@aim_rng@generic@projectile@shared@core", "idlerng_med", 1.0f, -1, 5000, 50, 0, false, false, false);
            await Delay(5000);

            TriggerServerEvent("Breathalyzer:Server:SubmitBacTest", targetPlayer.ServerId);
        }


        private void ResetTest(IDictionary<string, object> data, CallbackDelegate result) => SendNUIMessage(Json.Stringify(new { type = "RESET_BREATHALYZER_NUI" }));

        private void CloseNui(IDictionary<string, object> data, CallbackDelegate result) => DisplayNui(false);
        #endregion

        #region Methods
        private bool DisplayNui(bool display = true)
        {
            if (!display)
            {
                SetNuiFocus(false, false);
                SendNUIMessage(Json.Stringify(new { type = "CLOSE_BREATHALYZER_NUI" }));
                return false;
            }

            SetNuiFocus(true, true);
            SendNUIMessage(Json.Stringify(new
            {
                type = "DISPLAY_BREATHALYZER_NUI",
                level = "0.00"
            }));

            return true;
        }

        private void SetBacLevel(string[] args)
        {
            if (args.Length > 5)
            {
                Notify.Error("Your BAC level can't be more or than 5 characters.", true);
                return;
            }

            if (args.Length != 0 && args.Any(arg => arg.Contains(".")))
            {
                _bac = args[0];
                Notify.Success($"Your BAC level is now {_bac}.", true);
            }

            if (args.Any(arg => !arg.Contains(".")))
            {
                Notify.Error("You need to have a period in the arguments to set your BAC!", true);
                return;
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("Breathalyzer:Client:OpenBacSetter")]
        private async void OnOpenBacSetter(string[] args)
        {
            var bacInput = await Hud.GetUserInput("Set BAC Level (Legal Limit is 0.08)", 5);
            _bac = bacInput;

            if (string.IsNullOrEmpty(bacInput))
            {
                Notify.Error("You can't leave this blank!", true);
                return;
            }

            if (bacInput.Length > 5)
            {
                Notify.Error("BAC level can't be more than 5 characters!", true);
                return;
            }

            Notify.Success($"Your BAC level is now {_bac}.", true);
        }

        [EventHandler("Breathalyzer:Client:SubmitTest")]
        private void OnSubmitTest(string testerId) => TriggerServerEvent("Breathalyzer:Server:ReturnTest", testerId, _bac);

        [EventHandler("Breathalyzer:Client:ReturnLevel")]
        private async void OnReturnLevel(string bacLevel)
        {
            _bac = bacLevel;

            SendNUIMessage(Json.Stringify(new
            {
                type = "UPDATE_BAC_LEVEL",
                level = $"{bacLevel}"
            }));

            await Delay(500);
            PlaySoundFrontend(-1, "5_SEC_WARNING", "HUD_MINI_GAME_SOUNDSET", true);
        }
        #endregion
    }
}
