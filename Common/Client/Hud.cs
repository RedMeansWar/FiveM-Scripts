using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using Common.Client.Models;
using static CitizenFX.Core.Native.API;

namespace Common.Client
{
    public class Hud : BaseScript
    {
        #region Booleans
        /// <summary>
        /// Determines if the HUD is hidden.
        /// </summary>
        public static bool IsHudHidden => IsHudHidden();

        /// <summary>
        /// Determines if the Radar is hidden.
        /// </summary>
        public static bool IsRadarHidden => IsRadarHidden();

        /// <summary>
        /// Determines if the HUD is visable.
        /// </summary>
        public static bool IsHudVisible => Screen.Hud.IsVisible;

        /// <summary>
        /// Determines if the Radar is visable.
        /// </summary>
        public static bool IsRadarVisible => Screen.Hud.IsRadarVisible;
        #endregion

        #region HUD Display
        /// <summary>
        /// Hides the HUD and radar. This method checks if the HUD is already hidden 
        /// to prevent redundant calls and ensures the HUD and radar are properly hidden.
        /// </summary>
        public static void Hide()
        {
            if (!IsHudHidden)
            {
                DisplayHud(false);
                DisplayRadar(false);
            }
        }

        /// <summary>
        /// Shows the HUD and radar. This method checks if the HUD is currently hidden 
        /// to avoid unnecessary actions and restores the HUD and radar to their visible state.
        /// </summary>
        public static void Show()
        {
            if (IsHudHidden)
            {
                DisplayHud(true);
                DisplayRadar(true);
            }
        }
        #endregion

        #region Notifications
        /// <summary>
        /// Draws a notification, shorten down version of Screen.ShowNotification();
        /// </summary>
        /// <param name="message"></param>
        /// <param name="blink"></param>
        /// <returns></returns>
        public static string DisplayNotification(string message, bool blink = false)
        {
            Screen.ShowNotification(message, blink);
            return message;
        }

        /// <summary>
        /// Draws an image notification
        /// </summary>
        /// <param name="picture">The picture to draw.</param>
        /// <param name="icon">The icon of the notification.</param>
        /// <param name="title">The title of the notification.</param>
        /// <param name="subtitle">The subtitle of the notification.</param>
        /// <param name="message">The text of the notification.</param>
        public static void DrawImageNotification(string picture, int icon, string title, string subtitle, string message)
        {
            // Set the notification text entry mode to string.
            SetNotificationTextEntry("STRING");

            // Add the message text to the notification.
            AddTextComponentString(message);

            // Construct the notification with image and text elements.
            SetNotificationMessage(picture, picture, true, icon, title, subtitle);
        }

        /// <summary>
        /// Draws an image notification
        /// </summary>
        /// <param name="textureDict">The texture dictionary of the image to draw.</param>
        /// <param name="textureName">The texture name of the image to draw.</param>
        /// <param name="icon">The icon of the notification.</param>
        /// <param name="title">The title of the notification.</param>
        /// <param name="subtitle">The subtitle of the notification.</param>
        /// <param name="message">The text of the notification.</param>
        public static void DrawImageNotification(string textureDict, string textureName, int icon, string message, string title, string subtitle)
        {
            // Set the notification text entry mode to string.
            SetNotificationTextEntry("STRING");

            // Add the message text to the notification.
            AddTextComponentString(message);

            // Construct the notification with image and text elements.
            SetNotificationMessage(textureDict, textureName, true, icon, title, subtitle);
        }
        #endregion

        #region Rectangles
        /// <summary>
        /// Draws a rectangle on the screen that keeps its position when adjusting the safezone size or aspect ratio
        /// </summary>
        /// <param name="x">The X coordinate for the rectangle on the screen.</param>
        /// <param name="y">The Y coordinate for the rectangle on the screen.</param>
        /// <param name="width">The width of the rectangle on screen.</param>
        /// <param name="height">The height of the rectangle on screen.</param>
        /// <param name="r">The red component of the rectangle color (0-255). Defaults to 255.</param>
        /// <param name="g">The green component of the rectangle color (0-255). Defaults to 255.</param>
        /// <param name="b">The blue component of the rectangle color (0-255). Defaults to 255.</param>
        /// <param name="a">The alpha (transparency) component of the rectangle color (0-255). Defaults to 255 (fully opaque).</para
        public static void DrawRectangle(float x, float y, float width, float height, int r = 255, int g = 255, int b = 255, int a = 255)
        {
            // Fetch anchor for relative rectangle positioning
            Minimap anchor = Minimap.GetMinimapAnchor();

            // Draw rectangle with position based off player's anchor.
            DrawRect(anchor.LeftX + x + (width / 2), anchor.BottomY - y + (height / 2), width, height, r, g, b, a);
        }

