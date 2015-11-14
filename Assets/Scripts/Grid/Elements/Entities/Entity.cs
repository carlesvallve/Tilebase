using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {

	// properties

	protected Grid grid;
	protected SpriteRenderer img;

	public int x { get; private set; }
	public int y { get; private set; }
	public Color color { get; set; }
	public bool moving { get; private set; }

	// events

	public delegate void MoveUpdatedHandler();
	public event MoveUpdatedHandler OnMoveUpdated;

	public delegate void MoveEndedHandler();
	public event MoveEndedHandler OnMoveEnded;

	public delegate void PickupItemHandler(Item item);
	public event PickupItemHandler OnPickupItem;

	

	public virtual void Init (Grid grid, int x, int y, Color color) {
		this.grid = grid;
		this.x = x;
		this.y = y;
		
		img = transform.Find("Sprite").GetComponent<SpriteRenderer>();
		SetColor(color);

		grid.layers.Set<Entity>(y, x, this);

		LocateAtCoords(x, y);
	}


	public virtual void SetColor (Color color) {
		this.color = color;
		img.material.color = color;
	}


	protected virtual void LocateAtCoords (int x, int y) {
		grid.SetEntity(this.x, this.y, null);

		this.x = x;
		this.y = y;

		float ratio = grid.tileHeight / (float)grid.tileWidth;
		transform.localPosition = new Vector3(x, 0.4f + y * ratio, 0);

		img.sortingOrder = grid.height - this.y;

		grid.SetEntity(x, y, this);
	}


	public virtual void MoveToCoords (int x, int y, float duration) {
		if (CheckForDoors(x, y) != null) { return; }
		StartCoroutine(MoveToCoordsCoroutine(x, y, duration));
	}


	protected virtual IEnumerator MoveToCoordsCoroutine(int x, int y, float duration) {
		if (moving) { yield break; }

		if (x == this.x && y == this.y) {
			yield break;
		}

		grid.SetEntity(this.x, this.y, null);
		moving = true;

		Tile startingTile = grid.GetTile(transform.localPosition);

		float ratio = grid.tileHeight / (float)grid.tileWidth;
		Vector3 startPos = new Vector3(this.x, 0.4f + this.y * ratio, 0);
		Vector3 endPos = new Vector3(x, 0.4f + y * ratio, 0);

		Tile lastTile = startingTile;

		// anticipate zorder if we are moving downwards
		if (y - this.y < 0) { img.sortingOrder = grid.height - this.y + 1; }

		float t = 0f;
		while (t <= 1f) {
			t += Time.deltaTime / duration;
			transform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, Mathf.SmoothStep(0f, 1f, t)));
			
			// check for move update when entity changes tile in grid
			lastTile = CheckForMoveUpdate(lastTile);

			yield return null;
		}

		// update zorder
		img.sortingOrder = grid.height - this.y;

		this.x = x;
		this.y = y;
		transform.localPosition = endPos;

		yield return null;

		moving = false;
		grid.SetEntity(x, y, this);

		if (OnMoveEnded != null) {
			OnMoveEnded.Invoke ();
		}
	}


	protected Tile CheckForMoveUpdate (Tile lastTile) {
		Tile newTile = grid.GetTile(transform.localPosition);
		if (newTile != lastTile) {

			this.x = newTile.x;
			this.y = newTile.y;

			// pick collectables
			PickupItemAtPos(transform.localPosition);

			// update vision...

			// emit event
			if (OnMoveUpdated != null) {
				OnMoveUpdated.Invoke ();
			}

			lastTile = grid.GetTile(transform.localPosition);
		}

		return lastTile;
	}


	protected void PickupItemAtPos (Vector3 pos) {
		Entity entity = grid.GetEntity(pos);
		if (entity == null) { return; }


		if (entity is Item) {
			Item item = (Item)entity;
			if (item.HasBeenPickedUp()) { return; }

			if (OnPickupItem != null) {
				OnPickupItem.Invoke (item);
			}
		}
	}


	public Door CheckForDoors (int x, int y) {
		Entity entity = grid.GetEntity(x, y);
		if (entity != null && (entity is Door)) {
			Door door = (Door)entity;
			if (door.state == DoorStates.Closed) {
				door.Open();
				return door;
			}
		}

		return null;
	}
}


