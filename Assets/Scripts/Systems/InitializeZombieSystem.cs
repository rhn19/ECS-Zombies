using Unity.Entities;
using Unity.Burst;
using Unity.Collections;

namespace ECSZombies
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct InitializeZombieSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach(var zombie in SystemAPI.Query<ZombieWalkAspect>().WithAll<NewZombieTag>())
            {
                ecb.RemoveComponent<NewZombieTag>(zombie.Entity);
                ecb.SetComponentEnabled<ZombieWalkProperties>(zombie.Entity, false);
                ecb.SetComponentEnabled<ZombieEatProperties>(zombie.Entity, false);
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
