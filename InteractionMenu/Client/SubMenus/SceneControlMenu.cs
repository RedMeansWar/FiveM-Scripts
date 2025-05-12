using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Client;
using MenuAPI;
using CitizenFX.Core;
using SceneControl.Common;
using static CitizenFX.Core.Native.API;

namespace InteractionMenu.Client.SubMenus
{
    public class SceneControlMenu : ClientCommonScript
    {
        #region Variables
        internal static int _zoneRadius = 50;
        internal static float _zoneSpeed = 30;
        #endregion
        
        public static Menu GetMenu()
        {
            Menu sceneMenu = new Menu(Client.MenuTitle, "~b~Scene Management");

            List<string> propNames = SceneConstants.SceneProps.Select((prop, index) => $"{prop.DisplayName} ({index + 1})/{SceneConstants.SceneProps.Count})").ToList();
            
            sceneMenu.AddMenuItem(new MenuListItem("Spawn Prop", propNames, 0));
            sceneMenu.AddMenuItem(new MenuListItem("Speed Zone Radius", SceneConstants.SpeedZoneRadiuses.ConvertAll(x => $"{x}m"), 1));
            sceneMenu.AddMenuItem(new MenuListItem("Speed Zone Speed", SceneConstants.SpeedZoneSpeeds.ConvertAll(x => $"{x}mph"), 6));
            
            sceneMenu.AddMenuItem(new("Create Speed Zone", "Create a speed zone with the radius and speed selected."));
            sceneMenu.AddMenuItem(new("Delete Closest Speed Zone", "Delete the closest speed zone."));
            sceneMenu.AddMenuItem(new("Delete Closest Speed Zone", "Delete the closest prop to you."));
            
            sceneMenu.AddMenuItem(new(Constants.MenuGoBack));
            sceneMenu.AddMenuItem(new(Constants.MenuClose));
            
            sceneMenu.OnItemSelect += SceneMenu_OnItemSelect;
            sceneMenu.OnListItemSelect += SceneMenu_OnListItemSelect;
            sceneMenu.OnMenuOpen += SceneMenu_OnMenuOpen;
            sceneMenu.OnMenuClose += SceneMenu_OnMenuClose;
            sceneMenu.OnIndexChange += SceneMenu_OnIndexChange;
            sceneMenu.OnListIndexChange += SceneMenu_OnListIndexChange;
            
            return sceneMenu;
        }

        private static void SceneMenu_OnListIndexChange(Menu menu, MenuListItem listItem, int oldSelectionIndex, int newSelectionIndex, int itemIndex)
        {
            string item = listItem.Text;
            switch (item)
            {
                case "Speed Zone Radius": _zoneRadius = SceneConstants.SpeedZoneRadiuses[newSelectionIndex]; break;
                case "Speed Zone Speed": _zoneSpeed = SceneConstants.SpeedZoneSpeeds[newSelectionIndex]; break;
            }
        }

        private static void SceneMenu_OnIndexChange(Menu menu, MenuItem oldItem, dynamic newItem, int oldIndex, int newIndex)
        {
            if (menu.CurrentIndex == 0)
            {
                TriggerEvent("SceneControl:Client:VisualProp", newItem.ListIndex);
            }
            else
            {
                TriggerEvent("SceneControl:Client:ClearVisualizer");
            }
        }

        private static void SceneMenu_OnMenuClose(Menu menu) => TriggerEvent("SceneControl:Client:ClearVisualizer");

        private static void SceneMenu_OnMenuOpen(Menu menu)
        {
            if (menu.CurrentIndex == 0)
            {
                dynamic listItem = menu.GetCurrentMenuItem();
                TriggerEvent("SceneControl:Client:VisualProp", listItem.ListIndex);
            }
            else
            {
                TriggerEvent("SceneControl:Client:ClearVisualizer");
            }
        }

        private static void SceneMenu_OnListItemSelect(Menu menu, MenuListItem listItem, int selectedIndex, int itemIndex)
        {
            string item = listItem.Text;
            if (item == "Spawn Prop")
            {
                TriggerEvent("SceneControl:Client:VisualProp", itemIndex);
            }
        }

        private static void SceneMenu_OnItemSelect(Menu menu, MenuItem menuItem, int itemIndex)
        {
            string item = menuItem.Text;
            switch (item)
            {
                case "Create Speed Zone": TriggerServerEvent("SceneControl:Server:CreateSpeedZone", ClientPed.Position, _zoneRadius, _zoneSpeed); break;
                case "Delete Speed Zone": TriggerServerEvent("SceneControl:Server:DeleteSpeedZone", ClientPed.Position); break;
                case "Delete Closest Prop": TriggerEvent("SceneControl:Client:DeleteClosestProp"); break;
                case "~o~Back": menu.GoBack(); break;
                case "~r~Close": MenuController.CloseAllMenus(); break;
            }
        }
    }
}