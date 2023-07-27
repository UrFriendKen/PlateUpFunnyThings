using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;

namespace FunnyThings.GarageDoor
{
    public class CloseGarageDoorAfterDuration : FranchiseSystem, IModSystem
    {
        EntityQuery GarageDecorations;

        protected override void Initialise()
        {
            base.Initialise();
            GarageDecorations = GetEntityQuery(typeof(CGarageDecorations));
            RequireForUpdate(GarageDecorations);
        }

        protected override void OnUpdate()
        {
            float dt = Time.DeltaTime;

            using NativeArray<Entity> entities = GarageDecorations.ToEntityArray(Allocator.Temp);
            using NativeArray<CGarageDecorations> garageDecorations = GarageDecorations.ToComponentDataArray<CGarageDecorations>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                CGarageDecorations garageDecoration = garageDecorations[i];
                if (garageDecoration.IsOpen)
                {
                    garageDecoration.RemainingTime -= dt;
                }
                if (garageDecoration.RemainingTime < 0f)
                {
                    garageDecoration.IsOpen = false;
                }
                Set(entity, garageDecoration);
            }
        }
    }
}