        /// <summary>
        /// Draws a rectangle on the screen that keeps its position when adjusting the safezone size or aspect ratio
        /// </summary>
        /// <param name="x">The X coordinate for the rectangle on the screen.</param>
        /// <param name="y">The Y coordinate for the rectangle on the screen.</param>
        /// <param name="width">The width of the rectangle on screen.</param>
        /// <param name="height">The height of the rectangle on screen.</param>
        /// <param name="a">The alpha (transparency) component of the rectangle color (0-255). Defaults to 255 (fully opaque).</para
        public static void DrawRectangle(float x, float y, float width, float height, int a) => DrawRectangle(x, y, width, height, 255, 255, 255, a);
        #endregion

        #region Text
        /// <summary>
        /// Draws text on the screen in 2D that adjusts based on the aspect ratio and safezone settings.
        /// </summary>
        /// <param name="x">The X coordinate for the text on the screen.</param>
        /// <param name="y">The Y coordinate for the text on the screen.</param>
        /// <param name="size">The size of the text.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="r">The red component of the text color (0-255). Defaults to 255.</param>
        /// <param name="g">The green component of the text color (0-255). Defaults to 255.</param>
        /// <param name="b">The blue component of the text color (0-255). Defaults to 255.</param>
        /// <param name="a">The alpha (transparency) component of the text color (0-255). Defaults to 255 (fully opaque).</param>
        /// <param name="alignment">The alignment of the text (left, center, right). Defaults to left.</param>
        public static void DrawText2d(float x, float y, float size, string text, int r = 255, int g = 255, int b = 255, int a = 255, Alignment alignment = Alignment.Left)
        {
            // Fetch the minimap anchor for relative text positioning
            Minimap anchor = Minimap.GetMinimapAnchor();

            // Calculate screen coordinates based on the anchor's position, width, and height
            x = anchor.X + anchor.Width * x;
            y = anchor.Y - y; // Invert y-axis for screen drawing

            // Set the text font. Font ID 4 is the default. Other font IDs can be used for different styles.
            SetTextFont(4); // Font ID: 4 (adjustable)
            SetTextScale(size, size); // Uniform scaling for both horizontal and vertical dimensions

            // Set the color and transparency of the text
            SetTextColour(r, g, b, a);
            SetTextDropShadow(); // Add a drop shadow for better readability
            SetTextOutline(); // Outline the text for better visibility

            // Handle text alignment
            if (alignment == Alignment.Right)
            {
                // Right-align text, wrapping at the specified HUD coordinates
                SetTextWrap(0, x);
                SetTextJustification(2); // Right justification
            }
            else
            {
                // Center or left-align text based on the alignment parameter
                SetTextJustification(alignment == Alignment.Center ? 0 : 1);
            }

            // Prepare the text for rendering on the screen
            SetTextEntry("STRING");
            AddTextComponentString(text);

            // Draw the text at the calculated anchor coordinates
            DrawText(x, y);
        }

        /// <summary>
        /// Draws text on the screen in 2D that adjusts based on the aspect ratio and safezone settings.
        /// </summary>
        /// <param name="x">The X coordinate for the text on the screen.</param>
        /// <param name="y">The Y coordinate for the text on the screen.</param>
        /// <param name="size">The size of the text.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="r">The red component of the text color (0-255). Defaults to 255.</param>
        /// <param name="g">The green component of the text color (0-255). Defaults to 255.</param>
        /// <param name="b">The blue component of the text color (0-255). Defaults to 255.</param>
        /// <param name="a">The alpha (transparency) component of the text color (0-255). Defaults to 255 (fully opaque).</param>
        public static void DrawText2d(float x, float y, float size, string text, int r, int g, int b, int a) => DrawText2d(x, y, size, text, r, g, b, a, Alignment.Left);

        /// <summary>
        /// Draws text on the screen in 2D that adjusts based on the aspect ratio and safezone settings.
        /// </summary>
        /// <param name="x">The X coordinate for the text on the screen.</param>
        /// <param name="y">The Y coordinate for the text on the screen.</param>
        /// <param name="size">The size of the text.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="r">The red component of the text color (0-255). Defaults to 255.</param>
        /// <param name="g">The green component of the text color (0-255). Defaults to 255.</param>
        /// <param name="b">The blue component of the text color (0-255). Defaults to 255.</param>
        public static void DrawText2d(float x, float y, float size, string text, int r, int g, int b, Alignment alignment) => DrawText2d(x, y, size, text, r, g, b, 255, alignment);

