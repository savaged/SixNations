namespace savaged.mvvm.Data
{
    public interface ILocalCachedPropertiesService 
    {
        object Get(string key);
        void Set(string key, object value);
    }
}