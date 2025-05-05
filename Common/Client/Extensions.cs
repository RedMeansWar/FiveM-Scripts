using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Common.Models;
using Common.Client.Models;
using static CitizenFX.Core.Native.API;

namespace Common.Client
{
    public static class Extensions
    {
        #region Variables
        internal static readonly Random _random = new();
        internal static readonly IReadOnlyList<string> _wheels = new List<string>
        {
            "wheel_lf", "wheel_rf", "wheel_lm1", "wheel_rm1", "wheel_lm2", "wheel_rm2", "wheel_lm3", "wheel_rm3", "wheel_lr", "wheel_rr"
        };

        internal static readonly IReadOnlyList<int> _wheelIndex = new List<int>
        {
            0, 1, 2, 3, 45, 47, 46, 48, 4, 5
        };
        #endregion

        #region Ped Actions
        /// <summary>
        /// Determines if the Client cannot do a certain action.
        /// </summary>
        /// <param name="ped">The ped to check against.</param>
        /// <returns>If the ped can do an action or not.</returns>
        public static bool CannotDoAction(this Ped ped) =>
           ped.IsCuffed || ped.IsDead || ped.IsBeingStunned
           || ped.IsClimbing || ped.IsDiving || ped.IsFalling
           || ped.IsGettingIntoAVehicle || ped.IsJumping
           || ped.IsJumpingOutOfVehicle || ped.IsRagdoll
           || ped.IsSwimmingUnderWater || ped.IsVaulting;

        /// <summary>
        /// Determines if the Client can do a certain action.
        /// </summary>
        /// <param name="ped">The ped to check against.</param>
        /// <returns>If the ped can do an action or not.</returns>
        public static bool CanDoAction(this Ped ped) => !ped.CannotDoAction();

        /// <summary>
        /// Finds the closest vehicle within a specified radius of a ped.
        /// </summary>
        /// <param name="ped">The ped to search around.</param>
        /// <param name="radius">The search radius (default: 2f).</param>
        /// <returns>The closest vehicle found, or null if none is within the radius.</returns>
        public static Vehicle GetClosestVehicleToClient(this Ped ped, float radius = 2f)
        {
            // Gets the players position
            Vector3 plyrPos = ped.Position;

            // Perform a capsule-shaped raycast to detect and hit a vehicle that is a radius.
            RaycastResult raycast = World.RaycastCapsule(plyrPos, plyrPos, radius, (IntersectOptions)10, Game.PlayerPed);

            if (!Entity.Exists(raycast.HitEntity) || !raycast.HitEntity.Model.IsVehicle || !raycast.DitHitEntity)
            {
                // return null if there is no vehicle or raycast didn't hit the vehicle.
                return null;
            }
            else
            {
                // return the raycast that hit the vehicle.
                return (Vehicle)raycast.HitEntity;
            }
        }

        /// <summary>
        /// Makes the specified ped walk to a target position with a maximum number of attempts.
        /// </summary>
        /// <param name="ped">The ped that will walk to the target position.</param>
        /// <param name="position">The target position the ped should walk to.</param>
        /// <param name="attempts">The maximum number of attempts the ped can make to reach the target position. Defaults to 10.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task WalkTo(this Ped ped, Vector3 position, int attempts = 10)
        {
            // Loop to allow multiple attempts to reach the destination
            while (attempts > 0)
            {
                // Check if the ped is more than 0.5 units away from the target position
                if (ped.Position.DistanceTo(position) > 0.5f)
                {
                    attempts--; // Decrease the attempt count

                    // Command the ped to walk to the specified position
                    ped.Task.GoTo(position);

                    // Wait for 1 second before the next attempt
                    await BaseScript.Delay(1000);
                }
                else
                {
                    // If the ped is close enough to the target position, exit the method
                    return;
                }
            }
        }