        /// <summary>
        /// Draws text on the screen in 2D that adjusts based on the aspect ratio and safezone settings.
        /// </summary>
        /// <param name="x">The X coordinate for the text on the screen.</param>
        /// <param name="y">The Y coordinate for the text on the screen.</param>
        /// <param name="size">The size of the text.</param>
        /// <param name="text">The text to display.</param>
        public static void DrawText2d(float x, float y, float size, string text) => DrawText2d(x, y, size, text, 255, 255, 255, 255, Alignment.Left);

        /// <summary>
        /// Draws text on the screen in 2D that adjusts based on the aspect ratio and safezone settings.
        /// </summary>
        /// <param name="x">The X coordinate for the text on the screen.</param>
        /// <param name="y">The Y coordinate for the text on the screen.</param>
        /// <param name="a">The alpha (transparency) component of the text color (0-255).</param>
        /// <param name="size">The size of the text.</param>
        /// <param name="text">The text to display.</param>
        public static void DrawText2d(float x, float y, float size, string text, int a) => DrawText2d(x, y, size, text, 255, 255, 255, a, Alignment.Left);

        /// <summary>
        /// Draws text on the screen in 2D that adjusts based on the aspect ratio and safezone settings.
        /// </summary>
        /// <param name="x">The X coordinate for the text on the screen.</param>
        /// <param name="y">The Y coordinate for the text on the screen.</param>
        /// <param name="size">The size of the text.</param>
        /// <param name="text">The text to display.</param>'
        /// <param name="alignment">The alignment of the text (left, center, right).</param>
        public static void DrawText2d(float x, float y, float size, string text, Alignment alignment) => DrawText2d(x, y, size, text, 255, 255, 255, 255, alignment);

        /// <summary>
        /// Renders 3D text in the game world, visible to all clients.
        /// </summary>
        /// <param name="x">The X coordinate for the text in the game world.</param>
        /// <param name="y">The Y coordinate for the text in the game world.</param>
        /// <param name="z">The Z coordinate for the text in the game world.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="size">The size of the text.</param>
        /// <param name="radius">The maximum distance from which the text is visible.</param>
        /// <param name="r">The red component of the text color (0-255).</param>
        /// <param name="g">The green component of the text color (0-255).</param>
        /// <param name="b">The blue component of the text color (0-255).</param>
        /// <param name="a">The alpha (transparency) component of the text color (0-255). Defaults to 255 (fully opaque).</param>
        public static void DrawText3d(float x, float y, float z, string text, float size, float radius, int r, int g, int b, int a = 255)
        {
            // Get the player's current position.
            Vector3 PlayerPos = Game.PlayerPed.Position;

            float screenXPos = 0.0f; // Default screen X position for text.
            float screenYPos = 0.0f; // Default screen Y position for text.

            // Check if the distance between the player and the text position is within the specified radius.
            if (GetDistanceBetweenCoords(x, y, z, PlayerPos.X, PlayerPos.Y, PlayerPos.Z, true) < radius)
            {
                // Convert the 3D world coordinates to 2D screen coordinates.
                World3dToScreen2d(x, y, z, ref screenXPos, ref screenYPos);

                // Set text rendering properties.
                SetTextScale(0.0f, size); // Set the scale of the text.
                SetTextFont(0); // Set the font of the text (0 is the default font).
                SetTextColour(r, g, b, a); // Set the color and transparency of the text.
                SetTextDropshadow(0, 0, 0, 0, 255); // Set a black drop shadow for the text.
                SetTextOutline(); // Enable text outline.

                // Prepare the text for rendering.
                SetTextEntry("STRING");
                SetTextCentre(true); // Center the text.
                AddTextComponentString(text); // Add the text string.

                // Draw the text on the screen.
                DrawText(screenXPos, screenYPos);
                // Draw a black rectangle behind the text to improve readability.
                DrawRect(screenXPos, screenYPos + 0.125f, (float)text.Length / 300, 0.03f, 23, 23, 23, 70);
            }
        }

