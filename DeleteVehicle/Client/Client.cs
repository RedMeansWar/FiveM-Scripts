using System.Threading.Tasks;
using Common.Client;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace DeleteVehicle.Client
{
    public class Client : ClientCommonScript
    {
        #region Commands
        [Command("deletevehicle")]
        private async void DeleteVehicleCommand()
        {
            if (ClientCurrentVehicle is null)
            {
                Vehicle closestVehicle = GetClosestVehicle(3f);
                if (closestVehicle is null)
                {
                    Notify.Error("You must be in or near a vehicle", true);
                    return;
                }

                if (closestVehicle.Driver.Exists() && closestVehicle.Driver.IsPlayer)
                {
                    Notify.Error("That vehicle still has a driver.", true);
                    return;
                }

                if (NetworkGetEntityOwner(closestVehicle.Handle) == ClientPlayer.Handle)
                {
                    bool deleted = await DeleteVehicleAsync(closestVehicle);
                    ShowDeletedNotification(deleted);
                }
                else
                {
                    TriggerServerEvent("DeleteVehicle:Notes.Server:DeleteVehicle", closestVehicle.NetworkId);
                    int tempTimer = Game.GameTime;

                    while (closestVehicle.Exists())
                    {
                        if (Game.GameTime - tempTimer > 5000)
                        {
                            Notify.Error("Failed to delete vehicle, try again.", true);
                            return;
                        }

                        await Delay(0);
                    }

                    Notify.Success("Vehicle deleted.", true);
                }
            }
            else if (ClientPed.SeatIndex != VehicleSeat.Driver)
            {
                Notify.Error("You must be the driver.", true);
                return;
            }
            else
            {
                bool deleted = await DeleteVehicleAsync(ClientCurrentVehicle);
                ShowDeletedNotification(deleted);
            }
        }

        [Command("dv")]
        private void DvCommand() => DeleteVehicleCommand();

        [Command("delveh")]
        private void DelVehCommand() => DeleteVehicleCommand();
        #endregion

        #region Methods
        private async Task<bool> DeleteVehicleAsync(Vehicle vehicle)
        {
            vehicle.IsPersistent = true;
            vehicle.Delete();

            int tempTimer = Game.GameTime;
            while (vehicle.Exists())
            {
                if (Game.GameTime - tempTimer > 5000)
                {
                    return false;
                }

                await Delay(0);
            }

            return true;
        }

        private void ShowDeletedNotification(bool value)
        {
            if (!value) Notify.Error("Failed to delete vehicle, try again.", true);
            else Notify.Success("Vehicle deleted.", true);
        }
        #endregion
    }
}
