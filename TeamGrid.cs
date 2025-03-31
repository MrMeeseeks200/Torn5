using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Torn;

namespace Torn5
{
	class TeamGrid
	{
		public string ScoreText { get; private set; }
		public Color ScoreColor { get; set; }
		public double BackToBackPenalty { get; set; }
		public bool ContinueGenerating { get; set; }
		public readonly List<Colour> Colours = new List<Colour>();

		private double GamesPerTeam;
		private List<List<int>> previousGrid;
		private double previousBestScore;
		private double previousGamesPerTeam;
		private bool previousHasRef;
		private List<List<int>> previousExistingPlays;

		/// <summary>Generate a team fixture via hill-climbing.</summary>
		public void GenerateTeamGrid(League league, Fixture fixture, List<LeagueTeam> teams, int maxMillis, int gamesPerTeam, DateTime firstGame, TimeSpan timeBetweenGames)
		{
			fixture.Teams.Clear();
			fixture.Teams.Populate(teams);
			fixture.Games.Clear();

			int numberOfTeams = fixture.Teams.Count;
			bool hasRef = Colours.Contains(Colour.Referee);
			int teamsPerGame = Colours.Count;
			GamesPerTeam = gamesPerTeam + (hasRef ? (double)gamesPerTeam / (teamsPerGame - 1) : 0);

			List<List<int>> existingGrid = GetLeagueGrid(league);
			List<List<int>> existingPlays = CalcPlays(existingGrid, false, new List<List<int>>());
			LogGrid(existingPlays);

			List<List<int>> existingPlaysPadded = PadPlaysToTeamNumber((int)numberOfTeams, existingPlays);
			LogGrid(existingPlaysPadded);

			List<List<int>> grid = GetGrid(fixture.Teams, teamsPerGame, GamesPerTeam, hasRef, existingPlaysPadded, maxMillis);

			GridToFixtureGames(grid, fixture, firstGame, timeBetweenGames);
		}

		void LogGrid<T>(List<List<T>> grid)
		{
			string str = "***Grid***\n";
			foreach (List<T> list in grid)
			{
				foreach (T item in list)
				{
					str += item + ",\t";
				}
				str += "\n";
			}
			str += "**********";
			Console.WriteLine(str);
		}

		int CountDistinct<T>(List<List<T>> grid)
		{
			var flat = new List<T>();
			foreach (List<T> list in grid)
				flat.AddRange(list);

			return flat.Distinct().Count();
		}

		List<List<T>> TransposeGrid<T>(List<List<T>> grid)
		{
			return grid.SelectMany(inner => inner.Select((item, index) => new { item, index }))
				.GroupBy(i => i.index, i => i.item)
				.Select(g => g.ToList())
				.ToList();
		}

		List<List<int>> GetGrid(FixtureTeams teams, double teamsPerGame, double gamesPerTeam, bool hasRef, List<List<int>> existingPlays, int maxMillis)
		{
			List<List<int>> bestGrid = SetupGrid(teams, teamsPerGame, gamesPerTeam);

			double bestScore = CalcScore(bestGrid, gamesPerTeam, hasRef, existingPlays);

			return ContinueMixing(bestGrid, gamesPerTeam, hasRef, existingPlays, maxMillis, bestScore);
		}

		public void ContinueMixing(Fixture fixture, int maxMillis, DateTime firstGame, TimeSpan timeBetweenGames)
		{
			bool hasRef = Colours.Contains(Colour.Referee);
			var grid = ContinueMixing(previousGrid, GamesPerTeam, hasRef, previousExistingPlays, maxMillis, previousBestScore);
			GridToFixtureGames(grid, fixture, firstGame, timeBetweenGames);
		}

