using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace FunnyThings.Systems
{
    public class RespawnPlayers : GenericSystemBase, IModSystem
    {
        EntityQuery Players;
        EntityQuery SpawnLocations;

        protected override void Initialise()
        {
            base.Initialise();
            Players = GetEntityQuery(new QueryHelper()
                .All(typeof(CPlayer), typeof(CPosition), typeof(CPlayerAutomaticRespawn), typeof(CHideView)));
            SpawnLocations = GetEntityQuery(typeof(CPlayerSpawnLocation));
            RequireForUpdate(Players);
        }

        protected override void OnUpdate()
        {
            float dt = Time.DeltaTime;

            using NativeArray<Entity> entities = Players.ToEntityArray(Allocator.Temp);
            using NativeArray<CPlayer> players = Players.ToComponentDataArray<CPlayer>(Allocator.Temp);
            using NativeArray<CPosition> positions = Players.ToComponentDataArray<CPosition>(Allocator.Temp);
            using NativeArray<CPlayerAutomaticRespawn> respawns = Players.ToComponentDataArray<CPlayerAutomaticRespawn>(Allocator.Temp);
            using NativeArray<CPlayerSpawnLocation> spawnLocations = SpawnLocations.ToComponentDataArray<CPlayerSpawnLocation>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                CPosition position = positions[i];
                CPlayerAutomaticRespawn respawn = respawns[i];

                if (respawn.RespawnProgress > 0f)
                {
                    respawn.RespawnProgress -= dt;
                    Set(entity, respawn);
                    continue;
                }

                CPlayer player = players[i];
                for (int j = 0; j < spawnLocations.Length; j++)
                {
                    CPlayerSpawnLocation cPlayerSpawnLocation = spawnLocations[j];
                    if (cPlayerSpawnLocation.Index == -1 || cPlayerSpawnLocation.Index == player.Index || cPlayerSpawnLocation.Index == spawnLocations.Length - 1)
                    {
                        position = new CPosition(cPlayerSpawnLocation.Location)
                        {
                            ForceSnap = true
                        };
                        Set(entity, position);
                        break;
                    }
                }

                EntityManager.RemoveComponent<CHideView>(entity);
                EntityManager.RemoveComponent<CPlayerAutomaticRespawn>(entity);
            }
        }
    }
}
