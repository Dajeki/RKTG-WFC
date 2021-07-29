using System.Collections.Generic;
using System;

public class Level : IWaveCol {
	private int width;
	private int height;
	private double[] tileWeights;

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

	public void initSuperposition() {
		throw new System.NotImplementedException();
	}
}
