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
		for( int i = 0 ; i < levelGrid.Length ; i++ ) {
			levelGrid[ i ] = new Tile[ width ];
			for( int j = 0 ; j < levelGrid[ i ].Length ; j++ ) {

				PrefabRecord[] prefabsWithEmptySides = null;
				levelGrid[ i ][ j ] = new Tile();
				//If row is even and column is 0
				if( i % 2 == 0 && j == 0 ) {
					//if row is 0 or if the row is last make sure we have at least 3 empty sides
					/*
						* TODO: MAKE SURE THIS BOOLEAN CHECKS OUT. 
						*/
					if( i == 0 || i == levelGrid.Length - 1 ) {

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
				else if( i % 2 == 0 && j == levelGrid[ i ].Length - 1 ) {

					//if row is 0 or if the row is last make sure we have at least 4 empty sides
					if( i == 0 || i == levelGrid.Length - 1 ) {

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
				else if( i % 2 != 0 && j == 0 ) {

					//NEEDS 4 SIDES
					//if an odd row is last, its first index needs 4 empty sides
					if( i == levelGrid.Length - 1 ) {
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
				else if( i % 2 != 0 && j == levelGrid[ i ].Length - 1 ) {
					//if the odd row is last then the last column needs 3 empty sides
					if( i == levelGrid.Length - 1 ) {
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

				else if( ( i == 0 || i == levelGrid.Length - 1 ) && ( j != 0 || j != levelGrid[ i ].Length ) ) {
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
					levelGrid[ i ][ j ].acceptablePrefabs = new HashSet<PrefabRecord>( PrefabRecords );
				} else {
					levelGrid[ i ][ j ].acceptablePrefabs = new HashSet<PrefabRecord>( prefabsWithEmptySides );
				}

				PrefabRecord[] test = new PrefabRecord[ levelGrid[ i ][ j ].acceptablePrefabs.Count ];
				levelGrid[ i ][ j ].acceptablePrefabs.CopyTo( test );
				Debug.Log( string.Join<PrefabRecord>( ",", test ) );
			}
		}
	}
}
