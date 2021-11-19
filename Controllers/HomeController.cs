using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StoryPointCalculator.Models;

namespace StoryPointCalculator.Controllers
{
	public class HomeController : Controller
	{
		private static readonly Story Story = new Story();

		public ActionResult Index()
		{
			var cookie = Request.Cookies.Get("story-estimation");
			if (cookie != null)
			{
				Story.Name = cookie.Value;
			}
			
			return View(Story);
		}

		public ActionResult JoinEstimation(string name)
		{
			Story.NewPoint(name, new StoryPoint());

			Response.Cookies.Remove("story-estimation");
			var cookie = new HttpCookie("story-estimation", name)
			{
				Expires = DateTime.Now.AddDays(30)
			};
			this.Response.Cookies.Add(cookie);

			return View("index", Story);
		}

		public ActionResult NewPoint(string name, StoryPoint story)
		{
			Story.NewPoint(name, story);

			return View("index", Story);
		}

		public ActionResult ShowEstimations()
		{
			Story.Showing = !Story.Showing;

			return View("index", Story);
		}

		public ActionResult ClearAllEstimations()
		{
			foreach (var point in Story.Points.Values)
			{
				point.StartOver();
			}

			return View("index", Story);
		}
	}
}