		List<List<int>> ContinueMixing(List<List<int>> grid, double gamesPerTeam, bool hasRef, List<List<int>> existingPlays, int maxMillis, double startingScore)
		{
			List<List<int>> bestGrid = grid;
			double bestScore = startingScore;
			bool badFixture = bestScore > 10000;

			Console.WriteLine("initScore: {0}", bestScore);
			LogGrid(bestGrid);

			Stopwatch sw = new Stopwatch();
			sw.Start();
			ScoreText = "";
			int count = 0;

			for (int i = 0; i < 20000; i++)
			{
				count++;
				List<List<int>> mixedGrid = MixGrid(bestGrid);
				double score = CalcScore(mixedGrid, gamesPerTeam, hasRef, existingPlays);
				if (score <= bestScore)
				{
					bestScore = score;
					bestGrid = mixedGrid;
					if (bestScore > 10000)
					{
						badFixture = true;
					}
					else
					{
						badFixture = false;
					}
				}
			}

			while (badFixture)
			{
				count++;
				if (sw.ElapsedMilliseconds > maxMillis)
					break;
				List<List<int>> mixedGrid = MixGrid(bestGrid);
				double score = CalcScore(mixedGrid, gamesPerTeam, hasRef, existingPlays);
				if (score <= bestScore)
				{
					bestScore = score;
					bestGrid = mixedGrid;
					if (bestScore > 10000)
					{
						badFixture = true;
					}
					else
					{
						badFixture = false;
					}
				}
			}
			CalcScore(bestGrid, gamesPerTeam, hasRef, existingPlays, true);
			Console.WriteLine("Time Elapsed (ms): {0}", sw.ElapsedMilliseconds);
			Console.WriteLine("Iterations: {0}", count);

			Console.WriteLine("bestScore: {0}", bestScore);
			LogGrid(bestGrid);
			Console.WriteLine("Existing Plays");
			LogGrid(NormalisePlays(existingPlays));
			Console.WriteLine("Plays");
			LogGrid(NormalisePlays(CalcPlays(bestGrid, hasRef, existingPlays)));
			ScoreText = bestScore >= 10000 ? "Extend max time and regenerate: " + Math.Round(bestScore).ToString() : "Score: " + Math.Round(bestScore).ToString() + " (Lower is better)";
			ScoreColor = bestScore >= 10000 ? Color.FromName("red") : Color.Transparent;

			previousGrid = bestGrid;
			previousBestScore = bestScore;
			previousGamesPerTeam = gamesPerTeam;
			previousHasRef = hasRef;
			previousExistingPlays = existingPlays;
			ContinueGenerating = true;

			return bestGrid;
		}

		List<List<int>> MixGrid(List<List<int>> grid)
		{
			List<List<int>> newGrid = grid.ConvertAll(row => row.ConvertAll(cell => cell));
			double teamsPerGame = grid[0].Count;
			Random rnd = new Random();

			int game1 = rnd.Next(grid.Count);
			int game2 = rnd.Next(grid.Count);

			int player1 = rnd.Next((int)teamsPerGame);
			int player2 = rnd.Next((int)teamsPerGame);

			newGrid[game1][player1] = grid[game2][player2];
			newGrid[game2][player2] = grid[game1][player1];

			return newGrid;
		}

		List<int> SumRows(List<List<int>> grid)
		{
			List<int> totals = new List<int>();
			foreach (List<int> row in grid)
			{
				int total = 0;
				foreach (int value in row)
				{
					total += value;
				}
				totals.Add(total);
			}
			return totals;
		}

		List<List<int>> NormalisePlays(List<List<int>> grid)
		{
			List<int> playsTotals = SumRows(grid);
			double mostPlays = 0;
			List<List<int>> updatedGrid = new List<List<int>>();

			foreach (int totalPlays in playsTotals)
			{
				if (totalPlays > mostPlays)
				{
					mostPlays = totalPlays;
				}
			}


			for (int i = 0; i < grid.Count; i++)
			{
				List<int> row = grid[i];
				List<int> newRow = new List<int>();
				double teamPlays = playsTotals[i];
				double multiplier = teamPlays == 0 || mostPlays == 0 ? 1 : mostPlays / teamPlays;

				for (int j = 0; j < row.Count; j++)
				{
					double teamPlays2 = playsTotals[j];
					double multiplier2 = teamPlays2 == 0 || mostPlays == 0 ? 1 : mostPlays / teamPlays2;
					double mult = Math.Max(multiplier, multiplier2);
					double result = mult * row[j];
					newRow.Add((int)Math.Round(result));
				}
				updatedGrid.Add(newRow);
			}
			return updatedGrid;
		}