        /// <summary>
        /// Makes the specified ped walk to the position of a target entity with a maximum number of attempts.
        /// </summary>
        /// <param name="ped">The ped that will walk to the target entity's position.</param>
        /// <param name="entity">The target entity whose position the ped should walk to.</param>
        /// <param name="attempts">The maximum number of attempts the ped can make to reach the target entity's position. Defaults to 10.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task WalkTo(this Ped ped, Entity entity, int attempts = 10)
        {
            // Check if the entity exists before attempting to walk to its position
            if (!Entity.Exists(entity))
            {
                return; // If the entity doesn't exist, exit the method
            }

            // Loop to allow multiple attempts to reach the target entity's position
            while (attempts > 0)
            {
                // Check if the ped is more than 0.5 units away from the entity's position
                if (ped.Position.DistanceTo(entity.Position) > 0.5f)
                {
                    attempts--; // Decrease the attempt count

                    // Command the ped to walk to the entity's position
                    ped.Task.GoTo(entity.Position);

                    // Wait for 1 second before the next attempt
                    await BaseScript.Delay(1000);
                }
                else
                {
                    // If the ped is close enough to the entity, exit the method
                    return;
                }
            }
        }

        /// <summary>
        /// Modify the peds behavior and properties on a specific action or "flag".
        /// </summary>
        /// <param name="ped">The ped that have its flag modified.</param>
        /// <param name="flag">The flag to be modified/changed.</param>
        /// <param name="value">The value for the flag to be active.</param>
        public static void SetConfigFlag(this Ped ped, PedConfigFlag flag, bool value) => ped.SetConfigFlag((int)flag, value);

        /// <summary>
        /// Resets the peds behavior and properties on a specific action or "flag".
        /// </summary>
        /// <param name="ped">The ped that have its flag reset.</param>
        /// <param name="flag">The flag to be reset.</param>
        public static void ResetConfigFlag(this Ped ped, PedConfigFlag flag) => ped.ResetConfigFlag((int)flag);
        #endregion

