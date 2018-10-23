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
using UnityEngine.UI;
using System;
using System.Threading;

public class Pathfinding : MonoBehaviour
{
	public static Dictionary<string, GameObject> tileDict = null;
	public static int tunnelRadius = 0;

	public static int rampCost = 200; // Cost to build a ramp
	public static int bridgeCost = 1000; // Cost to build a bridge
	public static int tunnelCost = 1000; // Cost to dig a tunnel

	private enum PathfinderState {Idle, FindingPath, Done};

	private Dictionary<Vector3Int, Node> nodes;
	private Vector3Int startPos, endPos;
	private List<Node> path = null;
	private PathfinderState pathfinderState = PathfinderState.Idle;
	private bool abortPathfinding = false;

	private List<Node> openSet = new List<Node>();
	private Dictionary<Vector3Int, Node> closedSet = new Dictionary<Vector3Int, Node>();
	private object closedSetLock = new object();

	void Start()
	{
		setupTiles();

        SetTunnelRadius();

		SetRampCost();
		SetBridgeCost();
		SetTunnelCost();
	}

	void OnDisable()
	{
		abortPathfinding = true;
	}

	void OnDrawGizmos()
	{
		lock( closedSetLock )
		{
			Gizmos.color = Color.green;
			for( int i = 0; i < openSet.Count; i++ )
			{
				Gizmos.DrawCube( openSet[i].worldPos + Vector3.one * 0.50f, Vector3.one * 0.50f );
			}

			Gizmos.color = Color.red;
			foreach( KeyValuePair<Vector3Int, Node> pair in closedSet )
			{
				Gizmos.DrawCube( pair.Value.worldPos + Vector3.one * 0.50f, Vector3.one * 0.50f );
			}
		}
	}

	public void SetTunnelRadius()
	{
		Slider slider = GameObject.Find( "Canvas/TunnelRadius Slider" ).GetComponent<Slider>();

		tunnelRadius = Mathf.RoundToInt( slider.value );
		slider.transform.Find( "Text" ).GetComponent<Text>().text = "Tunnel Radius: " + tunnelRadius;
	}

	public void SetRampCost()
	{
		Slider slider = GameObject.Find( "Canvas/RampCost Slider" ).GetComponent<Slider>();

		rampCost = Mathf.RoundToInt( slider.value );
		slider.transform.Find( "Text" ).GetComponent<Text>().text = "Ramp Cost: " + rampCost;
	}

	public void SetBridgeCost()
	{
		Slider slider = GameObject.Find( "Canvas/BridgeCost Slider" ).GetComponent<Slider>();

		bridgeCost = Mathf.RoundToInt( slider.value );
		slider.transform.Find( "Text" ).GetComponent<Text>().text = "Bridge Cost: " + bridgeCost;
	}

	public void SetTunnelCost()
	{
		Slider slider = GameObject.Find( "Canvas/TunnelCost Slider" ).GetComponent<Slider>();

		tunnelCost = Mathf.RoundToInt( slider.value );
		slider.transform.Find( "Text" ).GetComponent<Text>().text = "Tunnel Cost: " + tunnelCost;
	}

	public void GeneratePath()
	{
		Vector3 startPoint = GameObject.Find("Start Point").transform.position;
		Vector3 endPoint = GameObject.Find("End Point").transform.position;

		startPos = new Vector3Int( Mathf.RoundToInt(startPoint.x), Mathf.RoundToInt(startPoint.y), Mathf.RoundToInt(startPoint.z) );
		endPos = new Vector3Int( Mathf.RoundToInt(endPoint.x), Mathf.RoundToInt(endPoint.y), Mathf.RoundToInt(endPoint.z) );

		StartCoroutine( drawPath() );
	}

	public void AbortPathfinding()
	{
		abortPathfinding = true;
	}

	private void setupTiles()
	{
		if( tileDict == null )
		{
			UnityEngine.Object[] tiles = Resources.LoadAll( "Tiles", typeof(GameObject) );
			tileDict = new Dictionary<string, GameObject>();

			for( int i = 0; i < tiles.Length; i++ )
			{
				tileDict.Add( tiles[i].name, (GameObject)tiles[i] );
			}
		}
	}

	private List<Node> retracePath( Node startNode, Node endNode )
	{
		List<Node> path = new List<Node>();
		Node curNode = endNode;

		while( curNode != startNode )
		{
			path.Add( curNode );
			curNode = curNode.parent;
		}
		path.Add( startNode );

		path.Reverse();
		return path;
	}

