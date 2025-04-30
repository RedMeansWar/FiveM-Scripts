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
            
            vehicleMenu.AddMenuItem(new(Constants.MenuGoBack));
            vehicleMenu.AddMenuItem(new(Constants.MenuClose));
            
            MenuController.AddSubmenu(vehicleMenu, confDeleteMenu);
            MenuItem noCancelBtn = new("NO, CANCEL", "NO, do NOT delete my vehicle and go back!");
            MenuItem yesDelBtn = new("~r~YES, DELETE", "Yes, I'm sure. Delete my vehicle, please. I understand this can't be undone.") { LeftIcon = MenuItem.Icon.WARNING };
            confDeleteMenu.AddMenuItem(noCancelBtn);
            confDeleteMenu.AddMenuItem(yesDelBtn);

            confDeleteMenu.OnItemSelect += (menu, item, select) =>
            {
                if (item == noCancelBtn)
                {
                    confDeleteMenu.GoBack();
                }
                else
                {
                    ExecuteCommand("dv");
                }
                
                confDeleteMenu.GoBack();
            };
            
            MenuController.BindMenuItem(vehicleMenu, confDeleteMenu, confDeleteBtn);
            
            vehicleMenu.OnItemSelect += VehicleMenuOnOnItemSelect;
            vehicleMenu.OnListItemSelect += VehicleMenuOnOnListItemSelect;
            
            return vehicleMenu;
        }

        private static void VehicleMenuOnOnItemSelect(Menu menu, MenuItem menuItem, int itemIndex)
        {
            string item = menuItem.Text;
            switch (item)
            {
                case "Toggle Engine": ExecuteCommand("eng"); break;
                case "Open/Close Hood": ExecuteCommand("hood"); break;
                case "Open/Close Trunk": ExecuteCommand("trunk"); break;
                case "Shuffle Seats": ExecuteCommand("shuffle"); break;
                case "~o~Back": menu.GoBack(); break;
                case "~r~Close": MenuController.CloseAllMenus(); break;
            }
        }

        private static void VehicleMenuOnOnListItemSelect(Menu menu, MenuListItem listItem, int selectedIndex, int itemIndex)
        {
            string item = listItem.Text;
            switch (item)
            {
                case "Vehicle Locks":
                    switch (selectedIndex)
                    {
                        case 0: LockHandler(true); break;
                        case 1: LockHandler(); break;
                    }
                    break;
            }
        }

        private static void LockHandler(bool unlocked = false)
        {
            if (ClientCurrentVehicle is null)
            {
                Notify.Error("You must be in a vehicle.", true);
                return;
            }

            if (ClientCurrentVehicle.Driver != ClientPed)
            {
                Notify.Error("You must be the driver.", true);
                return;
            }
            
            ClientCurrentVehicle.LockStatus = unlocked ? VehicleLockStatus.Unlocked : VehicleLockStatus.Locked;
            Notify.Success($"{(unlocked ? "Unlocked" : "Locked")} vehicle.", true);
        }
    }
}