        #region Player Actions
        /// <summary>
        /// Finds the closest player to the given player within a specified radius.
        /// </summary>
        /// <param name="player">The player to search around.</param>
        /// <param name="radius">The search radius (default: 2f).</param>
        /// <returns>The closest player found, or null if none is within the radius.</returns>
        public static Player GetClosestPlayerToClient(this Player player, float radius = 2f)
        {
            // Get the player's position for distance calculations.
            Vector3 playerPos = Game.PlayerPed.Position;

            // Initialize variables to track the closest player and distance.
            Player closestPlayer = null;
            float closestDist = float.MaxValue; // start with the maxium possible distance

            // Iterate through all players
            foreach (Player plyr in PlayerList.Players)
            {
                // Skip over players that are null or any self-references, and players without characters.
                if (plyr is null || plyr == Game.Player || !Entity.Exists(plyr.Character))
                {
                    continue; // continue if the all the condition aren't met
                }

                // Calculate the squared distance between the player and the secondary player.
                float distance = Vector3.DistanceSquared(plyr.Character.Position, playerPos);

                // Update the closest player and distance if a closer one is found within the specified radius.
                if (distance < closestDist && distance < radius)
                {
                    closestPlayer = plyr;
                    closestDist = distance;
                }
            }

            // Return the closest player, or null if none were found within the radius.
            return closestPlayer;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Player"/> instance is valid.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> instance to validate.</param>
        /// <returns>
        /// <c>true</c> if the <paramref name="player"/> is not null and its <see cref="Player.Character"/> property is not null; 
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValid(this Player player) => !(player is null) && !(player.Character is null);
        #endregion

        #region Vehicle Extensions
        /// <summary>
        /// Finds the closest seat in the vehicle to the specified entity.
        /// </summary>
        /// <param name="vehicle">The vehicle to search for available seats.</param>
        /// <param name="entity">The entity (e.g., player or NPC) for which the closest seat is to be found.</param>
        /// <param name="includeDriversSeat">A flag indicating whether to include the driver's seat in the search. Defaults to true.</param>
        /// <returns>The closest available seat to the specified entity, or `VehicleSeat.None` if no seat is found.</returns>
        public static VehicleSeat ClosestSeatToEntity(this Vehicle vehicle, Entity entity, bool includeDriversSeat = true)
        {
            // Get the position of the entity (e.g., player or NPC)
            Vector3 playerPosition = entity.Position;

            // If the vehicle doesn't exist, return 'None' as no seat is available
            if (!Entity.Exists(vehicle))
            {
                return VehicleSeat.None;
            }

            // Dictionary to store the seat positions and their distances from the entity's position
            Dictionary<VehicleSeat, double> results = new Dictionary<VehicleSeat, double>();

            // Check if the driver's seat should be included in the search
            if (includeDriversSeat)
            {
                // Get the position of the driver's seat (from the vehicle bone "handle_dside_f")
                Vector3 lfPos = vehicle.Bones["handle_dside_f"].Position;

                // Calculate the distance from the entity to the driver's seat
                double distLF = Math.Sqrt(Math.Pow(playerPosition.X - lfPos.X, 2) + Math.Pow(playerPosition.Y - lfPos.Y, 2));

                // If the entity is within 1.5 units of the driver's seat, add it to the results
                if (distLF < 1.5f)
                {
                    results.Add(VehicleSeat.Driver, distLF);
                }
            }

            // Get the position of the front passenger seat (from the vehicle bone "handle_pside_f")
            Vector3 rfPos = vehicle.Bones["handle_pside_f"].Position;

            // Calculate the distance from the entity to the front passenger seat
            double distRF = Math.Sqrt(Math.Pow(playerPosition.X - rfPos.X, 2) + Math.Pow(playerPosition.Y - rfPos.Y, 2));

            // If the entity is within 1.5 units of the front passenger seat, add it to the results
            if (distRF < 1.5f)
            {
                results.Add(VehicleSeat.Passenger, distRF);
            }

            // Check if the vehicle has a rear driver window or handle to determine if the rear driver seat is accessible
            bool hasRearDriverWindow = vehicle.Bones.HasBone("window_lr"),
                 hasRearDriverHandle = vehicle.Bones.HasBone("handle_dside_r");

            // If the rear driver seat has either a window or handle, calculate the distance and add to results if close enough
            if (hasRearDriverHandle || hasRearDriverWindow)
            {
                // Get the position of the rear driver seat (using either the "handle_dside_r" or "window_lr" bone)
                Vector3 lrPos = hasRearDriverHandle ? vehicle.Bones["handle_dside_r"].Position : vehicle.Bones["window_lr"].Position;

                // Calculate the distance from the entity to the rear driver seat
                double distLR = Math.Sqrt(Math.Pow(playerPosition.X - lrPos.X, 2) + Math.Pow(playerPosition.Y - lrPos.Y, 2));

                // If the entity is within 1.5 units of the rear driver seat, add it to the results
                if (distLR < 1.5f)
                {
                    results.Add(VehicleSeat.LeftRear, distLR);
                }
            }

            // Check if the vehicle has a rear passenger window or handle to determine if the rear passenger seat is accessible
            bool hasRearPassengerWindow = vehicle.Bones.HasBone("window_rr"),
                 hasRearPassengerHandle = vehicle.Bones.HasBone("handle_pside_r");

            // If the rear passenger seat has either a window or handle, calculate the distance and add to results if close enough
            if (hasRearPassengerHandle || hasRearPassengerWindow)
            {
                // Get the position of the rear passenger seat (using either the "handle_pside_r" or "window_rr" bone)
                Vector3 rrPos = hasRearPassengerHandle ? vehicle.Bones["handle_pside_r"].Position : vehicle.Bones["window_rr"].Position;

                // Calculate the distance from the entity to the rear passenger seat
                double distRR = Math.Sqrt(Math.Pow(playerPosition.X - rrPos.X, 2) + Math.Pow(playerPosition.Y - rrPos.Y, 2));

                // If the entity is within 1.5 units of the rear passenger seat, add it to the results
                if (distRR < 1.5f)
                {
                    results.Add(VehicleSeat.RightRear, distRR);
                }
            }

            // If no seats were found, return 'None'
            if (results.Count == 0)
            {
                return VehicleSeat.None;
            }

            // Otherwise, return the seat with the smallest distance (closest seat)
            return results.OrderBy(seat => seat.Value).FirstOrDefault().Key;
        }

        /// <summary>
        /// Finds the closest tire of a specified vehicle to the player within a given radius.
        /// </summary>
        /// <param name="vehicle">The vehicle whose tires are to be checked.</param>
        /// <param name="radius">The maximum distance within which to search for the closest tire. Default is 1.5 units.</param>
        /// <returns>
        /// A <see cref="Wheel"/> object representing the closest tire within the specified radius;
        /// otherwise, returns <c>null</c> if no tire is found within the radius.
        /// </returns>
        public static Wheel GetClosestWheel(this Vehicle vehicle, float radius = 1.5f)
        {
            // Initialize the closest distance with the maximum possible value for comparison.
            float closestDistance = float.MaxValue;

            // Initialize a variable to hold the closest Wheel, initially set to null.
            Wheel closestWheel = null;

            // Iterate over the list of Wheels for the vehicle.
            for (int i = 0; i < _wheels.Count; i++)
            {
                // Get the world position of the current Wheel bone on the vehicle.
                Vector3 WheelPos = vehicle.Bones[_wheels[i]].Position;

                // Calculate the distance between the player's position and the current Wheel's position.
                float distance = Game.PlayerPed.DistanceTo(WheelPos);

                // Check if the distance is within the specified radius and is the smallest distance found so far.
                if (distance < radius && distance < closestDistance)
                {
                    // Update the closest distance with the current smaller distance.
                    closestDistance = distance;

                    // Assign the current Wheel as the closest Wheel and store its details.
                    closestWheel = new()
                    {
                        Distance = distance,      // The distance to the Wheel from the player
                        BonePosition = WheelPos,   // The position of the Wheel bone
                        WheelIndex = _wheelIndex[i] // The index of the Wheel in the list
                    };
                }
            }

            // Return the closest tire found within the specified radius, or null if no tire met the criteria.
            return closestWheel;
        }
        /// <summary>
        /// Sets the color of the neon lights on the specified vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle whose neon lights color will be set.</param>
        /// <param name="color">The color to set for the neon lights.</param>
        public static void SetNeonLightsColor(this Vehicle vehicle, Color color)
            => SetVehicleNeonLightsColour(vehicle.Handle, color.R, color.G, color.B);


        /// <summary>
        /// Checks whether the specified neon light is enabled on the given vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle to check for neon light status.</param>
        /// <param name="neonLight">The specific neon light to check (e.g., front, back, left, right).</param>
        /// <returns>
        /// <c>true</c> if the specified neon light is enabled; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNeonLightEnabled(this Vehicle vehicle, NeonLights neonLight)
        {
            // Check if the specific neon light is enabled on the vehicle
            if (IsVehicleNeonLightEnabled(vehicle.Handle, (int)neonLight))
            {
                return true;
            }

            // Return false if the neon light is not enabled
            return false;
        }
        #endregion

        #region Conversions
        /// <summary>
        /// Converts a speed value from meters per second to miles per hour.
        /// </summary>
        /// <param name="speed">The speed in meters per second.</param>
        /// <returns>The speed in miles per hour.</returns>
        public static float ConvertToMph(this float speed) => speed * 2.236936f;

        /// <summary>
        /// Converts a speed value from miles per hour to meters per second.
        /// </summary>
        /// <param name="speed">The speed in miles per hour.</param>
        /// <returns>The speed in meters per second.</returns>
        public static float ConvertFromMph(this float speed) => speed * 0.44704f;

        /// <summary>
        /// Converts a speed value from meters per second to kilometers per hour.
        /// </summary>
        /// <param name="speed">The speed in meters per second.</param>
        /// <returns>The speed in kilometers per hour.</returns>
        public static float ConvertToKph(this float speed) => speed * 3.6f;

        /// <summary>
        /// Converts a height value from meters to feet.
        /// </summary>
        /// <param name="height">The height in meters.</param>
        /// <returns>The height in feet.</returns>
        public static float ConvertToFeet(this float height) => height * 3.28084f;
        #endregion

        #region Vector3 Extensions
        /// <summary>
        /// Returns a random point within a specified radius around the given <see cref="Vector3"/>.
        /// </summary>
        /// <param name="start">The starting position as a <see cref="Vector3"/>.</param>
        /// <param name="radius">The radius within which to generate a random point.</param>
        /// <returns>
        /// A new <see cref="Vector3"/> that represents a random position within the specified radius around the starting point.
        /// </returns>
        public static Vector3 Around(this Vector3 start, float radius) => start + (RandomXY() * radius);

        /// <summary>
        /// Returns a random point within a specified distance range around the given <see cref="Vector3"/>.
        /// </summary>
        /// <param name="start">The starting position as a <see cref="Vector3"/>.</param>
        /// <param name="minDistance">The minimum distance from the starting position.</param>
        /// <param name="maxDistance">The maximum distance from the starting position.</param>
        /// <returns>
        /// A new <see cref="Vector3"/> that represents a random position within the specified distance range around the starting point.
        /// </returns>
        public static Vector3 Around(this Vector3 start, float minDistance, float maxDistance) => start.Around(_random.Float(minDistance, maxDistance));

        /// <summary>
        /// Generates a random 2D unit vector (on the XY plane).
        /// </summary>
        /// <returns>
        /// A normalized <see cref="Vector3"/> with random X and Y components, and Z set to 0.
        /// </returns>
        /// <remarks>
        /// The method uses <see cref="System.Random"/> to generate random values for the X and Y components.
        /// The resulting vector is normalized to ensure its magnitude is 1.
        /// </remarks>
        public static Vector3 RandomXY()
        {
            // Create a new Vector3 with random X and Y values between -0.5 and 0.5.
            Vector3 v3 = new()
            {
                X = (float)(_random.NextDouble() - 0.5), // Random value for X
                Y = (float)(_random.NextDouble() - 0.5), // Random value for Y
                Z = 0f // Z is always 0 for 2D vector
            };

            // Normalize the vector to ensure its length is 1 (unit vector).
            v3.Normalize();
            return v3;
        }

        /// <summary>
        /// Retrieves the street name and crossing name at a specified 3D position.
        /// </summary>
        /// <param name="position">The 3D position (Vector3) to query for street and crossing names.</param>
        /// <returns>
        /// A string array where the first element is the main street name, 
        /// and the second element is the crossing street name (or an empty string if none exists).
        /// </returns>
        public static string[] GetStreetAndCrossAtCoords(this Vector3 position)
        {
            // Placeholder for the position of the closest vehicle node (not used directly)
            Vector3 outPos = Vector3.Zero;

            // Finds the nth closest vehicle node to the given position (currently not used directly)
            GetNthClosestVehicleNode(position.X, position.Y, position.Z, 0, ref outPos, 0, 0, 0);

            // Variables to store the hash keys for the street and crossing
            uint crossing = 1;
            uint street = 1;

            // Retrieves the street and crossing hash keys at the specified coordinates
            GetStreetNameAtCoord(position.X, position.Y, position.Z, ref street, ref crossing);

            // Converts the crossing hash key to a human-readable name
            string crossName = GetStreetNameFromHashKey(crossing);

            // Determines the suffix, ensuring it's valid and not null or placeholder values
            string suffix = (crossName != "" && crossName != "NULL" && crossName is not null) ? crossName : "";

            // Returns the main street name and the optional crossing name
            return [World.GetStreetName(position), suffix];
        }

        /// <summary>
        /// Retrieves the concatenated street name and crossing name at a specified 3D position.
        /// </summary>
        /// <param name="position">The 3D position (Vector3) to query for street and crossing names.</param>
        /// <param name="separator">The string used to separate the street and crossing names in the result.</param>
        /// <returns>
        /// A string containing the main street name and crossing name concatenated with the specified separator. 
        /// If no crossing name exists, only the main street name is returned.
        /// </returns>
        public static string GetStreetAndCrossWholeAtCoords(this Vector3 position, string separator)
        {
            // Get the street and crossing names as an array
            string[] text = GetStreetAndCrossAtCoords(position);

            // Check if the crossing name is empty and return either just the street name or both concatenated
            return text[1] == "" ? text[0] : string.Join(separator, text);
        }

        /// <summary>
        /// Calculates the heading angle (in degrees) from a starting position to a target position in 3D space.
        /// </summary>
        /// <param name="startPos">The starting position as a <see cref="Vector3"/>.</param>
        /// <param name="targetPos">The target position as a <see cref="Vector3"/>.</param>
        /// <returns>
        /// The heading angle (in degrees) towards the target position. 
        /// The angle is calculated based on the direction from the starting position to the target position.
        /// </returns>
        public static float CalculateHeadingTowardsPosition(this Vector3 startPos, Vector3 targetPos)
        {
            // Calculate the direction vector from the starting position to the target position
            Vector3 direction = targetPos - startPos;

            // Normalize the direction vector to ensure it's a unit vector
            direction.Normalize();

            // Convert the direction vector to a heading angle
            return GameMath.DirectionToHeading(direction);
        }

        /// <summary>
        /// Parses a string array into a Vector3 instance.
        /// </summary>
        /// <param name="Vector3">The Vector3 instance to modify.</param>
        /// <param name="stringArray">An array of strings containing at least 3 elements for x, y, and z values.</param>
        /// <returns>A new Vector3 with the parsed values.</returns>
        /// <exception cref="ArgumentException">Thrown if the input array has fewer than 3 elements.</exception>
        public static Vector3 Parse(this Vector3 Vector3, string[] stringArray)
        {
            // Ensure the input array has at least 3 components
            if (stringArray.Length < 3)
            {
                throw new ArgumentException("Input array must have at least 3 elements for a Vector3.");
            }

            // Parse the components into floats
            float x = float.Parse(stringArray[0]);
            float y = float.Parse(stringArray[1]);
            float z = float.Parse(stringArray[2]);

            // Assign the parsed values to a new Vector3 instance
            return Vector3 = new(x, y, z);
        }

        /// <summary>
        /// Parses a comma-separated string into a Vector3 instance.
        /// </summary>
        /// <param name="v3">The Vector3 instance to modify.</param>
        /// <param name="s">A comma-separated string containing x, y, and z values.</param>
        /// <returns>A new Vector3 with the parsed values.</returns>
        /// <exception cref="ArgumentException">Thrown if the string is null, empty, or has fewer than 3 components.</exception>
        public static Vector3 Parse(this Vector3 v3, string s)
        {
            // Ensure the input string is not null or empty
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentException("Input string cannot be null or empty.");
            }

            // Split the string into components and ensure there are enough parts
            string[] parts = s.Split(',');
            if (parts.Length < 3)
            {
                throw new ArgumentException("Input string must have at least 3 components separated by commas.");
            }

            // Parse the components into floats
            float x = float.Parse(parts[0].Trim());
            float y = float.Parse(parts[1].Trim());
            float z = float.Parse(parts[2].Trim());

            // Assign the parsed values to a new Vector3 instance
            return v3 = new(x, y, z);
        }

