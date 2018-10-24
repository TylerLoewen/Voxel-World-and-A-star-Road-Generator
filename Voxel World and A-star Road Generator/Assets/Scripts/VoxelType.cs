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

using UnityEngine;

[CreateAssetMenu()]
public class VoxelType : ScriptableObject
{
	public Color32 topColor = Color.white;
	public Color32 sideColor = Color.white;
	public Color32 bottomColor = Color.white;
}