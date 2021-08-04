using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

[System.Serializable]
public class PrefabRecord {
	public GameObject Prefab;
	public List<ESideTypes> PrefabSidesType = new List<ESideTypes>( 6 );
	public double TileWeight;
	public int rotation = 0;

	public PrefabRecord( GameObject prefab, List<ESideTypes> prefabSideType, double tileWeight, int rotation ) {
		this.Prefab = prefab;
		this.PrefabSidesType = prefabSideType;
		this.TileWeight = tileWeight;
		this.rotation = rotation;
	}
	public PrefabRecord() {

	}

	public override string ToString() {
		return Prefab.name;
	}

	/// <summary>
	/// Gets the side type of the tile starting from the top right side of the hexegon going clockwise to the last index of 5 on the top left.
	/// </summary>
	/// <param name="num">The number of the side</param>
	/// <returns>The ESideType for sides of the hexagon (0 - 5)</returns>
	public ESideTypes GetSideType( int num ) {
		if ( num > 5 || num < 0 ) {
			throw new IndexOutOfRangeException( "Please enter a number from 0 - 5 into GetSideTypes" );
		} else {
			return PrefabSidesType[ num ];
		}
	}

	/// <summary>
	/// Creates a new PrefabRecord with its sides rotated.
	/// </summary>
	/// <param name="num">The number of rotations clockwise to shift the prefab</param>
	/// <returns>A new PrefabRecord with the sides rotated and the proper rotation set.</returns>
	public PrefabRecord GetPrefabRotated( int numRotations ) {

		List<ESideTypes> shiftedSides = new List<ESideTypes>( PrefabSidesType );
		for ( int i = 0 ; i < numRotations ; i++ ) {
			shiftedSides.Add( shiftedSides[ 0 ] );
			shiftedSides.RemoveAt( 0 );
		}

		return new PrefabRecord( Prefab, shiftedSides, TileWeight, numRotations );
	}
}


public class Level : MonoBehaviour {
	//[Range( 0, 100 )]
	public int TileSides = 6;
	[Range( 1, 100 )]
	public int numAvailPrefabs = 1;
	[Range( 4, 100 )]
	public int width = 1;
	[Range( 4, 100 )]
	public int height = 1;

	public List<PrefabRecord> PrefabRecords = new List<PrefabRecord>( 0 );
	//Setup an array version of all Prefabs
	private PrefabRecord[] ArrPrefabRecords;

	public Tile[][] levelGrid;

	//Random Position set in Start method
	[HideInInspector]
	public int startPositionRow;
	[HideInInspector]
	public int startPositionColumn;

	public void Start() {
		startPositionRow = UnityEngine.Random.Range( 1, height );
		startPositionColumn = UnityEngine.Random.Range( 1, width );

		//Get all the rotations for all the prefabs in the PrefabRecords
		foreach ( PrefabRecord prefab in PrefabRecords ) {
			for ( int i = 1 ; i < TileSides ; i++ ) {
				PrefabRecords.Add( prefab.GetPrefabRotated( i ) );
			}
		}

		ArrPrefabRecords = PrefabRecords.ToArray();

		initSuperposition();
	}

	public void propogateGrid() {



	}

