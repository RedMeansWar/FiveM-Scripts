using Common.Client;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Essentials.Client
{
    public class Client : ClientCommonScript
    {
        public Client()
        {
            SetWeaponsNoAutoswap(true);
            SetWeaponsNoAutoreload(true);
            SetPlayerHealthRechargeMultiplier(ClientPlayer.Handle, 0f);
            SetFlashLightKeepOnWhileMoving(true);
            SetWeaponDamageModifier((uint)WeaponHash.Nightstick, 0.1f);
            SetWeaponDamageModifier((uint)WeaponHash.Unarmed, 0.1f);
            NetworkSetFriendlyFireOption(true);
        }
    }
}