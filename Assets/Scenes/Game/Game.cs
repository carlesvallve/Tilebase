using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* TODO:

- implement FOV system
- implement event system

- generate dungeon features:
	- monsters:
		- are organized in tribes (6 bugs, 3 goblins, 2 orcs, 1 giant...)
		- each monster type has a social stat that defines if is more likely to be in group or alone
		- monsters wont interact with chest, items or ladders for now
		- visible, memorized, and invisible monsters
		- memorized monsters will move each time player move
			- go towards the player
			- go towards the group
			- flee

	- chests: 
		must be in rooms + no obstacles that are not walls around
	- keys: 
		anywhere
	- items: 
		anywhere
	
	- living entities must mantain stats between levels 

	- player inventory:
		- grid list of items, some are selected, some arent
		- list of body parts and equipped item
*/


public class Game : MonoBehaviour {

	private Navigator navigator;
	private AudioManager sfx;
	private Hud hud;
	private Grid grid;
	private DungeonGenerator dungeonGenerator;
	private DungeonRenderer dungeonRenderer;


	private List<int> dungeonSeeds = new List<int>();
	private int currentDungeonLevel = -1;


	void Awake () {
		// Initialize singletons
		FpsCounter fps = FpsCounter.instance;
		fps.enabled = true;

		navigator = Navigator.instance;

		sfx = AudioManager.instance;
		sfx.Play("Audio/Bgm/Music/Alone", 0.5f, 1f, true);
		sfx.Play("Audio/Bgm/Ambient/BonusWind", 1f, 1f, true);
	}


	void Start () {
		// Get game components
		hud = GetComponent<Hud>();
		grid = GetComponent<Grid>();
		dungeonGenerator = GetComponent<DungeonGenerator>();
		dungeonRenderer = GetComponent<DungeonRenderer>();
		
		// initialize grid
		grid.Init();

		// generate dungeon
		GenerateDungeon(1);
	}


