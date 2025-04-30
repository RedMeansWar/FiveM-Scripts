using Common;
using Common.Client;
using MenuAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace InteractionMenu.Client.SubMenus
{
    public class SceneControlMenu : ClientCommonScript
    {
        public static Menu GetMenu()
        {
            Menu menu = new Menu(Client.MenuTitle, "~b~Scene Management");
            

            return menu;
        }
    }
}