/*
<Voxel and A-star Road Generator>
Copyright (C) <2018>  <Tyler Loewen, Matthew Kania>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/gpl.txt>.

Contact at tylerscottloewen@gmail.com
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;

public class World : MonoBehaviour
{
	public static World instance = null; // Singleton instance.
	public static Vector3Int worldSize = new Vector3Int( 4, 2, 4 ); // Size of world in chunks.
	public static WorldGenState worldGenState = WorldGenState.Idle;
	public static Dictionary<string, NoiseData> noiseDict = null;
	public static Dictionary<string, VoxelType> voxelTypeDict = null;
	public static int waterLevel = 24;
	public static bool useGreedyMeshing = true;

    public NoiseData noiseData;

	public Chunk[,,] chunks;

	public enum WorldGenState {Idle, SettingVoxels, UpdatingChunks, RenderingChunks};

	void Start()
	{
		// Enforce the Singleton design pattern by ensuring only one instance of the 'World' class exists at any time.
		if( instance == null )
		{
			instance = this;
		}
		else
		{
			Debug.LogWarning( "A second instance of 'World' was created. It has been destroyed." );
			Destroy( this );
		}

		ThreadPool.SetMinThreads( SystemInfo.processorCount, SystemInfo.processorCount );
		ThreadPool.SetMaxThreads( SystemInfo.processorCount, SystemInfo.processorCount );

		GameObject.Find( "Water" ).transform.position = Vector3.up * waterLevel;

		setupNoiseData();
		setupVoxelTypes();

		SetWorldSize();
		SetWaterLevel();
	}

	public void GenerateWorld()
	{
		if( worldGenState == WorldGenState.Idle )
		{
			clearWorld();
			StartCoroutine( "generateWorldCoroutine" );
		}
		else
		{
			Debug.LogWarning( "World is already being generated!" );
		}
	}

	public void SetNoiseData( Dropdown dropdown )
	{
		noiseData = noiseDict[dropdown.options[dropdown.value].text];
	}

	public void SetWorldSize()
	{
		Slider sliderX = GameObject.Find( "Canvas/WorldSizeX Slider" ).GetComponent<Slider>();
		Slider sliderY = GameObject.Find( "Canvas/WorldSizeY Slider" ).GetComponent<Slider>();
		Slider sliderZ = GameObject.Find( "Canvas/WorldSizeZ Slider" ).GetComponent<Slider>();

		worldSize = new Vector3Int( Mathf.RoundToInt(sliderX.value), Mathf.RoundToInt(sliderY.value), Mathf.RoundToInt(sliderZ.value) );

		sliderX.transform.Find( "Text" ).GetComponent<Text>().text = "World Size X: " + worldSize.x;
		sliderY.transform.Find( "Text" ).GetComponent<Text>().text = "World Size Y: " + worldSize.y;
		sliderZ.transform.Find( "Text" ).GetComponent<Text>().text = "World Size Z: " + worldSize.z;
	}

	public void SetWaterLevel()
	{
		Slider slider = GameObject.Find( "Canvas/WaterLevel Slider" ).GetComponent<Slider>();

		waterLevel = Mathf.RoundToInt( slider.value );
		GameObject.Find( "Water" ).transform.position = Vector3.up * waterLevel;

		slider.transform.Find( "Text" ).GetComponent<Text>().text = "Water Level: " + waterLevel;
	}

	public void SetGreedyMeshing( Toggle toggle )
	{
		useGreedyMeshing = toggle.isOn;
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void SetVoxel( Vector3Int pos, string voxelType )
	{
		SetVoxel( pos.x, pos.y, pos.z, voxelType );
	}

	public void SetVoxel( int x, int y, int z, string voxelType )
	{
		Chunk targetChunk = null;

		targetChunk = GetChunk( x, y, z );

		if( targetChunk != null )
		{
			Vector3Int posInChunk = new Vector3Int( x % Chunk.chunkSize, y % Chunk.chunkSize, z % Chunk.chunkSize );
			Chunk neighborChunk = null;

			targetChunk.SetVoxel( posInChunk, voxelType );

			// If the modified voxel is on the edge of the chunk, also update adjacent chunks
			if( worldGenState == WorldGenState.Idle )
			{
				if( posInChunk.x == 0 )
				{
					neighborChunk = GetChunk( x - 1, y, z );
				}
				else if( posInChunk.x == Chunk.chunkSize - 1 )
				{
					neighborChunk = GetChunk( x + 1, y, z );
				}
				if( neighborChunk != null )
				{
					neighborChunk.MarkChunkAsDirty();
					neighborChunk = null;
				}

				if( posInChunk.y == 0 )
				{
					neighborChunk = GetChunk( x, y - 1, z );
				}
				else if( posInChunk.y == Chunk.chunkSize - 1 )
				{
					neighborChunk = GetChunk( x, y + 1, z );
				}
				if( neighborChunk != null )
				{
					neighborChunk.MarkChunkAsDirty();
					neighborChunk = null;
				}

				if( posInChunk.z == 0 )
				{
					neighborChunk = GetChunk( x, y, z - 1 );
				}
				else if( posInChunk.z == Chunk.chunkSize - 1 )
				{
					neighborChunk = GetChunk( x, y, z + 1 );
				}
				if( neighborChunk != null )
				{
					neighborChunk.MarkChunkAsDirty();
					neighborChunk = null;
				}
			}
		}
	}

	public Chunk GetChunk( int x, int y, int z )
	{
		Chunk targetChunk = null;
		Vector3Int index = new Vector3Int( x, y, z );

		if( x < 0 || y < 0 || z < 0 )
		{
			return null;
		}

		// Round down to next lowest chunk.
		index.x /= Chunk.chunkSize;
		index.y /= Chunk.chunkSize;
		index.z /= Chunk.chunkSize;

		// If the index is outside the 3D-array, the supplied coordinate is outside the world, and 'null' is returned instead.
		if( index.x >= 0 && index.x < chunks.GetLength(0) && index.y >= 0 && index.y < chunks.GetLength(1) && index.z >= 0 && index.z < chunks.GetLength(2) )
		{
			targetChunk = chunks[index.x, index.y, index.z];
		}

		return targetChunk;
	}

	public Voxel GetVoxel( Vector3Int pos )
	{
		return GetVoxel( pos.x, pos.y, pos.z );
	}

	public Voxel GetVoxel( int x, int y, int z )
	{
		Chunk targetChunk = null;
		Voxel targetVoxel = null;

		if( x < 0 || y < 0 || z < 0 )
		{
			return targetVoxel;
		}

		targetChunk = GetChunk( x, y, z );

		if( targetChunk != null )
		{
			targetVoxel = targetChunk.GetVoxel( x % Chunk.chunkSize, y % Chunk.chunkSize, z % Chunk.chunkSize );
		}

		return targetVoxel;
	}

	private void clearWorld()
	{
		Transform pathfinding = GameObject.Find( "Pathfinding" ).transform;

		// Delete all road tiles
		foreach( Transform child in pathfinding )
		{
			Destroy( child.gameObject );
		}

		// Delete all chunks
		foreach( Transform child in transform )
		{
			Destroy( child.gameObject );
		}

		chunks = null;
	}

	private void setupNoiseData()
	{
		if( noiseDict == null )
		{
			// Locate all NoiseData objects and add them to the dropdown list in the UI
			UnityEngine.Object[] noiseData = Resources.LoadAll( "Noise Data", typeof(NoiseData) );
			noiseDict = new Dictionary<string, NoiseData>();

			for( int i = 0; i < noiseData.Length; i++ )
			{
				noiseDict.Add( noiseData[i].name, (NoiseData)noiseData[i] );
			}

			List<Dropdown.OptionData> noiseOptions = new List<Dropdown.OptionData>();
			foreach( KeyValuePair<string, NoiseData> pair in noiseDict )
			{
				noiseOptions.Add( new Dropdown.OptionData(pair.Key) );
			}

			Dropdown noiseDropdown = GameObject.Find( "Noise Dropdown" ).GetComponent<Dropdown>();
			noiseDropdown.options = noiseOptions;
			noiseDropdown.value = noiseOptions.FindIndex( o => o.text == "Smooth Hills" );
			SetNoiseData( noiseDropdown );
		}
	}

	private void setupVoxelTypes()
	{
		if( voxelTypeDict == null )
		{
			// Locate all VoxelType objects and place them in a dictionary
			UnityEngine.Object[] voxelTypes = Resources.LoadAll( "Voxel Types", typeof(VoxelType) );
			voxelTypeDict = new Dictionary<string, VoxelType>();

			for( int i = 0; i < voxelTypes.Length; i++ )
			{
				voxelTypeDict.Add( voxelTypes[i].name, (VoxelType)voxelTypes[i] );
			}
		}
	}

	private void fillWorld( object state )
	{
		try
		{
			int xVoxels = worldSize.x * Chunk.chunkSize;
			int zVoxels = worldSize.z * Chunk.chunkSize;
			int maxHeight = worldSize.y * Chunk.chunkSize;

			float [,] noiseMap = Noise.generateNoiseMap( xVoxels, zVoxels, noiseData.seed, noiseData.scale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, noiseData.offset );

			// Fill the world with voxels according to the values in the supplied perlin noise map
			for ( int x = 0; x < xVoxels; x++ )
			{
				for( int z = 0; z < zVoxels; z++ )
				{
					int curHeight = Mathf.FloorToInt( noiseMap[x,z] * maxHeight );

					for( int y = 0; y < curHeight; y++ )
					{
						SetVoxel( x, y, z, "Grass" );
					}
				}
			}

			print( "Setting voxels complete!" );
			worldGenState = WorldGenState.UpdatingChunks;
		}
		catch( Exception e )
		{
			print( e.ToString() );
		}
	}

	private void createChunk( int x, int y, int z )
	{
		Vector3Int worldPos = new Vector3Int( x, y, z );

		GameObject newChunkGO = new GameObject( "Chunk", typeof(MeshFilter), typeof(MeshRenderer) );
		Chunk newChunk = newChunkGO.AddComponent<Chunk>();
		newChunk.Initialize( this, worldPos );

		newChunkGO.transform.parent = transform;
		newChunkGO.transform.position = worldPos;

		chunks[worldPos.x / Chunk.chunkSize, worldPos.y / Chunk.chunkSize, worldPos.z / Chunk.chunkSize] = newChunk;
	}

	private IEnumerator generateWorldCoroutine()
	{
		// Fill the world with empty chunks.
		chunks = new Chunk[worldSize.x, worldSize.y, worldSize.z];
		for( int x = 0; x < worldSize.x; x++ )
		{
			for( int y = 0; y < worldSize.y; y++ )
			{
				for( int z = 0; z < worldSize.z; z++ )
				{
					createChunk( x * Chunk.chunkSize, y * Chunk.chunkSize, z * Chunk.chunkSize );
				}
			}
		}

		worldGenState = WorldGenState.SettingVoxels;

		print( "Setting voxels..." );
		ThreadPool.QueueUserWorkItem( new WaitCallback(fillWorld) );

		while( worldGenState == WorldGenState.SettingVoxels )
		{
			yield return null;
		}

		while( Chunk.updateCount > 0 )
		{
			yield return null;
		}

		worldGenState = WorldGenState.Idle;
		print( "World generation complete!" );
	}
}