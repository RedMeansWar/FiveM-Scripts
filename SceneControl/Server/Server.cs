using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Server;
using CitizenFX.Core;
using SceneControl.Common;
using static CitizenFX.Core.Native.API;

namespace SceneControl.Server
{
    public class Server : BaseScript
    {
        #region Variables
        internal readonly List<SpeedZone> _speedzones = new();
        #endregion

        #region Commands
        [Command("clearallzones")]
        private void ClearAllZonesCommand([FromSource] Player player)
        {
            if (IsPlayerAceAllowed(player.Handle, "sceneControl.ClearZones"))
            {
                player.TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "SYSTEM", "You don't have access to this command." } });
                return;
            }

            Log.InfoOrError($"Cleared all speed zones.", "SCENE CONTROL");

            _speedzones.Clear();
            TriggerClientEvent("SceneControl:Client:UpdateSpeedZones", Json.Stringify(_speedzones));

            player.TriggerEvent("chat:addMessage", new { color = new[] { 0, 255, 0 }, args = new[] { "SYSTEM", "Cleared all speed zones." } });
        }

        [Command("clearallspeedzones")]
        private void ClearAllSpeedZonesCommands([FromSource] Player player) => ClearAllZonesCommand(player);
        #endregion

        #region Event Handlers
        [EventHandler("playerDropped")]
        private void OnPlayerDropped([FromSource] Player player, string reason)
        {
            SpeedZone playerZone = _speedzones.FirstOrDefault(sz => sz.PlayerId == int.Parse(player.Handle));
            if (playerZone is not null)
            {
                Vector3 zonePos = new(playerZone.X, playerZone.Y, playerZone.Z);
                Log.InfoOrError($"{player.Name} disconnected, deleting their speed zones at \n{zonePos}", "SCENE CONTROL");

                _speedzones.Remove(playerZone);
                TriggerClientEvent("SceneControl:Client:UpdateSpeedZones", Json.Stringify(_speedzones));
            }
        }

        [EventHandler("SceneControl:Server:GetAllSpeedZones")]
        private void OnGetAllSpeedZones([FromSource] Player player)
        {
            if (_speedzones.Count > 0)
            {
                player.TriggerEvent("SceneControl:Client:UpdateSpeedZones", Json.Stringify(_speedzones));
            }
        }

        [EventHandler("SceneControl:Server:CreateSpeedZone")]
        private void OnCreateSpeedZone([FromSource] Player player, Vector3 zonePosition, int zoneRadius, float zoneSpeed)
        {
            Log.InfoOrError($"{player.Name} created a speed zone at {zonePosition} with radius {zoneRadius}m and a speed {zoneSpeed.ToString("0.##")}mph.", "SCENE CONTROL");

            SpeedZone playerZone = _speedzones.FirstOrDefault(sz => sz.PlayerId == int.Parse(player.Handle));
            if (_speedzones.Any(sz => zonePosition.DistanceTo(new(playerZone.X, playerZone.Y, playerZone.Z)) <= sz.Radius))
            {
                player.TriggerEvent("SceneControl:Client:Notify", "~r~Speed Zone~w~: There is already a speed limit set for this location.");
            }
            else if (playerZone is not null)
            {
                player.TriggerEvent("SceneControl:Client:Notify", $"~r~Speed Zone~w~: You already have a speed zone placed at {new Vector2(playerZone.X, playerZone.Y)}.");
            }
            else
            {
                _speedzones.Add(new SpeedZone { X = zonePosition.X, Y = zonePosition.Y, Z = zonePosition.Z, Radius = zoneRadius, Speed = zoneSpeed, PlayerId = int.Parse(player.Handle) });
                TriggerClientEvent("SceneControl:Client:UpdateSpeedZones", Json.Stringify(_speedzones));
            }
        }

        [EventHandler("SceneControl:Server:DeleteSpeedZone")]
        private void OnDeleteSpeedZone([FromSource] Player player, Vector3 playerPos)
        {
            SpeedZone closestZone = null;
            float closestDistance = SceneConstants.SpeedZoneRadiuses.Last() + 1;

            foreach (SpeedZone sz in _speedzones)
            {
                Vector3 zonePos = new(playerPos.X, playerPos.Y, playerPos.Z);
                float distance = playerPos.DistanceTo(zonePos);
                
                if (distance <= sz.Radius && distance < closestDistance)
                {
                    closestZone = sz;
                    closestDistance = distance;
                }
            }

            if (closestZone is not null)
            {
                Vector3 zonePos = new(closestZone.X, closestZone.Y, closestZone.Z);
                Log.InfoOrError($"{player.Name} deleted a speed zone at {zonePos}.", "SCENE CONTROL");

                _speedzones.Remove(closestZone);
                TriggerClientEvent("ceneControl:Client:UpdateSpeedZones", Json.Stringify(_speedzones));
            }
            else
            {
                player.TriggerEvent("SceneControl:Client:Notify", $"~r~Speed Zone~w~: You are not inside any active speed zones.");
            }
        }
        #endregion
    }
}
