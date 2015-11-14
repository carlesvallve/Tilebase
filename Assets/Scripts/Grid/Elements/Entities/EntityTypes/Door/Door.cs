using UnityEngine;
using System.Collections;

public class Door : Entity {

	public DoorTypes type { get; set; }

	public Sprite spriteHOpen;
	public Sprite spriteHClosed;
	public Sprite spriteVOpen;
	public Sprite spriteVClosed;


	public void SetDirection (DoorDirections direction) {
		if (direction == DoorDirections.Horizontal) {
			img.sprite = spriteHOpen;
		} else {
			img.sprite = spriteVOpen;
			img.transform.Translate(0, 0.2f, 0);
		}
	}
}
