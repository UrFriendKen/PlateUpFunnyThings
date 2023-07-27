using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace FunnyThings.Systems
{
    public class KillPlayers : GenericSystemBase, IModSystem
    {
        public struct CKillRequest : IComponentData, IModComponent
        {
            public float RespawnAfter;
            public bool HidePing;
        }

        EntityQuery Players;

        protected override void Initialise()
        {
            base.Initialise();
            Players = GetEntityQuery(new QueryHelper()
                .All(typeof(CPlayer), typeof(CPosition), typeof(CKillRequest)));
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> entities = Players.ToEntityArray(Allocator.Temp);
            using NativeArray<CPosition> positions = Players.ToComponentDataArray<CPosition>(Allocator.Temp);
            using NativeArray<CKillRequest> triggers = Players.ToComponentDataArray<CKillRequest>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                if (!Has<CHideView>(entity))
                {
                    CKillRequest trigger = triggers[i];
                    Set(entity, new CPlayerAutomaticRespawn()
                    {
                        RespawnProgress = trigger.RespawnAfter
                    });
                    Set<CHideView>(entity);

                    if (!trigger.HidePing)
                    {
                        if (!Require(entity, out CPlayerColour colour))
                        {
                            colour = new CPlayerColour()
                            {
                                Color = Color.red
                            };
                        }

                        Entity pingEntity = EntityManager.CreateEntity();
                        CPosition position = positions[i];
                        Set(pingEntity, new CRequiresView
                        {
                            Type = ViewType.Ping
                        });
                        Set(pingEntity, new CPosition
                        {
                            Position = position
                        });
                        Set(pingEntity, new CLifetime
                        {
                            RemainingLife = trigger.RespawnAfter
                        });
                        Set(pingEntity, new CPlayerPing
                        {
                            Colour = colour
                        });
                    }
                }
                EntityManager.RemoveComponent<CKillRequest>(entity);
            }
        }
    }
}
