    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Indicator : Singleton<Indicator>
{
    Vector3Int curPosition = new Vector3Int(0, -1, -1);
    Vector3 rayDir = Vector3.forward;

    int maxX;
    int maxY;
    int maxZ;

    //test
    [SerializeField] public GameObject target;

    public void Start()
    {
        
    }

    public void Init()
    {
        curPosition = new Vector3Int(0, -1, -1);
        this.transform.position = new Vector3(0, -1, -1);

        maxX = TileMapManager.GetInstance.maxX;
        maxY = TileMapManager.GetInstance.maxY;
        maxZ = TileMapManager.GetInstance.maxZ;
    }

    public GameObject[] Indicate()
    {
        if (curPosition.y < maxY - 1) this.transform.position += Vector3.up;
        else this.transform.position = new Vector3(curPosition.x + 1, 0, -1);

        curPosition = new Vector3Int((int)this.transform.position.x, (int)this.transform.position.y, (int)this.transform.position.z);

        RaycastHit[] hits = Physics.RaycastAll(this.transform.position + new Vector3(0, 0.5f, 0), rayDir, 2000f)
                        .OrderBy(h => h.distance).ToArray();

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

    public void CreateNewLevel()
    {
        Init();
        while (curPosition.x < maxX - 1)
        {
            GameObject[] blocks = Indicate();
            if (blocks == null) continue;

            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] == null) continue;

                GameObject block = blocks[i];
                TileMapManager.GetInstance.SetTileMap(curPosition.x, curPosition.y, i, block);
            }
        }
    }

    //public Block Indicate(Block[,,] tileMap)
    //{

    //}
}
