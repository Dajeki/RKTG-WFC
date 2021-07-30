using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
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

public class Level : MonoBehaviour, IWaveCol {
	[Range( 0, 100 )]
	public int TileSides = 5;
	[Range( 1, 100 )]
	public int numAvailPrefabs = 0;
	[Range( 3, 100 )]
	public int width = 1;
	[Range( 3, 100 )]
	public int height = 1;
	public PrefabRecord[] PrefabRecords = new PrefabRecord[ 0 ];
	public Tile[][] levelGrid;

	public void Start() {
		initSuperposition();
	}

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

	//Look at constraints to handle starting "superPosition"
	public void initSuperposition() {
		levelGrid = new Tile[ height ][];
		//Loop through the level grid initializing new tiles with available starting Tiles
		for( int row = 0 ; row < levelGrid.Length ; row++ ) {
			levelGrid[ row ] = new Tile[ width ];
			for( int column = 0 ; column < levelGrid[ row ].Length ; column++ ) {

				PrefabRecord[] prefabsWithEmptySides = null;
				levelGrid[ row ][ column ] = new Tile();
				//If row is even and column is 0
				if( row % 2 == 0 && column == 0 ) {
					//if row is 0 or if the row is last make sure we have at least 3 empty sides
					/*
						* TODO: MAKE SURE THIS BOOLEAN CHECKS OUT. 
						*/
					if( row == 0 || row == levelGrid.Length - 1 ) {

						prefabsWithEmptySides = Array.FindAll<PrefabRecord>( PrefabRecords,
						prefab => {
							return Array.FindAll<ESideTypes>( prefab.PrefabSidesType,
								sideType => sideType == ESideTypes.Nothing
							).Length > 2;
						} );
					}
					//else we can find one with only one side on it.
					else {

						prefabsWithEmptySides = Array.FindAll<PrefabRecord>( PrefabRecords,
						prefab => {
							return Array.FindAll<ESideTypes>( prefab.PrefabSidesType,
								sideType => sideType == ESideTypes.Nothing
							).Length > 0;
						} );
					}
				}

				//If the row is even and the column is the last one
				else if( row % 2 == 0 && column == levelGrid[ row ].Length - 1 ) {

					//if row is 0 or if the row is last make sure we have at least 4 empty sides
					if( row == 0 || row == levelGrid.Length - 1 ) {

						prefabsWithEmptySides = Array.FindAll<PrefabRecord>( PrefabRecords,
						prefab => {
							return Array.FindAll<ESideTypes>( prefab.PrefabSidesType,
								sideType => sideType == ESideTypes.Nothing
							).Length > 3;
						} );
					} else {
						prefabsWithEmptySides = Array.FindAll<PrefabRecord>( PrefabRecords,
						prefab => {
							return Array.FindAll<ESideTypes>( prefab.PrefabSidesType,
								sideType => sideType == ESideTypes.Nothing
							).Length > 2;
						} );
					}
				}

				//If row is odd and column is 0
				else if( row % 2 != 0 && column == 0 ) {

					//NEEDS 4 SIDES
					//if an odd row is last, its first index needs 4 empty sides
					if( row == levelGrid.Length - 1 ) {
						prefabsWithEmptySides = Array.FindAll<PrefabRecord>( PrefabRecords,
						prefab => {
							return Array.FindAll<ESideTypes>( prefab.PrefabSidesType,
								sideType => sideType == ESideTypes.Nothing
							).Length > 3;
						} );
					}
					//NEEDS 3 SIDES
					//else you only need 1 empty for the first index.
					else {
						prefabsWithEmptySides = Array.FindAll<PrefabRecord>( PrefabRecords,
						prefab => {
							return Array.FindAll<ESideTypes>( prefab.PrefabSidesType,
								sideType => sideType == ESideTypes.Nothing
							).Length > 2;
						} );
					}
				}

				//the row is odd and column is the last one
				else if( row % 2 != 0 && column == levelGrid[ row ].Length - 1 ) {
					//if the odd row is last then the last column needs 3 empty sides
					if( row == levelGrid.Length - 1 ) {
						prefabsWithEmptySides = Array.FindAll<PrefabRecord>( PrefabRecords,
						prefab => {
							return Array.FindAll<ESideTypes>( prefab.PrefabSidesType,
								sideType => sideType == ESideTypes.Nothing
							).Length > 2;
						} );
					}
					//the column only needs one
					else {
						prefabsWithEmptySides = Array.FindAll<PrefabRecord>( PrefabRecords,
						prefab => {
							return Array.FindAll<ESideTypes>( prefab.PrefabSidesType,
								sideType => sideType == ESideTypes.Nothing
							).Length > 0;
						} );
					}
				}

				/*
					if it is the first row or if it is the last row and it is not the first column or last column
					NEEDS 2 SIDES
					*/

				else if( ( row == 0 || row == levelGrid.Length - 1 ) && ( column != 0 || column != levelGrid[ row ].Length ) ) {
					prefabsWithEmptySides = Array.FindAll<PrefabRecord>( PrefabRecords,
						prefab => {
							return Array.FindAll<ESideTypes>( prefab.PrefabSidesType,
								sideType => sideType == ESideTypes.Nothing
							).Length > 0;
						} );
				}

				/*
					if nothing else has been retreived then it can be anything so copy the whole prefabrecords
					*/
				if( prefabsWithEmptySides == null ) {
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