        /// <summary>
        /// Renders 3D text in the game world, visible to all clients. Based on a script by JoeyTheDev
        /// </summary>
        /// <param name="x">The X coordinate for the text in the game world.</param>
        /// <param name="y">The Y coordinate for the text in the game world.</param>
        /// <param name="z">The Z coordinate for the text in the game world.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="size">The size of the text.</param>
        /// <param name="radius">The maximum distance from which the text is visible.</param>
        /// <param name="hasRectangle">Whether to display a rectangle behind the text for better readability.</param>
        /// <param name="r">The red component of the text color (0-255).</param>
        /// <param name="g">The green component of the text color (0-255).</param>
        /// <param name="b">The blue component of the text color (0-255).</param>
        /// <param name="a">The alpha (transparency) component of the text color (0-255). Defaults to 255 (fully opaque).</param>
        public static void DrawText3d(float x, float y, float z, string text, float size, float radius, bool hasRectangle = true, int r = 255, int g = 255, int b = 255, int a = 255)
        {
            // Get the player's current position.
            Vector3 PlayerPos = Game.PlayerPed.Position;

            float screenXPos = 0.0f; // Default screen X position for text.
            float screenYPos = 0.0f; // Default screen Y position for text.

            // Check if the distance between the player and the text position is within the specified radius.
            if (GetDistanceBetweenCoords(x, y, z, PlayerPos.X, PlayerPos.Y, PlayerPos.Z, true) < radius)
            {
                // Convert the 3D world coordinates to 2D screen coordinates.
                World3dToScreen2d(x, y, z, ref screenXPos, ref screenYPos);

                // Set text rendering properties.
                SetTextScale(0.0f, size); // Set the scale of the text.
                SetTextFont(0); // Set the font of the text (0 is the default font).
                SetTextColour(r, g, b, a); // Set the color and transparency of the text.
                SetTextDropshadow(0, 0, 0, 0, 255); // Set a black drop shadow for the text.
                SetTextOutline(); // Enable text outline.

                // Prepare the text for rendering.
                SetTextEntry("STRING");
                SetTextCentre(true); // Center the text.
                AddTextComponentString(text); // Add the text string.

                // Draw the text on the screen.
                DrawText(screenXPos, screenYPos);

                // Optionally draw a black rectangle behind the text for better readability.
                if (hasRectangle)
                {
                    DrawRect(screenXPos, screenYPos + 0.125f, (float)text.Length / 300, 0.03f, 23, 23, 23, 70);
                }
            }
        }

        /// <summary>
        /// Draws text in the top left corner
        /// </summary>
        /// <param name="text">The text to be drawn in the help box.</param>
        public static void DisplayHelpText(string text) => Screen.DisplayHelpTextThisFrame(text);

        /// <summary>
        /// Draws text in the top left corner
        /// </summary>
        /// <param name="text">The text to be drawn in the help box.</param>
        public static void DrawHelpText(string text) => Screen.DisplayHelpTextThisFrame(text);

        /// <summary>
        /// Draws/Displays a subtitle on the screen.
        /// </summary>
        /// <param name="text">The text that will appear on the subtitle.</param>
        /// <param name="duration">The duration that subtitle will show (in milliseconds).</param>
        public static void DisplaySubtitle(string text, int duration = 255) => Screen.ShowSubtitle(text, duration);

        /// <summary>
        /// Draws/Displays a subtitle on the screen.
        /// </summary>
        /// <param name="text">The text that will appear on the subtitle.</param>
        /// <param name="duration">The duration that subtitle will show (in milliseconds).</param>
        public static void DrawSubtitle(string text, int duration = 255) => Screen.ShowSubtitle(text, duration);

        /// <summary>
        /// Draws/Displays a subtitle on the screen.
        /// </summary>
        /// <param name="text">The text that will appear on the subtitle.</param>
        /// <param name="duration">The duration that subtitle will show (in milliseconds).</param>
        public static void ShowSubtitle(string text, int duration = 255) => Screen.ShowSubtitle(text, duration);

