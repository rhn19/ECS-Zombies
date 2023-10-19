using UnityEngine;
using Unity.Entities;

namespace ECSZombies
{
    public class BrainAuthoring : MonoBehaviour
    {
        public float BrainHealth;
    }

    public class BrainBaker : Baker<BrainAuthoring>
    {
        public override void Bake(BrainAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BrainTag());

            AddComponent(entity, new BrainHealth
            {
                Max = authoring.BrainHealth,
                Value = authoring.BrainHealth
            });

            AddBuffer<BrainDamageBufferElement>(entity);
        }
    }
}
