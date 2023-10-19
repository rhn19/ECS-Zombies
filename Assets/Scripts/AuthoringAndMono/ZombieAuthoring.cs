using UnityEngine;
using Unity.Entities;

namespace ECSZombies
{
    public class ZombieAuthoring : MonoBehaviour
    {
        public float ZombieRiseRate;
        public float WalkSpeed;
        public float WalkAmplitude;
        public float WalkFrequency;

        public float EatDamagePerSecond;
        public float EatAmplitude;
        public float EatFrequency;
    }

    public class ZombieBaker : Baker<ZombieAuthoring>
    {
        public override void Bake(ZombieAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ZombieRiseRate { Value = authoring.ZombieRiseRate});

            AddComponent(entity, new ZombieWalkProperties
            {
                WalkSpeed = authoring.WalkSpeed,
                WalkAmplitude = authoring.WalkAmplitude,
                WalkFrequency = authoring.WalkFrequency
            });

            AddComponent(entity, new ZombieTimer());

            AddComponent(entity, new ZombieHeading());

            AddComponent(entity, new NewZombieTag());

            AddComponent(entity, new ZombieEatProperties
            {
                EatDamagePerSecond = authoring.EatDamagePerSecond,
                EatAmplitude = authoring.EatAmplitude,
                EatFrequency = authoring.EatFrequency
            });
        }
    }
}
