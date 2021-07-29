using UnityEditor;
[CustomEditor( typeof( Level ) )]
public class Test_Data_Editor : Editor {

	public override void OnInspectorGUI() {

		Level test_Data = target as Level;

		DrawDefaultInspector();
		test_Data.tileWeights = new double[ test_Data.numAvailPrefabs ];
		test_Data.availablePrefabs = new UnityEngine.GameObject[ test_Data.numAvailPrefabs ];

	}
}