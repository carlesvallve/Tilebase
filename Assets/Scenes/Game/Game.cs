using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* TODO:

- implement FOV system
- implement event system

- generate dungeon features:
	- up and down stairs
	- monsters
	- chests
	- keys
	- items
*/


public class Game : MonoBehaviour {

	private AudioManager sfx;
	private Grid grid;
	private DungeonGenerator dungeonGenerator;
	private DungeonRenderer dungeonRenderer;


	void Awake () {
		// Initialize singletons
		FpsCounter fps = FpsCounter.instance;
		fps.enabled = true;

		sfx = AudioManager.instance;
	}


	void Start () {
		// Get game components
		grid = GetComponent<Grid>();
		dungeonRenderer = GetComponent<DungeonRenderer>();
		dungeonGenerator = GetComponent<DungeonGenerator>();

		// initialize grid
		grid.Init();

		// generate dungeon
		GenerateDungeon();
	}


	void Update () {
		if (Input.GetKeyDown("space")) {
			GenerateDungeon();
		}
	}


	public void GenerateDungeon () {
		// Generate a new random seed
		dungeonGenerator.seed = System.DateTime.Now.Millisecond * 1000 + System.DateTime.Now.Minute * 100;
		Random.seed = dungeonGenerator.seed;
		
		// Generate dungeon data
		dungeonGenerator.GenerateDungeon(dungeonGenerator.seed);

		// Render dungeon on grid
		dungeonRenderer.Init(dungeonGenerator, grid);

		// Generate player
		GeneratePlayer();

		sfx.Play("Audio/Sfx/gong", 0.5f, Random.Range(0.5f, 2f));
	}


	private void GeneratePlayer () {
		int x = 0, y = 0, c = 0;

		while (true) {
			x = Random.Range(0, grid.width);
			y = Random.Range(0, grid.height);
			Tile tile = grid.GetTile(x, y);
			
			c++;
			if (c == 100 || (tile != null && tile.IsOccupied())) {
				break;
			}
		}

		grid.player = grid.CreatePlayer(x, y, PlayerTypes.Player);
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
