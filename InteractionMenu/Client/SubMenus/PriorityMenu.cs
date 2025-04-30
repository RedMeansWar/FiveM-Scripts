using Common;
using Common.Client;
using MenuAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace InteractionMenu.Client.SubMenus
{
    public class PriorityMenu : ClientCommonScript
    {
        public static Menu GetMenu()
        {
            Menu priorityMenu = new(Client.MenuTitle, "~b~Priority Menu");
            
            
            
            return priorityMenu;
        }
    }
}