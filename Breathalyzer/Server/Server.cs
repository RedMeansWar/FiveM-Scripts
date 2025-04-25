using System;
using CitizenFX.Core;

namespace Breathalyzer.Server
{
    public class Server : BaseScript
    {
        #region Event Handlers
        [EventHandler("Breathalyzer:Server:SubmitTest")]
        private void OnSubmitTest([FromSource] Player testerPlayer, int testedId)
        {
            Player testedPlayer = Players[testedId];
            testedPlayer?.TriggerEvent("Breathalyzer:Client:SubmitTest", testedPlayer.Handle);
        }

        [EventHandler("Breathalyzer:Server:ReturnTest")]
        private void OnReturnTest(string testerId, string bacLevel)
        {
            Player testerPlayer = Players[int.Parse(testerId)];
            testerPlayer?.TriggerEvent("Breathalyzer:Client:ReturnLevel");
        }
        #endregion
    }
}
