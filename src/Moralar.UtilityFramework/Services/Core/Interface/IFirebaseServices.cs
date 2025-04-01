using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Services.Core.Interface
{
    public interface IFirebaseServices
    {
        T Get<T>(string child);

        void SavePost(string child, object data, JsonSerializerSettings configJson = null);

        void SavePut(string child, object data, JsonSerializerSettings configJson = null);

        void SavePatch(string child, object data, JsonSerializerSettings configJson = null);

        void Delete(string child);

        IEnumerable<T> List<T>(string child);

        Task<T> GetAsync<T>(string child, bool configureAwait = false);

        Task SavePostAsync(string child, object data, JsonSerializerSettings configJson = null, bool configureAwait = false);

        Task SavePutAsync(string child, object data, JsonSerializerSettings configJson = null, bool configureAwait = false);

        Task SavePatchAsync(string child, object data, JsonSerializerSettings configJson = null, bool configureAwait = false);

        Task DeleteAsync(string child, bool configureAwait = false);

        Task<IEnumerable<T>> ListAsync<T>(string child, bool configureAwait = false);
    }
}
