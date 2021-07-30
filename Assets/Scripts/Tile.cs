using UnityEngine;
using System.Collections.Generic;

public class Tile {
	public PrefabRecord info;
	public double entropy;
	public HashSet<PrefabRecord> acceptablePrefabs = new HashSet<PrefabRecord>();

	public double calcEntropy() {


		throw new System.NotImplementedException();
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
