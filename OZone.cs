using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Torn
{
	/// <summary>
	/// This represents a P&C Micro's O-Zone lasergame database server. 
	/// It connects to O-Zone as if we were an O-Zone print server.
	/// </summary>
	public class OZone : LaserGameServer
	{
		protected string server;
		protected string port;

		private List<ServerGame> serverGames = new List<ServerGame>();
		private List<LaserGamePlayer> laserPlayers = new List<LaserGamePlayer>();

		protected OZone() { }

		public OZone(string _server, string _port)
		{
			server = _server;
			port = _port;
		}

		public override List<ServerGame> GetGames()
		{
			string textToSend = "{\"command\": \"list\"}";
			string result = QueryServer(textToSend);


			List<ServerGame> games = new List<ServerGame>();

			string cleanedResult = result.Remove(0, 5);

			Console.WriteLine(cleanedResult);

			JObject root = JObject.Parse(cleanedResult);

			JToken gameList = root.SelectToken("$.gamelist");

			foreach (JObject jgame in gameList.Children())
			{
				var game = new ServerGame();
				if (jgame["gamenum"] != null)   game.GameId = Int32.Parse(jgame["gamenum"].ToString());
				if (jgame["gamename"] != null)   game.Description = jgame["gamename"].ToString();
				if (jgame["starttime"] != null)
				{
					string dateTimeStr = jgame["starttime"].ToString();
					game.Time = DateTime.Parse(dateTimeStr,
						System.Globalization.CultureInfo.InvariantCulture);
				}
				if (jgame["endtime"] != null)
				{
					try
					{
						string dateTimeStr = jgame["endtime"].ToString();
						game.EndTime = DateTime.Parse(dateTimeStr,
							System.Globalization.CultureInfo.InvariantCulture);
					} catch
                    {
						string dateTimeStr = jgame["starttime"].ToString();
						game.EndTime = DateTime.Parse(dateTimeStr,
							System.Globalization.CultureInfo.InvariantCulture);
					}
				}
				if (jgame["valid"] != null)
				{
					int isValid = Int16.Parse(jgame["valid"].ToString());
					if (isValid > 0) game.OnServer = true;
					else game.OnServer = false;
				}
				games.Add(game);
				serverGames.Add(game);
			}
			
			return games;
		}

		string ReadFromOzone(TcpClient client, NetworkStream nwStream)
        {
			try
			{
				string str = "";
				bool reading = true;
				int BYTE_LIMIT = 1024;


				while (reading)
				{
					byte[] bytesToRead = new byte[BYTE_LIMIT];
					int bytesRead = nwStream.Read(bytesToRead, 0, BYTE_LIMIT);
					string current = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
					Console.WriteLine(bytesRead);

					str += current;

					if (bytesRead < BYTE_LIMIT) reading = false;
				}

				return str;
			}
			catch
			{
				return "";
			}
		}

		private string QueryServer(string query)
		{
			//---create a TCPClient object at the IP and port no.---
			TcpClient client = new TcpClient(server, Int32.Parse(port));
			NetworkStream nwStream = client.GetStream();
			byte[] messageBytes = ASCIIEncoding.ASCII.GetBytes("(" + query);
			Thread.Sleep(1);

			nwStream.ReadTimeout = 1000;

			while (true)
            {

				string data = ReadFromOzone(client, nwStream);
				if(data == "")
                {
					break;
                }
            }



			int[] header = new int[] { query.Length, 0, 0, 0 };

			byte[] bytesToSend = new byte[header.Length + messageBytes.Length];
			System.Buffer.BlockCopy(header, 0, bytesToSend, 0, header.Length);
			System.Buffer.BlockCopy(messageBytes, 0, bytesToSend, header.Length, messageBytes.Length);

			Console.WriteLine("HERE");
			//---send the text---
			nwStream.Write(bytesToSend, 0, bytesToSend.Length);

			//---read back the text---
			string result = "";
			while (true)
			{

				string data = ReadFromOzone(client, nwStream);
				result += data;
				if (data == "")
				{
					break;
				}
			}

			Console.WriteLine(result);


			client.Close();

			return result;
		}

		public override void PopulateGame(ServerGame game)
		{
			if (!game.GameId.HasValue)
				return;

			if (game.Events.Count != 0)
				return;

			string textToSend = "{\"gamenumber\": " + game.GameId + ", \"command\": \"all\"}";
			string result = QueryServer(textToSend);
			string cleanedResult = result.Remove(0, 5);
			Console.WriteLine(cleanedResult);

			JObject root = JObject.Parse(cleanedResult);

			if (root["events"] != null)
			{
				string eventsStr = root["events"].ToString();
				var eventsDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(eventsStr);

				foreach (var evnt in eventsDictionary)
				{
					string eventContent = evnt.Value.ToString();
					JObject eventRoot = JObject.Parse(eventContent);

					int eventTime = 0;
					string eventPlayerId = "";
					int eventPlayerTeamId = -1;
					int eventType = -1;
					int score = 0;
					string eventOtherPlayerId = "";
					int eventOtherPlayerTeamId = -1;
					if (eventRoot["time"] != null) eventTime = Int32.Parse(eventRoot["time"].ToString());
					if (eventRoot["idf"] != null) eventPlayerId = eventRoot["idf"].ToString();
					if (eventRoot["tidf"] != null) eventPlayerTeamId = Int32.Parse(eventRoot["tidf"].ToString());
					if (eventRoot["evtyp"] != null) eventType = Int32.Parse(eventRoot["evtyp"].ToString());
					if (eventRoot["score"] != null) score = Int32.Parse(eventRoot["score"].ToString());
					if (eventRoot["ida"] != null) eventOtherPlayerId = eventRoot["ida"].ToString();
					if (eventRoot["tida"] != null) eventOtherPlayerTeamId = Int32.Parse(eventRoot["tida"].ToString());

					var gameEvent = new Event
					{
						Time = game.Time.AddSeconds(eventTime),
						ServerPlayerId = eventPlayerId,
						ServerTeamId = eventPlayerTeamId,
						Event_Type = eventType,
						Score = score,
						OtherPlayer = eventOtherPlayerId,
						OtherTeam = eventOtherPlayerTeamId,
					};
					game.Events.Add(gameEvent);
				}


			}

			if (root["players"] != null)
			{
				string playersStr = root["players"].ToString();
				var playersDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(playersStr);

				foreach(var player in playersDictionary)
                {
					string playerContent = player.Value.ToString();
					JObject playerRoot = JObject.Parse(playerContent);

					ServerPlayer serverPlayer = new ServerPlayer();
					if (playerRoot["alias"] != null) serverPlayer.Alias = playerRoot["alias"].ToString();
					if (playerRoot["score"] != null) serverPlayer.Score = Int32.Parse(playerRoot["score"].ToString());
					if (playerRoot["omid"] != null) 
					{ 
						serverPlayer.PlayerId = playerRoot["omid"].ToString(); 
						serverPlayer.ServerPlayerId = playerRoot["omid"].ToString(); 
					};
					if (playerRoot["tid"] != null)
					{
						serverPlayer.TeamId = Int32.Parse(playerRoot["tid"].ToString());
						serverPlayer.ServerTeamId = Int32.Parse(playerRoot["tid"].ToString());
						if (0 <= serverPlayer.ServerTeamId && serverPlayer.ServerTeamId < 8)
							serverPlayer.Colour = (Colour)(serverPlayer.ServerTeamId + 1);
						else
							serverPlayer.Colour = Colour.None;
					}
					if(!serverPlayer.IsPopulated()) serverPlayer.Populate(game.Events);

					game.Players.Add(serverPlayer);

				}


			}
				

		}

		public override List<LaserGamePlayer> GetPlayers(string mask)
		{

			GetGames();
			foreach (ServerGame game in serverGames)
			{
				string textToSend = "{\"gamenumber\": " + game.GameId + ", \"command\": \"all\"}";
				string result = QueryServer(textToSend);
				string[] separatingStrings = { "}{" };
				string[] objects = result.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

				string gameData = objects[0] + "}";


				JObject root = JObject.Parse(gameData);

				if (root["players"] != null)
				{
					string playersStr = root["players"].ToString();
					var playersDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(playersStr);

					foreach (var player in playersDictionary)
					{
						string playerContent = player.Value.ToString();
						JObject playerRoot = JObject.Parse(playerContent);

						LaserGamePlayer laserPlayer = new LaserGamePlayer();
						if (playerRoot["alias"] != null) laserPlayer.Alias = playerRoot["alias"].ToString();
						if (playerRoot["omid"] != null) laserPlayer.Id = playerRoot["omid"].ToString();
						if(laserPlayers.Find((p) => p.Id == laserPlayer.Id) == null) laserPlayers.Add(laserPlayer);
					}
				}

			}
			
			return laserPlayers;
		}
	}
}
