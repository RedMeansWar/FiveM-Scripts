using System.Collections.Generic;

namespace SceneControl
{
    public static class SceneConstants
    {
        public readonly static List<SceneProp> SceneProps = new()
        {
            new() { DisplayName = "Police Barrier", ModelName = "prop_barrier_work05" },
            new() { DisplayName = "Roadwork Ahead Barrier", ModelName = "prop_mp_barrier_02" },
            new() { DisplayName = "Type III Barrier", ModelName = "prop_mp_barrier_02b" },
            new() { DisplayName = "Small Cone", ModelName = "prop_roadcone02b" },
            new() { DisplayName = "Big Cone", ModelName = "prop_roadcone01a" },
            new() { DisplayName = "Drum Cone", ModelName = "prop_Barrier_wat_03b" },
            new() { DisplayName = "Tent", ModelName = "prop_gazebo_02" },
            new() { DisplayName = "Scene Lights", ModelName = "prop_worklight_03b", HeadingOffset = 180f }
        };

        public readonly static List<int> SpeedZoneRadiuses = new() { 25, 50, 75, 100 };

        public readonly static List<int> SpeedZoneSpeeds = new() { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60 };
    }
}