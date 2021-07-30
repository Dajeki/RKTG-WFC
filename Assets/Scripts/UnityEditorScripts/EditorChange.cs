using UnityEditor;
[CustomEditor( typeof( Level ) )]
public class Test_Data_Editor : Editor {

	public override void OnInspectorGUI() {

		Level test_Data = target as Level;

		DrawDefaultInspector();
		if( test_Data.numAvailPrefabs != test_Data.PrefabRecords.Length ) {
			test_Data.PrefabRecords = new PrefabRecord[ test_Data.numAvailPrefabs ];
			for( int i = 0 ; i < test_Data.PrefabRecords.Length ; i++ ) {

				test_Data.PrefabRecords[ i ] = new PrefabRecord();

				if( test_Data.PrefabRecords[ i ].TileWeight - 0 < .000001 ) {
					test_Data.PrefabRecords[ i ].TileWeight = 1;
				}

			}
		}
		for( int i = 0 ; i < test_Data.PrefabRecords.Length ; i++ ) {
			if( test_Data.TileSides != test_Data.PrefabRecords[ i ].PrefabSidesType.Length ) {

				test_Data.PrefabRecords[ i ].PrefabSidesType = new ESideTypes[ test_Data.TileSides ];
			}
		}
	}
}