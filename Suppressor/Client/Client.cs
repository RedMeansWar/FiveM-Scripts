using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Client;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Suppressor.Client
{
    public class Client : ClientCommonScript
    {
        #region Variables
        internal float _densityMultiplier = 1f;
        internal int _playerCount = 1;

        internal readonly List<string> _scenarioTypes = new()
        {
            "WORLD_VEHICLE_MILITARY_PLANES_SMALL", "WORLD_VEHICLE_MILITARY_PLANES_BIG", "WORLD_VEHICLE_AMBULANCE", "WORLD_VEHICLE_POLICE_NEXT_TO_CAR",
            "WORLD_VEHICLE_POLICE_CAR", "WORLD_VEHICLE_POLICE_BIKE", "WORLD_VEHICLE_DRIVE_PASSENGERS_LIMITED"
        };

        internal readonly List<string> _scenarioGroups = new()
        {
            "MP_POLICE", "ARMY_HELI", "POLICE_POUND1", "POLICE_POUND2", "POLICE_POUND3", "POLICE_POUND4", "POLICE_POUND5", "SANDY_PLANES", "ALAMO_PLANES",
            "GRAPESEED_PLANES", "LSA_PLANES", "NG_PLANES"
        };

        internal readonly List<string> _relationshipGroups = new()
        {
            "AMBIENT_GANG_HILLBILLY", "AMBIENT_GANG_BALLAS", "AMBIENT_GANG_MEXICAN", "AMBIENT_GANG_FAMILY", "AMBIENT_GANG_MARABUNTE",
            "AMBIENT_GANG_SALVA", "GANG_1", "GANG_2", "GANG_9", "GANG_10", "FIREMAN", "MEDIC", "COP"
        };

        internal readonly List<string> _suppressedModels = new()
        {
            "police", "police2", "police3", "police4", "policeb", "policeold1", "policeold2", "policet", "polmav", "pranger", "sheriff",
            "sheriff2", "stockade3", "buffalo3", "fbi", "fbi2", "firetruk", "lguard", "ambulance", "riot", "shamal", "luxor", "luxor2",
            "jet", "lazer", "titan", "barracks", "barracks2", "crusader", "rhino", "airtug", "ripley", "cargobob", "cargobob2", "cargobob3",
            "cargobob4", "cargobob5", "buzzard", "besra", "volatus"
        };

        internal readonly List<string> _suppressedPedModels = new()
        {
            "csb_cop", "s_f_y_cop_01", "s_m_m_snowcop_01", "s_m_m_prisguard_01", "s_m_y_cop_01", "s_m_y_hwaycop_01", "s_m_m_ciasec_01",
            "s_m_m_chemsec_01", "s_m_m_fibsec_01", "s_m_m_paramedic_01", "s_m_m_security_01", "s_m_y_blackops_01", "s_m_y_blackops_02",
            "s_m_y_blackops_03", "s_m_y_ranger_01", "s_m_y_sheriff_01", "s_m_y_swat_01", "s_f_y_ranger_01", "s_m_y_uscg_01", "s_f_y_sheriff_01",
            "a_c_mtlion"
        };
        #endregion

        #region Constructor
        public Client()
        {
            for (int i = 0; i < 16; i++) EnableDispatchService(i, false);
            
            uint player = Game.GenerateHashASCII("PLAYER");

            _scenarioTypes.ForEach(s => SetScenarioTypeEnabled(s, false));
            _scenarioGroups.ForEach(s => SetScenarioGroupEnabled(s, false));
            _relationshipGroups.ForEach(r => SetRelationshipBetweenGroups(1, Game.GenerateHashASCII(r), player));
            _suppressedModels.ForEach(s => SetVehicleModelIsSuppressed(Game.GenerateHashASCII(s), true));

            SetAudioFlag("PoliceScannerDisabled", true);
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task PrimaryTick()
        {
            foreach (Ped ped in World.GetAllPeds()) ped.BlockPermanentEvents = true;
            DistantCopCarSirens(false);

            await Delay(150);
        }

        [Tick]
        private async Task SecondaryTick()
        {
            foreach (Vehicle vehicle in World.GetAllVehicles().Where(v => _suppressedModels.Contains(v.DisplayName.ToLower()) && !v.PreviouslyOwnedByPlayer)) vehicle.Delete();
            foreach (Ped ped in World.GetAllPeds().Where(p => _suppressedPedModels.Contains(p.Model.ToString()) && !p.IsPlayer)) ped.Delete();
        }

        [Tick]
        private async Task TertiaryTick()
        {
            await Delay(55000);
            _densityMultiplier = _playerCount < 31 ? 1.0f : _playerCount is > 32 and < 50 ? 0.8f : _playerCount >= 50 ? 0.6f : 1.0f;

            SetVehicleDensityMultiplierThisFrame(_densityMultiplier);
            SetParkedVehicleDensityMultiplierThisFrame(_densityMultiplier);
            SetRandomVehicleDensityMultiplierThisFrame(_densityMultiplier);
            SetPedDensityMultiplierThisFrame(_densityMultiplier);
            SetScenarioPedDensityMultiplierThisFrame(_densityMultiplier, _densityMultiplier);
            DisablePlayerVehicleRewards(ClientPlayer.Handle);
        }
        #endregion
    }
}
