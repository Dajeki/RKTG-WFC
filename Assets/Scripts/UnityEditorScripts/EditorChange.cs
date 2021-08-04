using UnityEditor;
using System.Collections.Generic;
[CustomEditor( typeof( Level ) )]
public class Test_Data_Editor : Editor {

	public override void OnInspectorGUI() {

		Level test_Data = target as Level;

		DrawDefaultInspector();
		if ( test_Data.numAvailPrefabs != test_Data.PrefabRecords.Count ) {
			test_Data.PrefabRecords = new List<PrefabRecord>();
			for ( int i = 0 ; i < test_Data.numAvailPrefabs ; i++ ) {
				test_Data.PrefabRecords.Add( new PrefabRecord() );

				if ( test_Data.PrefabRecords[ i ].TileWeight - 0 < .000001 ) {
					test_Data.PrefabRecords[ i ].TileWeight = 1;
				}

			}
		}
		for ( int i = 0 ; i < test_Data.PrefabRecords.Count ; i++ ) {
			if ( test_Data.TileSides != test_Data.PrefabRecords[ i ].PrefabSidesType.Count ) {

				test_Data.PrefabRecords[ i ].PrefabSidesType = new List<ESideTypes>( test_Data.TileSides );
				for ( int j = 0 ; j < test_Data.TileSides ; j++ )
					test_Data.PrefabRecords[ i ].PrefabSidesType.Add( ESideTypes.Nothing );
			}
		}

	}
}