using CitizenFX.Core;
using CitizenFX.Core.Native;
using Common.Client.Models;
using Button = Common.Client.Models.Button;
using static CitizenFX.Core.Native.API;

namespace Common.Client
{
    public class Controls : ClientScript
    {
        /// <summary>
        /// Determines if the control was just pressed.
        /// </summary>
        /// <param name="control">The control that is specified</param>
        /// <param name="inputGroup">The input group that is mouse & keyboard or controller.</param>
        /// <returns>The enabled key that is pressed regardless of being disabled.</returns>
        public static bool IsControlJustPressed(Control control, int inputGroup = 0) => Game.IsControlJustPressed(inputGroup, control) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlJustPressed(Key key, int inputGroup = 0) => Game.IsControlJustPressed(inputGroup, (Control)key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlJustPressed(Button button, int inputGroup = 0) => Game.IsControlJustPressed(inputGroup, (Control)button) && Game.CurrentInputMode == InputMode.GamePad && UpdateOnscreenKeyboard() != 0;

        /// <summary>
        /// Determines if the control was pressed regardless of being disabled.
        /// </summary>
        /// <param name="control">The control that is specified.</param>
        /// <param name="inputGroup">The input group that is mouse & keyboard or controller.</param>
        /// <returns>The enabled key that is pressed regardless of being disabled.</returns>
        public static bool IsControlJustPressedRegardless(Control control, int inputGroup = 0) => Game.IsControlPressed(inputGroup, control) || Game.IsDisabledControlPressed(inputGroup, control) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlJustPressedRegardless(Key key, int inputGroup = 0) => Game.IsControlPressed(inputGroup, (Control)key) || Game.IsDisabledControlPressed(inputGroup, (Control)key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlJustPressedRegardless(Button button, int inputGroup = 0) => Game.IsControlPressed(inputGroup, (Control)button) || Game.IsDisabledControlPressed(inputGroup, (Control)button) && Game.CurrentInputMode == InputMode.GamePad && UpdateOnscreenKeyboard() != 0;

        /// <summary>
        /// Determines if the control was just pressed.
        /// </summary>
        /// <param name="control">The control that is specified.</param>
        /// <param name="inputGroup">The input group that is mouse & keyboard or controller.</param>
        /// <returns>The enabled key that is pressed.</returns>
        public static bool IsControlPressed(Control control, int inputGroup = 0) => Game.IsControlPressed(inputGroup, control) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlPressed(Key key, int inputGroup = 0) => Game.IsControlPressed(inputGroup, (Control)key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlPressed(Button button, int inputGroup = 2) => Game.IsControlPressed(inputGroup, (Control)button) && Game.CurrentInputMode == InputMode.GamePad && UpdateOnscreenKeyboard() != 0;

        /// <summary>
        /// Determines if the control was pressed regardless of being disabled.
        /// </summary>
        /// <param name="control">The control that is specified.</param>
        /// <param name="inputGroup">The input group that is mouse & keyboard or controller.</param>
        /// <returns>The enabled key that is pressed regardless of being disabled.</returns>
        public static bool IsControlPressedRegardless(Control control, int inputGroup = 0) => Game.IsControlPressed(inputGroup, control) || Game.IsDisabledControlPressed(inputGroup, control) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlPressedRegardless(Key key) => Game.IsControlPressed(0, (Control)key) || Game.IsDisabledControlPressed(0, (Control)key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlPressedRegardless(Button button) => Game.IsControlPressed(2, (Control)button) || Game.IsDisabledControlPressed(2, (Control)button) && Game.CurrentInputMode == InputMode.GamePad && UpdateOnscreenKeyboard() != 0;

        /// <summary>
        /// Checks if a disabled control was just released.
        /// </summary>
        /// <param name="control">The control that is specified.</param>
        /// <param name="padIndex">The input group that is mouse & keyboard or controller.</param>
        /// <returns>The disabled control just released value.</returns>
        public static bool IsDisabledControlJustReleased(Control control, int inputGroup = 0) => Game.IsDisabledControlJustReleased(inputGroup, control) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsDisabledControlJustReleased(Key key) => Game.IsDisabledControlJustReleased(0, (Control)key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsDisabledControlJustReleased(Button button) => Game.IsDisabledControlJustReleased(2, (Control)button) && Game.CurrentInputMode == InputMode.GamePad && UpdateOnscreenKeyboard() != 0;


        /// <summary>
        /// Determines if a disable control was just pressed
        /// </summary>
        /// <param name="control">The control that is specified.</param>
        /// <param name="inputGroup">The input group that is mouse & keyboard or controller.</param>
        /// <returns>The disabled key that is just pressed.</returns>
        public static bool IsDisabledControlJustPressed(Control control, int inputGroup = 0) => Game.IsDisabledControlJustPressed(inputGroup, control) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsDisabledControlJustPressed(Key key, int inputGroup = 0) => Game.IsDisabledControlJustPressed(inputGroup, (Control)key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsDisabledControlJustPressed(Button button, int inputGroup = 2) => Game.IsDisabledControlJustPressed(inputGroup, (Control)button) && Game.CurrentInputMode == InputMode.GamePad && UpdateOnscreenKeyboard() != 0;

        /// <summary>
        /// Checks if a disabled control was just pressed.
        /// </summary>
        /// <param name="control">The control that is specified.</param>
        /// <param name="padIndex">The input group that is mouse & keyboard or controller.</param>
        /// <returns>The disabled control just pressed value.</returns>
        public static bool IsDisabledControlPressed(Control control, int inputGroup = 0) => Game.IsDisabledControlPressed(inputGroup, control) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsDisabledControlPressed(Key key, int inputGroup = 0) => Game.IsDisabledControlPressed(inputGroup, (Control)key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsDisabledControlPressed(Button button, int inputGroup = 2) => Game.IsDisabledControlPressed(inputGroup, (Control)button) && Game.CurrentInputMode == InputMode.GamePad && UpdateOnscreenKeyboard() != 0;

        /// <summary>
        /// Determines if the control was just released
        /// </summary>
        /// <param name="control">The control that is specified.</param>
        /// <param name="inputGroup">The input group that is mouse & keyboard or controller.</param>
        /// <returns>The enabled key that is released.</returns>
        public static bool IsControlJustReleased(Control control, int inputGroup = 0) => Game.IsControlJustReleased(inputGroup, control) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlJustReleased(Key key) => Game.IsControlJustReleased(0, (Control)key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlJustReleased(Button button) => Game.IsControlJustReleased(2, (Control)button) && Game.CurrentInputMode == InputMode.GamePad && UpdateOnscreenKeyboard() != 0;

        /// <summary>
        /// Determines if the control was released.
        /// </summary>
        /// <param name="control">The control that is specified.</param>
        /// <param name="inputGroup">The input group that is mouse & keyboard or controller.</param>
        /// <returns>The enabled key that is released.</returns>
        public static bool IsControlReleased(Control control, int inputGroup = 0) => Game.IsControlJustReleased(inputGroup, control) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        /// <summary>
        /// Determines if the key was released.
        /// </summary>
        /// <param name="key">The key that is specified.</param>
        /// <returns>The enabled key that is released.</returns>
        public static bool IsControlReleased(Key key) => Game.IsControlJustReleased(0, (Control)key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        /// <summary>
        /// Determines if the button was released.
        /// </summary>
        /// <param name="button">The button that is specified.</param>
        /// <returns>The enabled button that is released.</returns>
        public static bool IsControlReleased(Button button) => Game.IsControlJustReleased(2, (Control)button) && Game.CurrentInputMode == InputMode.GamePad && UpdateOnscreenKeyboard() != 0;

        /// <summary>
        /// Determines if the control was released.
        /// </summary>
        /// <param name="control">The control that is specified.</param>
        /// <param name="inputGroup">The input group that is mouse & keyboard or controller.</param>
        /// <returns>The enabled key that is held.</returns>
        public static bool IsControlHeld(Control control, int inputGroup = 0) => Game.IsControlPressed(inputGroup, control) && UpdateOnscreenKeyboard() != 0;

        /// <summary>
        /// Determines if the key was held.
        /// </summary>
        /// <param name="key">The key that is specified.</param>
        /// <returns>The enabled key that is held.</returns>
        public static bool IsControlHeld(Key key) => Game.IsControlPressed(0, (Control)key) && UpdateOnscreenKeyboard() != 0;

        /// <summary>
        /// Determines if the button was held.
        /// </summary>
        /// <param name="button">The button that is specified.</param>
        /// <returns>The enabled button that is held.</returns>
        public static bool IsControlHeld(Button button) => Game.IsControlPressed(2, (Control)button) && UpdateOnscreenKeyboard() != 0;

        /// <summary>
        /// Disables a certain key for a frame.
        /// </summary>
        /// <param name="control">The control to disable</param>
        /// <param name="index">The input group that is mouse & keyboard or controller.</param>
        public static void DisableControlThisFrame(Control control, int index = 0) => Game.DisableControlThisFrame(index, control);

        public static void DisableControlThisFrame(Key key) => Game.DisableControlThisFrame(0, (Control)key);

        public static void DisableControlThisFrame(Button button) => Game.DisableControlThisFrame(2, (Control)button);

        /// <summary>
        /// This is for simulating player input.
        /// </summary>
        /// <param name="control">The control ID to check.</param>
        /// <param name="amount">An unbounded normal value.</param>
        /// <param name="index">The control system instance to use. (Mouse and keyboard or controller)</param>
        public static void SetControlNormal(Control control, float amount, int index = 0) => Game.SetControlNormal(index, control, amount);

        public static void SetControlNormal(Key key, float amount) => SetControlNormal((Control)key, amount, 0);

        public static void SetControlNormal(Button button, float amount) => SetControlNormal((Control)button, amount, 2);

        /// <summary>
        /// Gets an value from a control input.
        /// </summary>
        /// <param name="control">The control ID to check.></param>
        /// <param name="index">The control system instance to use.</param>
        /// <returns></returns>
        public static int GetControlValue(Control control, int index = 0) => Game.GetControlValue(index, control);

        public static int GetControlValue(Key key) => GetControlValue((Control)key, 0);

        public static int GetControlValue(Button button) => GetControlValue((Control)button, 2);

        /// <summary>
        /// This is for disabling a control action.
        /// </summary>
        /// <param name="control">The control to disable.</param>
        /// <param name="disable">Toggle wheather the control action is disabled.</param>
        public static void DisableControlAction(Control control, bool disable = true) => API.DisableControlAction(0, (int)control, disable);

        /// <summary>
        /// This is for disabling a key action.
        /// </summary>
        /// <param name="control">The key to disable.</param>
        /// <param name="disable">Toggle wheather the key action is disabled.</param>
        public static void DisableControlAction(Key key, bool disable = true) => DisableControlAction((Control)key, disable);

        /// <summary>
        /// This is for disabling a button action.
        /// </summary>
        /// <param name="control">The button to disable.</param>
        /// <param name="disable">Toggle wheather the control action is disabled.</param>
        public static void DisableControlAction(Button button, bool disable = true) => DisableControlAction((Control)button, disable);

        /// <summary>
        /// Enables a certain control for a frame.
        /// </summary>
        /// <param name="control">The control ID to check.</param>
        /// <param name="index">The control system instance to use.</param>
        public static void EnableControlThisFrame(Control control, int index = 0) => Game.EnableControlThisFrame(index, control);

        /// <summary>
        /// Enables a certain key for a frame.
        /// </summary>
        /// <param name="key">The key ID to check.</param>
        /// <param name="index">The control system instance to use.</param>
        public static void EnableControlThisFrame(Key key, int index = 0) => Game.EnableControlThisFrame(index, (Control)key);

        /// <summary>
        /// Enables a certain button for a frame.
        /// </summary>
        /// <param name="button">The button ID to check.</param>
        /// <param name="index">The control system instance to use.</param>
        public static void EnableControlThisFrame(Button button, int index = 0) => Game.EnableControlThisFrame(index, (Control)button);
    }
}