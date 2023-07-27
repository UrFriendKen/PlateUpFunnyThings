using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace FunnyThings
{
    [UpdateInGroup(typeof(InteractionGroup), OrderFirst = true)]
    [UpdateBefore(typeof(AttemptInteraction))]
    public class ClearInteractions : GenericSystemBase, IModSystem
    {
        EntityQuery Appliances;

        protected override void Initialise()
        {
            base.Initialise();
            Appliances = GetEntityQuery(new QueryHelper()
                .Any(typeof(CGarageDecorations)));
        }

        protected override void OnUpdate()
        {
            EntityManager.RemoveComponent(Appliances, typeof(CBeingGrabbed));
            EntityManager.RemoveComponent(Appliances, typeof(CBeingActedOn));
            EntityManager.RemoveComponent(Appliances, typeof(CBeingLookedAt));
            EntityManager.RemoveComponent(Appliances, typeof(CBeingActedOnBy));
            EntityManager.AddComponent(Appliances, typeof(CBeingActedOnBy));
        }
    }
}
