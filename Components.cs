using KitchenData;
using KitchenMods;
using System.Runtime.InteropServices;
using Unity.Entities;

namespace FunnyThings
{
    public struct CGarageDecorations : IApplianceProperty, IAttachableProperty, IComponentData, IModComponent
    {
        public bool IsOpen = false;
        public float OpenStartTime = 15f;
        public float RemainingTime = 0f;

        public CGarageDecorations() { }
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct CGarageDecorationsProxy : IComponentData, IModComponent { }
    public struct CPlayerAutomaticRespawn : IComponentData, IModComponent
    {
        public float RespawnProgress;
    }
}
