using System.Reflection.Emit;
using MenuAPI;
using Common;
using Common.Client;
using CitizenFX.Core;
using InteractionMenu.Client.Menus;
using static CitizenFX.Core.Native.API;

namespace InteractionMenu.Client
{
    public class Client : ClientCommonScript
    {
        #region Variables
        public static string MenuTitle = "RMenu";
        internal int _controlKey = 244;
        internal readonly Menu menu = new(MenuTitle, "~b~Main Menu");
        #endregion
        
        #region Constructors
        public Client()
        {
            ReadConfigFile();
            
            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)_controlKey;
            MenuController.AddMenu(menu);
            
            MenuItem policeBtn = new("Police Toolbox") { Label = Constants.MenuArrowForward, Description = "Open the Police Toolbox menu." };
            menu.AddMenuItem(policeBtn);
            MenuController.BindMenuItem(menu, PoliceToolbox.GetMenu(), policeBtn);
            
            MenuItem settingsBtn = new("Settings") { Label = Constants.MenuArrowForward, Description = "Open the settings menu." };
            menu.AddMenuItem(settingsBtn);
            MenuController.BindMenuItem(menu, SettingsMenu.GetMenu(), settingsBtn);
            
            
            menu.AddMenuItem(new(Constants.MenuClose));
            menu.OnItemSelect += Menu_OnItemSelect;
        }
        #endregion
        
        #region Methods
        private void ReadConfigFile()
        {
            string data = LoadResourceFile(GetCurrentResourceName(), "config.ini");

            _controlKey = Config.GetValue(data, "Menu", "OpenMenuKey", 244);
            MenuTitle = Config.GetValue(data, "Menu", "OpenMenuKey","RMenu");
            if (!Config.KeyExists(data, "Menu", "OpenMenuKey") || !Config.KeyExists(data, "Menu", "OpenMenuKey"))
            {
                Log.InfoOrError("ERROR: 'config.ini' not configured properly or is missing. Please check if the config is there or has any data inside of it.", "INTERACTION MENU");
            }
        }

        private void Menu_OnItemSelect(Menu menu, MenuItem item, int itemIndex)
        {
            string text = item.Text;
            if (text == "~r~Close")
            {
                MenuController.CloseAllMenus();
            }
        }
        #endregion
    }
}