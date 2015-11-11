using UnityEngine;
using System.Collections;


public class LevelManager : MonoBehaviour {

	private GameController game;
	private GameGrid grid;
	private Hud hud;


	public void Init (GameController game, GameGrid grid, Hud hud) {
		this.game = game;
		this.grid = grid;
		this.hud = hud;
	}


	// ===================================================
	// Button Triggers
	// ===================================================

	public void ButtonSave () {
		hud.ShowPopupLevels(this, "save");
	}


	public void ButtonLoad () {
		hud.ShowPopupLevels(this, "load");
	}


	// ===================================================
	// Update levels index
	// ===================================================

	public void UpdateLevelsIndex (string name) {
		JSONObject json = JsonFileManagerSync.LoadJsonFile("Data/LevelsIndex");

		if (json == null) {
			json = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject arr = new JSONObject(JSONObject.Type.ARRAY);
			json.AddField("levels", arr);
		}

		JSONObject levels = json["levels"];

		for (int i = 0; i < levels.list.Count; i++) {
			if (levels.list[i]["name"].str == name) {
				return;
			}
		}

		JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
		j.AddField("name", name);
		levels.Add(j);

		// save json data to file
		JsonFileManagerSync.SaveJsonFile("Data/LevelsIndex", json);
	}

	// ===================================================
	// Save Level
	// ===================================================

	public void SaveFile (string name) {
		// Note: your data can only be numbers and strings.
		JSONObject data = new JSONObject(JSONObject.Type.OBJECT);

		// level name
		data.AddField("name", name);

		// tiles
		JSONObject tiles = new JSONObject(JSONObject.Type.ARRAY);
		data.AddField("tiles", tiles);

		// obstacles
		JSONObject obstacles = new JSONObject(JSONObject.Type.ARRAY);
		data.AddField("obstacles", obstacles);

		// items
		JSONObject items = new JSONObject(JSONObject.Type.ARRAY);
		data.AddField("items", items);

		// stars
		JSONObject stars = new JSONObject(JSONObject.Type.ARRAY);
		data.AddField("stars", stars);

		// players
		JSONObject players = new JSONObject(JSONObject.Type.ARRAY);
		data.AddField("players", players);


		// initialize grid tiles
		for (int y = 0; y < grid.height; y++) {
			tiles.Add(new JSONObject(JSONObject.Type.ARRAY));
			obstacles.Add(new JSONObject(JSONObject.Type.ARRAY));
			items.Add(new JSONObject(JSONObject.Type.ARRAY));
			stars.Add(new JSONObject(JSONObject.Type.ARRAY));
			players.Add(new JSONObject(JSONObject.Type.ARRAY));

			for (int x = 0; x < grid.width; x++) {

				// write tiles
				Tile tile = grid.GetTile(x, y);
				tiles[y].Add(((int)tile.type + 1));

				// write entities
				Entity entity = grid.GetEntity(x, y);

				obstacles[y].Add(entity is Obstacle ? (int)((Obstacle)entity).type + 1 : 0);
				items[y].Add(entity is Item ? 1 : 0);
				stars[y].Add(entity is Star ? 1 : 0);
				players[y].Add(entity is Player ? 1 : 0);
			}
		}

		//save json data to file
		JsonFileManagerSync.SaveJsonFile("Data/Levels/" + name, data);

		// and update the levels index file
		UpdateLevelsIndex(data["name"].str);
	}


	// ===================================================
	// Load Level
	// ===================================================

	public void LoadFile (string fileName) {
		// load json data from file
		JSONObject json = JsonFileManagerSync.LoadJsonFile("Data/Levels/" + fileName);

		// and generate the level from it
		if (json != null) {
			GenerateLevelFromData(json);
		}
	}


	private void GenerateLevelFromData (JSONObject j) {
		// first, lets reset our grid
		grid.InitializeGrid();

		// iterate on grid bidimensional array
		for (int y = 0; y < grid.height; y++) {
			for (int x = 0; x < grid.width; x++) {

				// generate tiles
				Tile tile = grid.GetTile(x, y);
				TileTypes tileType = (TileTypes) ( (int)(j["tiles"][y][x].n) - 1 );
				grid.ChangeTile(tile, tileType);


				// generate obstacles
				int i = (int)(j["obstacles"][y][x].n);
				if (i > 0) {
					grid.CreateObstacle(x, y, (ObstacleTypes)(i - 1));
				}

				// generate items
				if ((int)(j["items"][y][x].n) > 0) {
					grid.CreateItem(x, y);
				}

				// generate stars
				if ((int)(j["stars"][y][x].n) > 0) {
					grid.CreateStar(x, y);
				}

				// generate players
				if ((int)(j["players"][y][x].n) > 0) {
					game.player = grid.CreatePlayer(x, y);
				}
			}
		}

		// set game listeners
		if (game.player) {
			game.SetPlayerListeners();
		}
	}

}
