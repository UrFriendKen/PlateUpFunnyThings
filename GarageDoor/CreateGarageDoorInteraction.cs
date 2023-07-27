using Kitchen;
using KitchenData;
using KitchenMods;
using Unity.Entities;
using UnityEngine;

namespace FunnyThings.GarageDoor
{
    public class CreateGarageDoorInteraction : FranchiseSystem, IModSystem
    {
        EntityQuery GarageDecorations;
        EntityQuery GarageDecorationsProxies;
        protected override void Initialise()
        {
            base.Initialise();
            GarageDecorations = GetEntityQuery(typeof(CGarageDecorations));
            GarageDecorationsProxies = GetEntityQuery(typeof(CGarageDecorationsProxy));
            RequireForUpdate(GarageDecorations);
        }

        protected override void OnUpdate()
        {
            if (!GarageDecorationsProxies.IsEmpty)
                return;
            Entity garageDoor = GarageDecorations.First();
            for (int i = 0; i < 2; i++)
            {
                Entity interactionProxy = Create(GameData.Main.Get<Appliance>(AssetReference.InteractionProxy), new Vector3(-11f, 0f, -6f - i), Vector3.forward);
                Set(interactionProxy, new CInteractionProxy()
                {
                    Target = garageDoor,
                    IsActive = true
                });
                Set<CGarageDecorationsProxy>(interactionProxy);
            }
        }
    }
}
