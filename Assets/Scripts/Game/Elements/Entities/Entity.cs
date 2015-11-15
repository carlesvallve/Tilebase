using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {

	// properties

	protected AudioManager sfx;
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
		sfx = AudioManager.instance;

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
		// check interactions with next tile
		Vector2 inc = CheckTileAtCoords(x, y);
		if (inc == Vector2.zero) { return; }

		// update final coords
		x = this.x + (int)inc.x;
		y = this.y + (int)inc.y;

		// move to coords
		StartCoroutine(MoveToCoordsCoroutine(x, y, duration));
	}


	protected virtual Vector2 CheckTileAtCoords (int x, int y) {
		int dx = x - this.x;
		int dy = y - this.y;

		// check obstacles
		if (!grid.GetTile(this.x + dx, this.y + dy).IsWalkable()) { 
			if (!grid.GetTile(this.x + dx, this.y).IsWalkable()) { dx = 0; }
			if (!grid.GetTile(this.x, this.y + dy).IsWalkable()) { dy = 0; }
			if (dx != 0  && dy != 0) {
				int r = Random.Range(1, 100);
				if (r < 50) { dx = 0; } else { dy = 0; }
			}
		}

		// check doors
		x = this.x + dx;
		y = this.y + dy;
		Entity entity = grid.GetEntity(x, y);
		if (entity != null && (entity is Door)) {
			Door door = (Door)entity;
			if (door.state == DoorStates.Closed) {
				door.Open();
				return new Vector2(0, 0);
			}
		}

		// return increments
		return new Vector2(dx, dy);
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

		sfx.Play("Audio/Sfx/Step/step", 1f, Random.Range(0.8f, 1.2f));
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
}


