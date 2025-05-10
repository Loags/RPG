namespace LB
{
    public interface IPooledObject
    {
        void OnObjectSpawnFromPool();

        void OnObjectReturnToPool();
    }
}