using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwitchShoppingNetworkLogger.Db.Interfaces
{
    public interface IDbContext
    {
        IList<T> Get<T>(object filter);
        Task InsertAsync<T>(T obj);
        Task UpdateAsync<T>(T obj, object filter);
        Task DeleteAsync<T>(object filter);
    }
}
