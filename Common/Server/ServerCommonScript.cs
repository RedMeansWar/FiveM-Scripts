using System;
using System.Linq;
using CitizenFX.Core;

namespace Common.Server
{
    public abstract class ServerCommonScript : ServerScript
    {
        #region Variables
        internal static ExportDictionary _resourceExports;
        internal static Player _player;
        #endregion

        #region Exports
        /// <summary>
        /// Retrieves a dynamic export from a resource.
        /// </summary>
        /// <param name="resource">The name of the resource to retrieve the export from.</param>
        /// <returns>The exported value, or null if not found.</returns>
        public static dynamic Exports(string resource) => _resourceExports[resource];
        #endregion

        #region Player References
        public static Player ClientPlayer = _player;
        #endregion

        #region Identifiers
        /// <summary>
        /// Gets the player's license ID.
        /// Reliability: Best, but may change on system resets.
        /// </summary>
        /// <param name="player">The player to get the identifier for.</param>
        /// <returns>The player's license ID, or an empty string if not found.</returns>
        public static string GetLicenseId(Player player) => GetIdentifierFromType(player, "license:");

        /// <summary>
        /// Gets the player's Discord ID.
        /// Reliability: Excellent
        /// </summary>
        /// <param name="player">The player to get the identifier for.</param>
        /// <returns>The player's Discord ID, or an empty string if not found.</returns>
        public static string GetDiscordId(Player player) => GetIdentifierFromType(player, "discord:");

        /// <summary>
        /// Gets the player's Steam Hex.
        /// Reliability: Excellent
        /// </summary>
        /// <param name="player">The player to get the identifier for.</param>
        /// <returns>The player's Steam Hex, or an empty string if not found.</returns>
        public static string GetSteamId(Player player) => GetIdentifierFromType(player, "steam:");

        /// <summary>
        /// Gets the player's Live ID.
        /// Reliability: Poor
        /// </summary>
        /// <param name="player">The player to get the identifier for.</param>
        /// <returns>The player's Xbox Live Id, or an empty string if not found.</returns>
        public static string GetLiveId(Player player) => GetIdentifierFromType(player, "live:");

        /// <summary>
        /// Gets the player's IP address.
        /// Reliability: Excellent
        /// </summary>
        /// <param name="player">The player to get the identifier for.</param>
        /// <returns>The player's IP address, or an empty string if not found.</returns>
        public static string GetIpId(Player player) => GetIdentifierFromType(player, "ip:");

        /// <summary>
        /// Gets the player's Xbox Live ID.
        /// Reliability: Poor
        /// </summary>
        /// <param name="player">The player to get the identifier for.</param>
        /// <returns>The player's Xbox Live Id, or an empty string if not found.</returns>
        public static string GetXblId(Player player) => GetIdentifierFromType(player, "xbl:");

        /// <summary>
        /// Gets the player's Public IP Address.
        /// Reliability: Best
        /// </summary>
        /// <param name="player"></param>
        /// <returns>The player's IP Address, or an empty string if not found.</returns>
        public static string GetIpAddress(Player player) => GetIdentifierFromType(player, "ip:");

        /// <summary>
        /// Gets the identifier from a player's identifiers that starts with a given prefix.
        /// </summary>
        /// <param name="player">The player to get the identifier from.</param>
        /// <param name="identifierPrefix">The prefix of the identifier to get.</param>
        /// <returns>The identifier that starts with the given prefix, or an empty string if not found.</returns>
        public static string GetIdentifierFromType(Player player, string identifierPrefix)
        {
            // Initialize an empty string to store the indentifier
            string id = "";

            // Check if the player and thheir identifiers are not null
            if (player is not null && player.Identifiers is not null)
            {
                // Use LINQ to find the first identifier that start with the given prefix
                //   - Where: filters the identifiers to only include those that start with the prefix
                //   - Select: removes the prefix from the identifier
                //   - FirstOrDefault: gets the first element from the filtered collection, or null if none found
                id = player.Identifiers
                    .Where(prefix => prefix.StartsWith(identifierPrefix))
                    .Select(s => s.Replace(identifierPrefix, ""))
                    .FirstOrDefault();

                // If the id is null or whitespace, set it to an empty string
                if (string.IsNullOrWhiteSpace(id))
                {
                    id = "";
                }
            }

            // Return the identifier.
            return id;
        }
        #endregion

        #region Vector3
        /// <summary>
        /// Calculates the offset in world coordinates for a given entity and offset.
        /// </summary>
        /// <param name="entity">The entity whose position and rotation are used for the calculation.</param>
        /// <param name="offset">The offset in local coordinates to apply to the entity's position.</param>
        public static Vector3 GetOffsetInWorldCoords(Entity entity, Vector3 offset) => GetOffsetInWorldCoords(entity.Position, entity.Rotation, offset);

        /// <summary>
        /// Calculates the offset in world coordinates based on a position, rotation, and local offset.
        /// </summary>
        /// <param name="position">The position in world coordinates to use for the calculation.</param>
        /// <param name="rotation">The rotation in world coordinates to apply to the offset.</param>
        /// <param name="offset">The offset in local coordinates to be transformed into world coordinates.</param>
        /// <returns>A Vector3 representing the offset in world coordinates.</returns>
        public static Vector3 GetOffsetInWorldCoords(Vector3 position, Vector3 rotation, Vector3 offset)
        {
            // Convert the rotation angles from degrees to radians for trigonometric calculations.
            float rX = MathUtil.DegreesToRadians(rotation.X);
            float rY = MathUtil.DegreesToRadians(rotation.Y);
            float rZ = MathUtil.DegreesToRadians(rotation.Z);

            // Calculate the cosine and sine of the rotation angles for all three axes.
            double cosRx = Math.Cos(rX);
            double cosRy = Math.Cos(rY);
            double cosRz = Math.Cos(rZ);
            double sinRx = Math.Sin(rX);
            double sinRy = Math.Sin(rY);
            double sinRz = Math.Sin(rZ);

            // Build a transformation matrix based on the rotation.
            Matrix matrix = new()
            {
                M11 = (float)((cosRz * cosRy) - (sinRz * sinRx * sinRy)),
                M12 = (float)((cosRy * sinRz) + (cosRz * sinRx * sinRy)),
                M13 = (float)(-cosRx * sinRy),
                M14 = 1,

                M21 = (float)(-cosRx * sinRz),
                M22 = (float)(cosRz * cosRx),
                M23 = (float)sinRx,
                M24 = 1,

                M31 = (float)((cosRz * sinRy) + (cosRy * sinRz * sinRx)),
                M32 = (float)((sinRz * sinRy) - (cosRz * cosRy * sinRx)),
                M33 = (float)(cosRx * cosRy),
                M34 = 1,

                // Translate the matrix by the given position, adjusting for a slight offset on the Z-axis (position.Z - 1f).
                Row4 = new Vector4(position.X, position.Y, position.Z - 1f, 1f)
            };

            // Apply the transformation matrix to the local offset and return the resulting world coordinates.
            return new()
            {
                X = (offset.X * matrix.M11) + (offset.Y * matrix.M21) + (offset.Z * matrix.M31) + matrix.M41,
                Y = (offset.X * matrix.M12) + (offset.Y * matrix.M22) + (offset.Z * matrix.M32) + matrix.M42,
                Z = (offset.X * matrix.M13) + (offset.Y * matrix.M23) + (offset.Z * matrix.M33) + matrix.M43
            };
        }
        #endregion
    }
}