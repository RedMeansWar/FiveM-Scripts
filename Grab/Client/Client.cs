using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grab.Common;
using Common.Client;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Grab.Client
{
    public class Client : ClientCommonScript
    {
        #region Variables
        internal Player _grabbedPlayer, _grabberPlayer;
        internal Random _random = new();
        #endregion

        #region Commands
        [Command("grab")]
        private void GrabCommand()
        {
            if (GrabConstants.Grabbed) return;

            Player closestPlayer = GetClosestPlayer(4f);
            if (closestPlayer is null)
            {
                Notify.Error("You must be closer to the person you wish to grab.", true);
                return;
            }

            _grabbedPlayer = closestPlayer;
            GrabConstants.GrabbedPlayer = closestPlayer.Handle;
            TriggerServerEvent("Grab:Server:GrabClosestPlayer", closestPlayer.ServerId);
        }
        #endregion

        #region Event Handlers
        [EventHandler("Grab:Client:GetGrabbedPlayer")]
        private void OnGetGrabbedPlayer(string sender)
        {
            GrabConstants.Grabbed = !GrabConstants.Grabbed;
            if (GrabConstants.Grabbed)
            {
                _grabberPlayer = Players[int.Parse(sender)];
                GrabConstants.GrabberPlayer = Players[int.Parse(sender)].Handle;

                Tick += DisableControlsTick;
                Tick += GrabTick;

                if (!ClientPed.IsDead)
                {
                    Hud.DisplayHelpText("Spam ~INPUT_FRONTEND_RDOWN~ for a chance to escape.");
                }
            }
            else
            {
                Tick -= DisableControlsTick;
                Tick -= GrabTick;

                _grabberPlayer = null;
                GrabConstants.GrabberPlayer = 0;
                ClientPed.Detach();
                GrabConstants.GrabEscapeAttempts = 0;
            }
        }

        [EventHandler("Grab:Client:Notify")]
        private void OnNotify(string message) => Hud.DisplayNotification(message, true);
        #endregion

        #region Ticks
        private async Task GrabTick()
        {
            if (ClientPed.IsCuffed) GrabConstants.GrabEscapeAttempts += 10;

            AttachEntityToEntity(ClientPed.Handle, _grabberPlayer.Character.Handle, 11816, 0.45f, 0.35f, 0f, 0f, 0f, 0f, false, false, false, false, 2, true);

            if (Controls.IsControlJustPressedRegardless(Control.FrontendRdown) && GrabConstants.GrabEscapeAttempts < 3 && !ClientPlayer.IsDead)
            {
                GrabConstants.GrabEscapeAttempts++;
                if (GrabConstants.GrabEscapeAttempts == 1)
                {
                    TriggerServerEvent("Grab:Server:Notify", _grabbedPlayer.ServerId, "The person you're grabbing is attempting to wiggle out from your grip!");
                }

                int random = _random.Next(100);
                if (random < GrabConstants.GrabEscapeAttempts)
                {
                    TriggerServerEvent("Grab:Server:Notify", _grabbedPlayer.ServerId, "They've wiggled out from your grip!");
                    ClientPed.Detach();

                    GrabConstants.GrabEscapeAttempts = 0;

                    Tick -= DisableControlsTick;
                    Tick -= GrabTick;
                }
                else if (GrabConstants.GrabEscapeAttempts > 3)
                {
                    Hud.DisplayNotification("~y~You failed to wiggle out from their grip!", true);
                    ClearHelp(true);
                }

                await Delay(500);
            }
        }

        private async Task DisableControlsTick()
        {
            Game.DisableAllControlsThisFrame(0);
            List<Control> controls = new() { Control.LookLeftRight, Control.LookUpDown, Control.MpTextChatAll, Control.PushToTalk, Control.Phone };
            controls.ForEach(c => Controls.EnableControlThisFrame(c));
        }
        #endregion
    }
}
