using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Smash_Notes.Shared;
using Microsoft.EntityFrameworkCore;

namespace Smash_Notes.Server.Data
{
	public class AuthService : IAuthService
	{
		public DataContext _context { get; }
		private readonly IConfiguration _configuration;
		public AuthService(DataContext context, IConfiguration configuration)
		{
			_configuration = configuration;
			_context = context;

		}

        public event Func<Task> LoggedIn;
		public event Func<Task> LoggedOut;

        public async Task<ServiceResponse<string>> Login(string username, string password)
		{
			var response = new ServiceResponse<string>();
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower().Equals(username.ToLower())); //turn username lowercase
			if (user is null)
			{
				response.Success = false;
				response.Message = "User not found";
			}
			else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
			{
				response.Success = false;
				response.Message = "Wrong password.";
			}
			else
			{
				response.Data = CreateToken(user);
			}
            await LoggedIn?.Invoke();
            return response;
		}

		public async Task<ServiceResponse<int>> Register(User user, string password)
		{
			var response = new ServiceResponse<int>();
			if (await UserExists(user.Username))
			{
				response.Success = false;
				response.Message = "User Already exists.";
				return response;
			}
			CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

			user.PasswordHash = passwordHash;
			user.PasswordSalt = passwordSalt;

			_context.Users.Add(user);
			await _context.SaveChangesAsync();
			response.Data = user.Id;
			return response;
		}

		public async Task<bool> UserExists(string username)
		{
			if (await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()))
			{
				return true;
			}
			return false;
		}

		private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new System.Security.Cryptography.HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}

		private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
		{
			using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
			{
				var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
				return computedHash.SequenceEqual(passwordHash);
			}
		}

		private string CreateToken(User user)
		{
			var claims = new List<Claim>{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, user.Username)
			};

			var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
			if (appSettingsToken is null)
				throw new Exception("AppSettings Token is null!");

			SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
				.GetBytes(appSettingsToken));
			SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.Now.AddDays(1),
				SigningCredentials = creds
			}; //a token descripter is created with the aquired claims and signing crenditals, along with expiration which is 1 day right now.

			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

			SecurityToken token = tokenHandler.CreateToken(tokenDescriptor); //a security token key is created which is based on the tokenDescripter.

            return tokenHandler.WriteToken(token); //becomes a string
		}
	}
}