	//Determine which tile or tiles has the least entropy to find the next place to start.
	//If you get null, it means that all the entropy in the map is 0 and we are finished propogating.
	public Tile getLeastEntropic() {
		List<Tile> leastEntropicTiles = new List<Tile>();
		double? leastVal = null;

		foreach ( Tile[] row in levelGrid ) {
			foreach ( Tile cell in row ) {

				if ( cell.entropy != 0 ) {

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
		}

		if ( leastEntropicTiles.Count > 0 ) {
			//return a random select one from the least entropic values
			return leastEntropicTiles[ UnityEngine.Random.Range( 0, leastEntropicTiles.Count ) ];
		} else {
			return null;
		}

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
	/// <param name="numOfType6">Quantity requirement for the sixth type side.</param>
	/// <param name="sideType6">The side type to look for constrained to the number specified in previous parameter.</param>
	/// <returns>An array of all PrefabRecords that match the constraints set for the side requirements of the prefab</returns>
	public PrefabRecord[] findAllPrefabsWithNumType(
		int numOfType, ESideTypes sideType,
		int? numOfType2 = null, ESideTypes? sideType2 = null,
		int? numOfType3 = null, ESideTypes? sideType3 = null,
		int? numOfType4 = null, ESideTypes? sideType4 = null,
		int? numOfType5 = null, ESideTypes? sideType5 = null,
		int? numOfType6 = null, ESideTypes? sideType6 = null
	) {

		//Finds all prefabRecords where the prefab has enough of the side type we need
		List<PrefabRecord> prefabsAdheringToConstraints = new List<PrefabRecord>();

		foreach ( PrefabRecord prefab in PrefabRecords ) {
			Dictionary<ESideTypes, int> countOfSides = new Dictionary<ESideTypes, int>();
			foreach ( int i in Enum.GetValues( typeof( ESideTypes ) ) ) {
				countOfSides.Add( (ESideTypes)i, 0 );
			}

			foreach ( ESideTypes prefabSideType in prefab.PrefabSidesType ) {
				countOfSides[ prefabSideType ] = ++countOfSides[ prefabSideType ];
			}

			if ( countOfSides[ sideType ] == numOfType &&
				( ( numOfType2 != null && sideType2 != null ) ? countOfSides[ (ESideTypes)sideType2 ] == numOfType2 : true ) &&
				( ( numOfType3 != null && sideType3 != null ) ? countOfSides[ (ESideTypes)sideType3 ] == numOfType3 : true ) &&
				( ( numOfType4 != null && sideType4 != null ) ? countOfSides[ (ESideTypes)sideType4 ] == numOfType4 : true ) &&
				( ( numOfType5 != null && sideType5 != null ) ? countOfSides[ (ESideTypes)sideType5 ] == numOfType5 : true ) &&
				( ( numOfType6 != null && sideType6 != null ) ? countOfSides[ (ESideTypes)sideType6 ] == numOfType6 : true )
				) {
				prefabsAdheringToConstraints.Add( prefab );
			}

		}
		return prefabsAdheringToConstraints.ToArray();
	}


	public PrefabRecord[] findAllPrefabsWithTypeAtSide(
		int side, ESideTypes requiredType,
		int? side2 = null, ESideTypes? requiredType2 = null,
		int? side3 = null, ESideTypes? requiredType3 = null,
		int? side4 = null, ESideTypes? requiredType4 = null,
		int? side5 = null, ESideTypes? requiredType5 = null,
		int? side6 = null, ESideTypes? requiredType6 = null
	) {
		return PrefabRecords.FindAll( prefab => {
			return prefab.GetSideType( side ) == requiredType &&
				   ( ( side2 != null && requiredType2 != null ) ? prefab.GetSideType( (int)side2 ) == requiredType2 : true ) &&
				   ( ( side3 != null && requiredType3 != null ) ? prefab.GetSideType( (int)side3 ) == requiredType3 : true ) &&
				   ( ( side4 != null && requiredType4 != null ) ? prefab.GetSideType( (int)side4 ) == requiredType4 : true ) &&
				   ( ( side5 != null && requiredType5 != null ) ? prefab.GetSideType( (int)side5 ) == requiredType5 : true ) &&
				   ( ( side6 != null && requiredType6 != null ) ? prefab.GetSideType( (int)side6 ) == requiredType6 : true );
		} ).ToArray();
	}

	/*
		Look at constraints to handle starting "superPosition"
		Sets up Tiles and acceptable prefabs for those tiles. When setting the acceptable prefabs, the tile setter will also end up calculating the entropy. 
		Info stays set to null untill an appropriate tile is selected
	*/
	private void initSuperposition() {
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

					levelGrid[ row ][ column ].acceptablePrefabs = ArrPrefabRecords;
				} else {

					levelGrid[ row ][ column ].acceptablePrefabs = prefabsWithEmptySides;
				}

				//Randomly select a tile and set it to a random road tile to be the starting position for propogation
				PrefabRecord[] baseRoadPrefabs = findAllPrefabsWithNumType( 1, ESideTypes.RoadBase );
				int randomRoadBase = UnityEngine.Random.Range( 0, baseRoadPrefabs.Length );


				levelGrid[ startPositionRow ][ startPositionColumn ].acceptablePrefabs = new PrefabRecord[] { baseRoadPrefabs[ randomRoadBase ] };

				/*
				//Debug Output
				// PrefabRecord[] test = new PrefabRecord[ levelGrid[ row ][ column ].acceptablePrefabs.Count ];
				// levelGrid[ row ][ column ].acceptablePrefabs.CopyTo( test );
				// Debug.Log( string.Join<PrefabRecord>( ",", test ) );
				*/
			}
		}
	}
}