        #endregion

        #region Vector4 Extensions
        /// <summary>
        /// Parses a string array into a Vector4 instance.
        /// </summary>
        /// <param name="v4">The Vector4 instance to modify.</param>
        /// <param name="stringArray">An array of strings containing at least 4 components representing x, y, z, and w values.</param>
        /// <returns>A new Vector4 with the parsed values.</returns>
        /// <exception cref="ArgumentException">Thrown if the input array has fewer than 4 components.</exception>
        public static Vector4 Parse(this Vector4 v4, string[] stringArray)
        {
            // Ensure the input array has at least 4 components
            if (stringArray.Length < 4)
            {
                throw new ArgumentException("Input string must have at least 4 components.");
            }

            // Parse the components into floats
            float x = float.Parse(stringArray[0]);
            float y = float.Parse(stringArray[1]);
            float z = float.Parse(stringArray[2]);
            float w = float.Parse(stringArray[3]);

            // Assign the parsed values to a new Vector4 instance
            return v4 = new(x, y, z, w);
        }

        /// <summary>
        /// Parses a comma-separated string into a Vector4 instance.
        /// </summary>
        /// <param name="v4">The Vector4 instance to modify.</param>
        /// <param name="s">A comma-separated string containing x, y, z, and w values.</param>
        /// <returns>A new Vector4 with the parsed values.</returns>
        /// <exception cref="ArgumentException">Thrown if the string is null, empty, or has fewer than 4 components.</exception>
        public static Vector4 Parse(this Vector4 v4, string s)
        {
            // Ensure the input string is not null or empty
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentException("Input string cannot be null or empty.");
            }

