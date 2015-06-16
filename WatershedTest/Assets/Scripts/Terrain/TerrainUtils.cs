using UnityEngine;
using System.Collections;

public static class TerrainUtils 
{
    /// <summary>
    /// Takes a World Point and projects it to be within the terrain boundaries as 2D point.
    /// The terrain essentially becomes a 2D rectangle that WorldPoint is projected into.
    /// </summary>
    /// <param name="WorldPoint"></param>
    /// <param name="terrain"></param>
    /// <returns></returns>
    static public Vector2 NormalizePointToTerrain(Vector3 WorldPoint,Terrain terrain)
    {
        Vector3 Origin = terrain.gameObject.transform.position;
        Vector3 Point = WorldPoint - Origin;
        return new Vector2(Point.x/terrain.terrainData.size.x, Point.z/terrain.terrainData.size.z);
    }


    /// <summary>
    /// Reprojects a 2D point in terrain space back into world space, but does not give the height of the terrain at that point.
    /// </summary>
    /// <param name="NormalizedPoint"></param>
    /// <param name="terrain"></param>
    /// <returns></returns>
    static public Vector3 NormalizedTerrainPointToWorld(Vector2 NormalizedPoint, Terrain terrain)
    {
        Vector2 ProjectedPoint = new Vector2(NormalizedPoint.x*terrain.terrainData.size.x,NormalizedPoint.y*terrain.terrainData.size.z);
        
        return new Vector3(ProjectedPoint.x,0,ProjectedPoint.y);
    }

    public static Texture2D GetHeightMapAsTexture( Terrain terrain)
    {
        var data = terrain.terrainData;
        int width = data.heightmapWidth;
        int height = data.heightmapHeight;
        float[,] HeightData = data.GetHeights(0,0,width,height);
        Texture2D TerrainTex = new Texture2D(width,height);
        Color[] Heights = new Color[width * height];

        for (int i = 0; i < width; i++ )
        {
            for(int j =0; j < height; j++)
            {
                Heights[i * height + j] = new Color(1, 0, 0, HeightData[i, j]);
            }
        }

        TerrainTex.SetPixels(Heights);
        TerrainTex.Apply();
        return TerrainTex;
    }

    public static Texture2D GetHeightMapAsTexture(float[,] Data)
    {
        //var data = terrain.terrainData;
        int width = Data.GetLength(0);
        int height = Data.GetLength(1);
        //float[,] HeightData = data.GetHeights(0, 0, width, height);
        Texture2D TerrainTex = new Texture2D(width, height);
        Color[] Heights = new Color[width * height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Heights[i * height + j] = new Color(1, 0, 0, Data[i, j]);
            }
        }

        TerrainTex.SetPixels(Heights);
        TerrainTex.Apply();
        return TerrainTex;
    }

    /// <summary>
    /// Takes a World Point and projects it to be within the terrain boundaries as 2D point.
    /// The terrain essentially becomes a 2D rectangle that WorldPoint is projected into.
    /// </summary>
    /// <param name="WorldPoint"></param>
    /// <param name="terrain"></param>
    /// <returns></returns>
    static public Vector2 NormalizePointToTerrain(Vector3 WorldPoint, Rect BoundingArea)
    {
        
           
        //Vector3 Origin = terrain.gameObject.transform.position;
        //Vector3 Point = WorldPoint - Origin;
        return Rect.PointToNormalized(BoundingArea, new Vector2(WorldPoint.x, WorldPoint.z));/// new Vector2(Point.x / terrain.terrainData.size.x, Point.z / terrain.terrainData.size.z);
    }
}
