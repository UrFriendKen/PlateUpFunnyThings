using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using static FunnyThings.Systems.KillPlayers;

namespace FunnyThings.GarageDoor
{
    public class KillPlayersInGarage : FranchiseSystem, IModSystem
    {
        public struct CKillAfterDelay : IComponentData, IModComponent
        {
            public float TimeRemaining;
        }

        EntityQuery KillRequests;
        EntityQuery Players;

        protected override void Initialise()
        {
            base.Initialise();
            KillRequests = GetEntityQuery(typeof(CKillAfterDelay));
            Players = GetEntityQuery(typeof(CPlayer), typeof(CPosition));
            RequireForUpdate(KillRequests);
        }

        protected override void OnUpdate()
        {
            float dt = Time.DeltaTime;

            using NativeArray<Entity> delayEntities = KillRequests.ToEntityArray(Allocator.Temp);
            using NativeArray<CKillAfterDelay> delays = KillRequests.ToComponentDataArray<CKillAfterDelay>(Allocator.Temp);
            for (int i = delays.Length - 1; i > -1; i--)
            {
                Entity delayEntity = delayEntities[i];
                CKillAfterDelay delay = delays[i];
                if (delay.TimeRemaining > 0f)
                {
                    delay.TimeRemaining -= dt;
                    Set(delayEntity, delay);
                }
                else
                {
                    using NativeArray<Entity> playerEntities = Players.ToEntityArray(Allocator.Temp);
                    using NativeArray<CPosition> playerPositions = Players.ToComponentDataArray<CPosition>(Allocator.Temp);
                    for (int j = 0; j < playerEntities.Length; j++)
                    {
                        Entity playerEntity = playerEntities[j];
                        CPosition position = playerPositions[j];
                        if (GetTile(position).Type == Kitchen.Layouts.RoomType.Garage)
                        {
                            Set(playerEntity, new CKillRequest()
                            {
                                RespawnAfter = 7f
                            });
                        }
                    }
                    EntityManager.DestroyEntity(delayEntity);
                }
            }
        }
    }
}
