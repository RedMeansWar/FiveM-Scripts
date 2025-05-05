using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Client;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace SceneControl.Client
{
    public class Client : ClientCommonScript
    {
        #region Variables
        public static Prop VisualizedProp;
        public static SceneProp VisualizedSceneProp;
        internal readonly List<SpeedZone> _speedzones = new();
        #endregion

        #region Constructor
        public Client() => TriggerServerEvent("SceneControl:Server:GetAllSpeedZones");
        #endregion

        #region Event Handlers
        [EventHandler("onResourceStop")]
        private void OnResourceStop(string resourceName)
        {
            if (resourceName == GetCurrentResourceName() && Entity.Exists(VisualizedProp))
            {
                VisualizedProp.Delete();
                VisualizedProp = null;
                VisualizedSceneProp = null;
            }
        }

        [EventHandler("SceneControl:Client:Notify")]
        private void OnNotify(string message) => Hud.DisplayNotification(message, true);

        [EventHandler("SceneControl:Client:SpawnProp")]
        private void OnSpawnProp(int propIndex)
        {
            if (ClientPed.IsSittingInVehicle())
            {
                Notify.Error("You can't spawn props while in a vehicle.", true);
                return;
            }

            SceneProp selectedProp = SceneConstants.SceneProps[propIndex];
            float playerHeading = ClientPed.Heading;
            Vector3 playerPos = ClientPed.Position;

            Prop spawnedProp = new(CreateObject(new Model(selectedProp.ModelName), playerPos.X, playerPos.Y, playerPos.Z - 1f, false, false, false));

            spawnedProp.Rotate(playerHeading, selectedProp.HeadingOffset);
            spawnedProp.IsPersistent = true;
            spawnedProp.IsPositionFrozen = true;

            SetNetworkIdCanMigrate(spawnedProp.NetworkId, true);
            SetNetworkIdExistsOnAllMachines(spawnedProp.NetworkId, true);
            NetworkRegisterEntityAsNetworked(ObjToNet(spawnedProp.Handle));
            PlaceObjectOnGroundProperly(spawnedProp.Handle);

            TriggerServerEvent("SceneControl:Server:Log", $"{ClientPlayer.Name} (#{ClientPlayer.ServerId}) spawned prop [{selectedProp.DisplayName}] at \n{playerPos}");
        }

        [EventHandler("SceneControl:Client:DeleteClosestProp")]
        private async void OnDeleteClosestProp()
        {
            Vector3 playerPos = ClientPed.Position;

            foreach (SceneProp sp in SceneConstants.SceneProps)
            {
                uint modelHash = (uint)GetHashKey(sp.ModelName);

                if (DoesObjectOfTypeExistAtCoords(playerPos.X, playerPos.Y, playerPos.Z, 0.9f, modelHash, false))
                {
                    int closestProp = GetClosestObjectOfType(playerPos.X, playerPos.Y, playerPos.Z, 0.9f, modelHash, false, false, false);

                    if (closestProp > 0)
                    {
                        NetworkRequestControlOfEntity(closestProp);

                        int timeout = 5000;
                        while (timeout > 0 && !NetworkHasControlOfEntity(closestProp))
                        {
                            await Delay(100);
                            timeout -= 100;
                        }

                        DeleteObject(ref closestProp);
                        break;
                    }
                }
            }
        }

        [EventHandler("SceneControl:Client:VisualProp")]
        private void OnVisualizeProp(int propIndex)
        {
            if (ClientPed.IsSittingInVehicle()) return;
            VisualizedSceneProp = SceneConstants.SceneProps[propIndex];
        }

        [EventHandler("SceneControl:Client:ClearVisualizer")]
        private void OnClearvisualizer() => VisualizedSceneProp = null;

        [EventHandler("SceneControl:Client:UpdateSpeedZones")]
        private void OnUpdateSpeedZones(string json)
        {
            Log.InfoOrError($"Recieved an SpeedZone update on {json}");
            List<SpeedZone> updatedSpeedzones = Json.Parse<List<SpeedZone>>(json);

            foreach (SpeedZone speedzone in _speedzones)
            {
                if (!updatedSpeedzones.Any(uz => uz == speedzone))
                {
                    if (speedzone.Blip > 0 && DoesBlipExist(speedzone.Blip))
                    {
                        int blip = speedzone.Blip;
                        RemoveBlip(ref blip);
                    }

                    if (speedzone.Zone > 0)
                    {
                        RemoveSpeedZone(speedzone.Blip);
                    }
                }
            }

            foreach (SpeedZone uz in updatedSpeedzones)
            {
                Vector3 zonePos = uz.Position;
                uz.Blip = AddBlipForRadius(zonePos.X, zonePos.Y, zonePos.Z, uz.Radius);

                SetBlipColour(uz.Blip, 3);
                SetBlipAlpha(uz.Blip, 80);
                SetBlipSprite(uz.Blip, 9);

                uz.Zone = AddSpeedZoneForCoord(zonePos.X, zonePos.Y, zonePos.Z, uz.Radius, uz.Speed, false);
            }
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task PropVisualizationTick()
        {
            if (Entity.Exists(VisualizedProp))
            {
                VisualizedProp.Delete();
                VisualizedProp = null;
            }

            if (VisualizedSceneProp is null || ClientPed.IsSittingInVehicle())
            {
                VisualizedProp = null;
                VisualizedSceneProp = null;
                await Delay(1000);
                return;
            }

            Vector3 playerPos = ClientPed.GetPositionOffset(new(0, 1f, 0));
            float playerHeading = ClientPed.Heading;

            VisualizedProp = new Prop(CreateObject(new Model(VisualizedSceneProp.ModelName), playerPos.X, playerPos.Y, playerPos.Z - 1f, false, false, false));

            PlaceObjectOnGroundProperly(VisualizedProp.Handle);
            SetEntityCollision(VisualizedProp.Handle, false, false);
            SetEntityAlpha(VisualizedProp.Handle, 100, 0);

            await Task.FromResult(0);
        }
        #endregion
    }
}
