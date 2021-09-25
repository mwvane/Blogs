using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blogs.Models
{
	public class Blog
	{
		private User _cachedUser { get; set; }
		public int Id { get; set; }
		public int UserId { get; set; }	
		[Required]
		public string Title { get; set; }
		[Required]
		public string ShortDescription { get; set; }
		[Required]
		public string Description { get; set; }
		public string ImageUrl { get; set; }

		public User GetUser()
		{
			if (_cachedUser != null)
				return _cachedUser;

			var context = db.CachedContext.GetContext();
			var user = context.Users.Where(u => u.Id == UserId).FirstOrDefault();
			_cachedUser = user;
			return user;
		}
	}
}