        /// <summary>
        /// Displays a dynamic subtitle prompting the user to hold for a specific duration.
        /// </summary>
        /// <param name="startTime">
        /// The initial game time when the countdown starts. This will be set to the current game time.
        /// </param>
        /// <param name="delay">
        /// The duration, in milliseconds, for which the subtitle will display the countdown.
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> that completes after the delay.
        /// </returns>
        /// <remarks>
        /// This method displays a subtitle in the format "Hold for X more second(s)", where X is a countdown 
        /// updated in real-time. It uses <see cref="Screen.ShowSubtitle"/> for rendering and awaits a delay of 0ms for smooth updates.
        /// </remarks>
        public static async Task ShowHoldForSubtitle(int startTime, int delay)
        {
            // Set the start time to the current game time
            startTime = Game.GameTime;

            // Continue showing the subtitle until the specified delay has elapsed
            while (Game.GameTime - startTime < delay)
            {
                // Calculate the remaining time in seconds and display the subtitle
                Screen.ShowSubtitle($"Hold for ~r~{Math.Ceiling((double)(startTime + delay - Game.GameTime) / 1000)} ~s~more second(s)", 110);

                // Wait for the next frame (0ms delay)
                await Delay(0);
            }

            // Optional additional delay at the end (can be removed if unnecessary)
            await Delay(0);
        }
        #endregion

        #region Miscellaneous HUD Methods
        /// <summary>
        /// Gets the screen resolution based on the aspect ratio.
        /// </summary>
        /// <returns>
        /// A dynamic object containing the calculated width and height.
        /// </returns>
        /// <remarks>
        /// This method assumes a base resolution of 1920x1080 and adjusts the width and height
        /// according to the current aspect ratio of the screen.
        /// </remarks>
        public static dynamic GetResolution()
        {
            // Calculate the width and height based on the aspect ratio
            double aspectRatio = GetAspectRatio(false);
            dynamic resolution = new
            {
                width = 1920.0 * aspectRatio, // Calculate width based on a base 1920x1080 resolution
                height = 1080.0 // Fixed base height of 1080
            };

            return resolution;
        }

        /// <summary>
        /// Gets the safezone of the player
        /// </summary>
        /// <returns>The safezone of the player assuming the resolution is 1920x1080 and the aspect ratio is 16:9</returns>
        public static dynamic GetSafeZone()
        {
            // Calculate the safezone size based on an internal safezone value.
            float size = 10 - ((float)Math.Round(GetSafeZone(), 2) * 100) - 90;

            // Initialize the safezone with base coordinates based on aspect ratio.
            dynamic safeZone = new
            {
                X = (int)Math.Round(size * (GetAspectRatio(false) * 5.4)),
                Y = (int)Math.Round(size * 5.4)
            };

            // Retrieve the actual screen resolution
            int screenWidth = 0, screenHeight = 0;
            GetScreenResolution(ref screenWidth, ref screenHeight);

            // Adjust the X coordinate for wider resolutions.
            if (screenWidth > 1920)
            {
                safeZone.X += (screenWidth - 1920) / 2;
            }

            return safeZone;
        }

        /// <summary>
        /// Gets the control name for the given control ID, or returns a placeholder for unknown inputs.
        /// </summary>
        /// <param name="controlId">The ID of the control to retrieve the name for.</param>
        /// <returns>
        /// A formatted string representing the control name if it exists, or "UNKNOWN_INPUT" if it is undefined.
        /// </returns>
        public static string GetControlContext(int controlId)
        {
            // Check if the controlId corresponds to a defined enum value in InputAction
            if (Enum.IsDefined(typeof(InputAction), controlId))
            {
#if DEBUG
                // Log debug information only in debug builds
                int totalControls = Enum.GetValues(typeof(InputAction)).Length;
                Log.InfoOrError($"Loaded {totalControls} Control Name(s) and found {controlId}");
#endif
                // Return the control name in a formatted string
                return $"~{(InputAction)controlId}~";
            }
            else
            {
                // Return a formatted string indicating the input is unknown
                return $"~r~UNKNOWN_INPUT~w~";
            }
        }


        /// <summary>
        /// Retrieves the context string for a specified control ID.
        /// </summary>
        /// <param name="controlId">The ID of the control to get the context for.</param>
        /// <returns>
        /// A formatted string representing the control's context if the control ID is valid; 
        /// otherwise, a string indicating an unknown input.
        /// </returns>
        public static string GetControlContext(InputAction action) => GetControlContext((int)action);
        #endregion

        #region Chat
        /// <summary>
        /// Sends a chat message to the in-game chat system with customizable text color and author name.
        /// This method triggers the "chat:addMessage" event to display the message in the chat window.
        /// </summary>
        /// <param name="message">The message content to display in the chat.</param>
        /// <param name="author">The name of the author sending the message. Default is "SYSTEM".</param>
        /// <param name="r">The red component of the message color. Default is 255 (full red).</param>
        /// <param name="g">The green component of the message color. Default is 255 (full green).</param>
        /// <param name="b">The blue component of the message color. Default is 255 (full blue).</param>
        public static void DisplayChatMessage(string message, string author = "SYSTEM", int r = 255, int g = 255, int b = 255)
            => TriggerEvent("chat:addMessage", new { color = new[] { r, g, b }, multiline = true, args = new[] { author, message } });