            // Split the string into components and ensure there are enough parts
            string[] parts = s.Split(',');
            if (parts.Length < 4)
            {
                throw new ArgumentException("Input string must have at least 4 components separated by commas.");
            }

            // Parse the components into floats
            float x = float.Parse(parts[0].Trim());
            float y = float.Parse(parts[1].Trim());
            float z = float.Parse(parts[2].Trim());
            float w = float.Parse(parts[3].Trim());

            // Assign the parsed values to a new Vector4 instance
            return v4 = new(x, y, z, w);
        }
        #endregion

        #region Vector2 Extensions
        /// <summary>
        /// Parses a string array into a Vector2 instance.
        /// </summary>
        /// <param name="v2">The Vector2 instance to modify.</param>
        /// <param name="stringArray">An array of strings containing at least 2 components representing x and y values.</param>
        /// <returns>A new Vector2 with the parsed values.</returns>
        /// <exception cref="ArgumentException">Thrown if the input array has fewer than 2 components.</exception>
        public static Vector2 Parse(this Vector2 v2, string[] stringArray)
        {
            // Ensure the input array has at least 2 components
            if (stringArray.Length < 2)
            {
                throw new ArgumentException("Input string must have at least 2 components.");
            }

            // Parse the components into floats
            float x = float.Parse(stringArray[0]);
            float y = float.Parse(stringArray[1]);

            // Assign the parsed values to a new Vector2 instance
            return v2 = new(x, y);
        }

