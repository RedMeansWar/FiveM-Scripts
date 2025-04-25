using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Client.Models;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Common.Client
{
    public abstract class ClientCommonScript : BaseScript
    {
        #region Player Variables
        /// <summary>
        /// Gets the local client's player.
        /// </summary>
        public static Player ClientPlayer => Game.Player;

        /// <summary>
        /// Gets the local client's ped.
        /// </summary>
        public static Ped ClientPed => Game.PlayerPed;

        /// <summary>
        /// Gets the local client's Character.
        /// </summary>
        public static Ped ClientPlayerCharacter => Game.Player.Character;
        #endregion

        #region Vehicle References
        /// <summary>
        /// Gets the local client's current vehicle.
        /// </summary>
        public static Vehicle ClientCurrentVehicle => ClientPed?.CurrentVehicle;

        /// <summary>
        /// Gets the local client's last vehicle.
        /// </summary>
        public static Vehicle ClientLastVehicle => ClientPed?.LastVehicle;
        #endregion

        #region Audio References
        /// <summary>
        /// Plays a sound at a specified position asynchronously and ensures it finishes playing before releasing the sound.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> position where the sound will be played.</param>
        /// <param name="soundName">The name of the sound to be played.</param>
        /// <param name="soundSet">Optional. The sound set to use. If null, the default sound set will be used.</param>
        public static async void PlayManagedSound(Vector3 position, string soundName, string soundSet = null)
        {
            // Play the sound at the specified position and get its unique sound ID.
            int soundId = Audio.PlaySoundAt(position, soundName, soundSet);

            // Wait until the sound finishes playing, checking every 200 milliseconds.
            while (!Audio.HasSoundFinished(soundId))
            {
                await Delay(200); // Delay to avoid blocking the main thread.
            }

            // Release the sound once it has finished playing.
            Audio.ReleaseSound(soundId);
        }

        /// <summary>
        /// Plays a sound from a specified entity asynchronously and ensures it finishes playing before releasing the sound.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> from which the sound will be played.</param>
        /// <param name="soundName">The name of the sound to be played.</param>
        /// <param name="soundSet">Optional. The sound set to use. If null, the default sound set will be used.</param>
        public static async void PlayManagedSound(Entity entity, string soundName, string soundSet = null)
        {
            // Play the sound from the specified entity and get its unique sound ID.
            int soundId = Audio.PlaySoundFromEntity(entity, soundName, soundSet);

            // Wait until the sound finishes playing, checking every 200 milliseconds.
            while (!Audio.HasSoundFinished(soundId))
            {
                await Delay(200); // Delay to allow other operations without blocking the main thread.
            }

            // Release the sound once it has finished playing.
            Audio.ReleaseSound(soundId);
        }

        /// <summary>
        /// Plays a frontend sound asynchronously and ensures it finishes playing before releasing the sound.
        /// </summary>
        /// <param name="soundName">The name of the sound to be played.</param>
        /// <param name="soundSet">Optional. The sound set to use. If null, the default sound set will be used.</param>
        public static async void PlayManagedSoundFrontend(string soundName, string soundSet = null)
        {
            // Play the frontend sound and get its unique sound ID.
            int soundId = Audio.PlaySoundFrontend(soundName, soundSet);

            // Wait until the sound finishes playing, checking every 200 milliseconds.
            while (!Audio.HasSoundFinished(soundId))
            {
                await Delay(200); // Delay to prevent blocking the main thread.
            }

            // Release the sound once it has finished playing.
            Audio.ReleaseSound(soundId);
        }
        #endregion

        #region NUI Handlers
        /// <summary>
        /// Registers a NUI (Native User Interface) callback for a specified event in a FiveM server.
        /// </summary>
        /// <param name="event">
        /// The name of the event to register for NUI callback handling.
        /// </param>
        /// <param name="action">
        /// The dynamic action to execute when the specified event is triggered.
        /// </param>
        /// <remarks>
        /// This method uses <see cref="RegisterNuiCallbackType"/> to register the event type 
        /// and adds the event handler to the global <c>EventHandlers</c> list.
        /// The callback always responds with an "ok" status after the action is executed.
        /// </remarks>
        /// <example>
        /// <code>
        /// RegisterNUICallback("exampleEvent", () =>
        /// {
        ///     Debug.WriteLine("NUI callback triggered!");
        /// }); 
        /// </code>
        /// </example>
        public void RegisterNUICallback(string @event, dynamic action)
        {
            // Register the NUI callback type for the specified event
            RegisterNuiCallbackType(@event);

            // Add a new event handler for the "__cfx_nui:<event>" event
            EventHandlers.Add($"__cfx_nui:{@event}", new Action<IDictionary<string, object>, CallbackDelegate>((data, callback) =>
            {
                // Execute the provided action
                action();

                // Send an "ok" response to the NUI callback
                callback("ok");
            }));
        }

        /// <summary>
        /// Registers a callback for NUI events that takes a single parameter of a specified type.
        /// </summary>
        /// <typeparam name="T">The type of the data passed to the callback.</typeparam>
        /// <param name="@event">The event name to register the callback for.</param>
        /// <param name="action">The action to be executed when the event is triggered, with the data of type T.</param>
        public void RegisterNUICallback<T>(string @event, Action<T> action) where T : class
        {
            RegisterNuiCallbackType(@event); // Registers the event type for NUI callbacks.

            // Adds the event handler to handle the NUI event.
            EventHandlers.Add($"__cfx_nui:{@event}", new Action<IDictionary<string, object>, CallbackDelegate>((data, callback) =>
            {
                // Deserialize data based on whether it's a simple type or not.
                T typedData = data.Count == 1 ? TypeCache<T>.IsSimpleType ? (T)data.Values.ElementAt(0) : Json.Parse<T>(Json.Stringify(data.Values.ElementAt(0))) : Json.Parse<T>(Json.Stringify(data));

                // Invoke the action with the deserialized data.
                action(typedData);

                // Send an "ok" response back to the callback.
                callback("ok");
            }));
        }

        /// <summary>
        /// Registers a callback for NUI events that return a value of type TReturn.
        /// </summary>
        /// <typeparam name="TReturn">The type of the result returned by the callback.</typeparam>
        /// <param name="@event">The event name to register the callback for.</param>
        /// <param name="action">The function to be executed when the event is triggered, returning a value of type TReturn.</param>
        public void RegisterNUICallback<TReturn>(string @event, Func<TReturn> action)
        {
            RegisterNuiCallbackType(@event); // Registers the event type for NUI callbacks.

            // Adds the event handler to handle the NUI event.
            EventHandlers.Add($"__cfx_nui:{@event}", new Action<IDictionary<string, object>, CallbackDelegate>((data, callback) =>
            {
                // Invoke the function and get the result.
                TReturn result = action();

                // Serialize the result and send it back to the callback.
                callback(Json.Stringify(result));
            }));
        }

        /// <summary>
        /// Registers a callback for NUI events that takes a parameter of type T and returns a value of type TReturn.
        /// </summary>
        /// <typeparam name="T">The type of the data passed to the callback.</typeparam>
        /// <typeparam name="TReturn">The type of the result returned by the callback.</typeparam>
        /// <param name="@event">The event name to register the callback for.</param>
        /// <param name="action">The function to be executed when the event is triggered, with the data of type T and returning a value of type TReturn.</param>
        public void RegisterNUICallback<T, TReturn>(string @event, Func<T, TReturn> action) where T : class
        {
            RegisterNuiCallbackType(@event); // Registers the event type for NUI callbacks.

            // Adds the event handler to handle the NUI event.
            EventHandlers.Add($"__cfx_nui:{@event}", new Action<IDictionary<string, object>, CallbackDelegate>((data, callback) =>
            {
                // Deserialize data based on whether it's a simple type or not.
                T typedData = data.Count == 1 ? TypeCache<T>.IsSimpleType ? (T)data.Values.ElementAt(0) : Json.Parse<T>(Json.Stringify(data.Values.ElementAt(0))) : Json.Parse<T>(Json.Stringify(data));

                // Invoke the function with the deserialized data and get the result.
                TReturn result = action(typedData);

                // Serialize the result and send it back to the callback.
                callback(Json.Stringify(result));
            }));
        }

        /// <summary>
        /// Registers a callback for an NUI event, where the callback takes a dictionary of string keys and object values, 
        /// and a callback delegate to be invoked after processing the event.
        /// </summary>
        /// <param name="callbackType">The type of callback to register for NUI events.</param>
        /// <param name="callback">The action to be executed when the event is triggered, taking the event data as a dictionary and a callback delegate.</param>
        public void RegisterNUICallback(string callbackType, Action<IDictionary<string, object>, CallbackDelegate> callback) => RegisterNuiCallback(callbackType, callback);

        /// <summary>
        /// Sends a message to the NUI  interface. This can be used to notify or pass data to the UI from the server.
        /// </summary>
        /// <param name="message">The message to be sent to the NUI interface.</param>
        public static void SendNUIMessage(string message) => SendNuiMessage(message);
        #endregion

        #region Shorten Extensions
        /// <summary>
        /// Shortened version of GetClosestPlayerToPlayer without Player to access it.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Player GetClosestPlayer(float radius = 2f) => ClientPlayer.GetClosestPlayerToClient(radius);

        /// <summary>
        /// Shortened version of GetClosestPlayerToPlayer without Player to access it.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Player GetClosestPlayerToPed(float radius = 2f) => ClientPlayer.GetClosestPlayerToClient(radius);

        /// <summary>
        /// Shortened version of GetClosestVehicle without ClientPed to access it.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Vehicle GetClosestVehicle(float radius = 2f) => ClientPed.GetClosestVehicleToClient(radius);

        /// <summary>
        /// Shortened version of GetClosestVehicle without ClientPed to access it.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Vehicle GetClosestVehicleToPlayer(float radius = 2f) => ClientPed.GetClosestVehicleToClient(radius);

        /// <summary>
        /// Shortened version of GetClosestVehicle without ClientPed to access it.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Vehicle GetClosestVehicleToPed(float radius = 2f) => ClientPed.GetClosestVehicleToClient(radius);
        #endregion

        #region Model Checker
        /// <summary>
        /// Checks if a model exist.
        /// </summary>
        /// <param name="modelHash"></param>
        /// <returns></returns>
        public static bool DoesModelExist(uint modelHash) => IsModelInCdimage(modelHash);

        /// <summary>
        /// Checks if a model exist.
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public static bool DoesModelExist(string modelName) => DoesModelExist((uint)GetHashKey(modelName));

        /// <summary>
        /// Checks if a model exist.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool DoesModelExist(Model model) => DoesModelExist((uint)model.Hash);
        #endregion

        #region Loading Assets
        /// <summary>
        /// Asynchronously loads the specified ambient audio bank into the game.
        /// </summary>
        /// <param name="audioBank">The name of the ambient audio bank to be loaded.</param>
        public static async Task LoadAmbientAudioBank(string audioBank)
        {
            // Continuously attempt to request the ambient audio bank to be loaded.
            // The second parameter (false) typically indicates whether to wait for the bank to load immediately or not.
            while (!RequestAmbientAudioBank(audioBank, false))
            {
                // Wait for the next frame to avoid blocking the game loop while waiting for the audio bank to load.
                await Delay(0);
            }
        }

        /// <summary>
        /// Asynchronously loads the weapon asset identified by the given weapon hash.
        /// </summary>
        /// <param name="weaponHash">The hash value representing the weapon whose asset is to be loaded.</param>
        public static async void LoadWeaponAsset(uint weaponHash)
        {
            // Request the weapon asset to be loaded with the specified hash. The parameters may include
            // the loading type (31) and additional flags (0) for the request.
            RequestWeaponAsset(weaponHash, 31, 0);

            // Continuously check if the weapon asset has finished loading.
            // The loop will yield control and wait for the next frame until the asset is loaded.
            while (!HasWeaponAssetLoaded(weaponHash))
            {
                // Wait for the next frame to avoid blocking the game loop while waiting for the asset to load.
                await Delay(0);
            }
        }

        /// <summary>
        /// Asynchronously loads the weapon asset identified by the given weapon hash.
        /// </summary>
        /// <param name="weaponHash">The hash value representing the weapon whose asset is to be loaded.</param>
        public static void LoadWeaponAsset(WeaponHash weaponHash) => LoadWeaponAsset((uint)weaponHash);
        #endregion

        #region Weapons
        /// <summary>
        /// Retrieves the maximum ammunition count for a specified weapon.
        /// </summary>
        /// <param name="weaponHash">The weapon hash representing the type of weapon.</param>
        /// <returns>
        /// The maximum ammunition count for the specified weapon if retrieval is successful; otherwise, returns -1.
        /// </returns>
        public static int GetWeaponMaxCount(WeaponHash weaponHash)
        {
            // Initialize a variable to hold the maximum ammo count for the weapon.
            int maxAmmo = 0;

            // Attempt to retrieve the max ammo for the weapon.
            // The success variable indicates if the max ammo retrieval was successful.
            bool success = GetMaxAmmo(ClientPed.Handle, (uint)weaponHash, ref maxAmmo);

            // If successful, return the max ammo count; otherwise, return -1.
            return success ? maxAmmo : -1;
        }

        /// <summary>
        /// Gives the player a list of weapons, sets their ammo, and optionally equips and preloads them.
        /// </summary>
        /// <param name="hashes">A list of <see cref="WeaponHash"/> values representing the weapons to be given.</param>
        /// <param name="ammo">The total ammo to give for each weapon. Defaults to 240.</param>
        /// <param name="equip">Indicates whether the weapon should be equipped immediately. Defaults to false.</param>
        /// <param name="loaded">Indicates whether the weapon should be preloaded with ammo. Defaults to false.</param>
        public static void GiveWeapons(List<WeaponHash> hashes, int ammo = 240, bool equip = false, bool loaded = false)
        {
            // Remove all current weapons from the player to start fresh.
            ClientPed.Weapons.RemoveAll();

            // Iterate through each weapon hash in the provided list.
            foreach (WeaponHash hash in hashes)
            {
                // Give the player the weapon with the specified ammo, equip, and loaded settings.
                ClientPed.Weapons.Give(hash, ammo, equip, loaded);

                // Retrieve the weapon instance to customize its ammo settings.
                Weapon weapon = ClientPed.Weapons[hash];

                // Set the weapon's ammo to its maximum capacity.
                weapon.Ammo = weapon.MaxAmmo;

                // Set the weapon's clip to its maximum capacity.
                weapon.AmmoInClip = weapon.MaxAmmoInClip;
            }
        }

        /// <summary>
        /// Adds or removes a specific component to/from a weapon by using the <see cref="WeaponHash"/> and <see cref="WeaponComponentHash"/> enums.
        /// </summary>
        /// <param name="weaponHash">The weapon's hash value as a <see cref="WeaponHash"/> enum.</param>
        /// <param name="componentHash">The weapon component's hash value as a <see cref="WeaponComponentHash"/> enum.</param>
        /// <param name="active">Indicates whether to activate (true) or deactivate (false) the component. Defaults to true.</param>
        public static void AddWeaponComponent(WeaponHash weaponHash, WeaponComponentHash componentHash, bool active = true)
            => ClientPed.Weapons[weaponHash].Components[componentHash].Active = active;

        /// <summary>
        /// Adds or removes a specific component to/from a weapon by using a <see cref="WeaponHash"/> enum and a component hash as a uint.
        /// </summary>
        /// <param name="weaponHash">The weapon's hash value as a <see cref="WeaponHash"/> enum.</param>
        /// <param name="componentHash">The weapon component's hash value as a uint.</param>
        /// <param name="active">Indicates whether to activate (true) or deactivate (false) the component. Defaults to true.</param>
        public static void AddWeaponComponent(WeaponHash weaponHash, uint componentHash, bool active = true)
            => ClientPed.Weapons[weaponHash].Components[(WeaponComponentHash)componentHash].Active = active;

        /// <summary>
        /// Adds or removes a specific component to/from a weapon by using a weapon hash as a uint and the <see cref="WeaponComponentHash"/> enum.
        /// </summary>
        /// <param name="weaponHash">The weapon's hash value as a uint.</param>
        /// <param name="componentHash">The weapon component's hash value as a <see cref="WeaponComponentHash"/> enum.</param>
        /// <param name="active">Indicates whether to activate (true) or deactivate (false) the component. Defaults to true.</param>
        public static void AddWeaponComponent(uint weaponHash, WeaponComponentHash componentHash, bool active = true)
            => ClientPed.Weapons[(WeaponHash)weaponHash].Components[componentHash].Active = active;

        /// <summary>
        /// Adds or removes a specific component to/from a weapon by using a <see cref="WeaponHash"/> enum and a component hash as a int.
        /// </summary>
        /// <param name="weaponHash">The weapon's hash value as a <see cref="WeaponHash"/> enum.</param>
        /// <param name="componentHash">The weapon component's hash value as a int.</param>
        /// <param name="active">Indicates whether to activate (true) or deactivate (false) the component. Defaults to true.</param>
        public static void AddWeaponComponent(WeaponHash weaponHash, int componentHash, bool active = true) => ClientPed.Weapons[weaponHash].Components[(WeaponComponentHash)componentHash].Active = active;

        /// <summary>
        /// Adds or removes a specific component to/from a weapon by using a weapon hash as a int and the <see cref="WeaponComponentHash"/> enum.
        /// </summary>
        /// <param name="weaponHash">The weapon's hash value as a int.</param>
        /// <param name="componentHash">The weapon component's hash value as a <see cref="WeaponComponentHash"/> enum.</param>
        /// <param name="active">Indicates whether to activate (true) or deactivate (false) the component. Defaults to true.</param>
        public static void AddWeaponComponent(int weaponHash, WeaponComponentHash componentHash, bool active = true)
            => ClientPed.Weapons[(WeaponHash)weaponHash].Components[componentHash].Active = active;

        /// <summary>
        /// Adds or removes a specific component to/from a weapon by using both weapon and component hashes as ints.
        /// </summary>
        /// <param name="weaponHash">The weapon's hash value as a int.</param>
        /// <param name="componentHash">The weapon component's hash value as a int.</param>
        /// <param name="active">Indicates whether to activate (true) or deactivate (false) the component. Defaults to true.</param>
        public static void AddWeaponComponent(int weaponHash, int componentHash, bool active = true)
            => ClientPed.Weapons[(WeaponHash)weaponHash].Components[(WeaponComponentHash)componentHash].Active = active;
        #endregion

        #region Tires
        /// <summary>
        /// Gets the closest tire to the player or entity within a specified radius.
        /// </summary>
        /// <param name="vehicle">The vehicle to search for the closest tire.</param>
        /// <param name="radius">The radius within which to search for the closest tire. Defaults to 1.5f.</param>
        /// <returns>The closest tire to the player or entity within the specified radius.</returns>
        public static Wheel GetClosestWheel(Vehicle vehicle, float radius = 1.5f) => vehicle.GetClosestWheel(radius);  // Delegates to the vehicle's method to find the closest tire within the given radius.

        /// <summary>
        /// Calculates the heading (angle) from the player to a specific tire using the tire's position.
        /// </summary>
        /// <param name="tire">The tire to calculate the heading for.</param>
        /// <returns>The heading (angle) in radians from the player to the tire.</returns>
        public static float GetClosestWheelHeading(Wheel tire)
            => GetHeadingFromVector_2d(tire.BonePosition.X - ClientPed.Position.X, tire.BonePosition.Y - ClientPed.Position.Y);
        #endregion

        #region Networking
        /// <summary>
        /// A more advanced version of `.FromNetworkId()` with checks and optional control network feature.
        /// </summary>
        /// <param name="networkId">The network ID of the entity to retrieve.</param>
        /// <param name="controlNetwork">Determines whether to request control of the network entity.</param>
        /// <returns>
        /// An <see cref="Entity"/> instance if successful; otherwise, <c>null</c>.
        /// </returns>
        public static async Task<Entity> GetEntityFromNetwork(int networkId, bool controlNetwork = true)
        {
            // Check if the network ID is valid
            if (networkId == 0 || !NetworkDoesNetworkIdExist(networkId))
            {
                Log.InfoOrError($"Couldn't request network id: '{networkId}' because it doesn't exist, bailing...");
                return null;
            }

            if (controlNetwork)
            {
                int timeout = 0;

                // Attempt to gain control of the network entity within a timeout limit
                while (!NetworkHasControlOfNetworkId(networkId) && timeout < 4)
                {
                    timeout++;
                    NetworkRequestControlOfNetworkId(networkId);
                    await Delay(500);
                }

                // If control could not be obtained, log the failure and return null
                if (NetworkHasControlOfNetworkId(networkId))
                {
                    Log.InfoOrError($"Couldn't request control of network ID: '{networkId}'.");
                    return null;
                }
            }

            // Return the entity from the network ID
            return Entity.FromNetworkId(networkId);
        }

        /// <summary>
        /// Configures the specified network ID to exist on all machines and allows it to migrate.
        /// </summary>
        /// <param name="networkId">The network ID to configure.</param>
        public static void SetNetworkIdNetworked(int networkId)
        {
            // Ensure the network ID exists on all machines
            SetNetworkIdExistsOnAllMachines(networkId, true);

            // Allow the network ID to migrate between machines
            SetNetworkIdCanMigrate(networkId, true);
        }
        #endregion
    }
}
