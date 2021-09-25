using Blogs.db;
using Blogs.Models;
using System.Linq;
using System.Web.Mvc;

namespace WebApplication10.Controllers
{
	public class AuthController : Controller
	{
		protected MyDbContext _context = new MyDbContext();

		// GET: Auth
		public ActionResult Index()
		{
			if (Request.Cookies[Constants.Constants.USERNAME] != null)
			{
				string username = Request.Cookies[Constants.Constants.USERNAME].Value;
				string password = Request.Cookies[Constants.Constants.PASSWORD].Value;
				var user = new User(username, password);
				return View(user);
			}

			return View(new User());
		}

		public ActionResult Signout()
		{
			Session.Remove(Constants.Constants.CURRENT_USER);
			Session.Remove(Constants.Constants.IS_LOGGED_IN);
			return RedirectToAction("Index", "Home");
		}

		private bool TryToLoginUser(User user)
		{
			var model = _context.Users
				.Where(u => u.Username == user.Username &&
					   u.Password == user.Password)
				.FirstOrDefault();

			if (model == null)
			{
				ModelState.AddModelError("Username", "Such user does not exist");
				return false;
			}

			saveUser(model);
			return true;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Login([Bind(Include = "username, password")] User user)
		{
			if (!ModelState.IsValid)
			{
				return View("index", user);
			}

			if (!TryToLoginUser(user))
			{
				return View("Index", user);	
			}

			return RedirectToAction("Index", "Home");
		}

		public ActionResult Registration()
		{
			return View(new User());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult RegisterUser([Bind(Include = "username, password")] User user, [Microsoft.AspNetCore.Mvc.FromForm] string confirmPassword)
		{
			if (!ModelState.IsValid)
			{
				return View("Registration", user);
			}

			if (user.Password != confirmPassword)
			{
				ModelState.AddModelError("Password", "confirm password does not match with this password");
				return View("Registration", user);
			}

			var exists = _context.Users
				.Where(u => u.Username == user.Username)
				.Count() > 0;

			if (exists)
			{
				ModelState.AddModelError("Username", "Such user already exists");
				return View("Registration", user);
			}

			User createdUser = _context.Users.Add(user);
			_context.SaveChanges();
			var d = 1;

			saveUser(createdUser);

			return RedirectToAction("Index", "Home");
		}

		private void saveUser(User user)
		{
			Session[Constants.Constants.IS_LOGGED_IN] = true;
			Session[Constants.Constants.CURRENT_USER] = user;

			var userCookie = new System.Web.HttpCookie(Constants.Constants.USERNAME, user.Username);
			userCookie.Expires = new System.DateTime(2050, 1, 1);

			var passwordCookie = new System.Web.HttpCookie(Constants.Constants.PASSWORD, user.Password);
			passwordCookie.Expires = new System.DateTime(2050, 1, 2);

			Response.Cookies.Add(userCookie);
			Response.Cookies.Add(passwordCookie);

		}
	}
}