        /// <summary>
        /// Sends a chat message to the in-game chat system, using the same functionality as <see cref="DisplayChatMessage"/>.
        /// This method is a wrapper for convenience, passing the same parameters to <see cref="DisplayChatMessage"/>.
        /// </summary>
        /// <param name="message">The message content to display in the chat.</param>
        /// <param name="author">The name of the author sending the message. Default is "SYSTEM".</param>
        /// <param name="r">The red component of the message color. Default is 255 (full red).</param>
        /// <param name="g">The green component of the message color. Default is 255 (full green).</param>
        /// <param name="b">The blue component of the message color. Default is 255 (full blue).</param>
        public static void SendChatMessage(string message, string author = "SYSTEM", int r = 255, int g = 255, int b = 255)
            => DisplayChatMessage(message, author, r, g, b);

        /// <summary>
        /// Sends a framework message to the in-game chat using the specified message type.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="type">
        /// The message type:
        /// 1 = Grey, 2 = Green, 3 = Red, 4 = Blue.
        /// </param>
        public static void SendFrameworkMessage(string message, int type = 1)
        {
            // Define a dictionary for message types and their corresponding template IDs
            var templates = new Dictionary<int, string>
            {
                { 1, "TemplateGrey" },
                { 2, "TemplateGreen" },
                { 3, "TemplateRed" },
                { 4, "TemplateBlue" }
            };

            // Check if the specified type exists in the dictionary
            if (templates.TryGetValue(type, out string templateId))
            {
                // Trigger the event with the selected template
                TriggerEvent("chat:addMessage", new
                {
                    templateId,
                    color = new[] { 255, 255, 255 },
                    multiline = true,
                    args = new[] { "", $"{message}" }
                });
            }
            else
            {
                // Optionally handle invalid types (e.g., log a warning or do nothing)
                #if DEBUG
                Log.InfoOrError($"Invalid message type '{type}'. Message not sent.");
                #endif
            }
        }

        #endregion

        #region User Input
        /// <summary>
        /// Get a user input text string.
        /// </summary>
        /// <param name="windowTitle">The text for the title of the window.</param>
        /// <param name="defaultText">The text that is already(default) in the window.</param>
        /// <param name="maxInputLength">The max amount of characters allowed in the text box.</param>
        /// <returns>The inputed text that is inside the window.</returns>
        public static async Task<string> GetUserInput(string windowTitle, string defaultText, int maxInputLength)
        {
            // Create the window title with clear formatting
            var spacer = "\t"; // Tab for visual space
            AddTextEntry($"{GetCurrentResourceName().ToUpper()}_WINDOW_TITLE", $"{windowTitle ?? "Enter"}:{spacer}(MAX {maxInputLength} Characters)");

            // Display onscreen keyboard with input constraints based off of what the user wants.
            DisplayOnscreenKeyboard(1, $"{GetCurrentResourceName().ToUpper()}_WINDOW_TITLE", "", defaultText ?? "", "", "", "", maxInputLength);
            await Delay(0); // slightly delay next task. Does this make a difference?

            // Monitor keyboard status until the enter key is pressed
            while (true)
            {
                var keyboardStatus = UpdateOnscreenKeyboard();

                switch (keyboardStatus)
                {
                    case 3: // Input field no longer displayed (unknown cause)
                    case 2: // User cancelled input
                        return null;

                    case 1: // User finished editing
                        return GetOnscreenKeyboardResult(); // Retrieve text
                    default:
                        await Delay(0); // Delay briefly for responsiveness without busy-waiting
                        break;
                }
            }
        }

        /// <summary>
        /// Get a user input text string.
        /// </summary>
        /// <returns>The inputed text that is inside the window.</returns>
        public static async Task<string> GetUserInput() => await GetUserInput(null, null, 30);

        /// <summary>
        /// Get a user input text string.
        /// </summary>
        /// <param name="maxInputLength">The amount of characters allowed in the window.</param>
        /// <returns>The text inputed by the user.</returns>
        public static async Task<string> GetUserInput(int maxInputLength) => await GetUserInput(null, null, maxInputLength);

        /// <summary>
        /// Get a user input text string with a max character count of 30.
        /// </summary>
        /// <param name="windowTitle">The title of the window.</param>
        /// <returns>The text inputed by the user.</returns>
        public static async Task<string> GetUserInput(string windowTitle) => await GetUserInput(windowTitle, null, 30);

