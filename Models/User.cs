using System.Linq;
using System.Web;
using WebApplication10.Constants;

namespace Blogs.Models
{
	public class User
	{
		public User()
		{

		}
		public User(string username, string password)
		{
			Username = username;
			Password = password;
		}
		public int Id { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string Avatar { get; set; }

		public static bool IsLoggedIn()
		{
			var isLogged = HttpContext.Current.Session[
				Constants.IS_LOGGED_IN
				];

			if (isLogged == null)
			{
				return false;
			}

			return (bool)isLogged;
		}

		public static User getCurrentUser()
		{
			var user = HttpContext.Current.Session[Constants.CURRENT_USER];
			return (User)user;
		}

		public string getAvatar()
		{
			if (Avatar != null)
			{
				return "/Assets/UploadedImages/" + Avatar;
			}
			return "/Assets/images/avatar.png";
		}

		public int GetBlogsCount()
		{
			var context = db.CachedContext.GetContext();
			return context.Blogs.Where(b => b.UserId == Id).Count();
		}
	}
}