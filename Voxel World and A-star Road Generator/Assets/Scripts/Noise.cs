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

/// <summary>
/// Generates Perlin noise
/// </summary>
public static class Noise
{
	/// <summary>
	/// Generates values of the noise map and returns them in a 2D array
	/// </summary>
	/// <param name="mapWidth"> Width of the map </param>
	/// <param name="mapHeight"> Height of the map </param>
	/// <param name="seed"> Map seed used for generation </param>
	/// <param name="scale"> SThe scale of the map </param>
	/// <param name="octaves"> Controls the amount of detail </param>
	/// <param name="persistance"> Determines how much the amplitude decreases between octaves </param>
	/// <param name="lacunarity"> Determines how much the frequency increases between octave </param>
    /// <param name="xyOffset"> x and y offset of the noise map </param>
	/// <returns> Returns a 2D float array of x and y values of the map </returns>
	public static float[,] generateNoiseMap( int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 xyOffset )
	{
		float maxHeight = 0;
		float amplitude = 1;
		float frequency = 1;

        // array containing points making up the noise map
        float[,] noiseMap = new float[mapWidth, mapHeight];

        // stores the offset values for each octave
        Vector2[] octaveOffsets = new Vector2[octaves];

		// allows the noise scale value to scale from the center of the texture not the corner
        float halfMapWidth = mapWidth / 2f;
        float halfMapHeight = mapHeight / 2f;

		// sudo-random number generator using a given seed value
		System.Random randomValue = new System.Random( seed );

		// iterates through all of the octaves
		for ( int i = 0; i < octaves; i++ )
		{
			// returns a random number between the two parameters +/- the offset
			// randomly offsets the octaves so that they are not perfectly overlapping
            float offsetX = randomValue.Next( -100000, 100000 ) + xyOffset.x;
            float offsetY = randomValue.Next( -100000, 100000 ) - xyOffset.y;
            octaveOffsets[i] = new Vector2( offsetX, offsetY );

			// calculates the highest point with all of the octaves combined
            maxHeight += amplitude;

			// decreses amplitude for each consecutive octave
			amplitude *= persistance;
        }

		// generates every point for the noise map
		for ( int y = 0; y < mapHeight; y++ )
		{
			for ( int x = 0; x < mapWidth; x++ )
			{
				amplitude = 1;
				frequency = 1;
				float noiseHeight = 0;

				// generates a noise map for each different octave
				for ( int i = 0; i < octaves; i++ )
                {
                    // insures the scale is never lower than the lowest working value
					scale = Mathf.Max( 2, scale );

                    // higher the frequency the more rapidly the perlin noise will change
                    // offsets the octaves to add more variation
                    float sampleX = ( ( x - halfMapWidth + octaveOffsets[i].x ) / scale ) * frequency;
                    float sampleY = ( ( y - halfMapHeight + octaveOffsets[i].y ) / scale ) * frequency;

					// generates a perlin value with a range 0 to 1 from the given x and y coordinates
					float perlinValue = Mathf.PerlinNoise( sampleX, sampleY );
					noiseHeight += perlinValue * amplitude;

					// decreses amplitude each octave
					amplitude *= persistance;

                    // increases frequency each octave
                    frequency *= lacunarity;
				}

                // if maxPossibleHeight is replaced with 1 the top voxels will clip
                noiseMap[x, y] = Mathf.InverseLerp( 0, maxHeight, noiseHeight );
			}
		}
		return noiseMap;
	}
}