        /// <summary>
        /// Get a user input text string.
        /// </summary>
        /// <param name="windowTitle">The title of the window.</param>
        /// <param name="maxInputLength">The max amount of characters allowed in the window.</param>
        /// <returns></returns>
        public static async Task<string> GetUserInput(string windowTitle, int maxInputLength) => await GetUserInput(windowTitle, null, maxInputLength);

        /// <summary>
        /// Get a user input text string.
        /// </summary>
        /// <param name="windowTitle">The title of the window.</param>
        /// <param name="defaultText">The text that is already(default) in the window.</param>
        /// <returns></returns>
        public static async Task<string> GetUserInput(string windowTitle, string defaultText) => await GetUserInput(windowTitle, defaultText, 30);
        #endregion

        #region Sprites
        /// <summary>
        /// Draws a sprite on the minimap at a specified location with the given size, rotation, and color.
        /// </summary>
        /// <param name="x">The X-coordinate of the sprite on the minimap (normalized to the minimap anchor).</param>
        /// <param name="y">The Y-coordinate of the sprite on the minimap (normalized to the minimap anchor).</param>
        /// <param name="width">The width of the sprite.</param>
        /// <param name="height">The height of the sprite.</param>
        /// <param name="heading">The rotation (angle) of the sprite in degrees.</param>
        /// <param name="textureDictionary">The texture dictionary that contains the sprite.</param>
        /// <param name="spriteName">The name of the sprite to be drawn.</param>
        /// <param name="r">The red component of the sprite's color (0-255).</param>
        /// <param name="g">The green component of the sprite's color (0-255).</param>
        /// <param name="b">The blue component of the sprite's color (0-255).</param>
        /// <param name="a">The alpha (transparency) component of the sprite's color (0-255).</param>
        public static void DrawSprite(float x, float y, float width, float height, float heading, string textureDictionary, string spriteName, int r, int g, int b, int a)
        {
            Minimap anchor = Minimap.GetMinimapAnchor();
            x = anchor.X + (anchor.Width * x);
            y = anchor.Y - y;

            API.DrawSprite(textureDictionary, spriteName, x, y, width, height, heading, r, g, b, a);
        }

        /// <summary>
        /// Draws a sprite on the minimap at a specified position and size with the given rotation and color.
        /// </summary>
        /// <param name="position">A Vector2 representing the X and Y coordinates of the sprite on the minimap (normalized to the minimap anchor).</param>
        /// <param name="size">A Vector2 representing the width and height of the sprite.</param>
        /// <param name="heading">The rotation (angle) of the sprite in degrees.</param>
        /// <param name="textureDictionary">The texture dictionary that contains the sprite.</param>
        /// <param name="spriteName">The name of the sprite to be drawn.</param>
        /// <param name="r">The red component of the sprite's color (0-255).</param>
        /// <param name="g">The green component of the sprite's color (0-255).</param>
        /// <param name="b">The blue component of the sprite's color (0-255).</param>
        /// <param name="a">The alpha (transparency) component of the sprite's color (0-255).</param>
        public static void DrawSprite(Vector2 position, Vector2 size, float heading, string textureDictionary, string spriteName, int r, int g, int b, int a)
            => DrawSprite(position.X, position.Y, size.X, size.Y, heading, textureDictionary, spriteName, r, g, b, a);

        /// <summary>
        /// Draws a sprite on the minimap at a specified position and size with the given rotation and color.
        /// The color is provided as a dynamic object containing RGBA properties.
        /// </summary>
        /// <param name="position">A Vector2 representing the X and Y coordinates of the sprite on the minimap (normalized to the minimap anchor).</param>
        /// <param name="size">A Vector2 representing the width and height of the sprite.</param>
        /// <param name="heading">The rotation (angle) of the sprite in degrees.</param>
        /// <param name="textureDictionary">The texture dictionary that contains the sprite.</param>
        /// <param name="spriteName">The name of the sprite to be drawn.</param>
        /// <param name="c">A dynamic object containing the RGBA components of the sprite's color (R, G, B, A).</param>
        public static void DrawSprite(Vector2 position, Vector2 size, float heading, string textureDictionary, string spriteName, dynamic c)
            => DrawSprite(position.X, position.Y, size.X, size.Y, heading, textureDictionary, spriteName, c.R, c.G, c.B, c.A);
        #endregion

