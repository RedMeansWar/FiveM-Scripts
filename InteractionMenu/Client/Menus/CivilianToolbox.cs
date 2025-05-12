using Common;
using Common.Client;
using MenuAPI;
using CitizenFX.Core;
using InteractionMenu.Client.SubMenus;
using static CitizenFX.Core.Native.API;

namespace InteractionMenu.Client.Menus
{
    public class CivilianToolbox : ClientCommonScript
    {
        public static Menu GetMenu()
        {
            Menu civMenu = new(Client.MenuTitle, "~b~Civilian Toolbox");

            MenuItem priorityMenuBtn = new("Priority Control") { Label = "Priority Control", Description = "open the priority control menu." };
            civMenu.AddMenuItem(priorityMenuBtn);
            MenuController.BindMenuItem(civMenu, PriorityMenu.GetMenu(), priorityMenuBtn);
            
            civMenu.AddMenuItem(new MenuListItem("Restrainment", ["Zip Tie", "Front Zip Tie"], 0));
            civMenu.AddMenuItem(new("Hands Up"));
            civMenu.AddMenuItem(new("Hands Up & Knees"));
            civMenu.AddMenuItem(new(Constants.MenuGoBack));
            civMenu.AddMenuItem(new(Constants.MenuClose));
            
            civMenu.OnItemSelect += CivMenu_OnItemSelect;
            civMenu.OnListItemSelect += CivMenu_OnListItemSelect;
            
            return civMenu;
        }

        private static void CivMenu_OnItemSelect(Menu menu, MenuItem menuItem, int itemIndex)
        {
            string item = menuItem.Text;
            switch (item)
            {
                case "Hands Up": ExecuteCommand("hu"); break;
                case "Hands Up & Knees": ExecuteCommand("huk"); break;
                case "~o~Back": menu.GoBack(); break;
                case "~r~Close": MenuController.CloseAllMenus(); break;
            }
        }
        
        private static void CivMenu_OnListItemSelect(Menu menu, MenuListItem listItem, int selectedIndex, int itemIndex)
        {
            string item = listItem.Text;
            switch (item)
            {
                case "Restrainment":
                    switch (selectedIndex)
                    {
                        case 0: ExecuteCommand("ziptie"); break;
                        case 1: ExecuteCommand("frontziptie"); break;
                    }
                    break;
            }
        }
    }
}