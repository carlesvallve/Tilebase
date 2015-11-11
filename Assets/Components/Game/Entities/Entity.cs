using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {

	public delegate void MoveEndedHandler();
	public event MoveEndedHandler OnMoveEnded;

	public delegate void PickupCollectableHandler(Collectable collectable);
	public event PickupCollectableHandler OnPickupCollectable;

	public int x { get; set; }
	public int y { get; set; }
	protected int lastX = -1;
	protected int lastY = -1;
	protected bool moving = false;

	protected GameGrid grid;
	protected SpriteRenderer img;
	protected GameObject selector;

	public bool Selected {
		get {
			return selector.activeSelf;
		}
		set {
			selector.SetActive(value);
		}
	}


	public virtual void Init (GameGrid grid, int x, int y) {
		this.grid = grid;
		this.x = x;
		this.y = y;

		img = transform.Find("Sprite").GetComponent<SpriteRenderer>();

		grid.layers.Set<Entity>(y, x, this);

		selector = transform.Find("Selector").gameObject;
		selector.SetActive(false);

		LocateAtCoords(x, y);
	}


	public virtual void LocateAtCoords (int x, int y) {
		grid.SetEntity(this.x, this.y, null);

		this.x = x;
		this.y = y;

		float ratio = grid.tileHeight / (float)grid.tileWidth;
		transform.localPosition = new Vector3(x, 0.4f + y * ratio, 0);

		img.sortingOrder = grid.height - y;

		grid.SetEntity(x, y, this);
	}


	public virtual void MoveToCoords (int x, int y, float duration) {
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
		Tile endingTile = grid.GetTile(x, y);

		float ratio = grid.tileHeight / (float)grid.tileWidth;
		Vector3 startPos = new Vector3(this.x, 0.4f + this.y * ratio, 0);
		Vector3 endPos = new Vector3(x, 0.4f + y * ratio, 0);

		float t = 0f;
		while (t <= 1f) {
			t += Time.deltaTime / duration;
			transform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
			img.sortingOrder = grid.height - y;

			// highlight tiles
			Tile currentTile = grid.GetTile(transform.localPosition);
			if (currentTile != endingTile) { 
				Vector3 dir = (endPos - startPos).normalized * 0.1f;
				Vector3 pos = transform.localPosition - dir;
				if (startingTile == currentTile) { pos = transform.localPosition; }
				HighlightTileAtPos(pos);
			}

			// pick collectables
			PickupCollectableAtPos(transform.localPosition);
			
			yield return null;
		}

		this.x = x;
		this.y = y;
		transform.localPosition = endPos;

		EndMove();
	}


	protected void HighlightTileAtPos (Vector3 pos) {
		if (pos.x < 0 || pos.y < 0 || pos.x > grid.width - 1 || pos.y > grid.height - 1) {
			return;
		}

		Tile tile = grid.GetTile(pos);
		if (tile == null) { return; }

		x = tile.x;
		y = tile.y;
		if (x == lastX && y == lastY) { return; }
		lastX = x;
		lastY = y;

		tile.UpdateState();
	}


	protected void PickupCollectableAtPos (Vector3 pos) {
		Entity entity = grid.GetEntity(pos);
		if (entity == null) { return; }


		if (entity is Collectable) {
			Collectable collectable = (Collectable)entity;
			if (collectable.HasBeenPickedUp()) { return; }

			if (OnPickupCollectable != null) {
				OnPickupCollectable.Invoke (collectable);
			}
		}
	}


	protected void EndMove () {
		moving = false;
		grid.SetEntity(x, y, this);

		if (OnMoveEnded != null) {
			OnMoveEnded.Invoke ();
		}
	}
}


