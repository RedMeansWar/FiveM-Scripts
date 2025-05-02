using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Models;
using Common.Client;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Chat.Client
{
    public class Client : ClientCommonScript
    {
        #region Variables
        internal bool _chatInit, _chatActive;
        internal string _twitterName;
        internal Character _currentCharacter;
        internal List<ChatSuggestion> _suggestions;
        #endregion

        #region Constructor
        public Client() => RegisterNUICallback("chatResult", ChatResult);
        #endregion

        #region Commands
        [Command("settwitter")]
        private void SetTwitterCommand(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
            {
                Hud.SendChatMessage("Invalid Twitter username. Usage: /settwitter [username]", r: 255, g: 0, b: 0);
                return;
            }

            string username = args[0];

            if (username.Length > 18)
            {
                Hud.SendChatMessage("Invalid Twitter username. Username must not exceed 18 characters. Usage: /settwitter [username]", r: 255, g: 0, b: 0);
                return;
            }

            if (!username.All(c => char.IsLetterOrDigit(c) || c.Equals("_")))
            {
                Hud.SendChatMessage("Invalid Twitter username. Username must not include special characters. Usage: /settwitter [username]", r: 255, g: 0, b: 0);
                return;
            }

            SetResourceKvp($"rd_chat_twitter_{_currentCharacter.CharacterId}", username);
            _twitterName = username;

            Hud.SendChatMessage($"Twitter username set to ^*{username}^r", r: 0, g: 255, b: 0);
            TriggerEvent("chat:chatMessage", "SYSTEM", new[] { 0, 255, 0 }, $"Twitter username set to ^*{username}^r");
        }
        #endregion

        #region NUI Callbacks
        private void ChatResult(IDictionary<string, object> data, CallbackDelegate result)
        {
            _chatActive = false;
            SetNuiFocus(false, false);

            string message = data.GetVal<string>("message", null);
            bool cancel = data.GetVal("canceled", false);

            if (!cancel && !string.IsNullOrWhiteSpace(message))
            {
                Vector3 plyPos = ClientPed.Position;

                message = message.Trim();
                string[] args = message.Split(' ');
                string firstArg = args[0].ToLower();
                string joinedArgs = string.Join(" ", args.Skip(1));

                switch (firstArg)
                {
                    case "/ooc":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("_chat:chatNearby", $"^* ^4[OOC] {Game.Player.Name} (#{Game.Player.ServerId})^r", new[] { 255, 140, 0 }, joinedArgs, Players.Where(p => Vector3.Distance(p.Character.Position, plyPos) < 35).Select(p => p.ServerId).ToList(), plyPos);
                        }
                        break;

                    case "/gooc":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("_chat:messageEntered", $"^*[GOOC] {Game.Player.Name} (#{Game.Player.ServerId})^r", new[] { 255, 140, 0 }, joinedArgs);
                        }
                        break;

                    case "/me":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("_chat:chatNearby", $"^* ^7[ME] {_currentCharacter?.FirstName} {_currentCharacter?.LastName} [{_currentCharacter?.Department}] (#{Game.Player.ServerId})^r", new[] { 255, 140, 0 }, joinedArgs, Players.Where(p => Vector3.Distance(p.Character.Position, plyPos) < 35).Select(p => p.ServerId).ToList(), plyPos);
                        }
                        break;

                    case "/mer":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("_chat:chatNearby", $"^* ^1[ME] {_currentCharacter?.FirstName} {_currentCharacter?.LastName} [{_currentCharacter?.Department}] (#{Game.Player.ServerId})^r", new[] { 255, 140, 0 }, joinedArgs, Players.Where(p => Vector3.Distance(p.Character.Position, plyPos) < 35).Select(p => p.ServerId).ToList(), plyPos);
                        }
                        break;

                    case "/meb":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("_chat:chatNearby", $"^* ^5[ME] {_currentCharacter?.FirstName} {_currentCharacter?.LastName} [{_currentCharacter?.Department}] (#{Game.Player.ServerId})^r", new[] { 255, 140, 0 }, joinedArgs, Players.Where(p => Vector3.Distance(p.Character.Position, plyPos) < 35).Select(p => p.ServerId).ToList(), plyPos);
                        }
                        break;

                    case "/rt":

                        if (_currentCharacter?.Department == "Civ")
                        {
                            TriggerEvent("chat:chatMessage", "SYSTEM", new[] { 194, 39, 39 }, "You aren't authorized to use the /rt command. You must not be and civilian character.");
                            result(new { success = false, message = "not authorized" });
                            return;
                        }

                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("_chat:radioMessage", joinedArgs);
                        }
                        break;

                    case "/twt":
                    case "/twitter":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            if (_currentCharacter?.Department is "LSFD")
                            {
                                TriggerServerEvent("_chat:twitterMessage", "@LSFDOFFICAL ✔️", joinedArgs);
                            }

                            if (_currentCharacter?.Department is "BCSO")
                            {
                                TriggerServerEvent("_chat:twitterMessage", "@BCSOOFFICAL ✔️", joinedArgs);
                            }

                            if (_currentCharacter?.Department is "SAHP")
                            {
                                TriggerServerEvent("_chat:twitterMessage", "@SAHOFFICAL ✔️", joinedArgs);
                            }

                            if (_currentCharacter?.Department is "LSPD")
                            {
                                TriggerServerEvent("_chat:twitterMessage", "@LSPDOFFICAL ✔️", joinedArgs);
                            }

                            TriggerServerEvent("_chat:twitterMessage", $"@{_twitterName}", joinedArgs);
                        }
                        break;

                    case "911":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            uint streetNameAsHash = 0;
                            uint crossingRoadAsHash = 0;

                            GetStreetNameAtCoord(ClientPed.Position.X, ClientPed.Position.Y, ClientPed.Position.Z, ref streetNameAsHash, ref crossingRoadAsHash);
                            string streetName = GetStreetNameFromHashKey(streetNameAsHash);
                            string crossingRoad = GetStreetNameFromHashKey(crossingRoadAsHash);

                            string location = $"{Exports["nearestpostal"].getClosestPostal(ClientPed.Position)}, {streetName}";

                            if (!string.IsNullOrWhiteSpace(crossingRoad))
                            {
                                location += $" / {crossingRoad}";
                            }

                            TriggerServerEvent("_chat:911Message", location, joinedArgs);
                        }
                        break;

                    case "311":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            uint streetNameAsHash = 0;
                            uint crossingRoadAsHash = 0;

                            GetStreetNameAtCoord(ClientPed.Position.X, ClientPed.Position.Y, ClientPed.Position.Z, ref streetNameAsHash, ref crossingRoadAsHash);
                            string streetName = GetStreetNameFromHashKey(streetNameAsHash);
                            string crossingRoad = GetStreetNameFromHashKey(crossingRoadAsHash);

                            string location = $"{Exports["nearestpostal"].getClosestPostal(ClientPed.Position)}, {streetName}";

                            if (!string.IsNullOrWhiteSpace(crossingRoad))
                            {
                                location += $" / {crossingRoad}";
                            }

                            TriggerServerEvent("_chat:311Message", location, joinedArgs);
                        }
                        break;

                    default:
                        if (message.StartsWith("/"))
                        {
                            ExecuteCommand(message.Substring(1));
                            CancelEvent();
                        }
                        else
                        {
                            TriggerServerEvent("_chat:messageEntered", "", new[] { 3, 129, 255 }, joinedArgs);
                        }
                        break;
                }
            }

            result(new { success = true, message = "success" });
        }
        #endregion

        #region Methods
        private void LoadSuggestions()
        {
            try
            {
                string json = LoadResourceFile(GetCurrentResourceName(), "suggestions.json");
                if (string.IsNullOrWhiteSpace(json))
                {
                    Log.InfoOrError("'suggesions.json' is null or whitespace? Won't be able to populate chat suggestion list. Contact a developer.", "CHAT");
                    _suggestions = new();
                    return;
                }

                List<ChatSuggestion> list = Json.Parse<List<ChatSuggestion>>(json);
                if (list is null)
                {
                    Log.InfoOrError("Couldn't populate chat suggestion list!", "CHAT");
                    _suggestions = new();
                    return;
                }

                _suggestions = list;
                Log.InfoOrError($"Loaded {_suggestions.Count} chat suggestion(s)", "CHAT");
            }
            catch (Exception ex)
            {
                Log.Error($"Exeception was thrown!", "CHAT", ex);
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("Framework:Notes.Notes.Client:SelectedCharacter")]
        private void OnSelectedCharacter(string json)
        {
            _currentCharacter = Json.Parse<Character>(json);
            _twitterName = GetResourceKvpString($"rd_chat_twitter_{_currentCharacter.CharacterId}");

            if (string.IsNullOrEmpty(_twitterName))
            {
                _twitterName = $"{_currentCharacter.FirstName}{_currentCharacter.LastName}{_currentCharacter.DoB:yy}";
            }
        }

        [EventHandler("chat:addMessage")]
        private void AddChatMsg(dynamic msg)
        {
            SendNUIMessage(Json.Stringify(new
            {
                type = "ON_MESSAGE",
                message = msg
            }));
        }

        [EventHandler("chat:addTemplate")]
        private void AddChatTemplate(string id, string template)
        {
            SendNUIMessage(Json.Stringify(new
            {
                type = "ON_TEMPLATE_ADD",
                template = new
                {
                    id,
                    html = template
                }
            }));
        }

        [EventHandler("chat:chatMessage")]
        private void ChatMessage(dynamic author, dynamic colors, dynamic msg, Vector3 authorPos)
        {
            string distanceString = "";
            if (!authorPos.IsZero)
            {
                distanceString = $" ({Math.Round(Vector3.Distance(authorPos, ClientPed.Position))})m";
            }

            SendNUIMessage(Json.Stringify(new
            {
                type = "ON_MESSAGE",
                template = author == "" ? "defaultAlt" : "default",
                message = new
                {
                    color = new[] { colors[0], colors[1], colors[2] },
                    multiline = true,
                    args = author == "" ? new[] { $"{msg}" } : new[] { $"{author}{distanceString}", $"{msg}" }
                }
            }));
        }

        [EventHandler("chat:radioMessage")]
        private void OnRadioMessage(string name, string message)
        {
            if (_currentCharacter?.Department is "Civ")
            {
                return;
            }

            SendNUIMessage(Json.Stringify(new
            {
                type = "ON_MESSAGE",
                message = new
                {
                    color = new[] { 60, 179, 113 },
                    multiline = true,
                    args = new[] { $"[Radio] {name}", message }
                }
            }));
        }

        [EventHandler("chat:twitterMessage")]
        private void OnTwitterMessage(string name, string message)
        {
            SendNUIMessage(Json.Stringify(new
            {
                type = "ON_MESSAGE",
                message = new
                {
                    color = new[] { 0, 172, 238 },
                    multiline = true,
                    args = new[] { $"[Twitter] {name}", message }
                }
            }));
        }

        [EventHandler("chat:911Message")]
        private void On911Message(string name, string message)
        {
            SendNUIMessage(Json.Stringify(new
            {
                type = "ON_MESSAGE",
                message = new
                {
                    color = new[] { 205, 92, 92 },
                    multiline = true,
                    args = new[] { $"[911] {name}", message }
                }
            }));
        }

        [EventHandler("chat:311Message")]
        private void On311Message(string name, string message)
        {
            SendNUIMessage(Json.Stringify(new
            {
                type = "ON_MESSAGE",
                message = new
                {
                    color = new[] { 205, 92, 92 },
                    multiline = true,
                    args = new[] { $"[311] {name}", message }
                }
            }));
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task ChatTick()
        {
            if (!_chatInit)
            {
                _chatInit = true;

                LoadSuggestions();

                foreach (ChatSuggestion suggestion in _suggestions)
                {
                    SendNUIMessage($"{{\"type\":\"ON_SUGGESTION_ADD\",\"suggestion\":{{\"name\":\"{suggestion.Command}\",\"help\":\"{suggestion.Example}\",\"params\":\"\"}}}}");
                }

                SetTextChatEnabled(false);
                await Delay(1000);
            }

            if (!_chatActive && Controls.IsControlJustPressed(Control.MpTextChatAll))
            {
                _chatActive = true;
                SetNuiFocus(true, false);

                SendNUIMessage("{\"type\":\"ON_OPEN\"}");
                await Delay(10);
            }
        }
        #endregion
    }
}
