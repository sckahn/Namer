using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum BlockType
{
    Null = -1,
    Tile = 0,
    Object = 1
}

public enum BlockPrefab
{
    GroundTile = -3,
    GlassTile,
    Null,
    NameStone,
    Ball,
    Rock,
    Bonfire,
    Tree,
    Water,
    Sun,
    Balloon,
    Crystal,
    Watermelon,
    Card,
}

[System.Serializable]
public class Block : MonoBehaviour
{
    [SerializeField] BlockType type = BlockType.Null;
    public BlockType Type
    {
        get { return type; }
        set { type = value; }
    }
    [SerializeField] BlockPrefab prefab = BlockPrefab.Null;
    public BlockPrefab Prefab
    {
        get { return prefab; }
        set { prefab = value; }
    }
    [SerializeField] int blockID = -1;
    public int BlockID
    {
        get { return blockID; }
        set
        {
            if (value < 0) return;
            blockID = value;
        }
    }

    [SerializeField]
    List<BlockProperty> properties = new List<BlockProperty>();
    public List<BlockProperty> Properties { get { return properties; } }

    public void InitProperties()
    {
        if (properties.Count == 0) return;
        properties = new List<BlockProperty>();
    }

    public void AddProperty(BlockProperty property)
    {
        if (properties.Contains(property)) return;
        properties.Add(property);
    }

    public void RemoveProperty(BlockProperty property)
    {
        if (!properties.Contains(property)) return;
        properties.Remove(property);
    }
}
