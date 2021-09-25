using Blogs.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blogs.Controllers
{
	public class HomeController : Controller
	{
		protected MyDbContext _context = new MyDbContext(); 
		public ActionResult Index()
		{
			var blogs = _context.Blogs.Take(10).OrderByDescending(t => t.Id).ToList();
			return View(blogs);
		}
	}
}