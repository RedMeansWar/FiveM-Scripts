namespace Common.Client
{
    public enum VehicleConfigFlag
    {
        PressingHorn = 1,
        Shooting = 2,
        SirenActive = 4,
        VehicleDead = 8,
        Aiming = 16,
        Driver = 32,
        HasAimData = 64,
        BurnOut = 128,
        ExitingVehicle = 256,
        PlayerDead = 512
    }
}