	private List<Node> getNeighbors( Node curNode )
	{
		List<Node> neighbors = new List<Node>();
		Node newNeighbor;
		Vector3Int curPos = curNode.worldPos;
		int maxX = World.worldSize.x * Chunk.chunkSize;
		int maxY = World.worldSize.y * Chunk.chunkSize;
		int maxZ = World.worldSize.z * Chunk.chunkSize;
		Vector3Int offset, neighborPos, parentDiff;

		if( curNode.parent == null )
		{
			parentDiff = Vector3Int.zero;
		}
		else
		{
			parentDiff = curPos - curNode.parent.worldPos;
		}

		for( int x = -1; x <= 1; x++ )
		{
			for( int z = -1; z <= 1; z++ )
			{
				for( int y = -1; y <= 1; y++ )
				{
					if( (x != 0 && z == 0) || (x == 0 && z != 0) )
					{
						offset = new Vector3Int( x, y, z );
						neighborPos = curPos + offset;

						if( neighborPos.x >= 0 && neighborPos.x < maxX && neighborPos.y > World.waterLevel && neighborPos.y < maxY && neighborPos.z >= 0 && neighborPos.z < maxZ )
						{
							if( (y == 0 && parentDiff.y == 0) || (offset.x == parentDiff.x && offset.z == parentDiff.z) )
							{
								if( nodes.ContainsKey(neighborPos) )
								{
									newNeighbor = nodes[neighborPos];
									neighbors.Add( newNeighbor );
								}
								else
								{
									newNeighbor = new Node(World.instance.GetVoxel( curPos.x + x, curPos.y + y, curPos.z + z), neighborPos );
									neighbors.Add( newNeighbor );
								}
							}
						}
					}
				}
			}
		}

		return neighbors;
	}

	private int getMoveCost( Node nodeA, Node nodeB )
	{
		int distY = Mathf.Abs( nodeA.worldPos.y - nodeB.worldPos.y );
		int cost = 0;

		// If the voxel directly underneath the current position is non-null, add the bridge cost multiplied by vertical distance to solid ground
		Vector3Int curPos = nodeB.worldPos;
		curPos.y--;
		while( curPos.y >= 0 && World.instance.GetVoxel(curPos.x, curPos.y, curPos.z) == null )
		{
			cost += bridgeCost;
			curPos.y--;
		}

		// If the voxel directly ahead is non-null, add tunneling cost
		if( World.instance.GetVoxel(nodeB.worldPos) != null )
		{
			cost += tunnelCost;
		}

		// If there is a change in elevation, add ramp cost
		if( distY > 0 )
		{
			cost += rampCost;
		}


		return cost;
	}

	private int getDistance( Node nodeA, Node nodeB )
	{
		int distX = Mathf.Abs( nodeA.worldPos.x - nodeB.worldPos.x );
		int distY = Mathf.Abs( nodeA.worldPos.y - nodeB.worldPos.y );
		int distZ = Mathf.Abs( nodeA.worldPos.z - nodeB.worldPos.z );

		return (distX + distZ) * 100 + (distY * 40);
	}

	void findPath( object state )
	{
		try
		{
			openSet = new List<Node>();
			closedSet = new Dictionary<Vector3Int, Node>();

			Node startNode = new Node( World.instance.GetVoxel(startPos), new Vector3Int(startPos.x, startPos.y, startPos.z) );
			Node endNode = new Node( World.instance.GetVoxel(endPos), new Vector3Int(endPos.x, endPos.y, endPos.z) );
			Node curNode;
			int newNeighborMoveCost;

			nodes = new Dictionary<Vector3Int, Node>();

			nodes.Add( startPos, startNode );

			openSet.Add( startNode );

			while( openSet.Count > 0 )
			{
				if( !abortPathfinding )
				{
					curNode = openSet[0];

					for( int i = 1; i < openSet.Count; i++ )
					{
						if( openSet[i].fCost < curNode.fCost || (openSet[i].fCost == curNode.fCost && openSet[i].hCost < curNode.hCost) )
						{
							curNode = openSet[i];
						}
					}

					openSet.Remove( curNode );

					lock( closedSetLock )
					{
						closedSet.Add( curNode.worldPos, curNode );
					}

					if( curNode.worldPos == endPos )
					{
						path = retracePath( startNode, curNode );
						pathfinderState = PathfinderState.Done;
						return;
					}

					List<Node> neighbors = getNeighbors(curNode);
					for( int i = 0; i < neighbors.Count; i++ )
					{
						if( !closedSet.ContainsKey(neighbors[i].worldPos) )
						{
							newNeighborMoveCost = curNode.gCost + getMoveCost( curNode, neighbors[i] );

							if( newNeighborMoveCost < neighbors[i].gCost || !openSet.Contains(neighbors[i]) )
							{
								neighbors[i].gCost = newNeighborMoveCost;
								neighbors[i].hCost = getDistance( neighbors[i], endNode );
								neighbors[i].parent = curNode;

								if( !openSet.Contains(neighbors[i]) )
								{
									openSet.Add( neighbors[i] );
								}
							}
						}
					}
				}
				else
				{
					print( "Pathfinding aborted." );
					abortPathfinding = false;
					path = null;
					pathfinderState = PathfinderState.Done;
					return;
				}
			}
		}
		catch( Exception e )
		{
			print( e.ToString() );
		}

		path = null;
		pathfinderState = PathfinderState.Done;
	}

