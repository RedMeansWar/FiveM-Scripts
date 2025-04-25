using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core.Native;
using CitizenFX.Core;
using Common.Server.Models;

namespace Common.Server
{
    public static class Extensions
    {
        #region Variables
        internal static readonly Random _random = new();
        #endregion

        #region Entity Extensions
        /// <summary>
        /// Determines whether this <see cref="Entity"/> exists.
        /// </summary>
        /// <returns>Returns true if it exists</returns>
        public static bool Exists(this Entity entity)
        {
            return entity != null && API.DoesEntityExist(entity.Handle);
        }

        /// <summary>
        /// Deletes the entity if it exists.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        public static void Delete(this Entity entity)
        {
            // Check if the entity exists in the game world.
            if (entity.Exists())
            {
                // If the entity exists, delete it by its handle using the API.
                API.DeleteEntity(entity.Handle);
            }
        }

        /// <summary>
        /// Casts and returns the <see cref="Entity"/> as a <see cref="Ped"/>.
        /// Only call this if this <see cref="Entity"/> is actually a <see cref="Ped"/>!
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        public static Ped ToPed(this Entity entity) => (Ped)entity;

        /// <summary>
        /// Casts and returns the <see cref="Entity"/> as a <see cref="Vehicle"/>.
        /// Only call this if this <see cref="Entity"/> is actually a <see cref="Vehicle"/>!
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        public static Vehicle ToVehicle(this Entity entity) => (Vehicle)entity;

        /// <summary>
        /// Returns an <see cref="Entity"/>'s current health
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        public static float Health(this Entity entity) => API.GetEntityHealth(entity.Handle);

        /// <summary>
        /// Returns an <see cref="Entity"/>'s max health
        /// </summary>
        /// <param name="entity"></param>
        public static float MaxHealth(this Entity entity) => API.GetEntityMaxHealth(entity.Handle);

        /// <summary>
        /// Checks if this <see cref="Entity"/> is dead
        /// </summary>
        public static bool IsDead(this Entity entity) => entity.Health() <= 0f;
        #endregion

        #region Ped Extensions
        /// <summary>
        /// Determines whether this <see cref="Ped"/> exists.
        /// </summary>
        /// <returns>Returns true if it exists</returns>
        public static bool Exists(this Ped entity)
        {
            return API.DoesEntityExist(entity.Handle);
        }

        /// <summary>
        /// Determines is ped is a <see cref="Player"/>
        /// </summary>
        public static bool IsPlayer(this Ped ped) => API.IsPedAPlayer(ped.Handle);

        /// <summary>
        /// Checks if this <see cref="Ped"/> is in any <see cref="Vehicle"/>
        /// </summary>
        /// <returns>Returns true if in a <see cref="Vehicle"/></returns>
        public static bool IsInVehicle(this Ped ped) => API.GetVehiclePedIsIn(ped.Handle, false) != 0;

        /// <summary>
        /// Returns the <see cref="Ped"/>'s current <see cref="Vehicle"/>
        /// </summary>
        /// <returns>The <see cref="Vehicle"/> the <see cref="Ped"/> is in, or `null`</returns>
        public static Vehicle CurrentVehicle(this Ped ped)
        {
            int vehicle = API.GetVehiclePedIsIn(ped.Handle, false);

            return vehicle != 0 ? new Vehicle(vehicle) : null;
        }

        /// <summary>
        /// Returns the closest <see cref="Vehicle"/> to a <see cref="Ped"/>
        /// </summary>
        public static Vehicle GetClosestVehicleToPed(this Ped ped, float limitRadius = 4.0f)
        {
            Vector3 pedPosition = ped.Position;
            var vehiclesHandles = API.GetAllVehicles();
            Dictionary<int, float> vehicles = new Dictionary<int, float>();

            foreach (int handle in vehiclesHandles)
            {
                float distance = API.GetEntityCoords(handle).DistanceTo(pedPosition);
                if (distance <= limitRadius)
                {
                    vehicles.Add(handle, distance);
                }
            }

            vehicles.OrderBy(i => i.Value);

            return vehicles.Count == 0 ? null : new Vehicle(vehicles.First().Key);
        }

        /// <summary>
        /// Set the <see cref="Ped"/>'s components
        /// </summary>
        public static void SetOutfit(this Ped ped, int componentId, int drawableId, int textureId, int paletteId) => API.SetPedComponentVariation(ped.Handle, componentId, drawableId, textureId, paletteId);

