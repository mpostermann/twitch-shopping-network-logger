using Microsoft.EntityFrameworkCore;

namespace TwitchShoppingNetworkLogger.Db.Migrate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var db = new Auditor.Db();
            db.Database.Migrate();
        }
    }
}