	IEnumerator drawPath()
	{
		GameObject newTile;
		Vector3Int direction = Vector3Int.zero;
		Vector3Int reverseDirection = Vector3Int.zero;
		bool directionChanged;

		GameObject.Find( "Canvas/Path Button" ).GetComponent<Button>().interactable = false;
		GameObject.Find( "Canvas/AbortPathfinding Button" ).GetComponent<Button>().interactable = true;

		pathfinderState = PathfinderState.FindingPath;
		ThreadPool.QueueUserWorkItem( new WaitCallback(findPath) );

		while( pathfinderState == PathfinderState.FindingPath )
		{
			yield return null;
		}

		if( path != null )
		{
			// Spawn road tiles at each point along the path. The tile type and orientation is determined based on adjacent terrain and path direction
			for( int i = 0; i < path.Count && !abortPathfinding; i++ )
			{
				if( i < path.Count - 1 )
				{
					direction = path[i+1].worldPos - path[i].worldPos;
				}
				else
				{
					direction = path[i].worldPos - path[i-1].worldPos;
				}

				if( direction.y > 0 )
				{
					if( path[i].worldPos.y > 0 && World.instance.GetVoxel(path[i].worldPos + Vector3Int.down) == null )
					{
						newTile = Instantiate( tileDict["Bridge Ramp"] );
					}
					else
					{
						newTile = Instantiate( tileDict["Ramp"] );
					}
					newTile.transform.parent = transform;
					newTile.transform.position = path[i].worldPos;

					if( direction.x == -1 )
					{
						newTile.transform.Find( "Mesh" ).eulerAngles += Vector3.up * 270.0f;
					}
					else if( direction.x == 1 )
					{
						newTile.transform.Find( "Mesh" ).eulerAngles += Vector3.up * 90.0f;
					}
					else if( direction.z == -1 )
					{
						newTile.transform.Find( "Mesh" ).eulerAngles += Vector3.up * 180.0f;
					}
				}
				else
				{
					if( i > 0 && i < path.Count - 1 )
					{
						reverseDirection = path[i].worldPos - path[i-1].worldPos;
						directionChanged = direction.x != reverseDirection.x || direction.z != reverseDirection.z;
					}
					else
					{
						directionChanged = false;
					}

					if( (i > 0 && i < path.Count - 1) && directionChanged )
					{
						if( path[i].worldPos.y > 0 && World.instance.GetVoxel(path[i].worldPos + Vector3Int.down) == null )
						{
							newTile = Instantiate( tileDict["Bridge Road Corner"] );
						}
						else
						{
							newTile = Instantiate( tileDict["Road Corner"] );
						}
						newTile.transform.parent = transform;
						newTile.transform.position = path[i].worldPos;

						if( direction.x == -1 )
						{
							newTile.transform.Find( "Mesh/Line A" ).eulerAngles += Vector3.up * 90.0f;
						}
						else if( direction.x == 1 )
						{
							newTile.transform.Find( "Mesh/Line A" ).eulerAngles += Vector3.up * 270.0f;
						}
						else if( direction.z == 1 )
						{
							newTile.transform.Find( "Mesh/Line A" ).eulerAngles += Vector3.up * 180.0f;
						}

						if( reverseDirection.x == -1 )
						{
							newTile.transform.Find( "Mesh/Line B" ).eulerAngles += Vector3.up * 90.0f;
						}
						else if( reverseDirection.x == 1 )
						{
							newTile.transform.Find( "Mesh/Line B" ).eulerAngles += Vector3.up * 270.0f;
						}
						else if( reverseDirection.z == 1 )
						{
							newTile.transform.Find( "Mesh/Line B" ).eulerAngles += Vector3.up * 180.0f;
						}
					}
					else
					{
						if( direction.y > 0 || reverseDirection.y < 0 )
						{
							if( path[i].worldPos.y > 0 && World.instance.GetVoxel(path[i].worldPos + Vector3Int.down) == null )
							{
								newTile = Instantiate( tileDict["Bridge Ramp"] );
							}
							else
							{
								newTile = Instantiate( tileDict["Ramp"] );
							}
						}
						else
						{
							if( path[i].worldPos.y > 0 && World.instance.GetVoxel(path[i].worldPos + Vector3Int.down) == null )
							{
								newTile = Instantiate( tileDict["Bridge Road"] );
							}
							else
							{
								newTile = Instantiate( tileDict["Road"] );
							}
						}
						newTile.transform.parent = transform;
						newTile.transform.position = path[i].worldPos;

						if( direction.x == -1 )
						{
							newTile.transform.Find( "Mesh" ).eulerAngles += Vector3.up * 90.0f;
						}
						else if( direction.x == 1 )
						{
							newTile.transform.Find( "Mesh" ).eulerAngles += Vector3.up * 270.0f;
						}
						else if( direction.z == 1 )
						{
							newTile.transform.Find( "Mesh" ).eulerAngles += Vector3.up * 180.0f;
						}
					}

					if( direction.y < 0 )
					{
						i++;
						if( path[i].worldPos.y > 0 && World.instance.GetVoxel(path[i].worldPos + Vector3Int.down) == null )
						{
							newTile = Instantiate( tileDict["Bridge Ramp"] );
						}
						else
						{
							newTile = Instantiate( tileDict["Ramp"] );
						}
						newTile.transform.parent = transform;
						newTile.transform.position = path[i].worldPos;

						if( direction.x == -1 )
						{
							newTile.transform.Find( "Mesh" ).eulerAngles += Vector3.up * 90.0f;
						}
						else if( direction.x == 1 )
						{
							newTile.transform.Find( "Mesh" ).eulerAngles += Vector3.up * 270.0f;
						}
						else if( direction.z == 1 )
						{
							newTile.transform.Find( "Mesh" ).eulerAngles += Vector3.up * 180.0f;
						}
					}
				}

				// If voxel is occupied, tunnel through it
				if( path[i].voxel != null )
				{
					for( int x = -tunnelRadius; x <= tunnelRadius; x++ )
					{
						for( int z = -tunnelRadius; z <= tunnelRadius; z++ )
						{
							for( int y = 0; y <= tunnelRadius; y++ )
							{
								World.instance.SetVoxel( path[i].worldPos + new Vector3Int(x, y, z), "" );
							}
						}
					}

					// Wait until affected chunks have finished updating before continuing
					while( Chunk.updateCount > 0 )
					{
						yield return null;
					}
				}

				yield return new WaitForSeconds(0.020f); // Visualize the path being built by adding a tiny delay between laying each tile
			}
		}
		else
		{
			print( "No valid path found!" );
		}

		if( abortPathfinding )
		{
			print( "Pathfinding aborted." );
			abortPathfinding = false;
		}
		path = null;

		GameObject.Find( "Canvas/Path Button" ).GetComponent<Button>().interactable = true;
		GameObject.Find( "Canvas/AbortPathfinding Button" ).GetComponent<Button>().interactable = false;
	}
}

public class Node
{
	public Voxel voxel;
	public Vector3Int worldPos;
	public Node parent;
	public int gCost;
	public int hCost;

	public int fCost
	{
		get
		{
			return gCost + hCost;
		}
	}

	public Node( Voxel voxel, Vector3Int worldPos )
	{
		this.voxel = voxel;
		this.worldPos = worldPos;
	}

	public override bool Equals( object obj )
	{
		if( obj != null && this.GetType() == obj.GetType() )
		{
			Node other = (Node)obj;

			if( this.worldPos == other.worldPos )
			{
				return true;
			}
		}
		return false;
	}

	public override int GetHashCode()
	{
		return worldPos.GetHashCode();
	}
}