        /// <summary>
        /// Randomizes the <see cref="Ped"/>'s components
        /// </summary>
        public static void RandomizeOutfit(this Ped ped) => API.SetPedRandomComponentVariation(ped.Handle, false);

        /// <summary>
        /// Randomizes the <see cref="Ped"/>'s props
        /// </summary>
        public static void RandomizeProps(this Ped ped) => API.SetPedRandomProps(ped.Handle);

        /// <summary>
        /// Tasks the <see cref="Ped"/> to go to the specified <see cref="Vector3"/>
        /// </summary>
        /// <param name="coords"><see cref="Vector3"/> to go to</param>
        /// <param name="speed">Speed to travel at as <see cref="float"/></param>
        public static void GoTo(this Ped ped, Vector3 coords, float speed = 10f) => API.TaskGoStraightToCoord(ped.Handle, coords.X, coords.Y, coords.Z, speed, -1, GameMath.DirectionToHeading(coords), 0);

        /// <summary>
        /// Tasks the entity to go to a specified <see cref="Ped"/>
        /// </summary>
        /// <param name="target"><see cref="Ped"/> to go to</param>
        /// <param name="duration">Max time in ms as <see cref="int"/> to get to target, -1 for no timeout</param>
        /// <param name="distance">Distance as <see cref="float"/> from target to be considered sucessful</param>
        /// <param name="speed">Speed as <see cref="float"/> to travel at</param>
        public static void GoTo(this Ped ped, Ped target, int duration = -1, float distance = 2f, float speed = 10f) => API.TaskGoToEntity(ped.Handle, target.Handle, duration, distance, speed, 0, 0);

        /// <summary>
        /// Makes the <see cref="Ped"/> attack another <see cref="Ped"/>.
        /// </summary>
        /// <param name="target">The ped to attack</param>
        public static void Attack(this Ped ped, Ped target) => API.TaskCombatPed(ped.Handle, target.Handle, 0, 16);

        /// <summary>
        /// Makes this <see cref="Ped"/> play an animation
        /// </summary>
        /// <param name="entity"><see cref="Ped"/> to play animation on</param>
        /// <param name="animationDictionary">Animation dictionary</param>
        /// <param name="animationName">Animation name</param>
        /// <param name="blendInSpeed">Animation blend in speed</param>
        /// <param name="blendOutSpeed">Animation blend out speed</param>
        /// <param name="duration">Animation duration. -1 for loop</param>
        /// <param name="flag">Animation flag. 1 for loop</param>
        /// <param name="playbackRate">Animation playback speed</param>
        public static void PlayAnimation(this Ped ped, string animationDictionary, string animationName, float blendInSpeed, float blendOutSpeed, int duration, int flag, float playbackRate) => API.TaskPlayAnim(ped.Handle, animationDictionary, animationName, blendInSpeed, blendOutSpeed, duration, flag, playbackRate, false, false, false);

        /// <summary>
        /// Makes this <see cref="Ped"/> enter a <see cref="Vehicle"/>
        /// </summary>
        /// <param name=""></param>
        public static void EnterVehicle(this Ped ped, Vehicle vehicle, int timeout = -1, int seatIndex = -2, float speed = 1f, int flag = 1) => API.TaskEnterVehicle(ped.Handle, vehicle.Handle, timeout, seatIndex, speed, flag, 0);

        /// <summary>
        /// Clears all the <see cref="Peds"/> tasks
        /// </summary>
        /// <param name="immediately">True to clear right away</param>
        public static void ClearAllTasks(this Ped ped, bool immediately = false)
        {
            if (immediately)
            {
                API.ClearPedTasksImmediately(ped.Handle);
            }
            else
            {
                API.ClearPedTasks(ped.Handle);
            }
        }

        /// <summary>
        /// Sets this <see cref="Ped"/>s config flags
        /// </summary>
        /// <param name="entity"><see cref="Ped"/> to set config flags for</param>
        /// <param name="flags">Array for flags</param>
        /// <param name="value">True or false</param>
        public static void SetConfigFlags(this Ped ped, int[] flags, bool value)
        {
            foreach (int flag in flags)
            {
                API.SetPedConfigFlag(ped.Handle, flag, value);
            }
        }
        #endregion

        #region Prop Extensions
        /// <summary>
        /// Determines whether this <see cref="Prop"/> exists.
        /// </summary>
        /// <returns>Returns true if it exists</returns>
        public static bool Exists(this Prop entity)
        {
            return API.DoesEntityExist(entity.Handle);
        }
        #endregion

