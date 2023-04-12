using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int length = 32;
    public int width = 32;
    public int maxheight = 32;
    public float scale = 20f;
    public GameObject blockPrefab;

    private Block[,,] blocks;

    private void Start()
    {
        blocks = new Block[length, width, maxheight];
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        for (int currLength = 0; currLength < length; currLength++)
        {
            for (int currWidth = 0; currWidth < width; currWidth++)
            {
                int height = CalculateHeight(currLength, currWidth);
                Vector3 position = new Vector3(currLength, height, currWidth);
                GameObject blockObj = Instantiate(blockPrefab, position, Quaternion.identity);
                blockObj.name = $"Block({currLength},{currWidth})";
                blocks[currLength, currWidth, height] = new Block(blockObj, GetBlockType(height));

                for (int i = 0; i < height; i++)
                {
                    int newHeight = i;
                    Vector3 newPosition = new Vector3(currLength, newHeight, currWidth);
                    GameObject newBlockObj = Instantiate(blockPrefab, newPosition, Quaternion.identity);
                    newBlockObj.name = $"Block({currLength},{currWidth},{newHeight})";
                    blocks[currLength, currWidth, newHeight] = new Block(newBlockObj, GetBlockType(newHeight));
                }
            }
        }
    }

    Block.BlockType GetBlockType(float height)
    {
        //Heightmap rule definitions
        if (height <= 2) return Block.BlockType.Stone;
        if (height <= 4) return Block.BlockType.Dirt;
        return Block.BlockType.Grass;
    }

    int CalculateHeight(int x, int z)
    {
        float xCoord = (float)x / length * scale;
        float zCoord = (float)z / width * scale;

        return Mathf.FloorToInt(Mathf.PerlinNoise(xCoord, zCoord) * 10);
    }
}
