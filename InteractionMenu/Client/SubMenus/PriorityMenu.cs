using System.Collections.Generic;
using Common;
using Common.Client;
using MenuAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace InteractionMenu.Client.SubMenus
{
    public class PriorityMenu : ClientCommonScript
    {
        #region Variables
        internal static int _priorityTimer = 10;
        internal static List<int> times = new() { 10, 30 };
        #endregion
        
        public static Menu GetMenu()
        {
            Menu priorityMenu = new(Client.MenuTitle, "~b~Priority Menu");
            
            priorityMenu.AddMenuItem(new MenuListItem("Priority Type", new() { "Vehicle Priority", "Foot Priority" }, 0));
            priorityMenu.AddMenuItem(new("Start Priority"));
            priorityMenu.AddMenuItem(new("End Priority"));
            priorityMenu.AddMenuItem(new(Constants.MenuGoBack));
            priorityMenu.AddMenuItem(new(Constants.MenuClose));

            priorityMenu.OnItemSelect += PriorityMenuOnOnItemSelect;
            priorityMenu.OnListIndexChange += PriorityMenuOnOnListIndexChange;
            
            return priorityMenu;
        }

        private static void PriorityMenuOnOnItemSelect(Menu menu, MenuItem menuItem, int itemIndex)
        {
            string item = menuItem.Text;
            switch (item)
            {
                case "Start Priority": TriggerEvent("Priority:Client:StartPriority"); break;
                case "End Priority": TriggerEvent("Priority:Client:EndPriority", _priorityTimer); break;
                case "~o~Back": menu.GoBack(); break;
                case "~r~Close": MenuController.CloseAllMenus(); break;
            }
        }

        private static void PriorityMenuOnOnListIndexChange(Menu menu, MenuListItem listItem, int oldSelectionIndex, int newSelectionIndex, int itemIndex)
        {
            string item = listItem.Text;
            _priorityTimer = item switch
            {
                "Priority Type" => times[newSelectionIndex],
                _ => _priorityTimer
            };
        }
    }
}