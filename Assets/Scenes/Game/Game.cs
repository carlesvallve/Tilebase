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
	private Dungeon dungeon;


	// ===============================================================
	// Game Initialization
	// ===============================================================

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
		dungeon = GetComponent<Dungeon>();

		// initialize grid
		grid.Init();

		// generate dungeon
		dungeon.GenerateDungeon(1);

		// Initialize events
		InitializeGameEvents();
		InitializeDungeonLevelEvents();
	}


	void Update () {
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			dungeon.ExitLevel(-1);
		}

		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			dungeon.ExitLevel(1);
		}
	}


	public void Navigate (string sceneName) {
		sfx.Stop("Audio/Bgm/Ambient/BonusWind");
		navigator.Open("Home");
	}


	// ===============================================================
	// Game Events
	// ===============================================================

	public void InitializeGameEvents () {
		dungeon.OnDungeonGenerated += (int dungeonLevel) => {
			print ("generated level" + dungeonLevel);
			sfx.Play("Audio/Sfx/Musical/gong", 0.5f, Random.Range(1.0f, 2.5f));
			hud.UpdateLog("Welcome to dungeon level " + (dungeonLevel + 1) + ".");
			hud.UpdateHeader(dungeonLevel);

			// re-initialize dungeon level events
			InitializeDungeonLevelEvents();
		};
	}


	public void InitializeDungeonLevelEvents () {
		// Dungeon Level handlers
		grid.player.OnDungeonEscape += () => {
			hud.UpdateLog("The door to the outside world is sealed...");
		};

		grid.player.OnExitLevel += (int direction) => {
			dungeon.ExitLevel(direction);
		};

		// Move handlers
		grid.player.OnMoveStart += () => {
			hud.UpdateLog("");
		};

		grid.player.OnMoveUpdate += () => {
		};

		grid.player.OnMoveEnd += () => {
			grid.NextSwipe(); // move again if there are queued swipes
		};

		// Interaction handlers
		grid.player.OnPickupItem += (Item item) => {
			hud.UpdateLog("You picked up an " + item);
		};

		grid.player.OnOpenDoor += (Door door) => {
			hud.UpdateLog("You opened the door.");
		};
	}


	/*private void UpdateVision() {
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
