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

public class CameraControl : MonoBehaviour
{
	Camera cam;
	Transform camTransform;
	Transform cursor;

	void Start()
	{
		cam = GetComponent<Camera>();
		camTransform = cam.transform;
		cursor = GameObject.Find( "Cursor" ).transform;
	}
	
	void Update()
	{
		float vert = Input.GetAxis( "Vertical" ) * 120.0f * Time.deltaTime;
		float hori = Input.GetAxis( "Horizontal" ) * 120.0f * Time.deltaTime;
		float mouseX = Input.GetAxis( "Mouse X" ) * 240.0f * Time.deltaTime;
		float mouseY = Input.GetAxis( "Mouse Y" ) * 240.0f * Time.deltaTime;

		// Camera movement
		camTransform.position += (camTransform.forward * vert) + (camTransform.right * hori);

		// Camera rotation
		if( Input.GetMouseButton(1) )
		{
			Vector3	camRot = camTransform.eulerAngles + new Vector3( -mouseY, mouseX, 0.0f );

			// Limit camera's vertical angle
			camRot.x = (camRot.x + 180.0f) % 360.0f;
			camRot.x = camRot.x < 95.0f ? 95.0f : camRot.x;
			camRot.x = camRot.x > 265.0f ? 265.0f : camRot.x;
			camRot.x -= 180.0f;
			camTransform.eulerAngles = camRot;
		}

		if( Input.GetMouseButtonDown(0) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) )
		{
			GameObject.Find("Start Point").transform.position = raycastVoxel();
		}
		else if( Input.GetMouseButtonDown(0) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) )
		{
			GameObject.Find("End Point").transform.position = raycastVoxel();
		}

		if( World.instance.chunks != null )
		{
			raycastVoxel();
		}
	}

	private Vector3Int raycastVoxel()
	{
		Ray ray = cam.ScreenPointToRay( Input.mousePosition );
		int maxHeight = World.worldSize.y * Chunk.chunkSize;
		int startHeight = Mathf.Min( maxHeight, Mathf.FloorToInt(camTransform.position.y) );
		Plane plane;
		float tValue;
		Vector3 hitPos;
		int x, z;
		Voxel hitVoxel = null;

		// Determines the voxel at the current mouse position by performing ray-plane intersections starting at the current camera height down to the ground.
		// Stops when either the bottom of the world is reached, or the voxel at the intersection location is non-null.
		for( int y = startHeight; y > 0; y-- )
		{
			plane = new Plane( Vector3.up, Vector3.up * y );
			plane.Raycast( ray, out tValue );
			hitPos = ray.GetPoint( tValue );
			x = Mathf.FloorToInt( hitPos.x );
			z = Mathf.FloorToInt( hitPos.z );
			hitVoxel = World.instance.GetVoxel( x, y - 1, z );
			if( hitVoxel != null && World.instance.GetVoxel( x, y, z ) == null )
			{
				cursor.position = new Vector3(x,y,z);
				return new Vector3Int(x,y,z);
			}
		}

		return new Vector3Int(0,0,0);
	}
}