        #region Vehicle Extensions
        /// <summary>
        /// Determines whether this <see cref="Vehicle"/> exists.
        /// </summary>
        /// <returns>Returns true if it exists</returns>
        public static bool Exists(this Vehicle entity)
        {
            return API.DoesEntityExist(entity.Handle);
        }

        /// <summary>
        /// Returns a <see cref="Vehicle"/>'s model as an <see cref="int"/>
        /// </summary>
        public static int Model(this Vehicle vehicle) => API.GetEntityModel(vehicle.Handle);

        /// <summary>
        /// Returns a <see cref="Vehicle"/>'s type in all caps
        /// </summary>
        public static string Type(this Vehicle vehicle) => API.GetVehicleType(vehicle.Handle).ToUpper();

        /// <summary>
        /// Checks if the <see cref="Vehicle"/>'s engine is running
        /// </summary>
        /// <returns>True is engine is running</returns>
        public static bool IsEngineOn(this Vehicle vehicle) => API.GetIsVehicleEngineRunning(vehicle.Handle);

        /// <summary>
        /// Checks if this <see cref="Vehicle"/> is moving
        /// </summary>
        /// <returns>True is moving</returns>
        public static bool IsMoving(this Vehicle vehicle) => vehicle.Velocity != Vector3.Zero;

        /// <summary>
        /// Returns a <see cref="Vehicle"/>'s livery
        /// </summary>
        /// <returns>0 = No livery (vehicle not loaded)</returns>
        public static int GetLivery(this Vehicle vehicle) => API.GetVehicleLivery(vehicle.Handle);

        /// <summary>
        /// Sets the <see cref="Vehicle"/>s primary and secondary colors
        /// https://wiki.gtanet.work/index.php?title=Vehicle_Colors
        /// </summary>
        /// <param name="entity">The <see cref="Vehicle"/> to change the color of</param>
        /// <param name="primaryColor">Primary color</param>
        /// <param name="secondaryColor">Secondary color</param>
        public static void SetColors(this Vehicle vehicle, int primaryColor, int secondaryColor) => API.SetVehicleColours(vehicle.Handle, primaryColor, secondaryColor);

        /// <summary>
        /// Sets the <see cref="Vehicle"/>'s dirt level.
        /// Values must be between 0 and 15
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="firstLevel"></param>
        public static void SetDirtLevel(this Vehicle vehicle, float dirtLevel) => API.SetVehicleDirtLevel(vehicle.Handle, dirtLevel);

        /// <summary>
        /// Sets a <see cref="Vehicle"/>'s fuel tank to full
        /// </summary>
        public static void SetVehicleFuelFull(this Vehicle vehicle) => vehicle.State.Set("fuel", -1, true);

        /// <summary>
        /// Sets a <see cref="Vehicle"/>'s license plate
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="licensePlate"></param>
        public static void SetLicensePlate(this Vehicle vehicle, string licensePlate) => API.SetVehicleNumberPlateText(vehicle.Handle, licensePlate);

        /// <summary>
        /// Return the <see cref="Ped"/> in the driver's seat
        /// </summary>
        public static Ped Driver(this Vehicle vehicle)
        {
            int vehicleHandle = API.GetPedInVehicleSeat(vehicle.Handle, (int)VehicleSeat.Driver);

            return vehicleHandle != 0 ? new Ped(vehicleHandle) : null;
        }

        /// <summary>
        /// Returns all the <see cref="Ped"/>s in a <see cref="Vehicle"/> including the driver
        /// </summary>
        public static List<Ped> Occupants(this Vehicle vehicle)
        {
            try
            {
                Ped driver = vehicle.Driver();

                if (!(driver is null) && API.DoesEntityExist(driver.Handle))
                {
                    vehicle.Passengers().Insert(0, driver);
                }

                return vehicle.Passengers();
            }
            catch
            {
                return new List<Ped>();
            }
        }

        /// <summary>
        /// Returns all the <see cref="Ped"/>s in a <see cref="Vehicle"/> not including the driver
        /// </summary>
        public static List<Ped> Passengers(this Vehicle vehicle)
        {
            List<Ped> peds = new List<Ped>();
            int vehicleHandle = vehicle.Handle;

            try
            {
                for (int i = 0; i < 20; i++)
                {
                    int pedHandle = API.GetPedInVehicleSeat(vehicleHandle, i);

                    if (pedHandle != 0)
                    {
                        peds.Add(new Ped(pedHandle));
                    }
                }
            }
            catch
            {
                // do nothing
            }

            return peds;
        }
        #endregion

