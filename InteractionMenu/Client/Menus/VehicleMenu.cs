using Common;
using Common.Client;
using MenuAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace InteractionMenu.Client.Menus
{
    public class VehicleMenu : ClientCommonScript
    {
        public static Menu GetMenu()
        {
            Menu vehicleMenu = new(Client.MenuTitle, "~b~Vehicle Menu");
            Menu confDeleteMenu = new("Confirm Deletion", "Delete vehicle, are you sure?");
            
            vehicleMenu.AddMenuItem(new("Toggle Engine"));
            vehicleMenu.AddMenuItem(new("Open/Close Hood"));
            vehicleMenu.AddMenuItem(new("Open/Close Trunk"));
            vehicleMenu.AddMenuItem(new("Shuffle Seats"));
            
            vehicleMenu.AddMenuItem(new MenuListItem("Vehicle Locks", ["Unlock Doors", "Lock Doors"], 0));
            
            MenuItem confDeleteBtn = new("~r~Delete Vehicle") { LeftIcon = MenuItem.Icon.WARNING, Label = Constants.MenuArrowForward };
            vehicleMenu.AddMenuItem(confDeleteBtn);
            
            
            return vehicleMenu;
        }
    }
}