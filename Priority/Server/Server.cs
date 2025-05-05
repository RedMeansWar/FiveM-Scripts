using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using Common;
using static CitizenFX.Core.Native.API;

namespace Priority.Server
{
    public class ServerMain : BaseScript
    {
        #region Variables
        internal long? _priorityHoldExpiresAt;
        internal long? _priorityTimerExpiresAt;
        internal long? _priorityForceEndAt;
        internal string _priorityHolder;
        internal string _priorityHolderLicense;
        #endregion

        #region Methods
        private void UpdatePriorityState(Player player = null)
        {
            if (player is null)
            {
                TriggerClientEvent("Priority:Client:UpdatePriorityStatus", _priorityHolder, _priorityTimerExpiresAt, _priorityHoldExpiresAt, _priorityForceEndAt);
            }
            else
            {
                player.TriggerEvent("Priority:Client:UpdatePriorityStatus", _priorityHolder, _priorityTimerExpiresAt, _priorityHoldExpiresAt, _priorityForceEndAt);
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("playerDropped")]
        private void OnPlayerDropped([FromSource] Player player, string reason)
        {
            if (_priorityHolder is not null && _priorityHolderLicense == player.Identifiers["license"])
            {
                _priorityForceEndAt = DateTime.UtcNow.AddMinutes(5).Ticks;
                UpdatePriorityState();
            }
        }

        [EventHandler("playerConnecting")]
        private void OnPlayerConnecting([FromSource] Player player)
        {
            if (_priorityHolder is not null && _priorityHolderLicense == player.Identifiers["license"])
            {
                _priorityForceEndAt = null;
                UpdatePriorityState();
            }
        }

        [EventHandler("Priority:Server:StartPriority")]
        private void OnStartPriority([FromSource] Player player)
        {
            if (_priorityHolder is not null)
            {
                player.TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "[Priority]", "There is already an ongoing priority." } });
            }
            else if (_priorityTimerExpiresAt.HasValue && _priorityTimerExpiresAt.Value > DateTime.UtcNow.Ticks)
            {
                player.TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "[Priority]", "Priority is currently on cooldown, please wait." } });
            }
            else if (_priorityHoldExpiresAt.HasValue && _priorityHoldExpiresAt.Value > DateTime.UtcNow.Ticks)
            {
                player.TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "[Priority]", "There is currently an active priority hold." } });
            }
            else
            {
                Log.InfoOrError($"{player.Name} started a priority.", "Priority");

                _priorityHolder = player.Name;
                _priorityHolderLicense = player.Identifiers["license"];

                UpdatePriorityState();
            }
        }

        [EventHandler("Priority:Server:EndPriority")]
        private void OnEndPriority([FromSource] Player player, double endDuration = 10)
        {
            if (_priorityHolder is null)
            {
                player.TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "[Priority]", " There is no current active priority to end." } });
            }
            else if (_priorityHolderLicense != player.Identifiers["license"] && !IsPlayerAceAllowed(player.Handle, "priority.EndPriority"))
            {
                player.TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "[Priority]", "Only the starter of the priority may end the priority." } });
            }
            else
            {
                _priorityHolder = null;
                _priorityHolderLicense = null;
                _priorityTimerExpiresAt = DateTime.UtcNow.AddMinutes(endDuration).Ticks;

                UpdatePriorityState();
            }
        }
        
        // honestly I don't remember why I added this. I will leave it here if I can use it later?
        // [EventHandler("Priority:Server:TransferPriority")]
        // private void OnTransferPrioriity([FromSource] Player player)
        // {
        //     if (_priorityHolder is null)
        //     {
        //         player.TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "[Priority]", "There is no current active priority to transfer ownership for." } });
        //     }
        //     else if (_priorityForceEndAt == null)
        //     {
        //         player.TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "[Priority]", "The starter of the priority must be disconnected from the server in order to transfer ownership." } });
        //     }
        //     else
        //     {
        //         Log.InfoOrError($"{player.Name} transferred the ownership of the current priority.", "Priority");
        //
        //         _priorityHolder = player.Name;
        //         _priorityHolderLicense = player.Identifiers["license"];
        //         _priorityForceEndAt = null;
        //
        //         UpdatePriorityState();
        //     }
        // }

        [EventHandler("Priority:Server:HoldPriority")]
        private void OnHoldPriority([FromSource] Player player, double holdDuration = 0)
        {
            if (!IsPlayerAceAllowed(player.Handle, "priority.HoldPriority"))
            {
                player.TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "[Priority]", "You don't have permission to this command." } });
                return;
            }

            if (holdDuration == 0)
            {
                _priorityHoldExpiresAt = null;
            }
            else if (holdDuration > 60)
            {
                player.TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "[Priority]", "Priority hold duration must be between 0 and 60 minutes" } });
                return;
            }
            else
            {
                _priorityHoldExpiresAt = DateTime.UtcNow.AddMinutes(holdDuration).Ticks;
            }

            _priorityHolder = null;
            _priorityHolderLicense = null;
            _priorityTimerExpiresAt = null;
            _priorityForceEndAt = null;

            UpdatePriorityState();
        }

        [EventHandler("Priority:Server:ResetPriority")]
        private void OnResetPriority([FromSource] Player player)
        {
            if (!IsPlayerAceAllowed(player.Handle, "priority.ResetPriority"))
            {
                player.TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "[Priority]", "You don't have permission to this command." } });
                return;
            }

            _priorityHolder = null;
            _priorityHolderLicense = null;
            _priorityTimerExpiresAt = null;
            _priorityForceEndAt = null;

            UpdatePriorityState();
            player.TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "[Priority]", "Priority status has been reset." } });
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task CheckPriorityOwnerConnectedTick()
        {
            if (_priorityForceEndAt.HasValue && _priorityForceEndAt.Value < DateTime.UtcNow.Ticks)
            {
                _priorityHolder = null;
                _priorityHolderLicense = null;
                _priorityTimerExpiresAt = null;
                _priorityHoldExpiresAt = null;

                UpdatePriorityState();
            }
        }
        #endregion
    }
}