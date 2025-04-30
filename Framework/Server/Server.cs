using Common;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Framework.Server
{
    public class Server : BaseScript
    {
        #region Variables
        internal string _aopSetter = "SYSTEM";
        internal string _currentAop = "Statewide";
        #endregion

        #region Constructor
        public Server()
        {
            SetMapName("San Andreas");
            SetConvarServerInfo("AreaOfPatrol", "");
        }
        #endregion

        #region Commands
        [Command("kick")]
        private void KickCommand(int playerId)
        {
            Player player = Players[playerId];
            player.Drop("You have been kicked from the server.");
        }

        [Command("aop")]
        private void AopCommand([FromSource] Player player, string[] args)
        {
            if (args.Length != 0)
            {
                if (IsPlayerAceAllowed(player.Handle, "framework.ChangeAop"))
                {
                    _currentAop = string.Join(" ", args);
                    TriggerClientEvent("Framework:Notes.Notes.Client:ChangeAop", _currentAop);
                    TriggerClientEvent("chat:addMessage", new { templateId = "TemplateGrey", color = new[] { 255, 255, 255 }, multiline = true, args = new[] { "", $"Current AOP is now ^5^*{_currentAop}^r^7" } });

                    SetConvarServerInfo("AreaOfPatrol", _currentAop);
                    Log.InfoOrError($"{player.Name} used the command: /aop with a AOP set to {_currentAop}", "FRAMEWORK");
                }
                else
                {
                    player.TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, multiline = true, args = new[] { "SYSTEM", "You don't have permission to this command." } });
                    Log.InfoOrError($"{player.Name} attempted to use the /aop command with arguments but failed because they permissions (permission ace: framework.ChangeAop).", "FRAMEWORK");
                }
            }
            else
            {
                player.TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, multiline = true, args = new[] { "SYSTEM", $"Current AOP is now {_currentAop}" } });
            }
        }

        [Command("resetaop")]
        private void ResetAopCommand([FromSource] Player player)
        {
            if (IsPlayerAceAllowed(player.Handle, "framework.ResetAop"))
            {
                _currentAop = "Statewide";
                SetConvarServerInfo("AreaOfPatrol", "Statewide");

                TriggerClientEvent("Framework:Notes.Notes.Client:ChangeAop", _currentAop);
                TriggerClientEvent("chat:addMessage", new { templateId = "TemplateGrey", color = new[] { 255, 255, 255 }, multiline = true, args = new[] { "", "Current AOP is now set to Statewide" } });
            }
            else
            {
                player.TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, multiline = true, args = new[] { "SYSTEM", "You don't have access to this command." } });
                Log.InfoOrError($"{player.Name} attempted to use the /resetaop command with arguments but failed because they permissions (permission ace: framework.ResetAop).", "FRAMEWORK");
            }
        }

        [Command("setaop")]
        private void SetAopCommand([FromSource] Player player, string[] args)
        {
            if (args.Length != 0)
            {
                if (IsPlayerAceAllowed(player.Handle, "framework.ChangeAop"))
                {
                    _currentAop = string.Join(" ", args);
                    TriggerClientEvent("Framework:Notes.Notes.Client:ChangeAop", _currentAop);
                    TriggerClientEvent("chat:addMessage", new { templateId = "TemplateGrey", color = new[] { 255, 255, 255 }, multiline = true, args = new[] { "", $"Current AOP is now ^5^*{_currentAop}^r^7" } });

                    SetConvarServerInfo("AreaOfPatrol", _currentAop);
                    Log.InfoOrError($"{player.Name} used the command: /aop with a AOP set to {_currentAop}", "FRAMEWORK");
                }
                else
                {
                    player.TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, multiline = true, args = new[] { "SYSTEM", "You don't have access to this command." } });
                    Log.InfoOrError($"{player.Name} attempted to use the /setaop command but couldn't because they permissions (permission ace: framework.ChangeAop)", "FRAMEWORK");
                }
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("Framework:DropUser")]
        private void OnDropUser([FromSource] Player player) => player.Drop("Dropped via framework.");
        #endregion
    }
}
