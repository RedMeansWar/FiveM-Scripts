using Common.Client;
using MenuAPI;
using static CitizenFX.Core.Native.API;

namespace InteractionMenu.Client.Menus
{
    public class SettingsMenu : ClientCommonScript
    {
        public static Menu GetMenu()
        {
            Menu settingsMenu = new(Client.MenuTitle, "~b~Settings");
            
            settingsMenu.AddMenuItem(new MenuCheckboxItem("Right-Align Menu", "Put the menu to right instead of to the left."));
            settingsMenu.AddMenuItem(new("Save Settings", "Save your settings."));
            settingsMenu.AddMenuItem(new(Constants.MenuGoBack) { Label = Constants.MenuArrowBackward });
            settingsMenu.AddMenuItem(new(Constants.MenuClose));

            settingsMenu.OnItemSelect += SettingsMenuOnOnItemSelect;
            settingsMenu.OnCheckboxChange += SettingsMenuOnOnCheckboxChange;
            
            return settingsMenu;
        }

        #region Properties
        public static bool LeftAlignMenu
        {
            get => GetSettingsBool("LeftAlignMenu");
            set => SetSavedSettingsBool("LeftAlignMenu", value);
        }
        #endregion

        #region Methods
        private static void SettingsMenuOnOnItemSelect(Menu menu, MenuItem menuItem, int itemIndex)
        {
            string item = menuItem.Label;
            switch (item)
            {
                case "~o~Back": menu.GoBack(); break;
                case "~r~Close": MenuController.CloseAllMenus(); break;
            }
        }

        
        private static void SettingsMenuOnOnCheckboxChange(Menu menu, MenuCheckboxItem menuItem, int itemIndex, bool newCheckedState)
        {
            string item = menuItem.Label;
            if (item == "Right-Align Menu" && newCheckedState)
            {
                MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
            }
            else
            {
                MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Left;
            }
        }
        
        private static bool GetSettingsBool(string kvpString)
        {
            string savedVal = GetResourceKvpString($"rmenu{kvpString}");
            bool exist =  !string.IsNullOrEmpty(savedVal);

            if (!exist)
            {
                switch (kvpString)
                {
                    case "rmenuLeftAlignMenu":
                        SetSavedSettingsBool(kvpString, true);
                        return true;
                    default:
                        SetSavedSettingsBool(kvpString, false);
                        return false;
                }
            }

            return GetResourceKvpString($"rmenu{kvpString}") == "true";
        }

        private static void SetSavedSettingsBool(string kvpString, bool value) => SetResourceKvp("rmenu" + kvpString, value.ToString());
        #endregion
    }
}