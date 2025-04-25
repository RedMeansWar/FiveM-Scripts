using System;
using System.Collections.Generic;
using Common;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Chat.Server
{
    public class Server : BaseScript
    {
        #region Commands
        [Command("say")]
        private void SayCommand([FromSource] Player player, string[] args)
        {
            if (player.Handle != "0")
            {
                return;
            }

            TriggerLatentClientEvent("chat:chatMessage", 5000, "SYSTEM", new int[] { 194, 39, 39 }, string.Join(" ", args));
        }
        #endregion

        #region Event Handlers
        [EventHandler("_chat:chatNearby")]
        private void OnChatMessageNearby([FromSource] Player player, dynamic author, dynamic color, dynamic message, dynamic nearbyPlayers, Vector3 authorPos)
        {
            Log.InfoOrError($"{author}: {message}", "CHAT");
            foreach (var playerId in nearbyPlayers)
            {
                if (Players[playerId] is Player ply && ply.Name is not null)
                {
                    List<dynamic> chatMessageArgs = new() { author, color, message };

                    if (ply.Handle != player.Handle)
                    {
                        chatMessageArgs.Add(authorPos);
                    }

                    ply.TriggerEvent("chat:chatMessage", chatMessageArgs.ToArray());
                }
            }
        }

        [EventHandler("_chat:messageEntered")]
        private void OnMessageEntered([FromSource] Player player, dynamic author, dynamic color, dynamic message)
        {
            Log.InfoOrError($"{author}: {message}", "CHAT");

            if (!WasEventCanceled() && !message.StartsWith("/"))
            {
                TriggerClientEvent("chat:chatMessage", author, color, message);
            }
        }

        [EventHandler("_chat:radioMessage")]
        private void OnRadioMessage([FromSource] Player player, string message)
        {
            Log.InfoOrError($"{player.Name}: {message}", "CHAT");
            TriggerClientEvent("chat:radioMessage", $"{player.Name} (#{int.Parse(player.Handle)})", message);
        }

        [EventHandler("_chat:twitterMessage")]
        private void OnTwitterMessage([FromSource] Player player, string username, string message)
        {
            Log.InfoOrError($"{username}: {message}", "CHAT");
            TriggerClientEvent("chat:twitterMessage", username, message);
        }

        [EventHandler("_chat:911Message")]
        private void On911Message([FromSource] Player player, string location, string message)
        {
            Log.InfoOrError($"{location}: {message}", "CHAT");
            TriggerClientEvent("chat:911Message", $"{player.Name} [{location}] (#{int.Parse(player.Handle)})");
        }

        [EventHandler("_chat:311Message")]
        private void On311Message([FromSource] Player player, string location, string message)
        {
            Log.InfoOrError($"{location}: {message}", "CHAT");
            TriggerClientEvent("chat:311Message", $"{player.Name} [{location}] (#{int.Parse(player.Handle)})");
        }
        #endregion
    }
}
