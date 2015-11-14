using UnityEngine;
using System.Collections;

public class Door : Entity {

	public DoorTypes type { get; set; }
	public DoorStates state { get; set; }
	public DoorDirections direction { get; set; }

	public Sprite spriteH;
	public Sprite spriteV;


	public void SetDirection (DoorDirections direction) {
		this.direction = direction;

		if (direction == DoorDirections.Horizontal) {
			img.sprite = spriteH;
		} else {
			img.sprite = spriteV;
			img.transform.Translate(0, 0.2f, 0);
		}
	}


	public void Open () {
		// TODO: call the spriteRenderer.material.setFloat(property, value) from code.
		//_MinX, _MaxX, _MinY, _MaxY

		// TODO: in the Update function the animation started flickering, 
		// and thats because the update was being called after the sprite is rendered, 
		// so I had to use the Main Camera OnPreRender event to update the material properties.
		
		this.state = DoorStates.Open;
		//img.sortingOrder = grid.height - this.y - 2;
		img.transform.Translate(GetOpendirection());
	}


	public void Close () {
		this.state = DoorStates.Closed;
		//img.sortingOrder = grid.height - this.y;
		img.transform.Translate(-GetOpendirection());
	}


	private Vector3 GetOpendirection () {
		float d ;
		Entity wall = null;
		if (direction == DoorDirections.Horizontal) {
			d = 0.75f;
			wall = grid.GetEntity(x, y - 1);
			if (wall != null && (wall is Obstacle) && ((Obstacle)wall).type == ObstacleTypes.Wall) {
				//wall.SetColor(Color.black);
				return new Vector3(0, -d, 0);
			}
			wall = grid.GetEntity(x, y + 1);
			if (wall != null && (wall is Obstacle) && ((Obstacle)wall).type == ObstacleTypes.Wall) {
				//wall.SetColor(Color.black);
				return new Vector3(0, d, 0);
			}
		} else {
			d = 0.9f;
			wall = grid.GetEntity(x - 1, y);
			if (wall != null && (wall is Obstacle) && ((Obstacle)wall).type == ObstacleTypes.Wall) {
				//wallTop.SetColor(Color.black);
				return new Vector3(-d, 0, 0);
			}
			wall = grid.GetEntity(x + 1, y);
			if (wall != null && (wall is Obstacle) && ((Obstacle)wall).type == ObstacleTypes.Wall) {
				//wallTop.SetColor(Color.black);
				return new Vector3(d, 0, 0);
			}
		}

		return Vector3.zero;
	}


	
}
