using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace FunnyThings.GarageDoor
{
    public class OpenGarageDoor : ItemInteractionSystem, IModSystem
    {
        protected override InteractionType RequiredType => InteractionType.Act;

        private CGarageDecorations GarageDecorations;

        protected override bool IsPossible(ref InteractionData data)
        {
            if (!Require(data.Target, out GarageDecorations) || GarageDecorations.IsOpen)
                return false;
            return true;
        }

        protected override void Perform(ref InteractionData data)
        {
            GarageDecorations.IsOpen = true;
            GarageDecorations.RemainingTime = GarageDecorations.OpenStartTime;
            Set(data.Target, GarageDecorations);

            Entity entity = EntityManager.CreateEntity();
            Set(entity, new KillPlayersInGarage.CKillAfterDelay()
            {
                TimeRemaining = 5.15f
            });
        }
    }
}