        /// <summary>
        /// Parses a comma-separated string into a Vector2 instance.
        /// </summary>
        /// <param name="v2">The Vector2 instance to modify.</param>
        /// <param name="s">A comma-separated string containing x and y values.</param>
        /// <returns>A new Vector2 with the parsed values.</returns>
        /// <exception cref="ArgumentException">Thrown if the string is null, empty, or has fewer than 2 components.</exception>
        public static Vector2 Parse(this Vector2 v2, string s)
        {
            // Ensure the input string is not null or empty
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentException("Input string cannot be null or empty.");
            }

            // Split the string into components and ensure there are enough parts
            string[] parts = s.Split(',');
            if (parts.Length < 2)
            {
                throw new ArgumentException("Input string must have at least 2 components separated by commas.");
            }

            // Parse the components into floats
            float x = float.Parse(parts[0].Trim());
            float y = float.Parse(parts[1].Trim());

            // Assign the parsed values to a new Vector2 instance
            return v2 = new(x, y);
        }
        #endregion

        #region Distance Calculations
        /// <summary>
        /// Calculates the squared distance between a ped and a target position.
        /// </summary>
        /// <param name="ped">The blip to calculate the distance from.</param>
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

        /// <summary>
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

        /// <summary>
        /// Calculates the distance between a vehicle and a target position.
        /// </summary>
        /// <param name="vehicle">The vehicle to calculate the distance from.</param>
        /// <param name="targetPos">The target position to measure the distance to.</param>
        /// <returns>The distance between the vehicle and the target position.</returns>
        public static float DistanceTo(this Vehicle vehicle, Vector3 targetPos)
        {
            // Retrieve the vehicle's position in the world.
            Vector3 vehPosition = vehicle.Position;

            // Calculate the distance between the vehicle and the target position.
            float distance = Vector3.DistanceSquared(vehPosition, targetPos);

            return distance;
        }

        /// <summary>
        /// Calculates the distance between a prop and a target position.
        /// </summary>
        /// <param name="prop">The prop to calculate the distance from.</param>
        /// <param name="targetPos">The target position to measure the distance to.</param>
        /// <returns>The distance between the prop and the target position.</returns>
        public static float DistanceTo(this Prop prop, Vector3 targetPos)
        {
            // Retrieve the prop's position in the world.
            Vector3 propPos = prop.Position;

            // Calculate the distance between the prop and the target position.
            float distance = Vector3.DistanceSquared(propPos, targetPos);

            return distance;
        }
        #endregion

        #region Hash Keys
        /// <summary>
        /// Gets the Hash Key of the vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle to get a hash from.</param>
        /// <returns>The hash of a vehicle.</returns>
        public static uint GetHashKey(this Vehicle vehicle) => (uint)API.GetHashKey(vehicle.DisplayName);

        /// <summary>
        /// Gets the Hash Key of the vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle to get a hash from.</param>
        /// <returns>The hash of a vehicle.</returns>
        public static int GetHaskKey(this Vehicle vehicle) => API.GetHashKey(vehicle.DisplayName);

        /// <summary>
        /// Gets the Hash Key of the weapon.
        /// </summary>
        /// <param name="weapon">The weapon to get a hash from.</param>
        /// <returns>The hash of a weapon.</returns>
        public static uint GetHashKey(this Weapon weapon) => (uint)API.GetHashKey(weapon.DisplayName);
        #endregion

        #region Entity Extensions
        /// <summary>
        /// Calculates the corrected heading for an entity, converting it to a clockwise orientation.
        /// </summary>
        /// <param name="e">The entity whose heading needs correction.</param>
        /// <returns>
        /// A corrected heading value in degrees, ensuring it is within the valid range of 0 to 360.
        /// </returns>
        public static float CorrectHeading(this Entity e)
        {
            // Calculate the corrected heading by flipping the direction to clockwise
            float hdg = 360 - e.Heading;

            // Ensure the heading value remains within the valid range (0-360)
            if (hdg > 360)
            {
                hdg -= 360;
            }

            // Return the corrected heading
            return hdg;
        }

        /// <summary>
        /// Retrieves the street name and crossing name at the entity's current position.
        /// </summary>
        /// <param name="entity">The entity whose position will be used to query street and crossing names.</param>
        /// <returns>
        /// A string array where the first element is the main street name, 
        /// and the second element is the crossing street name (or an empty string if none exists).
        /// </returns>
        public static string[] GetStreetAndCrossAtEntityPosition(this Entity entity)
            => GetStreetAndCrossAtCoords(entity.Position);

        /// <summary>
        /// Retrieves a concatenated string of the street name and crossing name at the entity's current position.
        /// </summary>
        /// <param name="entity">The entity whose position will be used to query street and crossing names.</param>
        /// <param name="separator">The string used to separate the street and crossing names in the result.</param>
        /// <returns>
        /// A string containing the main street name and crossing name concatenated with the specified separator. 
        /// If no crossing name exists, only the main street name is returned.
        /// </returns>
        public static string GetStreetAndCrossWholeAtEntityPosition(this Entity entity, string separator)
            => GetStreetAndCrossWholeAtCoords(entity.Position, separator);

        /// <summary>
        /// Calculates the speed of the entity in the specified unit.
        /// </summary>
        /// <param name="entity">The entity whose speed is being measured.</param>
        /// <param name="unit">
        /// The unit of speed. Accepts "mph" for miles per hour or "kmh" for kilometers per hour. 
        /// Defaults to "mph".
        /// </param>
        /// <returns>
        /// The speed of the entity in the specified unit, or 0 if an invalid unit is provided.
        /// </returns>
        public static float Speed(this Entity entity, string unit = "mph")
            => unit == "mph" ? GetEntitySpeed(entity.Handle) * 2.236936f : (unit == "kmh" ? GetEntitySpeed(entity.Handle) * 3.6f : 0f);

        /// <summary>
        /// Calculates the heading angle (in degrees) from the current entity to the target entity.
        /// </summary>
        /// <param name="entity">The entity whose heading will be calculated.</param>
        /// <param name="targetEntity">The target entity to which the heading will be calculated.</param>
        /// <returns>
        /// The heading angle (in degrees) from the current entity to the target entity. 
        /// The angle is calculated based on the direction from the current entity's position to the target entity's position.
        /// </returns>
        public static float CalculateHeadingTowardsEntity(this Entity entity, Entity targetEntity)
        {
            // Calculate the direction vector from the current entity's position to the target entity's position
            Vector3 direction = targetEntity.Position - entity.Position;

            // Normalize the direction vector to ensure it's a unit vector
            direction.Normalize();

            // Convert the normalized direction vector to a heading angle and return the result
            return GameMath.DirectionToHeading(direction);
        }

        /// <summary>
        /// Rotates an <see cref="Entity"/> by applying an offset to a raw heading value.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> to rotate (this parameter is not actually used in the calculation but is part of the extension method signature).</param>
        /// <param name="rawHeading">The raw heading angle in degrees.</param>
        /// <param name="offsetDegrees">The offset angle in degrees to apply to the raw heading.</param>
        /// <returns>The final orientation angle in degrees, normalized to the range [0, 360).</returns>
        public static float Rotate(this Entity entity, float rawHeading, float offsetDegrees)
        {
            // Calculate the initial orientation by adding the offset to the raw heading.
            float orientation = (rawHeading + offsetDegrees) % 360;

            // Ensure the orientation is within the range [0, 360) by adding 360 if it's negative.
            if (orientation < 0)
            {
                orientation += 360;
            }

            // Return the final, normalized orientation angle.
            return orientation;
        }
        #endregion

        #region Networking
        /// <summary>
        /// Attempts to network the specified entity, ensuring it is registered and assigned a network ID.
        /// </summary>
        /// <param name="entity">The entity to be networked.</param>
        public static async void Network(this Entity entity)
        {
            // Check if the entity exists before proceeding
            if (!entity.Exists())
            {
                Log.InfoOrError($"Failed to network entity, entity does not exist.");
                return;
            }

            int start = Game.GameTime;

            // Attempt to register the entity as networked within a 5-second window
            while (!NetworkGetEntityIsNetworked(entity.Handle) && (Game.GameTime - start) < 5000)
            {
                NetworkRegisterEntityAsNetworked(entity.Handle);
                await BaseScript.Delay(100);
            }

            // Retrieve the network ID and set it as networked
            int networkId = entity.NetworkId;
            ClientCommonScript.SetNetworkIdNetworked(networkId);

            Log.InfoOrError($"Got network ID '{networkId}' from entity ID {entity.Handle}");
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