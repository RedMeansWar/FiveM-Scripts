using System.Collections.Generic;
using Common;
using Common.Client;
using MenuAPI;
using CitizenFX.Core;
using InteractionMenu.Client.SubMenus;
using static CitizenFX.Core.Native.API;

namespace InteractionMenu.Client.Menus
{
    public class PoliceToolbox : ClientCommonScript {
        public static Menu GetMenu()
        {
            Menu policeMenu = new(Client.MenuTitle, "~b~Police Toolbox");
            
            MenuItem sceneManageBtn = new("Scene Management", "Open the scene management menu.") { Label = Constants.MenuArrowForward };
            policeMenu.AddMenuItem(sceneManageBtn);
            MenuController.BindMenuItem(policeMenu, SceneControlMenu.GetMenu(), sceneManageBtn);
            
            policeMenu.AddMenuItem(new MenuListItem("Hands On", ["Grab", "Seat", "Unseat"], 0));
            policeMenu.AddMenuItem(new MenuListItem("Cuff", ["Cuff", "Front Cuff", "Zip tie", "Front Zip tie"], 0));

            policeMenu.AddMenuItem(new("Refill Taser Cartridges"));
            
            policeMenu.AddMenuItem(new MenuListItem("Duty Loadout", ["Standard", "Swat"], 0));
            policeMenu.AddMenuItem(new MenuListItem("Deploy Spike Strips", ["2", "3", "4"], 0));
            
            policeMenu.AddMenuItem(new("Remove Spike Strips"));
            policeMenu.AddMenuItem(new MenuListItem("Weapon Retention", ["Long Gun", "Shotgun", "Less Lethal Shotgun"], 0));
            policeMenu.AddMenuItem(new("Toggle LiDAR"));
            policeMenu.AddMenuItem(new("Toggle Radar Remote"));
            
            policeMenu.AddMenuItem(new(Constants.MenuGoBack));
            policeMenu.AddMenuItem(new(Constants.MenuClose));
            
            policeMenu.OnItemSelect += PoliceMenu_OnItemSelect;
            policeMenu.OnListItemSelect += PoliceMenu_OnListItemSelect;
            
            return policeMenu;
        }

        private static void PoliceMenu_OnItemSelect(Menu menu, MenuItem menuItem, int itemIndex)
        {
            string item = menuItem.Text;
            Vehicle closestVehicle = GetClosestVehicle(1f);
            
            switch (item)
            {
                case "Remove Spike Strips": TriggerEvent("SpikeStrips:Client:RemoveSpikes"); break;
                case "Toggle LiDAR":
                    if (ClientPed.IsInPoliceVehicle || closestVehicle?.ClassType == VehicleClass.Emergency)
                    {
                        Notify.Success(
                            ClientPed.Weapons.HasWeapon((WeaponHash)Game.GenerateHashASCII("WEAPON_PROLASER4"))
                                ? "You've put the LiDAR gun back on the passanger seat."
                                : "You've taken the LiDAR gun off the passanger seat.");

                        ExecuteCommand("lidarweapon");
                    }
                    break;
                
                case "Toggle Radar Remote":
                    if (ClientPed.IsInPoliceVehicle)
                    {
                        TriggerEvent("wk:openRemote");
                    }
                    else
                    {
                        Notify.Error("You must be in a police cruiser to use this.");
                    }
                    break;
                
                case "Refill Taser Cartridges":
                    if (ClientPed.IsInPoliceVehicle || closestVehicle?.ClassType == VehicleClass.Emergency)
                    {
                        ExecuteCommand("refill");
                    }
                    else
                    {
                        Notify.Error("You must be in or near a police cruiser to use this.");
                    }
                    break;
                case "~o~Back": menu.GoBack(); break;
                case "~r~Close": MenuController.CloseAllMenus(); break;
            }
        }

        private static void PoliceMenu_OnListItemSelect(Menu menu, MenuListItem listItem, int selectedIndex, int itemIndex)
        {
            string item = listItem.Text;
            switch (item)
            {
                case "Hands On":
                    switch (selectedIndex)
                    {
                        case 0: ExecuteCommand("grab"); break;
                        case 1: ExecuteCommand("seat"); break;
                        case 2: ExecuteCommand("unseat"); break;
                    }
                    break;
                
                case "Cuff":
                    switch (selectedIndex)
                    {
                        case 0: ExecuteCommand("cuff"); break;
                        case 1: ExecuteCommand("frontcuff"); break;
                        case 2: ExecuteCommand("ziptie"); break;
                        case 3: ExecuteCommand("frontziptie"); break;
                    }
                    break;
                
                case "Deploy Spike Strips":
                    switch (selectedIndex)
                    {
                        case 0: TriggerEvent("SpikeStrips:Client:SetSpikes", 2); break;
                        case 1: TriggerEvent("SpikeStrips:Client:SetSpikes", 3); break;
                        case 2: TriggerEvent("SpikeStrips:Client:SetSpikes", 4); break;
                    }
                    break;
                
                case "Weapon Retention":
                    switch (selectedIndex)
                    {
                        case 0: WeaponSystem(WeaponHash.CarbineRifle); break;
                        case 1: WeaponSystem(WeaponHash.PumpShotgun); break;
                    }
                    break;
            }
        }

        private static void WeaponSystem(WeaponHash hash)
        {
            Vehicle closestVehicle = GetClosestVehicle(1f);
            string gun = hash switch
            {
                WeaponHash.CarbineRifle => "long gun",
                WeaponHash.PumpShotgun => "12 gauge shotgun",
                _ => "gun"
            };

            if (ClientPed.IsInPoliceVehicle && ClientPed.CanDoAction() || closestVehicle?.ClassType == VehicleClass.Emergency)
            {
                Weapon playerWeapon = ClientPed.Weapons[hash];
                if (playerWeapon is not null)
                {
                    Notify.Success($"You've unequipped and locked your {gun}.", true);    
                }
                else
                {
                    playerWeapon = ClientPed.Weapons.Give(hash, 0, true, true);
                    playerWeapon.Ammo = playerWeapon.MaxAmmoInClip * 3;
                    playerWeapon.Components[WeaponComponentHash.AtArFlsh].Active = true;

                    if (hash == WeaponHash.CarbineRifle)
                    {
                        List<WeaponComponentHash> weaponComponents = [WeaponComponentHash.AtPiFlsh, WeaponComponentHash.AtArAfGrip, WeaponComponentHash.AtScopeMedium];
                        weaponComponents.ForEach(c => AddWeaponComponent(playerWeapon, c));
                    }

                    Notify.Success($"You've unlocked and equipped your {gun}", true);
                }
            }
            else
            {
                Notify.Error("You must be in or near a police cruiser to use this.");
            }
        }
    }
}