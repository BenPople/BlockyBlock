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
    public Transform player;
    public int viewDistance = 50;
    public int chunkSize = 16;


    private Block[,,] blocks;
    private Dictionary<Vector3Int, GameObject> activeChunks;

    private void Start()
    {
        blocks = new Block[length, width, maxheight];
        activeChunks = new Dictionary<Vector3Int, GameObject>();
        GenerateTerrain();
    }

    private void Update()
    {
        LoadChunksInViewDistance();
    }

    void GenerateTerrain()
    {
        for (int currLength = 0; currLength < length; currLength++)
        {
            for (int currWidth = 0; currWidth < width; currWidth++)
            {
                int height = CalculateHeight(currLength, currWidth);
                blocks[currLength, currWidth, height] = new Block(GetBlockType(height));

                for (int i = 0; i < height; i++)
                {
                    int newHeight = i;
                    blocks[currLength, currWidth, newHeight] = new Block(GetBlockType(newHeight));
                }
            }
        }
    }

    void LoadChunksInViewDistance()
    {
        int currentChunkX = Mathf.FloorToInt(player.position.x / chunkSize);
        int currentChunkZ = Mathf.FloorToInt(player.position.z / chunkSize);

        for (int xOffset = -1; xOffset <= 1; xOffset++)
        {
            for (int zOffset = -1; zOffset <= 1; zOffset++)
            {
                Vector3Int chunkPosition = new Vector3Int(currentChunkX + xOffset, 0, currentChunkZ + zOffset);

                if (IsChunkInViewDistance(chunkPosition) && !activeChunks.ContainsKey(chunkPosition))
                {
                    GameObject chunk = CreateChunk(chunkPosition);
                    activeChunks.Add(chunkPosition, chunk);
                }
            }
        }

        List<Vector3Int> chunksToRemove = new List<Vector3Int>();

        foreach (var chunk in activeChunks)
        {
            if (!IsChunkInViewDistance(chunk.Key))
            {
                chunksToRemove.Add(chunk.Key);
                Destroy(chunk.Value);
            }
        }

        foreach (var chunkToRemove in chunksToRemove)
        {
            activeChunks.Remove(chunkToRemove);
        }
    }

    bool IsChunkInViewDistance(Vector3Int chunkPosition)
    {
        float distance = Vector3.Distance(player.position, chunkPosition * chunkSize);
        return distance < viewDistance;
    }

    GameObject CreateChunk(Vector3Int chunkPosition)
    {
        GameObject chunk = new GameObject($"Chunk({chunkPosition.x},{chunkPosition.y},{chunkPosition.z})");
        chunk.transform.position = chunkPosition * chunkSize;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                for (int y = 0; y < maxheight; y++)
                {
                    int worldX = chunkPosition.x * chunkSize + x;
                    int worldZ = chunkPosition.z * chunkSize + z;

                    if (worldX >= 0 && worldX < length && worldZ >= 0 && worldZ < width && blocks[worldX, worldZ, y] != null)
                    {
                        Block currentBlock = blocks[worldX, worldZ, y];
                        Vector3 blockPosition = new Vector3(worldX, y, worldZ);
                        GameObject blockObj = Instantiate(blockPrefab, blockPosition, Quaternion.identity);
                        blockObj.name = $"Block({worldX},{y},{worldZ})";
                        blockObj.GetComponent<MeshRenderer>().material = Block.GetMaterialForBlockType(currentBlock.blockType);
                        blockObj.transform.parent = chunk.transform;
                    }
                }
            }
        }

        return chunk;
    }


    Block.BlockType GetBlockType(float height)
    {
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