        #region Texture Dictionary
        /// <summary>
        /// Draws a texture on the screen.
        /// </summary>
        /// <param name="textureDict"></param>
        public static async void RequestTextureDictionary(string textureDict)
        {
            // Request the streaming of the specified texture dictionary.
            RequestStreamedTextureDict(textureDict, true);

            // Wait for the texture dictionary to load asynchronously.
            while (!HasStreamedTextureDictLoaded(textureDict))
            {
                await Delay(0);
            }
        }

        /// <summary>
        /// Draws a texture on the screen.
        /// </summary>
        /// <param name="textureDict"></param>
        public static async void RequestTextureDict(string textureDict) => RequestTextureDictionary(textureDict);
        #endregion
    }

    public class Notify
    {
        /// <summary>
        /// Gives a success notification using bold, green, and white text.
        /// </summary>
        /// <param name="message">The text that is shown in the notification.</param>
        /// <param name="blink">Toggle whether or not the notification blinks.</param>
        /// <returns>A success notification displayed on the screen.</returns>
        public static string Success(string message, bool blink = false)
        {
            Screen.ShowNotification($"~g~~h~Success~h~~s~: {message}", blink);
            return message;
        }

        /// <summary>
        /// Gives a error notification using bold, red, and white text
        /// </summary>
        /// <param name="message">The text that is shown in the notification.</param>
        /// <param name="blink">Toggle whether or not the notification blinks.</param>
        /// <returns>A error notification displayed on the screen.</returns>
        public static string Error(string message, bool blink = false)
        {
            Screen.ShowNotification($"~r~~h~Error~h~~s~: {message}", blink);
            return message;
        }

        /// <summary>
        /// Gives a alert notification using bold, yello, and white text
        /// </summary>
        /// <param name="message">The text that is shown in the notification.</param>
        /// <param name="blink">Toggle whether or not the notification blinks.</param>
        /// <returns>A alert notification displayed on the screen.</returns>
        public static string Alert(string message, bool blink = false)
        {
            Screen.ShowNotification($"~y~~h~Alert~h~~s~: {message}", blink);
            return message;
        }
    }

    public class Ui
    {
        /// <summary>
        /// Draws a sphere on screen.
        /// </summary>
        /// <param name="pos">The position of the sphere to be placed.</param>
        /// <param name="radius">The radius the sphere displays.</param>
        /// <param name="r">The red component of the sphere color (0-255).</param>
        /// <param name="g">The green component of the sphere color (0-255).</param>
        /// <param name="b">The blue component of the sphere color (0-255).</param>
        /// <param name="a">The alpha (transparency) component of the sphere color (0-255). Defaults to 255 (fully opaque).</param>
        public static void DrawSphere(Vector3 pos, float radius, int r = 255, int g = 255, int b = 255, int a = 255)
        {
            DrawMarker(28, pos.X, pos.Y, pos.Z, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, radius, radius, radius, r, g, b, a, false, false, 2, false, null, null, false);
        }

        /// <summary>
        /// Draws a sphere on screen.
        /// </summary>
        /// <param name="pos">The position of the sphere to be placed.</param>
        /// <param name="radius">The radius the sphere displays.</param>
        /// <param name="r">The red component of the sphere color (0-255).</param>
        /// <param name="g">The green component of the sphere color (0-255).</param>
        /// <param name="b">The blue component of the sphere color (0-255).</param>
        /// <param name="a">The alpha (transparency) component of the sphere color (0-255). Defaults to 255 (fully opaque).</param>
        public static void DrawSphere(Vector3 pos, float radius, int r, int g, int b) => DrawSphere(pos, radius, r, g, b);

        /// <summary>
        /// Draws a sphere on screen.
        /// </summary>
        /// <param name="x">The X coordinate for the sphere to placed.</param>
        /// <param name="y">The Y coordinate for the sphere to placed.</param>
        /// <param name="z">The Z coordinate for the sphere to placed.</param>
        /// <param name="radius">The radius the sphere displays.</param>
        /// <param name="r">The red component of the sphere color (0-255).</param>
        /// <param name="g">The green component of the sphere color (0-255).</param>
        /// <param name="b">The blue component of the sphere color (0-255).</param>
        /// <param name="a">The alpha (transparency) component of the sphere color (0-255). Defaults to 255 (fully opaque).</param>
        public static void DrawSphere(float x, float y, float z, float radius, int r = 255, int g = 255, int b = 255, int a = 255) => DrawSphere(new(x, y, z), radius, r, g, b, a);
    }
}
