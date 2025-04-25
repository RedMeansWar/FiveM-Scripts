using System;
using CitizenFX.Core;
using Common.Client.Models;

namespace Common.Client
{
    public class PedStyle
    {
        protected readonly Style _style;

        public PedStyle(Style style) => _style = style;

        public PedComponent this[PedComponentType componentType]
        {
            get
            {
                var pedComponent = componentType switch
                {
                    PedComponentType.Head => PedComponents.Face,
                    PedComponentType.Beard => PedComponents.Head,
                    PedComponentType.Hair => PedComponents.Hair,
                    PedComponentType.UpperBody => PedComponents.Torso,
                    PedComponentType.Legs => PedComponents.Legs,
                    PedComponentType.Hands => PedComponents.Hands,
                    PedComponentType.Shoes => PedComponents.Shoes,
                    PedComponentType.NeckScarfs => PedComponents.Special1,
                    PedComponentType.ShirtAccessories => PedComponents.Special2,
                    PedComponentType.BodyArmor => PedComponents.Special3,
                    PedComponentType.Decals => PedComponents.Textures,
                    PedComponentType.ShirtJacket => PedComponents.Torso2,
                    _ => throw new ArgumentOutOfRangeException(nameof(componentType), componentType, null)
                };

                return _style[pedComponent];
            }
        }

        public PedComponent this[PedComponents componentType]
        {
            get
            {
                return _style[componentType];
            }
        }

        public PedProp this[PedProps propType]
        {
            get
            {
                return _style[propType];
            }
        }
    }
}