	void Update () {
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			StartCoroutine(ExitLevel(-1));
		}

		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			StartCoroutine(ExitLevel(1));
		}
	}


	public void Navigate (string sceneName) {
		sfx.Stop("Audio/Bgm/Ambient/BonusWind");
		navigator.Open("Home");
	}


	// ===============================================================
	// Dungeon Level Generation 
	// ===============================================================

	private IEnumerator ExitLevel (int direction) {
		yield return StartCoroutine(Navigator.instance.FadeOut(0.5f));
		GenerateDungeon(direction);
		yield return StartCoroutine(Navigator.instance.FadeIn(0.5f));
	}

	// (TODO: We may want to move this to a new class)

	public void GenerateDungeon (int direction) {
		// Update current dungeon level
		currentDungeonLevel += direction;
		
		// Set random seed
		int seed;
		if (currentDungeonLevel > dungeonSeeds.Count - 1) {
			// Set a random seed if we are entering a new dungeon level
			seed = System.DateTime.Now.Millisecond * 1000 + System.DateTime.Now.Minute * 100;
			dungeonSeeds.Add(seed);
		} else {
			// Recover a previously stored seed on current dungeon level
			seed = dungeonSeeds[currentDungeonLevel];
		}

		// Apply random seed
		dungeonGenerator.seed = seed;
		Random.seed = seed;
		
		// Generate dungeon data
		dungeonGenerator.GenerateDungeon(dungeonGenerator.seed);

		// Render dungeon on grid
		dungeonRenderer.Init(dungeonGenerator, grid);

		// Generate ladders
		GenerateLadders();

		// Generate player
		Ladder ladder = direction == 1 ? grid.ladderUp : grid.ladderDown;
		GeneratePlayer(ladder.x, ladder.y - 1);

		// Arrival feedback
		//print ("Welcome to dungeon level " + currentDungeonLevel + ".");
		hud.UpdateLog("Welcome to dungeon level " + (currentDungeonLevel + 1) + ".");
		hud.UpdateHeader(currentDungeonLevel);
		sfx.Play("Audio/Sfx/Musical/gong", 0.5f, Random.Range(1.0f, 2.5f));

		// Initialize game events
		InitializeGameEvents();

		
	}


	private void GeneratePlayer (int x, int y) {
		grid.player = grid.CreatePlayer(x, y, PlayerTypes.Player);
		grid.player.currentDungeonLevel = currentDungeonLevel;
	}


	private void GenerateLadders () {
		Tile tile = null;

		// locate ladderUp so it has no entities on 1 tile radius
		tile = GetRandomFreeTile(1);
		if (tile != null) {
			grid.ladderUp = grid.CreateLadder(tile.x, tile.y, LadderTypes.Wood, LadderDirections.Up);
		}

		// locate ladderDown so it has no entities on 1 tile radius
		tile = GetRandomFreeTile(1);
		if (tile != null) {
			grid.ladderDown = grid.CreateLadder(tile.x, tile.y, LadderTypes.Wood, LadderDirections.Down);
		}
	}


	private Tile GetRandomFreeTile (int radius = 0) {
		Tile tile = null;
		Tile tile2 = null;
		int c = 0;

		while (true) {
			tile = grid.GetTile(
				Random.Range(0, grid.width), 
				Random.Range(0, grid.height)
			);

			bool ok = false;

			if (tile != null && !tile.IsOccupied()) {
				ok = true;

				// iterate on all surounding tiles
				for (int i = 1; i <= radius; i++) {
					tile2 = grid.GetTile(tile.x - i, tile.y);
					if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { ok = false; }

					tile2 = grid.GetTile(tile.x + i, tile.y);
					if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { ok = false; }

					tile2 = grid.GetTile(tile.x, tile.y - i);
					if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { ok = false; }

					tile2 = grid.GetTile(tile.x, tile.y + i);
					if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { ok = false; }


					tile2 = grid.GetTile(tile.x - i, tile.y - i);
					if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { ok = false; }

					tile2 = grid.GetTile(tile.x + i, tile.y - i);
					if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { ok = false; }

					tile2 = grid.GetTile(tile.x - i, tile.y + i);
					if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { ok = false; }

					tile2 = grid.GetTile(tile.x + i, tile.y + i);
					if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { ok = false; }
				}
			}

			// escape if tile is placeable
			if (ok) {
				break;
			}
			
			c++;
			if (c == 100) {
				Debug.LogError("Tile could not be placed. Escaping...");
				tile = null;
				break;
			}
		}

		return tile;
	}


	// ===============================================================
	// Game Events
	// ===============================================================

	public void InitializeGameEvents () {
		grid.player.OnPickupItem += (Item item) => {
			hud.UpdateLog("You picked up an " + item);
		};

		grid.player.OnOpenDoor += (Door door) => {
			hud.UpdateLog("You opened the door.");
		};

		grid.player.OnExitLevel += (int direction) => {
			//GenerateDungeon (direction);
			StartCoroutine(ExitLevel(direction));
		};

		grid.player.OnMoveStart += () => {
			hud.UpdateLog("");
		};

		grid.player.OnMoveUpdate += () => {
			//hud.UpdateLog("");
		};

		grid.player.OnMoveEnd += () => {
			// move again if there are queued swipes
			grid.NextSwipe();
		};
	}


	/*public void SetPlayerListeners () {
		// pick up collectable
		player.OnPickupCollectable += (Collectable collectable) => {
			collectables.Add(collectable);
			hud.AddCollectable(collectable);

			collectable.Pickup();
		};

		// move update
		player.OnMoveUpdated += () => {
			UpdateVision();
		};

		// move end
		player.OnMoveEnded += () => {
			moves += 1;
			hud.UpdateMoves(moves);

			if (maxItems > 0) {
				int itemCount = GetCollected(typeof(Item));
				if (itemCount == maxItems) {
					StartCoroutine(GameWin());
					return;
				}
			}

			// move again if there are queued swipes
			grid.NextSwipe();
		};
	}


	private void UpdateVision() {
		// get lit array from shadowcaster class
		bool[,] lit = new bool[grid.width, grid.height];
		int radius = 5;
		ShadowCaster.ComputeFieldOfViewWithShadowCasting(
			player.x, player.y, radius,
			(x1, y1) => !grid.GetTile(x1, y1).IsWalkable(),
			(x2, y2) => { lit[x2, y2] = true; });

		// iterate grid tiles and render them
		for (int y = 0; y < grid.height; y++) {
			for (int x = 0; x < grid.width; x++) {
				// render tiles
				Tile tile = grid.GetTile(x, y);
				if (tile != null) {
					tile.gameObject.SetActive(lit[x, y] || tile.visited);
					Material tileMat = tile.transform.Find("Sprite").GetComponent<SpriteRenderer>().material;
					tileMat.color = lit[x, y] ? Color.white : Color.gray;

					// render entities
					Entity entity = grid.GetEntity(x, y);
					if (entity != null) {
						entity.gameObject.SetActive(lit[x, y] || tile.visited);
						Material entMat = entity.transform.Find("Sprite").GetComponent<SpriteRenderer>().material;
						entMat = tileMat;
					}

					// mark lit tiles as visited
					if (lit[x, y]) { 
						tile.visited = true; 
					}
				}
			}
		}
	}*/
}
