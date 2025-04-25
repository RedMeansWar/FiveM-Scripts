using CitizenFX.Core;

namespace Common.Client.Models
{
    public class Wheel
    {
        public Vector3 BonePosition { get; set; }
        public string BoneName { get; set; }
        public int WheelIndex { get; set; }
        public float Distance { get; set; }

        public Wheel() { }

        public Wheel(int inIndex, string inName)
        {
            WheelIndex = inIndex;
            BoneName = inName;
        }

        public string GetBoneName()
        {
            return BoneName;
        }

        public int GetIndex()
        {
            return WheelIndex;
        }
    }
}