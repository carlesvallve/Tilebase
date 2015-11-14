using UnityEngine;
using System.Collections;
using System.IO;


public class DungeonRenderer : MonoBehaviour {

	public bool exportTexture = false; 

	private Grid grid;
	private DungeonGenerator generator;
	private Texture2D dungeonTexture;

	// =====================================================
	// Initialization
	// =====================================================

	public void Init (DungeonGenerator generator, Grid grid) {
		this.generator = generator;
		this.grid = grid;

		if (exportTexture) {
			dungeonTexture = DungeonToTexture();
			TextureToFile(dungeonTexture, "Dungeons/Dungeon" + generator.seed);
		}


		RenderDungeonToGrid();
	}

	// =====================================================
	// Render dungeon in the game grid
	// =====================================================

	private void RenderDungeonToGrid () {
		// init grid
		grid.InitializeGrid (generator.MAP_WIDTH, generator.MAP_HEIGHT);

		// generate grid elements for each tree quad
		GenerateGridOnTreeQuad(generator.quadTree);
		
		// batch all grid elements
		grid.BatchGrid();
	}


	private void GenerateGridOnTreeQuad (QuadTree _quadtree) {
		if (_quadtree.HasChildren() == false) {

			for (int y = _quadtree.boundary.BottomTile(); y <= _quadtree.boundary.TopTile() - 1; y++) {
				for (int x = _quadtree.boundary.LeftTile(); x <= _quadtree.boundary.RightTile() - 1; x++) {
					// get dungeon tile on the quadtree zone
					DungeonTile dtile = generator.tiles[y, x];

					// set render color 
					Color color = _quadtree.color;
	
					// create floors
					if (dtile.id == DungeonTileType.ROOM || dtile.id == DungeonTileType.CORRIDOR || 
						dtile.id == DungeonTileType.DOORH || dtile.id == DungeonTileType.DOORV) {
						grid.CreateTile(x, y, TileTypes.Floor, color);
					}

					// create walls
					if (dtile.id == DungeonTileType.WALL || dtile.id == DungeonTileType.WALLCORNER) {
						float d = 0.9f;
						Color wallColor = new Color(color.r * d, color.g * d, color.b * d, 1f);
						grid.CreateTile(x, y, TileTypes.Floor, wallColor);
						grid.CreateObstacle(x, y, ObstacleTypes.Wall, wallColor);
					}

					// create doors
					if (dtile.id == DungeonTileType.DOORH) {
						grid.CreateDoor(x, y, DoorTypes.Wood, DoorDirections.Horizontal);
					}

					if (dtile.id == DungeonTileType.DOORV) {
						grid.CreateDoor(x, y, DoorTypes.Wood, DoorDirections.Vertical);
					}
				}
			}
		} else {
			// Keep iterating on the quadtree
			GenerateGridOnTreeQuad(_quadtree.northWest);
			GenerateGridOnTreeQuad(_quadtree.northEast);
			GenerateGridOnTreeQuad(_quadtree.southWest);
			GenerateGridOnTreeQuad(_quadtree.southEast);
		}
	}


	// =====================================================
	// Render dungeon in a texture and save it as a png file
	// =====================================================

	private Texture2D DungeonToTexture() {
		Texture2D texOutput = new Texture2D((int)(generator.MAP_WIDTH), (int)(generator.MAP_HEIGHT), TextureFormat.ARGB32, false);
		PaintDungeonTexture(ref texOutput);
		texOutput.filterMode = FilterMode.Point;
		texOutput.wrapMode = TextureWrapMode.Clamp;
		texOutput.Apply();
		return texOutput;
	}


	private void PaintDungeonTexture(ref Texture2D t) {
		for (int i = 0; i < generator.MAP_WIDTH; i++) {
			for (int j = 0; j < generator.MAP_HEIGHT; j++) {
				switch (generator.tiles[j,i].id) {
				case DungeonTileType.EMPTY:
					t.SetPixel(i,j,Color.black);
					break;
				case DungeonTileType.ROOM:
					t.SetPixel(i,j,Color.white);
					break;
				case DungeonTileType.CORRIDOR:
					t.SetPixel(i,j,Color.grey);
					break;
				case DungeonTileType.WALL:
					t.SetPixel(i,j,Color.blue);
					break;
				case DungeonTileType.WALLCORNER:
					t.SetPixel(i,j,Color.blue);
					break;
				}
			}
		}
	}

	
	// Export a texture to a file
	private void TextureToFile(Texture2D t, string fileName) {
		string path = null;

		#if UNITY_EDITOR
			path = Application.dataPath + "/Resources/" + fileName + ".png";
		#else
			path = Application.persistentDataPath + "/" + fileName + ".png";  
		#endif

		print ("Saving dungeon texture at " + path);

		Directory.CreateDirectory(Path.GetDirectoryName(path));

		byte[] bytes = t.EncodeToPNG();
		FileStream myFile = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		myFile.Write(bytes,0,bytes.Length);
		myFile.Close();

		// refresh the editor
		#if UNITY_EDITOR
			UnityEditor.AssetDatabase.Refresh ();
		#endif
	}
}
