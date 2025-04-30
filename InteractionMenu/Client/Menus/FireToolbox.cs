using Common;
using Common.Client;
using MenuAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace InteractionMenu.Client.Menus
{
    public class FireToolbox : ClientCommonScript
    {
        public static Menu GetMenu()
        {
            Menu fireMenu = new(Client.MenuTitle, "~b~Fire Toolbox");
            
            
            
            return fireMenu;
        }
    }
}