		double GetAveragePlays(List<List<int>> grid)
		{
			double total = 0;
			double count = 0;
			for (int i = 0; i < grid.Count; i++)
			{
				for (int j = 0; j < grid[i].Count; j++)
				{
					if (i != j)
					{
						total += grid[i][j];
						count++;
					}
				}
			}
			return total / count;
		}

		double CalcScore(List<List<int>> grid, double gamesPerTeam, bool hasRef, List<List<int>> existingPlays, bool log = false)
		{
			List<List<int>> normalisedExistingPlays = NormalisePlays(existingPlays);
			int totalTeams = CountDistinct(grid);
			int teamsPerGame = grid[0].Count;
			double previousAveragePlays = GetAveragePlays(normalisedExistingPlays);
			List<List<int>> plays = NormalisePlays(CalcPlays(grid, hasRef, existingPlays));
			double averagePlays = GetAveragePlays(plays);

			double score = 0;

			for (int player1 = 0; player1 < plays.Count; player1++)
			{
				score += plays[player1][player1] * 100000; // penalty for playing themselves
				for (int player2 = player1 + 1; player2 < plays[player1].Count; player2++)
				{
					score += Math.Pow(plays[player1][player2] - averagePlays, 4);
				}
			}

			List<List<int>> transposedGrid = TransposeGrid(grid);

			//penalty for same colour
			foreach (List<int> colour in transposedGrid)
			{
				int numberOfGames = colour.Count;
				int numberOfColours = transposedGrid.Count;
				int uniqueTeams = colour.Distinct().Count();
				double gamesOnEachColour = gamesPerTeam / numberOfColours;
				int penalties = colour.FindAll(team => colour.FindAll(t => t == team).Count > gamesOnEachColour).Count;

				double penalty = penalties * 100000;
				score += penalty;
			}

			foreach (List<int> row in grid)
			{
				int uniquePlayers = row.Distinct().Count();
				score += Math.Abs(uniquePlayers - teamsPerGame) * 100000; // penalty for playing themselves
			}

			for (int game = 0; game < grid.Count - 1; game++)
			{
				foreach (var team in grid[game])
				{
					var nextGame = grid[game + 1];
					if (nextGame.Contains(team))
					{
						var isRefTho = hasRef && (nextGame.IndexOf(team) == (nextGame.Count - 1) || grid[game].IndexOf(team) == (grid[game].Count - 1));
						if (log) { Console.WriteLine("team " + team + " is in games " + game + " and " + (game + 1) + " back to back" + (isRefTho ? " but one is a ref game" : "")); }
						score += BackToBackPenalty - (isRefTho ? BackToBackPenalty * (0.5) : 0);
					}
				}
			}

			return score;
		}

		List<List<int>> AddGrids(List<List<int>> grid1, List<List<int>> grid2)
		{

			if (grid1.Count > 0 && grid1.Count == grid2.Count && grid1[0].Count == grid2[0].Count)
			{
				List<List<int>> grid3 = new List<List<int>>();
				for (int i = 0; i < grid1.Count; i++)
				{
					List<int> row = new List<int>();
					for (int j = 0; j < grid1[i].Count; j++)
					{
						if (grid2.Count > i && grid2[i].Count > j)
						{
							row.Add(grid1[i][j] + grid2[i][j]);
						}
						else
						{
							row.Add(grid1[i][j]);
						}
					}
					grid3.Add(row);
				}
				return grid3;
			}
			else
			{
				return grid1;
			}
		}

		List<List<int>> CalcPlays(List<List<int>> grid, bool hasRef, List<List<int>> existingPlays)
		{
			List<List<int>> newGrid = grid.ConvertAll(row => row.ConvertAll(cell => cell));
			// ignore ref games
			if (hasRef)
			{
				List<List<int>> transposedGrid = TransposeGrid(newGrid);
				transposedGrid.RemoveAt(transposedGrid.Count - 1);
				newGrid = TransposeGrid(transposedGrid);
			}

			int totalTeams = CountDistinct(newGrid);
			List<List<int>> plays = new List<List<int>>();
			for (int team1 = 0; team1 < totalTeams; team1++)
			{
				List<int> row = new List<int>();
				for (int team2 = 0; team2 < totalTeams; team2++)
				{
					if (team1 == team2)
					{
						List<List<int>> games = newGrid.FindAll(g => g.FindAll(t => t == team1).Count > 1);
						row.Add(games.Count);
					}
					else
					{
						List<List<int>> games = newGrid.FindAll(g => g.Contains(team1) && g.Contains(team2));
						row.Add(games.Count);
					}
				}
				plays.Add(row);
			}

			return AddGrids(plays, existingPlays);
		}

