using Unity.Entities;
using Unity.Mathematics;

namespace ECSZombies
{
    public struct ZombieSpawnPoints : IComponentData
    {
        public BlobArray<float3> Value;
    }

    public struct SpawnPointsBlob: IComponentData
    {
        public BlobAssetReference<ZombieSpawnPoints> Blob;
    }
}