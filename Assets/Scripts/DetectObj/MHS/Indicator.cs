using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Indicator : Singleton<Indicator>
{
    Vector3Int curPosition = new Vector3Int(0, 0, -1);
    Vector3 rayDir = Vector3.forward;

    TileMapManager tilemapManager;
    int maxX;
    int maxY;
    int maxZ;

    //test
    [SerializeField] public GameObject target;

    public void Start()
    {
        tilemapManager = TileMapManager.GetInstance;
        Init();
    }

    public void Init()
    {
        curPosition = new Vector3Int(0, 0, -1);
        this.transform.position = new Vector3(0, 0, -1);

        maxX = tilemapManager.maxX;
        maxY = tilemapManager.maxY;
        maxZ = tilemapManager.maxZ;
    }

    public GameObject[] Indicate()
    {
        if (curPosition.y < maxY - 1) this.transform.position += Vector3.up;
        else this.transform.position = new Vector3(curPosition.x + 1, 0, -1);

        //Debug.Log("y : " + this.transform.position.y);
        curPosition = new Vector3Int((int)this.transform.position.x, (int)this.transform.position.y, (int)this.transform.position.z);

        RaycastHit[] hits = Physics.RaycastAll(this.transform.position + new Vector3(0, 0.5f, 0), rayDir, 2000f)
                        .OrderBy(h => h.distance).ToArray();
        //Debug.Log(hits.Length);
        if (hits == null || hits.Length == 0) return null;
        GameObject[] blocks = new GameObject[maxZ];
        for (int i = 0; i < hits.Length; i++)
        {
            Block block;
            hits[i].transform.TryGetComponent<Block>(out block);
            if (block == null)
                continue;
            blocks[(int)hits[i].transform.position.z] = hits[i].transform.gameObject;
        }
        return blocks;
    }

    [ContextMenu("CreateNewLevel")]
    public void CreateNewLevel()
    {
        Init();
        while (curPosition.x < maxX - 1)
        {
            //Debug.Log(curPosition.x);
            GameObject[] blocks = Indicate();
            if (blocks == null) continue;

            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] == null) continue;

                GameObject block = blocks[i];
                //Debug.Log($"x : {curPosition.x}, y : {curPosition.y}, z : {i}, block : {block.Prefab.ToString()}");
                tilemapManager.SetTileMap(curPosition.x, curPosition.y, i, block);
            }
        }
    }

    //public Block Indicate(Block[,,] tileMap)
    //{

    //}
}
