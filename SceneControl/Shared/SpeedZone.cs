using CitizenFX.Core;

namespace SceneControl
{
    public class SpeedZone
    {
        public Vector3 Position { get; set; }
        public int Radius { get; set; }
        public float Speed { get; set; }
        public int Blip { get; set; }
        public int Zone { get; set; }
        public int PlayerId { get; set; }
    }
}