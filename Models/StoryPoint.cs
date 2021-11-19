using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoryPointCalculator.Models
{
	public class StoryPoint
	{
		public int Complexity { get; set; }
		public int Effort { get; set; }
		public int Uncertainty { get; set; }


		public StoryPoint()
		{
			StartOver();
		}

		public StoryPoint(int complexity, int effort, int uncertainty)
		{
			Complexity = complexity;
			Effort = effort;
			Uncertainty = uncertainty;
		}

		public void StartOver()
		{
			Complexity = 0;
			Uncertainty = 0;
			Effort = 0;
		}

		public int Point
		{
			get
			{
				var point = GetPointFromScore(Complexity) * 0.4f + GetPointFromScore(Effort) * 0.4f + GetPointFromScore(Uncertainty) * 0.2f;
				return ToFabracheNumber(point);
			}
		}

		public bool IsEmpty => Complexity + Effort + Uncertainty == 0;

		public static int ToFabracheNumber(float point)
		{
			var fabrache = new int[] { 1, 2, 3, 5, 8, 13 };
			var lastFabrache = 0;

			foreach (var n in fabrache)
			{
				if (point >= n)
				{
					lastFabrache = n;
				}
				else
				{
					var average = (lastFabrache + n) / 2;
					lastFabrache = point > average ? n : lastFabrache;
					break;
				}
			}

			return lastFabrache;
		}

		public static string GetScoreExplanation(int score)
		{
			switch (score)
			{
				case 1:
				case 2:
				return $"{score} (Very Low)";

				case 3:
				case 4:
					return $"{score} (Low)";

				case 5:
				case 6:
					return $"{score} (Medium)";

				case 7:
				case 8:
					return $"{score} (High)";

				case 9:
				case 10:
					return $"{score} (Very High)";

				default:
					return $"{score} (Very Low)";
			}
		}

		public static int GetPointFromScore(int score)
		{
			switch (score)
			{
				case 1:
				case 2:
					return score;

				case 3:
				case 4:
					return 3;

				case 5:
				case 6:
					return 5;

				case 7:
				case 8:
					return 8;

				case 9:
				case 10:
					return 13;

				default:
					return 1;
			}
		}
	}
}