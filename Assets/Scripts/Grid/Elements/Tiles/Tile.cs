using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	protected Grid grid;
	protected SpriteRenderer img;

	public TileTypes type { get; set; }
	public bool visited { get; set; }
	public int x { get; set; }
	public int y { get; set; }
	public Color color { get; set; }
	

	public virtual void Init (Grid grid, TileTypes type, int x, int y, Color color) {
		this.grid = grid;
		this.type = type;
		this.x = x;
		this.y = y;
		this.color = color;

		// locate tile
		float ratio = grid.tileHeight / (float)grid.tileWidth;
		transform.localPosition = new Vector3(x, y * ratio, 0);

		// set tile image
		img = transform.Find("Sprite").GetComponent<SpriteRenderer>();
		img.sortingOrder = grid.height - y - 10;

		img.material.color = color;
	}


	public bool IsWalkable () {
		// tiles that contain obstacles are always unwalkable
		Entity entity = grid.GetEntity(x, y);
		if (entity != null) {
			if (entity is Obstacle) { 
				return false; 
			}
		}

		return true;
	}
}
