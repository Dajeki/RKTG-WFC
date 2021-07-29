using UnityEngine;

public class Tile {
	public int TotalSides;
	public ESideTypes[] tileSideTypes;
	public GameObject model;
	public int rotation;
	public double entropy;

	public double calcEntropy() {
		throw new System.NotImplementedException();
	}
}
