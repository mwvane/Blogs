using Blogs.db;
using Blogs.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Blogs.Controllers
{
	public class BlogController : Controller
	{
		protected MyDbContext _context = new MyDbContext();

		// GET: Blo
		public ActionResult Index()
		{
			if (!Blogs.Models.User.IsLoggedIn())
			{
				return RedirectToAction("Index", "Auth");
			}

			int userId = Models.User.getCurrentUser().Id;
			var blogs = _context.Blogs.Where(b => b.UserId == userId).OrderByDescending(t => t.Id)
				.ToList();

			return View(blogs);
		}

		public ActionResult Show(int? id)
		{
			if (id == null)
			{
				return RedirectToAction("Index");
			}

			var blog = _context.Blogs.Where(b => b.Id == id).FirstOrDefault();
			if (blog == null)
			{
				throw new Exception("Such blog does not exist");
			}
			return View(blog);
		}

		[HttpGet]
		public ActionResult Create()
		{
			if (!Blogs.Models.User.IsLoggedIn())
			{
				return RedirectToAction("Index", "Auth");
			}

			return View(new Blog());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CreateBlog([Bind(Include = "Id, Title, ShortDescription, Description")] Blog blog, HttpPostedFileBase ImageUrl)
		{
			if (!Blogs.Models.User.IsLoggedIn())
			{
				return RedirectToAction("Index", "Auth");
			}

			if (!ModelState.IsValid)
			{
				return View("Create", blog);
			}

			var oldBlog = _context.Blogs.Where(b => b.Id == blog.Id).FirstOrDefault();
			if (oldBlog == null)
			{
				throw new Exception("Please, provide correct blog id");
			}
			if (oldBlog.UserId != Models.User.getCurrentUser().Id)
			{
				throw new Exception("You dont't have access to this blog");
			}


			blog.UserId = Blogs.Models.User.getCurrentUser().Id;

			string oldImageUrl = "";
			if (ImageUrl == null)
			{
				if (blog.Id == 0)
				{
					ModelState.AddModelError("ImageUrl", "Please, upload an image");
					return View("Create", blog);
				}
				else
				{
					

					oldImageUrl = oldBlog.ImageUrl;
					blog.ImageUrl = oldBlog.ImageUrl;

					_context.Set<Blog>().AddOrUpdate(blog);
					_context.SaveChanges();
					return RedirectToAction("Index");
				}
			}

			string type = ImageUrl.ContentType;
			var list = new List<String>()
			{
				"image/png", "image/jpeg", "image/jpg"
			};

			if (!list.Contains(type))
			{
				ModelState.AddModelError("ImageUrl", "File format is not valid");
				return View("Create", blog);
			}

			string imageUrl = "image" + "-" + DateTime.Now.Ticks.ToString() + ImageUrl.FileName;
			string path = Path.Combine(Server.MapPath("~/Assets/UploadedFiles"), imageUrl);
			ImageUrl.SaveAs(path);

			blog.ImageUrl = imageUrl;

			if (blog.Id == 0)
			{
				_context.Blogs.Add(blog);
			}
			else
			{
				var fullPath = Server.MapPath("~/Assets/UploadedFiles") + "/" + oldBlog.ImageUrl;
				if (System.IO.File.Exists(fullPath))
				{
					System.IO.File.Delete(fullPath);
				}
				_context.Set<Blog>().AddOrUpdate(blog);
			}
			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		public ActionResult Edit(int id)
		{

			var blog = _context.Blogs.FirstOrDefault(t => t.Id == id);
			if (blog == null)
			{
				throw new Exception("Such blog with id " + id + "does not exist");
			}

			if (blog.UserId != Models.User.getCurrentUser().Id)
			{
				throw new Exception("You dont't have access to this blog");
			}

			return View("Create", blog);
		}

		public ActionResult Delete(int id)
		{
			var blog = _context.Blogs.FirstOrDefault(t => t.Id == id);
			if (blog == null)
			{
				throw new Exception("Such blog does not exist");
			}

			if (blog.ImageUrl != null)
			{
				var fullPath = Server.MapPath("~/Assets/UploadedFiles") + "/" + blog.ImageUrl;
				if (System.IO.File.Exists(fullPath))
				{
					System.IO.File.Delete(fullPath);
				}
			}

			_context.Blogs.Remove(blog);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}