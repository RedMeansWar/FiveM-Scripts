using CitizenFX.Core;

namespace Gsr.Server
{
    public class Server : BaseScript
    {
        [EventHandler("Gsr:Server:SubmitTest")]
        private void OnSubmitTest([FromSource] Player testerPlayer, int testedId)
        {
            Player testedPlayer = Players[testedId];
            testedPlayer?.TriggerEvent("Gsr:Client:SubmitTest", testedPlayer.Handle);
        }

        [EventHandler("Gsr:Server:ReturnTest")]
        private void OnReturnTest(bool shotRecently, string testedId)
        {
            Player testedPlayer = Players[int.Parse(testedId)];
            testedPlayer?.TriggerEvent("Gsr:Client:Notify", shotRecently);
        }
    }
}
