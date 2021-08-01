using UnityEngine;
using System.Collections.Generic;

public class Tile {
	public PrefabRecord info;
	double _entropy;
	HashSet<PrefabRecord> _acceptablePrefabs = new HashSet<PrefabRecord>();
	public ESideTypes[] neighborSides = new ESideTypes[ 5 ];

	//Getters and Setters
	public double entropy {
		get { return _entropy; }
	}
	//Changing this hashset is the key to running the entropy calclation and setting the info for the Tile.
	public HashSet<PrefabRecord> acceptablePrefabs {
		get { return _acceptablePrefabs; }
		set {
			calcEntropy();
			_acceptablePrefabs = value;
			//change the info as the only available prefab and the information for the current tile.
			if ( _acceptablePrefabs.Count <= 1 ) {
				info = _acceptablePrefabs.GetEnumerator().Current;
			}
		}
	}

	//Call this when the _acceptablePrefab set is changed.
	//Using the same entropy calculation that a WFC would use.
	/*
	* entropy = 
	*	log2(total sum of weights) - 
	*	( log2(weight 1) + log2(weight 3) + ... / 
	*	total sum of weights)
	* https://gridbugs.org/wave-function-collapse/
	*/
	private void calcEntropy() {

		if ( _acceptablePrefabs.Count <= 1 ) {
			_entropy = 0;
		} else {
			double totalSumWeights = 0;
			double totalSumLogWeights = 0;
			foreach ( PrefabRecord record in _acceptablePrefabs ) {
				totalSumWeights += record.TileWeight;
				totalSumLogWeights += Mathf.Log( (float)record.TileWeight, 2 );
			}
			float logTotal = Mathf.Log( (float)totalSumWeights, 2 );

			_entropy = logTotal - ( totalSumLogWeights / totalSumWeights ) + Random.Range( 0f, .2f );
		}
	}

	//Parameter to show what side you want on the array
	public ESideTypes? GetSideTypes( int num ) {

		if ( num > info.PrefabSidesType.Length - 1 || info == null ) {
			return null;
		} else {
			int index = num < info.rotation ?
				( info.PrefabSidesType.Length ) - ( info.rotation - num ) :
				( num - info.rotation );

			return info.PrefabSidesType[ index ];
		}
	}
}
