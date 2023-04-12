using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public enum BlockType { Grass, Dirt, Stone, Air };

    public BlockType blockType;
    public bool isSolid;

    public Block(BlockType type)
    {
        blockType = type;
        isSolid = type != BlockType.Air;

        Material blockMaterial = GetMaterialForBlockType(type);
        //blockObj.GetComponent<MeshRenderer>().material = blockMaterial;
    }

    public static Material GetMaterialForBlockType(BlockType type)
    {
        switch (type)
        {
            case BlockType.Grass:
                return Resources.Load<Material>("Grass");
            case BlockType.Dirt:
                return Resources.Load<Material>("Dirt");
            case BlockType.Stone:
                return Resources.Load<Material>("Stone");
            case BlockType.Air:
            default:
                return null;
        }
    }
}