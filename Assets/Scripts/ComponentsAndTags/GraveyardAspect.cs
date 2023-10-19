using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace ECSZombies
{
    public readonly partial struct GraveyardAspect : IAspect
    {
        public readonly Entity Entity;
        private readonly RefRO<LocalTransform> _transform;
        private readonly RefRO<GraveyardProperties> _graveyardProperties;
        private readonly RefRW<GraveyardRandom> _graveyardRandom;
        private readonly RefRW<ZombieSpawnPoints> _zombieSpawnPoints;
        private readonly RefRW<SpawnPointsBlob> _spawnPointsBlob;
        private readonly RefRW<ZombieSpawnTimer> _zombieSpawnTimer;

        public float3 Position => _transform.ValueRO.Position;

        public int NumTombstones => _graveyardProperties.ValueRO.NumTombstones;
        public Entity TombstonePrefab => _graveyardProperties.ValueRO.TombstonePrefab;
        public Entity ZombiePrefab => _graveyardProperties.ValueRO.ZombiePrefab;

        public BlobAssetReference<ZombieSpawnPoints> ZombieSpawnPoints
        {
            get
            {
                return _spawnPointsBlob.ValueRO.Blob;
            }
            set => _spawnPointsBlob.ValueRW.Blob = value;
        }

        private float3 GetPointFromBlob(BlobAssetReference<ZombieSpawnPoints> blob, int numZombies)
        {
            ref ZombieSpawnPoints zombieSpawnPoints = ref blob.Value;
            return zombieSpawnPoints.Value[_graveyardRandom.ValueRW.Value.NextInt(numZombies)];
        }

        public LocalTransform GetRandomTombstoneTransform()
        {
            return new LocalTransform {
                Position = GetRandomPosition(),
                Rotation = GetRandomRotation(),
                Scale = GetRandomScale(0.25f)
            };
        }

        public float3 GetRandomPosition()
        {
            float3 randomPos;
            do
            {
                randomPos = _graveyardRandom.ValueRW.Value.NextFloat3(_minCorner, _maxCorner);
            } while (math.distancesq(_transform.ValueRO.Position, randomPos) <= BRAIN_SAFETY_RADIUS_SQ);

            return randomPos;
        }

        private float3 _minCorner => _transform.ValueRO.Position - _halfDimensions;
        private float3 _maxCorner => _transform.ValueRO.Position + _halfDimensions;

        private float3 _halfDimensions => new() {
            x = _graveyardProperties.ValueRO.FieldDimensions.x / 2.0f,
            y = 0.0f,
            z = _graveyardProperties.ValueRO.FieldDimensions.y / 2.0f
        };

        private const float BRAIN_SAFETY_RADIUS_SQ = 100.0f;

        private quaternion GetRandomRotation() => quaternion.RotateY(_graveyardRandom.ValueRW.Value.NextFloat(-0.25f, 0.25f));
        private float GetRandomScale(float min) => _graveyardRandom.ValueRW.Value.NextFloat(min, 1.0f);

        public float ZombieSpawnTimer
        {
            get => _zombieSpawnTimer.ValueRO.Value;
            set => _zombieSpawnTimer.ValueRW.Value = value;
        }

        public bool TimeToSpawnZombie => ZombieSpawnTimer <= 0.0f;

        public float ZombieSpawnRate => _graveyardProperties.ValueRO.ZombieSpawnRate;

        public LocalTransform GetZombieSpawnPoint()
        {
            var position = GetPointFromBlob(ZombieSpawnPoints, NumTombstones);
            return new LocalTransform
            {
                Position = position,
                Rotation = quaternion.RotateY(MathHelpers.GetHeading(position, _transform.ValueRO.Position)),
                Scale = 1f
            };
        }
    }
}