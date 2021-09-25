using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blogs.db
{
	public static class CachedContext
	{
		public static MyDbContext _context = new MyDbContext();
		public static MyDbContext GetContext()
		{
			return _context;
		}
	}
}