using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Common.Client;
using Common;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace NearestPostal.Client
{
    public class Client : ClientCommonScript
    {
        #region Variables
        internal readonly List<Postal> _postals;
        internal string _routedPostal, _closestPostal;
        internal Blip _blip;
        internal bool _displayUi = true;
        #endregion

        #region Constructor
        public Client()
        {
            try
            {
                string json = LoadResourceFile(GetCurrentResourceName(), "postals.json") ?? "[]";
                if (!string.IsNullOrWhiteSpace(json))
                {
                    _postals = JsonConvert.DeserializeObject<List<Postal>>(json, new JsonSerializerSettings
                    {
                        Converters = new List<JsonConverter> { new Vector2Converter() }
                    });

                    for (int i = 0; i < _postals.Count; i++)
                    {
                        _postals[i] = new(_postals[i].Code, _postals[i].Location.X, _postals[i].Location.Y);
                    }

                    Log.InfoOrError($"Loaded {_postals.Count} postal(s)", "POSTALS");
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error was thrown.", "POSTALS", ex);
            }

            Tick += UpdateClosestPostalTick;
            Tick += PostalDisplayTick;

            Exports.Add("getClosestPostal", GetClosestPostal);
        }
        #endregion

        #region Commands
        [Command("p")]
        private void OnPCommand(string[] args) => OnPostalCommand(args);

        [Command("postal")]
        private void OnPostalCommand(string[] args)
        {
            if (args.Length < 1 && _blip is not null)
            {
                Hud.DisplayNotification($"~d~~h~Postals~h~~s~: Removed GPS route for postal {_routedPostal}.", true);
                _blip.Delete();
                _blip = null;
                _routedPostal = "000";
                return;
            }

            if (args.Length < 1)
            {
                Hud.DisplayNotification("~d~~h~Postals~h~~s~: You have no GPS route to remove.", true);
                return;
            }

            string input = args[0].ToUpper();

            Postal foundPostal = _postals.FirstOrDefault(postal => input == postal.Code);

            if (foundPostal is not null)
            {
                _blip?.Delete();

                _blip = World.CreateBlip(new(foundPostal.Location.X, foundPostal.Location.Y, 0f));
                _blip.ShowRoute = true;
                _blip.Color = (BlipColor)29;
                SetBlipRouteColour(_blip.Handle, 29);
                _blip.Name = $"Postal {foundPostal.Code}";
                _routedPostal = foundPostal.Code;

                Hud.DisplayNotification($"~d~~h~Postals~h~~s~: You've programmed your GPS to postal {foundPostal.Code}.", true);
            }
            else
            {
                Hud.DisplayNotification($"~d~~h~Postals~h~~s~: Postal {input} doesn't seem to exist.", true);
            }
        }
        #endregion

        #region Methods
        private async Task UpdateClosestPostalTick()
        {
            _closestPostal = GetClosestPostal(Game.PlayerPed.Position);
            await Delay(1000);
        }

        private async Task PostalDisplayTick()
        {
            if (_blip is not null && Vector2.DistanceSquared(new(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y), new(_blip.Position.X, _blip.Position.Y)) < 5000f)
            {
                Hud.DisplayNotification("~d~~h~Postals~h~~s~: You've arrived at the postal.", true);
                _blip.Delete();
                _blip = null;
            }

            if (!Hud.IsHudVisible)
            {
                await Delay(1000);
                return;
            }

            if (_displayUi)
            {
                Hud.DrawText2d(1.203f, -0.095f, 0.419f, $"Nearby Postal: ~c~{_closestPostal}", 255, 255, 255, 255);
            }
        }

        public string GetClosestPostal(Vector3 position)
        {
            Dictionary<string, float> results = new();

            foreach (Postal postal in _postals)
            {
                float dist = Vector3.DistanceSquared((Vector3)postal.Location, position);

                if (!results.ContainsKey(postal.Code))
                {
                    results.Add(postal.Code, dist);
                }
            }

            return results.OrderBy(pair => pair.Value).First().Key;
        }
        #endregion

        #region Event Handlers
        [EventHandler("NearestPostal:Client:DisplayUi")]
        private void OnDisplayUi(bool visable) => _displayUi = visable;
        #endregion
    }
}
