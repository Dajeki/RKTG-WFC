using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
//setup
public class PrefabRecord {
	public GameObject Prefab;
	public ESideTypes[] PrefabSidesType = new ESideTypes[ 0 ];
	public double TileWeight;
	//Integer numbers from 0 - numSides
	public int rotation;

	public override string ToString() {
		return Prefab.name;
	}
}

//These are clamped in the unity editor up to 100. Must maintain range or might cause overflow
public class intVector2Key {
	int x = 0;
	int y = 0;

	public override int GetHashCode() {
		return Int32.Parse( $"{x}{y}" );
	}

	public override bool Equals( object obj ) {
		if ( obj == null || !( obj is intVector2Key ) )
			return false;
		else
			return x == ( (intVector2Key)obj ).x && y == ( (intVector2Key)obj ).y;
	}

	public override string ToString() {
		return $"[ {x},{y} ]";
	}
}


public class Level : MonoBehaviour, IWaveCol {
	//[Range( 0, 100 )]
	public readonly int TileSides = 5;
	[Range( 1, 100 )]
	public int numAvailPrefabs = 0;
	[Range( 4, 100 )]
	public int width = 1;
	[Range( 4, 100 )]
	public int height = 1;
	public PrefabRecord[] PrefabRecords = new PrefabRecord[ 0 ];
	public Tile[][] levelGrid;

	public void Start() {
		initSuperposition();
	}

	public void propogateGrid() {
		//TODO: Choose a random number in the grid.
		int randomX = 0;
		int randomY = 0;

		PrefabRecord[] baseRoadPrefabs = findAllPrefabsWithNumType( 1, ESideTypes.RoadBase );
		int randomRoadBase = UnityEngine.Random.Range( 0, baseRoadPrefabs.Length );


		levelGrid[ randomX ][ randomY ].acceptablePrefabs = new HashSet<PrefabRecord>( new PrefabRecord[] { baseRoadPrefabs[ randomRoadBase ] } );
	}

	//Determine which tile or tiles has the least entropy to find the next place to start.
	public Tile[] getLeastEntropic() {
		List<Tile> leastEntropicTiles = new List<Tile>();
		double? leastVal = null;

		foreach ( Tile[] row in levelGrid ) {
			foreach ( Tile cell in row ) {
				if ( leastVal == null ) {
					leastVal = cell.entropy;
					leastEntropicTiles.Add( cell );
				} else if ( ( cell.entropy - leastVal ) < 0.00001 ) {
					leastEntropicTiles.Add( cell );
				} else if ( ( cell.entropy - leastVal ) > 0.00001 ) {
					leastEntropicTiles = new List<Tile>();
					leastVal = cell.entropy;
					leastEntropicTiles.Add( cell );
				}
			}
		}
		return leastEntropicTiles.ToArray();
	}


	//REFACTOR: Try to find a way to not recreate an array each time the constraint prefabs is reduced.
	/// <summary>
	/// Used to get all prefabs that will abide by the constraints given to it
	/// </summary>
	/// <param name="numOfType">Quantity requirement for the first type side.</param>
	/// <param name="sideType">The side type to look for constrained to the number specified in previous parameter. </param>
	/// <param name="numOfType2">Quantity requirement for the second type side.</param>
	/// <param name="sideType2">The side type to look for constrained to the number specified in previous parameter.</param>
	/// <param name="numOfType3">Quantity requirement for the third type side.</param>
	/// <param name="sideType3">The side type to look for constrained to the number specified in previous parameter.</param>
	/// <param name="numOfType4">Quantity requirement for the fourth type side.</param>
	/// <param name="sideType4">The side type to look for constrained to the number specified in previous parameter.</param>
	/// <param name="numOfType5">Quantity requirement for the fifth type side.</param>
	/// <param name="sideType5">The side type to look for constrained to the number specified in previous parameter.</param>
	/// <returns>An array of all PrefabRecords that match the constraints set for the side requirements of the prefab</returns>
	public PrefabRecord[] findAllPrefabsWithNumType(
		int numOfType, ESideTypes sideType,
		int? numOfType2 = null, ESideTypes? sideType2 = null,
		int? numOfType3 = null, ESideTypes? sideType3 = null,
		int? numOfType4 = null, ESideTypes? sideType4 = null,
		int? numOfType5 = null, ESideTypes? sideType5 = null
		) {

		//Finds all prefabRecords where the prefab has enough of the side type we need
		PrefabRecord[] prefabsAdheringToConstraints =
			Array.FindAll<PrefabRecord>( PrefabRecords,
				prefab => {
					return Array.FindAll<ESideTypes>( prefab.PrefabSidesType,
						sideTypeOfPrefab => sideTypeOfPrefab == sideType
					).Length > numOfType - 1;
				} );

		if ( numOfType2 != null && sideType2 != null ) {
			prefabsAdheringToConstraints =
				Array.FindAll<PrefabRecord>( prefabsAdheringToConstraints,
				prefab => {
					return Array.FindAll<ESideTypes>( prefab.PrefabSidesType,
						sideTypeOfPrefab => sideTypeOfPrefab == sideType2
					).Length > numOfType2 - 1;
				} );
		}
		if ( numOfType3 != null && sideType3 != null ) {
			prefabsAdheringToConstraints =
				Array.FindAll<PrefabRecord>( prefabsAdheringToConstraints,
				prefab => {
					return Array.FindAll<ESideTypes>( prefab.PrefabSidesType,
						sideTypeOfPrefab => sideTypeOfPrefab == sideType3
					).Length > numOfType3 - 1;
				} );
		}
		if ( numOfType4 != null && sideType4 != null ) {
			prefabsAdheringToConstraints =
				Array.FindAll<PrefabRecord>( prefabsAdheringToConstraints,
				prefab => {
					return Array.FindAll<ESideTypes>( prefab.PrefabSidesType,
						sideTypeOfPrefab => sideTypeOfPrefab == sideType4
					).Length > numOfType4 - 1;
				} );
		}
		if ( numOfType5 != null && sideType5 != null ) {
			prefabsAdheringToConstraints =
				Array.FindAll<PrefabRecord>( prefabsAdheringToConstraints,
				prefab => {
					return Array.FindAll<ESideTypes>( prefab.PrefabSidesType,
						sideTypeOfPrefab => sideTypeOfPrefab == sideType5
					).Length > numOfType5 - 1;
				} );
		}

		return prefabsAdheringToConstraints;
	}

