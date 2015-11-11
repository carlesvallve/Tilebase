using UnityEngine;
using System.Collections;

public class Tree : Obstacle {

	public override void Init (GameGrid grid, int x, int y) {
		base.Init(grid, x, y);
		type = ObstacleTypes.TREE;
	}
	
}
