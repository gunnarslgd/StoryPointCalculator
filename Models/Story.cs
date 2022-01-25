using System;
using System.Collections.Generic;
using System.Linq;

namespace StoryPointCalculator.Models
{
	public class Story
	{
		public const int MaxDifference = 5;

		private Story()
		{
			Points = new Dictionary<string, StoryPoint>();
		}

		private static DateTime _expiryDateTime;
		private static Story _current;
		public static Story Current()
		{
			if (_expiryDateTime < DateTime.Now)
			{
				_current = null;
			}

			if (_current == null)
			{
				_current = new Story();
				_expiryDateTime = DateTime.Now.AddHours(2);
			}

			return _current;
		}

		public Dictionary<string, StoryPoint> Points { get; }

		public bool Showing { get; set; }

		public  List<KeyValuePair<string, StoryPoint>> PointsWithAverage
		{
			get
			{
				var result = new List<KeyValuePair<string, StoryPoint>> { new KeyValuePair<string, StoryPoint>("Average", FinalPoint()) };
				result.AddRange(Points.Keys.OrderBy(k => k)
					.Select(name => new KeyValuePair<string, StoryPoint>(name, Points[name])));

				return result;
			}
		}

		public string Name { get; set; }

		public StoryPoint CurrentStoryPoint => Points == null || string.IsNullOrEmpty(Name) ? new StoryPoint() : Points.TryGetValue(Name, out var point) ? point : new StoryPoint();

		public void StartOver()
		{
			foreach (var point in Points.Values)
			{
				point.StartOver();
			}
		}

		public void JoinJudge(string name)
		{
			Points[name] = new StoryPoint();
		}

		public void QuitJudge(string name)
		{
			Points.Remove(name);
		}

		public void NewPoint(string name, StoryPoint point)
		{
			Points[name] = point;
		}

		public StoryPoint FinalPoint()
		{
			if (Points.Count == 0) return new StoryPoint();
			if (Points.Count == 1) return Points.Values.First();

			var allComplexity = Points.Values.Select(p => (float)p.Complexity)
				.OrderBy(p => p)
				.ToList();
			var allEffort = Points.Values.Select(p => (float)p.Effort)
				.OrderBy(p => p)
				.ToList();
			var allUncertainty = Points.Values.Select(p => (float)p.Uncertainty)
				.OrderBy(p => p)
				.ToList();

			var avgComplexity = GetAverage(allComplexity);
			var avgEffort = GetAverage(allEffort);
			var avgUncertainty = GetAverage(allUncertainty);

			return new StoryPoint
			{
				Complexity = StoryPoint.ToFabracheNumber(avgComplexity),
				Effort = StoryPoint.ToFabracheNumber(avgEffort),
				Uncertainty = StoryPoint.ToFabracheNumber(avgUncertainty)
			};
		}

		private float GetAverage(IReadOnlyList<float> allPoints)
		{
			var total = allPoints.Sum();
			var average = total / Points.Count;

			if (allPoints.Count <= 3) return average;

			if (Math.Abs(allPoints[0] - average) > MaxDifference)
			{
				average = (total - allPoints[0]) / (Points.Count - 1);
			}
			else if (Math.Abs(allPoints[Points.Count - 1] - average) > MaxDifference)
			{
				average = (total - allPoints[Points.Count - 1]) / (Points.Count - 1);
			}

			return average;
		}
	}
}