		List<T> Shuffle<T>(List<T> list)
		{
			Random random = new Random();
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = random.Next(n + 1);
				(list[n], list[k]) = (list[k], list[n]);
			}
			return list;
		}

		List<List<int>> RandomiseGrid(List<List<int>> grid)
		{
			for (int i = 0; i < grid.Count; i++)
			{
				grid[i] = Shuffle(grid[i]);
			}

			for (int i = 0; i < grid.Count; i++)
			{
				grid[i] = Shuffle(grid[i]);
			}
			return TransposeGrid(grid);

		}

		List<List<int>> SetupGrid(FixtureTeams teams, double teamsPerGame, double gamesPerTeam)
		{
			int numberOfTeams = teams.Count();
			double numGames = (numberOfTeams / teamsPerGame) * gamesPerTeam;

			// set up grid
			List<int> arr = Enumerable.Repeat(-1, (int)teamsPerGame * (int)Math.Ceiling(numGames)).ToList();
			int index = 0;
			List<int> updatedArr = new List<int>();
			foreach (int el in arr)
			{
				updatedArr.Add((int)(index % numberOfTeams));
				index++;
			}
			List<List<int>> grid = updatedArr.ChunkBy((int)teamsPerGame);
			for (int i = 0; i < 5000; i++)
			{
				RandomiseGrid(grid);
			}


			return grid;

		}

		public List<List<int>> GetLeagueGrid(League league)
		{
			List<List<int>> grid = new List<List<int>>();
			foreach (Game game in league.AllGames)
			{
				List<int> row = new List<int>();
				foreach (GameTeam team in game.Teams)
				{
					row.Add(team.TeamId - 1 ?? -1);
				}
				grid.Add(row);
			}

			return grid;
		}

		public List<List<int>> PadPlaysToTeamNumber(int numberOfTeams, List<List<int>> plays)
		{
			Console.WriteLine("PadPlaysToTeamNumber {0}", numberOfTeams);
			List<List<int>> newGrid = plays.ConvertAll(row => row.ConvertAll(cell => cell));
			int teamPlays = newGrid.Count;
			if (numberOfTeams > teamPlays)
			{
				for (int i = 0; i < teamPlays; i++)
				{
					for (int j = teamPlays; j < numberOfTeams; j++)
					{
						newGrid[i].Add(0);
					}
				}
				for (int i = teamPlays; i < numberOfTeams; i++)
				{
					List<int> arr = Enumerable.Repeat(0, numberOfTeams).ToList();
					newGrid.Add(arr);
				}
				return newGrid;
			}
			else
			{
				return plays;
			}
		}

		/// <summary>Convert grid to FixtureGames.</summary>
		/// <param name="grid">Each value in the grid is an index into fixture.Teams.</param>
		void GridToFixtureGames(List<List<int>> grid, Fixture fixture, DateTime firstGame, TimeSpan timeBetweenGames)
		{
			var time = firstGame;
			foreach (List<int> row in grid)
			{
				var fg = new FixtureGame { Time = time };

				for (int i = 0; i < row.Count; i++)
				{
					var team = fixture.Teams[row[i]];

					if (fg.Teams.ContainsKey(team))
						Console.WriteLine("Team {0} (TeamId {1}, Colour {2}) is already in game {3}. Oh no!", team.ToString(), team.TeamId, Colours[i], fg.ToString());
					else
						fg.Teams.Add(team, Colours[i]);
				}
				fixture.Games.Add(fg);
				time = time.Add(timeBetweenGames);
			}
		}
	}
}

public static class ListExtensions
{
	public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
	{
		return source
			.Select((x, i) => new { Index = i, Value = x })
			.GroupBy(x => x.Index / chunkSize)
			.Select(x => x.Select(v => v.Value).ToList())
			.ToList();
	}
}