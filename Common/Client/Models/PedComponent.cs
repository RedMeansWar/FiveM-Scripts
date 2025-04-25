using CitizenFX.Core;

namespace Common.Client.Models
{
    public enum PedComponentType
    {
        /// <summary>
        /// This is an enum that we have no idea what it changes maybe the head in the
        /// MP Player Customization?
        /// </summary>
        Head = PedComponents.Face,

        /// <summary>
        /// This is an enum that we have no idea what it changes maybe the beard in the
        /// MP Player Customization?
        /// </summary>
        Beard = PedComponents.Head,

        /// <summary>
        /// Hair Style / Color
        /// </summary>
        Hair = PedComponents.Hair,

        /// <summary>
        /// Hands / Upper Body
        /// </summary>
        UpperBody = PedComponents.Torso,

        /// <summary>
        /// Legs / Pants
        /// </summary>
        Legs = PedComponents.Legs,

        /// <summary>
        /// Hands
        /// </summary>
        Hands = PedComponents.Hands,

        /// <summary>
        /// Shoes
        /// </summary>
        Shoes = PedComponents.Shoes,

        /// <summary>
        /// Neck / Scarfs
        /// </summary>
        NeckScarfs = PedComponents.Special1,

        /// <summary>
        /// Shirt / Accessory
        /// </summary>
        ShirtAccessories = PedComponents.Special2,

        /// <summary>
        /// Body Armor / Accessory 2
        /// </summary>
        BodyArmor = PedComponents.Special3,

        /// <summary>
        /// Badges / Logos (Also known as decals)
        /// </summary>
        Decals = PedComponents.Textures,

        /// <summary>
        /// Shirt Overlay / Jackets
        /// </summary>
        ShirtJacket = PedComponents.Torso2
    }
}