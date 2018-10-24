using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class Chunk : MonoBehaviour
{
	public static int chunkSize = 32; // Size of chunk in voxels

	private Voxel[,,] voxels;
	private MeshFilter meshFilter = null;
	private Mesh chunkMesh = null;
	private MeshData meshData;

	private World world;
	private Vector3Int chunkPos;

	private bool updateRequired = false; // Indicates whether the chunk's mesh needs to be re-calculated (ie. due to adding/deleting voxels)
	private bool updateFinished = false;

	void Start()
	{
		
	}
	
	void Update()
	{
		// If 'dirty', the chunk's mesh will be re-calculated once per frame, rather than after every single modification.
		if( updateRequired && World.worldGenState != World.WorldGenState.SettingVoxels )
		{
			updateRequired = false;
			StartCoroutine( "updateChunkCoroutine" );
		}
	}

	public void Initialize( World world, Vector3Int worldPos )
	{
		GetComponent<MeshRenderer>().sharedMaterial = (Material)Resources.Load( "Materials/VoxelMat" );
		voxels = new Voxel[chunkSize, chunkSize, chunkSize];

		this.world = world;
		this.chunkPos = worldPos;
	}

	public void SetVoxel( int x, int y, int z )
	{
		voxels[x,y,z] = new Voxel( "Grass", new Vector3Int(x,y,z) );
		updateRequired = true; // If the chunk is modified in any way, mark it as 'dirty', and its mesh will be re-calculated on the next frame.
	}

	public Voxel GetVoxel( int x, int y, int z )
	{
		if( (x < 0 || x >= chunkSize) || (y < 0 || y >= chunkSize) || (z < 0 || z >= chunkSize) )
		{
			// convert x, y, z from chunk position to world position
			//print( x + ", " + y + ", " + z);
			return world.GetVoxel( chunkPos.x + x, chunkPos.y + y, chunkPos.z + z );
		}
		else
		{
			return voxels[x,y,z];
		}
	}

	public Vector3Int GetPos()
	{
		return chunkPos;
	}

	private void updateChunk( object state )
	{
		try
		{
			//Voxel curVoxel;

			meshData = new MeshData();

			/*for( int x = 0; x < chunkSize; x++ )
			{
				for( int y = 0; y < chunkSize; y++ )
				{
					for( int z = 0; z < chunkSize; z++ )
					{
						curVoxel = voxels[x,y,z];

						if( curVoxel != null )
						{
							meshData = curVoxel.GetMeshData( x, y, z, chunkPos, meshData);
						}
					}
				}
			}*/

			generateChunkFaceTop( meshData );
			generateChunkFaceBottom( meshData );
			generateChunkFaceFront( meshData );
			generateChunkFaceBack( meshData );
			generateChunkFaceLeft( meshData );
			generateChunkFaceRight( meshData );

			meshData.CalculateTriangles();

			print( "Chunk update complete!" );
			updateFinished = true;
		}
		catch( Exception e )
		{
			print( e.ToString() );
		}
	}

	private void renderChunk(MeshData meshData )
	{
		if( meshFilter == null || chunkMesh == null )
		{
			meshFilter = GetComponent<MeshFilter>();
			chunkMesh = new Mesh();

			meshFilter.sharedMesh = chunkMesh;
		}

		chunkMesh.SetVertices( meshData.vertices );
		chunkMesh.SetTriangles( meshData.triangles, 0 );
		chunkMesh.SetUVs( 0, meshData.uvs );
		chunkMesh.SetColors( meshData.colors );

		chunkMesh.RecalculateBounds();
		chunkMesh.RecalculateNormals();
		chunkMesh.RecalculateTangents();
	}

	private IEnumerator updateChunkCoroutine()
	{
		while( World.worldGenState == World.WorldGenState.SettingVoxels )
		{
			yield return null;
		}

		ThreadPool.QueueUserWorkItem( new WaitCallback(updateChunk) );

		while( !updateFinished )
		{
			yield return null;
		}

		renderChunk(meshData);
	}


	//-----

	void generateChunkFaceTop( MeshData meshData )
	{
		bool[,,] wasMeshed = new bool[chunkSize, chunkSize, chunkSize];
		Voxel curVoxel;
		int i, j, k;

		for( int y = 0; y < chunkSize; y++ )
		{
			for( int x = 0; x < chunkSize; x++ )
			{
				for( int z = 0; z < chunkSize; z++ )
				{
					curVoxel = GetVoxel(x, y, z);

					if( curVoxel != null && !wasMeshed[x,y,z] && GetVoxel(x, y + 1, z) == null )
					{
						for( i = x + 1; i < chunkSize; i++ )
						{
							if( curVoxel.Equals(GetVoxel(i, y, z)) && !wasMeshed[i,y,z] && GetVoxel(i, y + 1, z) == null )
							{
								wasMeshed[i,y,z] = true;
							}
							else
							{
								break;
							}
						}
						i--;

						for( j = z + 1; j < chunkSize; j++ )
						{
							for( k = x; k <= i; k++ )
							{
								if( !curVoxel.Equals(GetVoxel(k, y, j)) || wasMeshed[k,y,j] || GetVoxel(k, y + 1, j) != null )
								{
									break;
								}
							}
							k--;

							if( k == i )
							{
								for( k = x; k <= i; k++ )
								{
									wasMeshed[k,y,j] = true;
								}
							}
							else
							{
								break;
							}
						}
						j--;

						meshData.vertices.Add( new Vector3(x, y + 1, z) );
						meshData.vertices.Add( new Vector3(x, y + 1, j + 1) );
						meshData.vertices.Add( new Vector3(i + 1, y + 1, j + 1) );
						meshData.vertices.Add( new Vector3(i + 1, y + 1, z) );

						meshData.colors.Add( curVoxel.voxelType.topColor );
						meshData.colors.Add( curVoxel.voxelType.topColor );
						meshData.colors.Add( curVoxel.voxelType.topColor );
						meshData.colors.Add( curVoxel.voxelType.topColor );
					}
				}
			}
		}
	}

	void generateChunkFaceBottom( MeshData meshData )
	{
		bool[,,] wasMeshed = new bool[chunkSize, chunkSize, chunkSize];
		Voxel curVoxel;
		int i, j, k;

		for( int y = 0; y < chunkSize; y++ )
		{
			for( int x = 0; x < chunkSize; x++ )
			{
				for( int z = 0; z < chunkSize; z++ )
				{
					curVoxel = GetVoxel(x, y, z);

					if( curVoxel != null && !wasMeshed[x,y,z] && GetVoxel(x, y - 1, z) == null )
					{
						for( i = x + 1; i < chunkSize; i++ )
						{
							if( curVoxel.Equals(GetVoxel(i, y, z)) && !wasMeshed[i,y,z] && GetVoxel(i, y - 1, z) == null )
							{
								wasMeshed[i,y,z] = true;
							}
							else
							{
								break;
							}
						}
						i--;

						for( j = z + 1; j < chunkSize; j++ )
						{
							for( k = x; k <= i; k++ )
							{
								if( !curVoxel.Equals(GetVoxel(k, y, j)) || wasMeshed[k,y,j] || GetVoxel(k, y - 1, j) != null )
								{
									break;
								}
							}
							k--;

							if( k == i )
							{
								for( k = x; k <= i; k++ )
								{
									wasMeshed[k,y,j] = true;
								}
							}
							else
							{
								break;
							}
						}
						j--;

						meshData.vertices.Add( new Vector3(x, y, z) );
						meshData.vertices.Add( new Vector3(i + 1, y, z) );
						meshData.vertices.Add( new Vector3(i + 1, y, j + 1) );
						meshData.vertices.Add( new Vector3(x, y, j + 1) );

						meshData.colors.Add( curVoxel.voxelType.bottomColor );
						meshData.colors.Add( curVoxel.voxelType.bottomColor );
						meshData.colors.Add( curVoxel.voxelType.bottomColor );
						meshData.colors.Add( curVoxel.voxelType.bottomColor );
					}
				}
			}
		}
	}

	void generateChunkFaceFront( MeshData meshData )
	{
		bool[,,] wasMeshed = new bool[chunkSize, chunkSize, chunkSize];
		Voxel curVoxel;
		int i, j, k;

		for( int z = 0; z < chunkSize; z++ )
		{
			for( int x = 0; x < chunkSize; x++ )
			{
				for( int y = 0; y < chunkSize; y++ )
				{
					curVoxel = GetVoxel(x, y, z);

					if( curVoxel != null && !wasMeshed[x,y,z] && GetVoxel(x, y, z + 1) == null )
					{
						for( i = x + 1; i < chunkSize; i++ )
						{
							if( curVoxel.Equals(GetVoxel(i, y, z)) && !wasMeshed[i,y,z] && GetVoxel(i, y, z + 1) == null )
							{
								wasMeshed[i,y,z] = true;
							}
							else
							{
								break;
							}
						}
						i--;

						for( j = y + 1; j < chunkSize; j++ )
						{
							for( k = x; k <= i; k++ )
							{
								if( !curVoxel.Equals(GetVoxel(k, j, z)) || wasMeshed[k,j,z] || GetVoxel(k, j, z + 1) != null )
								{
									break;
								}
							}
							k--;

							if( k == i )
							{
								for( k = x; k <= i; k++ )
								{
									wasMeshed[k,j,z] = true;
								}
							}
							else
							{
								break;
							}
						}
						j--;

						meshData.vertices.Add( new Vector3(x, y, z + 1) );
						meshData.vertices.Add( new Vector3(i + 1, y, z + 1) );
						meshData.vertices.Add( new Vector3(i + 1, j + 1, z + 1) );
						meshData.vertices.Add( new Vector3(x, j + 1, z + 1) );

						meshData.colors.Add( curVoxel.voxelType.sideColor );
						meshData.colors.Add( curVoxel.voxelType.sideColor );
						meshData.colors.Add( curVoxel.voxelType.sideColor );
						meshData.colors.Add( curVoxel.voxelType.sideColor );
					}
				}
			}
		}
	}

	void generateChunkFaceBack( MeshData meshData )
	{
		bool[,,] wasMeshed = new bool[chunkSize, chunkSize, chunkSize];
		Voxel curVoxel;
		int i, j, k;

		for( int z = 0; z < chunkSize; z++ )
		{
			for( int x = 0; x < chunkSize; x++ )
			{
				for( int y = 0; y < chunkSize; y++ )
				{
					curVoxel = GetVoxel(x, y, z);

					if( curVoxel != null && !wasMeshed[x,y,z] && GetVoxel(x, y, z - 1) == null )
					{
						for( i = x + 1; i < chunkSize; i++ )
						{
							if( curVoxel.Equals(GetVoxel(i, y, z)) && !wasMeshed[i,y,z] && GetVoxel(i, y, z - 1) == null )
							{
								wasMeshed[i,y,z] = true;
							}
							else
							{
								break;
							}
						}
						i--;

						for( j = y + 1; j < chunkSize; j++ )
						{
							for( k = x; k <= i; k++ )
							{
								if( !curVoxel.Equals(GetVoxel(k, j, z)) || wasMeshed[k,j,z] || GetVoxel(k, j, z - 1) != null )
								{
									break;
								}
							}
							k--;

							if( k == i )
							{
								for( k = x; k <= i; k++ )
								{
									wasMeshed[k,j,z] = true;
								}
							}
							else
							{
								break;
							}
						}
						j--;

						meshData.vertices.Add( new Vector3(x, y, z) );
						meshData.vertices.Add( new Vector3(x, j + 1, z) );
						meshData.vertices.Add( new Vector3(i + 1, j + 1, z) );
						meshData.vertices.Add( new Vector3(i + 1, y, z) );

						meshData.colors.Add( curVoxel.voxelType.sideColor );
						meshData.colors.Add( curVoxel.voxelType.sideColor );
						meshData.colors.Add( curVoxel.voxelType.sideColor );
						meshData.colors.Add( curVoxel.voxelType.sideColor );
					}
				}
			}
		}
	}

	void generateChunkFaceLeft( MeshData meshData )
	{
		bool[,,] wasMeshed = new bool[chunkSize, chunkSize, chunkSize];
		Voxel curVoxel;
		int i, j, k;

		for( int x = 0; x < chunkSize; x++ )
		{
			for( int z = 0; z < chunkSize; z++ )
			{
				for( int y = 0; y < chunkSize; y++ )
				{
					curVoxel = GetVoxel(x, y, z);

					if( curVoxel != null && !wasMeshed[x,y,z] && GetVoxel(x - 1, y, z) == null )
					{
						for( i = z + 1; i < chunkSize; i++ )
						{
							if( curVoxel.Equals(GetVoxel(x, y, i)) && !wasMeshed[x,y,i] && GetVoxel(x - 1, y, i) == null )
							{
								wasMeshed[x,y,i] = true;
							}
							else
							{
								break;
							}
						}
						i--;

						for( j = y + 1; j < chunkSize; j++ )
						{
							for( k = z; k <= i; k++ )
							{
								if( !curVoxel.Equals(GetVoxel(x, j, k)) || wasMeshed[x,j,k] || GetVoxel(x - 1, j, k) != null )
								{
									break;
								}
							}
							k--;

							if( k == i )
							{
								for( k = z; k <= i; k++ )
								{
									wasMeshed[x,j,k] = true;
								}
							}
							else
							{
								break;
							}
						}
						j--;

						meshData.vertices.Add( new Vector3(x, y, z) );
						meshData.vertices.Add( new Vector3(x, y, i + 1) );
						meshData.vertices.Add( new Vector3(x, j + 1, i + 1) );
						meshData.vertices.Add( new Vector3(x, j + 1, z) );

						meshData.colors.Add( curVoxel.voxelType.sideColor );
						meshData.colors.Add( curVoxel.voxelType.sideColor );
						meshData.colors.Add( curVoxel.voxelType.sideColor );
						meshData.colors.Add( curVoxel.voxelType.sideColor );
					}
				}
			}
		}
	}

	void generateChunkFaceRight( MeshData meshData )
	{
		bool[,,] wasMeshed = new bool[chunkSize, chunkSize, chunkSize];
		Voxel curVoxel;
		int i, j, k;

		for( int x = 0; x < chunkSize; x++ )
		{
			for( int z = 0; z < chunkSize; z++ )
			{
				for( int y = 0; y < chunkSize; y++ )
				{
					curVoxel = GetVoxel(x, y, z);

					if( curVoxel != null && !wasMeshed[x,y,z] && GetVoxel(x + 1, y, z) == null )
					{
						for( i = z + 1; i < chunkSize; i++ )
						{
							if( curVoxel.Equals(GetVoxel(x, y, i)) && !wasMeshed[x,y,i] && GetVoxel(x + 1, y, i) == null )
							{
								wasMeshed[x,y,i] = true;
							}
							else
							{
								break;
							}
						}
						i--;

						for( j = y + 1; j < chunkSize; j++ )
						{
							for( k = z; k <= i; k++ )
							{
								if( !curVoxel.Equals(GetVoxel(x, j, k)) || wasMeshed[x,j,k] || GetVoxel(x + 1, j, k) != null )
								{
									break;
								}
							}
							k--;

							if( k == i )
							{
								for( k = z; k <= i; k++ )
								{
									wasMeshed[x,j,k] = true;
								}
							}
							else
							{
								break;
							}
						}
						j--;

						meshData.vertices.Add( new Vector3(x + 1, y, z) );
						meshData.vertices.Add( new Vector3(x + 1, j + 1, z) );
						meshData.vertices.Add( new Vector3(x + 1, j + 1, i + 1) );
						meshData.vertices.Add( new Vector3(x + 1, y, i + 1) );

						meshData.colors.Add( curVoxel.voxelType.sideColor );
						meshData.colors.Add( curVoxel.voxelType.sideColor );
						meshData.colors.Add( curVoxel.voxelType.sideColor );
						meshData.colors.Add( curVoxel.voxelType.sideColor );
					}
				}
			}
		}
	}
}