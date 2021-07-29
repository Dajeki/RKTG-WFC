using System.Collections.Generic;
using System;
using UnityEngine;

public class Level : MonoBehaviour, IWaveCol {
	public int width = 1;
	public int height = 1;
	public int numAvailPrefabs = 0;
	public GameObject[] availablePrefabs = new GameObject[ 0 ];
	public double[] tileWeights = new double[ 0 ];
	public Tile[][] levelGrid;

	public void propogateGrid( int sideNumber ) {
		throw new System.NotImplementedException();
	}

	//Determine which tile or tiles has the least entropy to find the next place to start.
	public Tile[] getLeastEntropic() {
		List<Tile> leastEntropicTiles = new List<Tile>();
		double? leastVal = null;

		foreach( Tile[] row in levelGrid ) {
			foreach( Tile cell in row ) {
				if( leastVal == null ) {
					leastVal = cell.entropy;
					leastEntropicTiles.Add( cell );
				} else if( ( cell.entropy - leastVal ) < 0.00001 ) {
					leastEntropicTiles.Add( cell );
				} else if( ( cell.entropy - leastVal ) > 0.00001 ) {
					leastEntropicTiles = new List<Tile>();
					leastVal = cell.entropy;
					leastEntropicTiles.Add( cell );
				}
			}
		}
		return leastEntropicTiles.ToArray();
	}

	//Look at constraints to handle starting superPosition
	public void initSuperposition() {
		throw new System.NotImplementedException();
	}
}
