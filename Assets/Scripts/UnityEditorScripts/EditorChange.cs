using UnityEditor;
[CustomEditor( typeof( Level ) )]
public class Test_Data_Editor : Editor {

	public override void OnInspectorGUI() {

		Level test_Data = target as Level;

		DrawDefaultInspector();
		if( test_Data.numAvailPrefabs != test_Data.tileWeights.Length ) {
			test_Data.tileWeights = new double[ test_Data.numAvailPrefabs ];
		}
		if( test_Data.numAvailPrefabs != test_Data.availablePrefabs.Length ) {
			test_Data.availablePrefabs = new UnityEngine.GameObject[ test_Data.numAvailPrefabs ];
		}
	}
}