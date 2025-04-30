using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using static CitizenFX.Core.Native.API;

namespace Common.Client
{
    public class Weapons
    {
        #region Variables
        internal static readonly Dictionary<uint, string> _weaponDisplayNames = new();
        #endregion

        #region Methods
        public static void LoadWeaponData()
        {
            try
            {
                string json = LoadResourceFile(GetCurrentResourceName(), "weapon_data.json");
                if (string.IsNullOrWhiteSpace(json))
                {
                    
                    return;
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Gets the display name of a weapon from its hash.
        /// </summary>
        /// <param name="weaponHash">The hash of the weapon.</param>
        /// <returns>The display name of the weapon, or "Unknown Weapon" if not found.</returns>
        public static string GetWeaponDisplayNameFromHash(uint weaponHash)
        {
            if (_weaponDisplayNames.TryGetValue(weaponHash, out string displayName))
            {
                return displayName;
            }

            return "Unknown Weapon";
        }
        #endregion
    }

    internal class WeaponData
    {
        public uint WeaponHash { get; set; }
        public string WeaponName { get; set; }
    }
}