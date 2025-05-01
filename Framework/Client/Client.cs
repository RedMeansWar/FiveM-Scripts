using System;
using System.Collections.Generic;
using Common;
using Common.Client;
using Common.Models;
using Newtonsoft.Json;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Framework.Client
{
    public class Client : ClientCommonScript
    {
        #region Variables
        internal bool _ran;
        internal string _currentAop = "Statewide";
        internal ISet<string> _allowedDepts = new HashSet<string>();
        public static Character _currentCharacter;
        #endregion

        #region Constructor
        public Client()
        {
            RegisterNUICallback("selectCharacter", SelectCharacter);
            RegisterNUICallback("createCharacter", CreateCharacter);
            RegisterNUICallback("editCharacter", EditCharacter);
            RegisterNUICallback("deleteCharacter", DeleteCharacter);
            RegisterNUICallback("closeFrameworkNui", CloseFrameworkNui);
            RegisterNUICallback("quitGame", QuitGame);
            RegisterNUICallback("disconnect", Disconnect);

            TriggerServerEvent("Framework:Notes.Server:GetDiscordRoles");
            StartAudioScene("CHARACTER_CHANGE_IN_SKY_SCENE");

            if (!_ran)
            {
                TriggerEvent("chat:addTemplate", "TemplateGreen", "<div style='background-color: rgba(0, 153, 0, 0.4); padding-top: 10px; padding-bottom: 10px; border-radius: 10px; text-align: center;'>{1}</div");
                TriggerEvent("chat:addTemplate", "TemplateGrey", "<div style='background-color: rgba(34, 34, 34, 0.4); padding-top: 10px; padding-bottom: 10px; border-radius: 10px; text-align: center;'>{1}</div");
                TriggerEvent("chat:addTemplate", "TemplateRed", "<div style='background-color: rgba(255, 0, 0, 0.4); padding-top: 10px; padding-bottom: 10px; border-radius: 10px; text-align: center;'>{1}</div");
                TriggerEvent("chat:addTemplate", "TemplateBlue", "<div style='background-color: rgba(0, 128, 255, 0.4); padding-top: 10px; padding-bottom: 10px; border-radius: 10px; text-align: center;'>{1}</div");

                Log.InfoOrError("Successfully registered chat templates.", "FRAMEWORK");
            }
        }
        #endregion

        #region Commands
        [Command("framework")]
        private void FrameworkCommand() => OpenFrameworkUi();

        [Command("fw")]
        private void FwCommand() => OpenFrameworkUi();

        [Command("selectcharacter")]
        private void SelectCharacterCommand() => OpenFrameworkUi();

        [Command("switchcharacter")]
        private void SwitchCharacterCommand() => OpenFrameworkUi();

        [Command("createcharacter")]
        private void CreateCharacterCommand()
        {
            // open the framework ui
            OpenFrameworkUi();

            // send a nui message that opens the create character modal
            SendNUIMessage(Json.Stringify(new { type = "FRAMEWORK_OPEN_CREATE_CHARACTER_MODAL" }));
        }

        [Command("dob")]
        private void DobCommand()
        {
            if (_currentCharacter is null)
            {
                // if the character is null then
                Hud.SendChatMessage("You must have a character selected in the framework to use this command.", r: 255, g: 0, b: 0);
                return;
            }

            Hud.SendChatMessage($"{_currentCharacter.FirstName} {_currentCharacter.LastName}'s date of birth is {_currentCharacter.DoB:MM/dd/yyyy} (Age: {CalculateAge(_currentCharacter.DoB)})");
        }
        #endregion

        #region NUI Callbacks
        private void SelectCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            string characterId = data.GetVal("charId", "0");
            string firstName = data.GetVal<string>("firstName", null);
            string lastName = data.GetVal<string>("lastName", null);
            string gender = data.GetVal<string>("gender", null);
            string department = data.GetVal<string>("department", null);
            string dob = data.GetVal<string>("dob", null);
            string cash = data.GetVal("cash", "-1");
            string bank = data.GetVal("bank", "-1.0");

            // we check if any of the JavaScript data is null or whitespace if so we through an error
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(gender) || string.IsNullOrWhiteSpace(department) || string.IsNullOrWhiteSpace(dob) || cash == "-1.0" || bank == "-1.0")
            {
                // send a framework error message saying that there was invalid data given to select the character
                SendNUIMessage(Json.Stringify(new { type = "FRAMEWORK_ERROR", msg = "We ran into a problem while selecting this character, please try again." }));

                // log the invalid info data
                Log.InfoOrError("Invalid character data while selecting this character!");

                // return the result as false
                result(new { success = false, message = "invalid character data" });

                return;
            }

            // set character attributes!
            Character selectedCharacter = CreateCharacter(firstName, lastName, gender, department, dob, cash, bank, characterId);

            // set the current character as the selected character
            _currentCharacter = selectedCharacter;

            // close the framework ui
            SendNUIMessage(Json.Stringify(new { type = "CLOSE_FRAMEWORK_NUI" }));
            SetNuiFocus(false, false); // set the nui focus to false so the player can actually play the game

            // send a framework message saying that they are playing the character
            Hud.SendFrameworkMessage($"You're now playing as {selectedCharacter.FirstName} {selectedCharacter.LastName} ({selectedCharacter.Department})");

            // trigger an event for selected character data (across scripts that require this framework to work)
            TriggerEvent("Framework:Notes.Notes.Client:SelectedCharacter", Json.Stringify(_currentCharacter));

            // return the result as a success
            result(new { success = true, message = "success" });
        }

        private void CreateCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            // Extract character information from the data dictionary with default values
            string firstName = data.GetVal<string>("firstName", null);
            string lastName = data.GetVal<string>("lastName", null);
            string gender = data.GetVal<string>("gender", null);
            string department = data.GetVal<string>("department", null);
            string dob = data.GetVal<string>("dob", null);
            string cash = data.GetVal("cash", "-1");
            string bank = data.GetVal("bank", "-1");

            // Validate character data
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(gender) || string.IsNullOrWhiteSpace(department) || string.IsNullOrWhiteSpace(dob) || cash == "-1" || bank == "-1")
            {
                Log.InfoOrError("An attempt was made trying to create a character but failed because there wasn't valid character input data.", "FRAMEWORK");

                // Display error modal to client and return failure result
                SendNUIMessage(Json.Stringify(new
                {
                    type = "FRAMEWORK_ERROR",
                    msg = "We ran into an unexpected error creating this character, try again."
                }));

                result(new { success = false, message = "not valid character data" });
                return;
            }

            // Create the character object
            Character createdCharacter = CreateCharacter(firstName, lastName, gender, department, dob, cash, bank, "0");

            // Trigger server event to handle server-sided character creation.
            TriggerServerEvent("Framework:Notes.Server:CreateCharacter", Json.Stringify(createdCharacter));

            // Display success modal to client and return success result
            SendNUIMessage(Json.Stringify(new
            {
                type = "FRAMEWORK_SUCCESS",
                msg = $"{firstName} {lastName} ({department}) has been created!"
            }));

            result(new { success = true, message = "success" });
        } 

        private void EditCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            string firstName = data.GetVal<string>("firstName", null);
            string lastName = data.GetVal<string>("lastName", null);
            string gender = data.GetVal<string>("gender", null);
            string department = data.GetVal<string>("department", null);
            string dob = data.GetVal<string>("dob", null);
            string charId = data.GetVal("charId", "-1");

            // Validate character data
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(gender) || string.IsNullOrWhiteSpace(department) || string.IsNullOrWhiteSpace(dob))
            {
                Log.InfoOrError($"Attempted to edit character: {charId} but couldn't because of invalid character data.", "FRAMEWORK");

                // Display error modal to client and return failure result
                SendNUIMessage(Json.Stringify(new
                {
                    type = "FRAMEWORK_ERROR",
                    msg = "We ran into an unexpected error editing this character, try again."
                }));
                result(new { success = false, message = "not valid character data" });
                return;
            }

            // Create the edited character object
            Character editedCharacter = new()
            {
                CharacterId = int.Parse(charId),
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                DoB = DateTime.Parse(dob),
                Department = department
            };

            // Trigger server event to handle server-sided character edit.
            TriggerServerEvent("Framework:Notes.Server:EditCharacter", Json.Stringify(editedCharacter));

            // Display success modal to client and return success result
            SendNUIMessage(Json.Stringify(new
            {
                type = "FRAMEWORK_SUCCESS",
                msg = "Character edited!"
            }));

            result(new { success = true, message = "success" });
        }

        private void DeleteCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            Log.InfoOrError("Successfully deleted character.", "FRAMEWORK");

            // Extract character information from the data dictionary with default values
            string characterId = data.GetVal<string>("characterId", null);

            // Trigger server event to handle server-sided character deletion.
            TriggerServerEvent("Framework:Notes.Server:DeleteCharacter", long.Parse(characterId));

            // Display success modal to client and return success result
            SendNUIMessage(Json.Stringify(new
            {
                type = "FRAMEWORK_SUCCESS",
                msg = "Character deleted!"
            }));

            result(new { success = true, message = "success" });
        }

        private void CloseFrameworkNui(IDictionary<string, object> data, CallbackDelegate result)
        {
            SetNuiFocus(false, false);
            SendNUIMessage(Json.Stringify(new { type = "CLOSE_FRAMEWORK_NUI" }));
            result(new { success = true, message = "success" });
        }

        private void QuitGame(IDictionary<string, object> data, CallbackDelegate result)
        {
            ForceSocialClubUpdate();
            result(new { success = true, message = "success" });
        }

        private void Disconnect(IDictionary<string, object> data, CallbackDelegate result)
        {
            TriggerServerEvent("Framework:DropPlayer");
            result(new { success = true, message = "success" });
        }
        #endregion

        #region Methods
        /// <summary>
        /// Opens the framework UI
        /// </summary>
        private void OpenFrameworkUi()
        {
            if (IsNuiFocused()) return;
            TriggerServerEvent("Framework:Notes.Server:GetCharacters");
        }

        /// <summary>
        /// Adds a department to a user in the framework if the don't have it.
        /// </summary>
        /// <param name="dept">The department to add.</param>
        private void AddDeptIfNotExists(string dept)
        {
            if (!_allowedDepts.Contains(dept))
            {
                _allowedDepts.Add(dept);
            }
        }

        private Character CreateCharacter(string firstName, string lastName, string gender, string department, string dob, string cash, string bank, string charId)
        {
            Character createdCharacter = new()
            {
                CharacterId = int.Parse(charId),
                DoB = DateTime.Parse(dob),
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                Department = department,
                Cash = int.Parse(cash),
                Bank = int.Parse(bank)
            };

            return createdCharacter;
        }

        /// <summary>
        /// Calculate age based on DateTime.
        /// </summary>
        /// <param name="dob">The Date to check against.</param>
        /// <returns>The age of a object.</returns>
        private int CalculateAge(DateTime dob)
        {
            int age = DateTime.Now.Year - dob.Year;
            if (dob.AddYears(age) > DateTime.Today)
            {
                age--;
            }

            return age;
        }

        private void AddDeptIfNotExistIfCivilian(Dictionary<string, string> rolesJson)
        {
            if (rolesJson.ContainsValue("CIV") || rolesJson.ContainsValue("Civ") || rolesJson.ContainsValue("Civilian"))
            {
                AddDeptIfNotExists("CIV");
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("playerSpawned")]
        private async void OnPlayerSpawned()
        {
            if (!_ran)
            {
                TriggerServerEvent("Framework:Notes.Server:GetCharacters");

                Exports["spawnmanager"].spawnPlayer(true);
                await Delay(3000);
                Exports["spawnmanager"].setAutoSpawn(false);

                _ran = true;
            }
        }

        [EventHandler("Framework:Notes.Notes.Client:GetCharacters")]
        private void OnGetCharacters(dynamic characters)
        {
            List<Character> characterList = JsonConvert.DeserializeObject<List<Character>>(JsonConvert.SerializeObject(characters));
            Log.InfoOrError($"Returned {characterList.Count} character(s).", "FRAMEWORK");

            SetNuiFocus(true, true);
            SendNUIMessage(Json.Stringify(new
            {
                type = "SHOW_FRAMEWORK_NUI",
                characters = characterList,
                departments = _allowedDepts,
                firstLoad = _currentCharacter is null,
                aop = $"{_currentAop}"
            }));
        }

        [EventHandler("Framework:Notes.Notes.Client:ChangeAop")]
        private void OnChangeAop(string newAop)
        {
            _currentAop = newAop;
            SendNUIMessage(Json.Stringify(new
            {
                type = "UPDATE_FRAMEWORK_AOP",
                aop = $"AOP: {newAop}"
            }));
        }

        [EventHandler("Framework:Notes.Notes.Client:GetDiscordRoles")]
        private void OnGetDiscordRoles(dynamic rolesJson)
        {
            Dictionary<string, string> roles = JsonConvert.DeserializeObject<Dictionary<string, string>>(rolesJson);

            if
            (
                roles.ContainsValue("Development") || roles.ContainsValue("Dev") || roles.ContainsValue("Developer") || roles.ContainsValue("Founder") ||
                roles.ContainsValue("Head Admin") || roles.ContainsValue("Owner") || roles.ContainsValue("Senior Admin") || roles.ContainsValue("Senior Administration") ||
                roles.ContainsValue("Head Administrator") || roles.ContainsValue("Head Administration")
            )
            {
                List<string> rolesList = new() { "LSPD", "SAHP", "BCSO", "LSFD", "CIV" };
                rolesList.ForEach(AddDeptIfNotExists);
            }

            if (roles.ContainsValue("LSPD")) AddDeptIfNotExists("LSPD");
            if (roles.ContainsValue("SAHP")) AddDeptIfNotExists("SAHP");
            if (roles.ContainsValue("BCSO")) AddDeptIfNotExists("BCSO");
            if (roles.ContainsValue("LSFD")) AddDeptIfNotExists("LSFD");
            if (roles.ContainsValue("Civilian Operations")) AddDeptIfNotExists("CIV");
        }
        #endregion
    }
}
