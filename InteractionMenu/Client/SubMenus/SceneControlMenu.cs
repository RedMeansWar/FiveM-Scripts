using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Client;
using MenuAPI;
using CitizenFX.Core;
//using SceneControl;
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
            Menu menu = new Menu(Client.MenuTitle, "~b~Scene Management");

            //List<string> propNames = SceneConstants.SceneProps.Select((prop, index) => $"{prop.DisplayName} ({index + 1})/{SceneConstants.SceneProps.Count})").ToList();
            
            //menu.AddMenuItem(new MenuListItem("Spawn Prop", propNames, 0));
            //menu.AddMenuItem(new MenuListItem("Speed Zone Radius", SceneConstants.SpeedZoneRadiuses.ConvertAll(x => $"{x}m"), 1));
            //menu.AddMenuItem(new MenuListItem("Speed Zone Speed", SceneConstants.SpeedZoneSpeeds.ConvertAll(x => $"{x}mph"), 6));
            
            menu.AddMenuItem(new("Create Speed Zone", "Create a speed zone with the radius and speed selected."));
            menu.AddMenuItem(new("Dellete Closest Speed Zone", "Delete closest speed zone."));
            menu.AddMenuItem(new(Constants.MenuGoBack));
            menu.AddMenuItem(new(Constants.MenuClose));
            
            return menu;
        }
    }
}