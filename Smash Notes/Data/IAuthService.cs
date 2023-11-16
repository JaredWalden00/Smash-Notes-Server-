using Smash_Notes.Shared;

namespace Smash_Notes.Server.Data
{
	public interface IAuthService
	{
        public event Func<Task> LoggedIn;
        public event Func<Task> LoggedOut;
        Task<ServiceResponse<int>> Register(User user, string password);
		Task<ServiceResponse<string>> Login(string username, string password);
		Task<bool> UserExists(string username);
	}
}
