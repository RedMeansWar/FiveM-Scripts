using System;
using CitizenFX.Core;

namespace Breathalyzer.Server
{
    public class Server : BaseScript
    {
        #region Event Handlers
        [EventHandler("Breathalyzer:Notes.Server:SubmitTest")]
        private void OnSubmitTest([FromSource] Player testerPlayer, int testedId)
        {
            Player testedPlayer = Players[testedId];
            testedPlayer?.TriggerEvent("Breathalyzer:Notes.Notes.Client:SubmitTest", testedPlayer.Handle);
        }

        [EventHandler("Breathalyzer:Notes.Server:ReturnTest")]
        private void OnReturnTest(string testerId, string bacLevel)
        {
            Player testerPlayer = Players[int.Parse(testerId)];
            testerPlayer?.TriggerEvent("Breathalyzer:Notes.Notes.Client:ReturnLevel");
        }
        #endregion
    }
}
