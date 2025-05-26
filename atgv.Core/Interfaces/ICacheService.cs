namespace atgv.Core.Interfaces
{
    public interface ICacheService
    {
        T? Get<T>(string key);
        bool Set<T>(string key, T value, int expirationInMins);
        bool Remove(string key);
    }
}