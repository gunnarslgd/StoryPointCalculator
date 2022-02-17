using System;
using System.Web;
using System.Web.Mvc;
using StoryPointCalculator.Models;

namespace StoryPointCalculator.Controllers
{
	public class HomeController : Controller
	{
		private readonly Story _story = Story.Current();

		public ActionResult Index()
		{
			var cookie = Request.Cookies.Get("story-estimation");
			if (cookie != null)
			{
				_story.Name = cookie.Value;
			}
			
			return View(_story);
		}

		public ActionResult JoinEstimation(string name)
		{
			_story.NewPoint(name, new StoryPoint());

			Response.Cookies.Remove("story-estimation");
			var cookie = new HttpCookie("story-estimation", name)
			{
				Expires = DateTime.Now.AddDays(30)
			};
			this.Response.Cookies.Add(cookie);

			return View("index", _story);
		}

		public ActionResult NewPoint(string name, StoryPoint story)
		{
			story.Complexity = story.Complexity == 0 ? 1 : story.Complexity;
			story.Effort = story.Effort == 0 ? 1 : story.Effort;
			story.Uncertainty = story.Uncertainty == 0 ? 1 : story.Uncertainty;
			_story.NewPoint(name, story);

			return Json(_story);
		}

		public ActionResult NewPointSilently(string name, StoryPoint story)
		{
			story.Complexity = story.Complexity == 0 ? 1 : story.Complexity;
			story.Effort = story.Effort == 0 ? 1 : story.Effort;
			story.Uncertainty = story.Uncertainty == 0 ? 1 : story.Uncertainty;
			_story.NewPoint(name, story);

			return Json(new
			{
				Complexity = StoryPoint.GetScoreExplanation(story.Complexity),
				Effort = StoryPoint.GetScoreExplanation(story.Effort),
				Uncertainty = StoryPoint.GetScoreExplanation(story.Uncertainty),
				Point = story.Point
			});
		}

		public ActionResult CurrentStory()
		{
			return Json(_story, JsonRequestBehavior.AllowGet);
		}

		public ActionResult ShowEstimations()
		{
			_story.SetShowing(!_story.Showing);

			return View("index", _story);
		}

		public ActionResult StartNewVote()
		{
			foreach (var point in _story.Points.Values)
			{
				point.StartOver();
			}

			_story.SetShowing(false);
			return View("index", _story);
		}
	}
}