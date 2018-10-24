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

public class MeshData
{
	public List<Vector3> vertices;
	public List<int> triangles;
	public List<Vector2> uvs;
	public List<Color32> colors;

	public MeshData()
	{
		vertices = new List<Vector3>();
		triangles = new List<int>();
		uvs = new List<Vector2>();
		colors = new List<Color32>();
	}

	// Because the vertices are arranged predictably into quads, we can calculate all the triangles at once.
	public void CalculateTriangles()
	{
		triangles.Clear();

		if( vertices.Count % 4 == 0 )
		{
			for( int i = 0; i < vertices.Count; i+=4 )
			{
				triangles.Add( i );
				triangles.Add( i+1 );
				triangles.Add( i+2 );
				triangles.Add( i );
				triangles.Add( i+2 );
				triangles.Add( i+3 );
			}
		}
		else
		{
			Debug.LogError( "Cannot calculate triangles; vertex count is not a multiple of 4!" );
		}
	}
}