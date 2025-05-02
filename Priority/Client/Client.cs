using System;
using System.Threading.Tasks;
using Common;
using Common.Client;
using Common.Models;
using CitizenFX.Core;

namespace Priority.Client
{
    public class Client : ClientCommonScript
    {
        #region Variables
        internal bool _displayUi = true;
        internal Character _currentCharacter;
        internal long? _priorityHoldExpiresAt, _priorityTimerExpiresAt, _priorityForceEndAt;
        internal string _priorityHolder;
        #endregion

        #region Commands
        [Command("priority")]
        private void PriorityCommand(string[] args)
        {
            if (args.Length == 0)
            {
                Hud.SendChatMessage("Usage: /priority [start(s) | end(e) <duration> | hold(h) <seconds>]", "Priority", 255, 0, 0);
            }
            else
            {
                switch (args[0].ToString().ToLower())
                {
                    case "start":
                    case "s":
                        TriggerEvent("Priority:Client:StartPriority");
                        break;

                    case "end":
                    case "e":
                        double duration = args.Length == 2 && (args[1] == "10" || args[1] == "30") ? int.Parse(args[1]) : 10;
                        TriggerEvent("Priority:Client:EndPriority", duration);
                        break;

                    case "hold":
                    case "h":
                        TriggerEvent("Priority:Client:HoldPriority", args.Length == 2 ? int.Parse(args[1].ToString()) : 0);
                        break;

                    case "transfer":
                    case "t":
                        TriggerEvent("Priority:Client:TransferPriority");
                        break;

                    case "reset":
                    case "r":
                        TriggerEvent("Priority:Client:ResetPriority");
                        break;

                    default:
                        break;
                }
            }
        }

        [Command("p")]
        private void PCommand(string[] args) => PriorityCommand(args);
        #endregion

        #region Methods
        private string TimeSince(DateTime fromDate, DateTime toDate)
        {
            TimeSpan timeDist = fromDate - toDate;

            string totalMin = timeDist.TotalMinutes.ToString("0");
            string totalSec = timeDist.TotalSeconds.ToString("0");

            return timeDist.TotalMinutes >= 1 ? $"{totalMin}m" : $"{totalSec}s";
        }
        #endregion

        #region Event Handlers
        [EventHandler("Framework:Client:SelectedCharacter")]
        private void OnSelectCharacter(string json) => _currentCharacter = Json.Parse<Character>(json);

        [EventHandler("Priority:Client:ToggleUi")]
        private void OnToggleUi(bool displayed) => _displayUi = displayed;

        [EventHandler("Priority:Client:UpdatePriorityStatus")]
        private void OnUpdatePriorityStatus(string priorityHolder, long? priorityTimerExpiresAt, long? priorityHoldExpiresAt, long? priorityForceEndAt)
        {
            _priorityHolder = priorityHolder;
            _priorityTimerExpiresAt = priorityTimerExpiresAt;
            _priorityHoldExpiresAt = priorityHoldExpiresAt;
            _priorityForceEndAt = priorityForceEndAt;
        }

        [EventHandler("Priority:Client:StartPriority")]
        private void OnStartPriority()
        {
            if (_currentCharacter.Department is not "Civ")
            {
                Hud.SendChatMessage("You need to be a civilian character to do this.", "[Priority]", 255, 0, 0);
                return;
            }

            TriggerServerEvent("Priority:Notes.Server:StartPriority");
        }

        [EventHandler("Priority:Client:EndPriority")]
        private void OnEndPriority(int endDuration)
        {
            if (_currentCharacter.Department is not "Civ")
            {
                Hud.SendChatMessage("You need to be a civilian character to do this.", "[Priority]", 255, 0, 0);
                return;
            }

            TriggerServerEvent("Priority:Notes.Server:EndPriority", endDuration);
        }

        [EventHandler("Priority:Client:TransferPriority")]
        private void OnTransferPriority()
        {
            if (_currentCharacter.Department is not "Civ")
            {
                Hud.SendChatMessage("You need to be a civilian character to do this.", "[Priority]", 255, 0, 0);
                return;
            }

            TriggerServerEvent("Priority:Notes.Server:TransferPriority");
        }

        [EventHandler("Priority:Client:holdPriority")]
        private void OnHoldPriority(int holdDuration)
        {
            if (_currentCharacter.Department is not "Civ")
            {
                Hud.SendChatMessage("You need to be a civilian character to do this.", "[Priority]", 255, 0, 0);
                return;
            }

            TriggerServerEvent("Priority:Notes.Server:HoldPriority", holdDuration);
        }

        [EventHandler("Priority:Client:ResetPriority")]
        private void OnResetPriority() => TriggerServerEvent("Priority:Notes.Server:ResetPriority");
        #endregion

        #region Ticks
        [Tick]
        private async Task PriorityTick()
        {
            if (_currentCharacter is null || _currentCharacter.Department is not "Civ")
            {
                await Delay(1000);
                return;
            }

            if (_displayUi && !Hud.IsHudHidden)
            {
                string priorityStatus;

                if (_priorityForceEndAt.HasValue && _priorityForceEndAt.Value > DateTime.UtcNow.Ticks)
                {
                    priorityStatus = $"~r~Disconnected ~c~({TimeSince(new DateTime(_priorityForceEndAt.Value), DateTime.UtcNow)} remaining)";
                }
                else if (_priorityHoldExpiresAt.HasValue && _priorityHoldExpiresAt.Value > DateTime.UtcNow.Ticks)
                {
                    priorityStatus = $"~r~Held ~c~({TimeSince(new DateTime(_priorityHoldExpiresAt.Value), DateTime.UtcNow)} remaining)";
                }
                else if (_priorityHolder is not null)
                {
                    priorityStatus = $"~r~Active ~c~({_priorityHolder})";
                }
                else if (_priorityTimerExpiresAt.HasValue && _priorityTimerExpiresAt.Value < DateTime.UtcNow.Ticks)
                {
                    priorityStatus = $"~g~Inactive ~c~({TimeSince(DateTime.UtcNow, new(_priorityTimerExpiresAt.Value))} ago)";
                }
                else if (_priorityTimerExpiresAt.HasValue && _priorityTimerExpiresAt.Value >= DateTime.UtcNow.Ticks)
                {
                    priorityStatus = $"~r~Cooldown ~c~({TimeSince(new(_priorityTimerExpiresAt.Value), DateTime.UtcNow)} remaining)";
                }
                else
                {
                    priorityStatus = $"~g~Inactive";
                }

                if (priorityStatus is not null)
                {
                    Hud.DrawText2d(1.203f, -0.075f, 0.419f, $"Priority Status: {priorityStatus}", 255, 255, 255, 255);
                }
            }
        }
        #endregion
    }
}
