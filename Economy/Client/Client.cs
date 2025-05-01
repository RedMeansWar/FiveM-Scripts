using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Client;
using Common.Models;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Economy.Client
{
    public class Client : ClientCommonScript
    {
        #region Variables
        internal int // these are config variables
            _interactionKey = 51, // E key
            _dailySalary = 500, // $500
            _giveDuration = 30, // 30 minutes
            _emergencySalary = 100; // $100
            
        internal Prop _closestAtmProp;
        internal Character _currentCharacter;

        internal readonly List<Model> _atmModels = new()
        {
            new("prop_atm_01"), new("prop_atm_02"), new("prop_atm_03"), new("prop_fleeca_atm")
        };
        #endregion
        
        #region Constructors
        public Client()
        {
            ReadConfigFile();
            
            RegisterNUICallback("closeAtmNui", CloseAtmNui);
        }
        #endregion
        
        #region NUI Callbacks
        private async void CloseAtmNui(IDictionary<string, object> data, CallbackDelegate result)
        {
            SetNuiFocus(false, false);
            SendNUIMessage(Json.Stringify(new { type = "ECONOMY_CLOSE_ATM" }));

            int soundId = Audio.PlaySoundFrontend("PIN_BUTTON", "ATM_SOUNDS");
            while (!Audio.HasSoundFinished(soundId))
            {
                await Delay(0);
            }
            
            Audio.ReleaseSound(soundId);
            result(new { success = true, message = "success" });
        }
        #endregion
        
        #region Methods
        private void ReadConfigFile()
        {
            string data = LoadResourceFile(GetCurrentResourceName(), "config.ini");

            _interactionKey = Config.GetValue(data, "Economy", "InteractionKey", 51);
            _dailySalary = Config.GetValue(data, "Economy", "DailyWelfareAmount", 500);
            _emergencySalary = Config.GetValue(data, "Economy", "EmergencyServicesSalary", 100);
            _giveDuration = Config.GetValue(data, "Economy", "GiveEvery", 30);
            
            if (!Config.KeyExists(data, "Economy", "InteractionKey"))
            {
                Log.InfoOrError("ERROR: 'config.ini' not properly configured or is missing. Please check if the config is there or has any data inside of it.", "ECONOMY");
            }
        }

        private bool IsEmergencyService()
        {
            return _currentCharacter.Department == "BCSO" || _currentCharacter.Department == "SAHP" ||
                   _currentCharacter.Department == "LSPD" || _currentCharacter.Department == "LSFD";
        }
        #endregion
        
        #region Event Handlers
        [EventHandler("Framework:Client:SelectedCharacter")]
        private void OnSelectedCharacter(string json) => _currentCharacter = Json.Parse<Character>(json);
        #endregion
        
        #region Ticks
        [Tick]
        private async Task PrimaryTick()
        {
            Vector3 playerPos = ClientPed.Position;
            if (_closestAtmProp is not null && playerPos.DistanceTo(_closestAtmProp.Position) < 1.5f)
            {
                Hud.DisplayHelpText($"Press {Hud.GetControlContext(_interactionKey)} to access the ATM.");
                if (Controls.IsControlPressed((Control)_interactionKey))
                {
                    SetNuiFocus(true, true);
                    SendNUIMessage(Json.Stringify(new
                    {
                        type = "ECONOMY_DISPLAY_ATM",
                        status = true
                    }));
                }

                return;
            }

            _closestAtmProp = null;

            foreach (Prop prop in _atmModels.Select(atmModel => World.GetAllProps()
                         .Where(p => p.Model.Hash == atmModel.Hash)
                         .OrderBy(p => p.Position.DistanceTo(_closestAtmProp.Position))
                         .FirstOrDefault()).Where(prop => prop is not null && prop.Position.DistanceTo(playerPos) <= 1.5f))
            {
                _closestAtmProp = prop;
                break;
            }

            if (_closestAtmProp is null)
            {
                await Delay(1500);
            }
            else
            {
                await Delay(500);
            }
        }
        #endregion
    }
}