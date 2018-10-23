/*
<Prodecural Voxel and A* Road Generation>
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

public class Voxel
{
	public VoxelType voxelType;
	
	public Voxel( string voxelTypeName )
	{
		voxelType = World.voxelTypeDict[voxelTypeName];
	}

	public override bool Equals( object obj )
	{
		if( obj != null && this.GetType() == obj.GetType() )
		{
			Voxel other = (Voxel)obj;

			if( this.voxelType.GetType() == other.voxelType.GetType() )
			{
				return true;
			}
		}
		return false;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public MeshData GetMeshData( int x, int y, int z, Vector3Int chunkPos, MeshData meshData )
	{
		// Check which faces of the voxel are exposed, and add their vertices to the final chunk mesh.
		// Note: A face is 'exposed' if the voxel next to it is either null or outside the world.
		if( World.instance.GetVoxel(chunkPos.x + x - 1, chunkPos.y + y, chunkPos.z + z) == null )
		{
			getMeshDataLeft( x, y, z, meshData );
		}
		if( World.instance.GetVoxel(chunkPos.x + x + 1, chunkPos.y + y, chunkPos.z + z) == null )
		{
			getMeshDataRight( x, y, z, meshData );
		}
		if( World.instance.GetVoxel(chunkPos.x + x, chunkPos.y + y - 1, chunkPos.z + z) == null )
		{
			getMeshDataDown( x, y, z, meshData );
		}
		if( World.instance.GetVoxel(chunkPos.x + x, chunkPos.y + y + 1, chunkPos.z + z) == null )
		{
			getMeshDataUp( x, y, z, meshData );
		}
		if( World.instance.GetVoxel(chunkPos.x + x, chunkPos.y + y, chunkPos.z + z - 1) == null )
		{
			getMeshDataBack( x, y, z, meshData );
		}
		if( World.instance.GetVoxel(chunkPos.x + x, chunkPos.y + y, chunkPos.z + z + 1) == null )
		{
			getMeshDataForward( x, y, z, meshData );
		}

		return meshData;
	}

	private MeshData getMeshDataLeft( int x, int y, int z, MeshData meshData )
	{
		meshData.vertices.Add( new Vector3(x, y, z+1) );
		meshData.vertices.Add( new Vector3(x, y+1, z+1) );
		meshData.vertices.Add( new Vector3(x, y+1, z) );
		meshData.vertices.Add( new Vector3(x, y, z) );

		meshData.colors.Add( voxelType.sideColor );
		meshData.colors.Add( voxelType.sideColor );
		meshData.colors.Add( voxelType.sideColor );
		meshData.colors.Add( voxelType.sideColor );

		return meshData;
	}

	private MeshData getMeshDataRight( int x, int y, int z, MeshData meshData )
	{
		meshData.vertices.Add( new Vector3(x+1, y, z) );
		meshData.vertices.Add( new Vector3(x+1, y+1, z) );
		meshData.vertices.Add( new Vector3(x+1, y+1, z+1) );
		meshData.vertices.Add( new Vector3(x+1, y, z+1) );

		meshData.colors.Add( voxelType.sideColor );
		meshData.colors.Add( voxelType.sideColor );
		meshData.colors.Add( voxelType.sideColor );
		meshData.colors.Add( voxelType.sideColor );

		return meshData;
	}

	private MeshData getMeshDataDown( int x, int y, int z, MeshData meshData )
	{
		meshData.vertices.Add( new Vector3(x+1, y, z) );
		meshData.vertices.Add( new Vector3(x+1, y, z+1) );
		meshData.vertices.Add( new Vector3(x, y, z+1) );
		meshData.vertices.Add( new Vector3(x, y, z) );

		meshData.colors.Add( voxelType.bottomColor );
		meshData.colors.Add( voxelType.bottomColor );
		meshData.colors.Add( voxelType.bottomColor );
		meshData.colors.Add( voxelType.bottomColor );

		return meshData;
	}

	private MeshData getMeshDataUp( int x, int y, int z, MeshData meshData )
	{
		meshData.vertices.Add( new Vector3(x, y+1, z) );
		meshData.vertices.Add( new Vector3(x, y+1, z+1) );
		meshData.vertices.Add( new Vector3(x+1, y+1, z+1) );
		meshData.vertices.Add( new Vector3(x+1, y+1, z) );

		meshData.colors.Add( voxelType.topColor );
		meshData.colors.Add( voxelType.topColor );
		meshData.colors.Add( voxelType.topColor );
		meshData.colors.Add( voxelType.topColor );

		return meshData;
	}

	private MeshData getMeshDataBack( int x, int y, int z, MeshData meshData )
	{
		meshData.vertices.Add( new Vector3(x, y, z) );
		meshData.vertices.Add( new Vector3(x, y+1, z) );
		meshData.vertices.Add( new Vector3(x+1, y+1, z) );
		meshData.vertices.Add( new Vector3(x+1, y, z) );

		meshData.colors.Add( voxelType.sideColor );
		meshData.colors.Add( voxelType.sideColor );
		meshData.colors.Add( voxelType.sideColor );
		meshData.colors.Add( voxelType.sideColor );

		return meshData;
	}

	private MeshData getMeshDataForward( int x, int y, int z, MeshData meshData )
	{
		meshData.vertices.Add( new Vector3(x+1, y, z+1) );
		meshData.vertices.Add( new Vector3(x+1, y+1, z+1) );
		meshData.vertices.Add( new Vector3(x, y+1, z+1) );
		meshData.vertices.Add( new Vector3(x, y, z+1) );

		meshData.colors.Add( voxelType.sideColor );
		meshData.colors.Add( voxelType.sideColor );
		meshData.colors.Add( voxelType.sideColor );
		meshData.colors.Add( voxelType.sideColor );

		return meshData;
	}
}