	/*
		Look at constraints to handle starting "superPosition"
		Sets up Tiles and acceptable prefabs for those tiles. When setting the acceptable prefabs, the tile setter will also end up calculating the entropy. 
		Info stays set to null untill an appropriate tile is selected
	*/
	public void initSuperposition() {
		levelGrid = new Tile[ height ][];
		//Loop through the level grid initializing new tiles with available starting Tiles
		for ( int row = 0 ; row < levelGrid.Length ; row++ ) {

			levelGrid[ row ] = new Tile[ width ];
			for ( int column = 0 ; column < levelGrid[ row ].Length ; column++ ) {

				PrefabRecord[] prefabsWithEmptySides = null;
				levelGrid[ row ][ column ] = new Tile();
				//If row is even and column is 0
				if ( row % 2 == 0 && column == 0 ) {
					//if row is 0 or if the row is last make sure we have at least 3 empty sides
					if ( row == 0 || row == levelGrid.Length - 1 ) {

						prefabsWithEmptySides = findAllPrefabsWithNumType( 3, ESideTypes.Nothing );
					} else {

						prefabsWithEmptySides = findAllPrefabsWithNumType( 1, ESideTypes.Nothing );
					}
				}

				//If the row is even and the column is the last one
				else if ( row % 2 == 0 && column == levelGrid[ row ].Length - 1 ) {

					//if row is 0 or if the row is last make sure we have at least 4 empty sides
					if ( row == 0 || row == levelGrid.Length - 1 ) {

						prefabsWithEmptySides = findAllPrefabsWithNumType( 4, ESideTypes.Nothing );
					} else {

						prefabsWithEmptySides = findAllPrefabsWithNumType( 3, ESideTypes.Nothing );
					}
				}

				//If row is odd and column is 0
				else if ( row % 2 != 0 && column == 0 ) {

					//if an odd row is last, its first index needs 4 empty sides
					if ( row == levelGrid.Length - 1 ) {

						prefabsWithEmptySides = findAllPrefabsWithNumType( 4, ESideTypes.Nothing );
					}
					//else you need 3 empty for the first index of odd.
					else {

						prefabsWithEmptySides = findAllPrefabsWithNumType( 3, ESideTypes.Nothing );
					}
				}

				//the row is odd and column is the last one
				else if ( row % 2 != 0 && column == levelGrid[ row ].Length - 1 ) {
					//if the odd row is last then the last column needs 3 empty sides
					if ( row == levelGrid.Length - 1 ) {

						prefabsWithEmptySides = findAllPrefabsWithNumType( 3, ESideTypes.Nothing );
					}
					//the column only needs one
					else {

						prefabsWithEmptySides = findAllPrefabsWithNumType( 1, ESideTypes.Nothing );
					}
				}

				/*
					if it is the first row or if it is the last row and it is not the first column or last column
					NEEDS 2 SIDES
					*/
				else if ( ( row == 0 || row == levelGrid.Length - 1 ) && ( column != 0 || column != levelGrid[ row ].Length ) ) {

					prefabsWithEmptySides = findAllPrefabsWithNumType( 2, ESideTypes.Nothing );
				}

				/*
					if nothing else has been retreived then it can be anything so copy the whole prefabrecords
					*/
				if ( prefabsWithEmptySides == null ) {

					levelGrid[ row ][ column ].acceptablePrefabs = new HashSet<PrefabRecord>( PrefabRecords );
				} else {

					levelGrid[ row ][ column ].acceptablePrefabs = new HashSet<PrefabRecord>( prefabsWithEmptySides );
				}

				//Debug Output
				// PrefabRecord[] test = new PrefabRecord[ levelGrid[ row ][ column ].acceptablePrefabs.Count ];
				// levelGrid[ row ][ column ].acceptablePrefabs.CopyTo( test );
				// Debug.Log( string.Join<PrefabRecord>( ",", test ) );
			}
		}
	}
}
