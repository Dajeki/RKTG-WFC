using UnityEngine;
using System.Collections.Generic;

public class Tile {
	public PrefabRecord info;
	double _entropy;
	HashSet<PrefabRecord> _acceptablePrefabs = new HashSet<PrefabRecord>();

	public double entropy {
		get { return entropy; }
		protected set {
			_entropy = value;
		}
	}
	public HashSet<PrefabRecord> acceptablePrefabs {
		get { return _acceptablePrefabs; }
		set {
			calcEntropy();
			_acceptablePrefabs = value;
		}
	}

	//Call this when the _acceptablePrefab set is changed.
	private void calcEntropy() {
		double totalSumWeights = 0;
		double totalSumLogWeights = 0;
		foreach( PrefabRecord record in _acceptablePrefabs ) {
			totalSumWeights += record.TileWeight;
			totalSumLogWeights += Mathf.Log( (float)record.TileWeight, 2 );
		}
		float logTotal = Mathf.Log( (float)totalSumWeights, 2 );

		_entropy = logTotal - ( totalSumLogWeights / totalSumWeights ) + Random.Range( 0f, .2f );
	}

	//Parameter to show what side you want on the array
	public ESideTypes? GetSideTypes( int num ) {

		if( num > info.PrefabSidesType.Length - 1 || info == null ) {
			return null;
		} else {
			int index = num < info.rotation ?
				( info.PrefabSidesType.Length ) - ( info.rotation - num ) :
				( num - info.rotation );

			return info.PrefabSidesType[ index ];
		}
	}
}
