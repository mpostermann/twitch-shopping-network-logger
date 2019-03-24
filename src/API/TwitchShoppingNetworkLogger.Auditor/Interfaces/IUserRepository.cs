
namespace TwitchShoppingNetworkLogger.Auditor.Interfaces
{
    public interface IUserRepository
    {
        IUser GetUserByUsername(string username);

        /// <summary>
        /// Returns true if the user is allowed to use this application.
        /// </summary>
        bool IsUserAuthorized(string username);
    }
}