        #region Vector3 Extensions
        /// <summary>
        /// Calculates the squared distance between a ped and a target position.
        /// </summary>
        /// <param name="blip">The blip to calculate the distance from.</param>
        /// <param name="targetPos">The target position to measure the distance to.</param>
        /// <returns>The distance between the ped and the target position.</returns>
        public static float DistanceTo(this Ped ped, Vector3 targetPos)
        {
            // Retrieve the ped's position in the world.
            Vector3 pedPosition = ped.Position;

            // Calculate the distance between the ped and the target position.
            float distance = Vector3.DistanceSquared(pedPosition, targetPos);

            return distance;
        }

        // <summary>
        /// Calculates the 3D distance between two <see cref="Vector3"/> points.
        /// </summary>
        /// <param name="startingPos">The first <see cref="Vector3"/> point.</param>
        /// <param name="targetPos">The second <see cref="Vector3"/> point.</param>
        /// <returns>The 3D distance between <paramref name="startingPos"/> and <paramref name="targetPos"/>.</returns>
        public static float DistanceTo(this Vector3 startingPos, Vector3 targetPos) => CalculateDistanceTo(startingPos, targetPos, true);

        /// <summary>
        /// Calculates the 2D distance (ignoring the Z-axis) between two <see cref="Vector3"/> points.
        /// </summary>
        /// <param name="startingPos">The first <see cref="Vector3"/> point.</param>
        /// <param name="targetPos">The second <see cref="Vector3"/> point.</param>
        /// <returns>The 2D distance between <paramref name="startingPos"/> and <paramref name="targetPos"/>.</returns>
        public static float DistanceTo2d(this Vector3 startingPos, Vector3 targetPos) => CalculateDistanceTo(startingPos, targetPos, false);

        /// <summary>
        /// Calculates the distance between two <see cref="Vector3"/> points, optionally considering the Z-axis.
        /// </summary>
        /// <param name="startingPos">The first <see cref="Vector3"/> point.</param>
        /// <param name="targetPos">The second <see cref="Vector3"/> point.</param>
        /// <param name="useZ">
        /// <c>true</c> to calculate the 3D distance (considering the Z-axis); 
        /// <c>false</c> to calculate the 2D distance (ignoring the Z-axis).
        /// </param>
        /// <returns>The distance between <paramref name="startingPos"/> and <paramref name="targetPos"/>.</returns>
        public static float CalculateDistanceTo(Vector3 startingPos, Vector3 targetPos, bool useZ)
        {
            if (useZ)
            {
                // Calculate the 3D distance using the squared distance method.
                return (float)Math.Sqrt(startingPos.DistanceToSquared(targetPos));
            }
            else
            {
                // Calculate the 2D distance by ignoring the Z-axis.
                return (float)Math.Sqrt(Math.Pow(targetPos.X - startingPos.X, 2) + Math.Pow(targetPos.Y - startingPos.Y, 2));
            }
        }
        #endregion

        #region Random Extensions
        /// <summary>
        /// Generates a random floating-point number within the specified range.
        /// </summary>
        /// <param name="random">The instance of <see cref="Random"/> to generate the number.</param>
        /// <param name="minimum">The inclusive lower bound of the random number.</param>
        /// <param name="maximum">The exclusive upper bound of the random number.</param>
        /// <returns>A random <see cref="float"/> between <paramref name="minimum"/> and <paramref name="maximum"/>.</returns>
        public static float Float(this Random random, float minimum, float maximum) => ((float)random.NextDouble() * (maximum - minimum)) + minimum;

        /// <summary>
        /// Generates a random boolean value, with an adjustable likelihood of returning <c>true</c>.
        /// </summary>
        /// <param name="random">The instance of <see cref="Random"/> to generate the boolean.</param>
        /// <param name="truePercentage">
        /// The percentage chance of the method returning <c>true</c>. Must be between 0 and 100.
        /// Default is 50, meaning equal chance for <c>true</c> or <c>false</c>.
        /// </param>
        /// <returns>
        /// A boolean value: <c>true</c> with the specified probability, or <c>false</c> otherwise.
        /// </returns>
        public static bool NextBool(this Random random, int truePercentage = 50) => random.NextDouble() < truePercentage / 100.0;
        #endregion
    }
}