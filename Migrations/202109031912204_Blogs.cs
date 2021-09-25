namespace Blogs.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class Blogs : DbMigration
	{
		public override void Up()
		{
			CreateTable(
				"dbo.Blogs",
				c => new
				{
					Id = c.Int(nullable: false, identity: true),
					UserId = c.Int(nullable: false),
					Title = c.String(),
					ShortDescription = c.String(),
					Description = c.String(),
					ImageUrl = c.String(),
				})
				.PrimaryKey(t => t.Id)
				.ForeignKey("dbo.Users", t => t.UserId);

		}

		public override void Down()
		{
			DropTable("dbo.Blogs");
		}
	}
}
