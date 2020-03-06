using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlocksController : MonoBehaviour
{
    //public Tile myTile;

    public GameObject myTileObject = null;

    public Tilemap myTilemap;

    //BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();

    // Start is called before the first frame update
    void Start()
    {
        //myTilemap.SetTile(new Vector3Int(0,-3,0), myTile);


        //perlin noise

        for (int i = 0; i < 20f; i++)
        {
            GameObject spikyWall = GameObject.Instantiate(myTileObject);

            float xposition = 3 * i + Mathf.PerlinNoise(i / 1.2f, 0) * 4,
                yposition = Mathf.PerlinNoise(i / 1.2f, 0) * 4;

            spikyWall.transform.position = new Vector3Int((int)xposition, (int)(yposition - 4.5), 0);

            //myTilemap.SetTile(new Vector3Int((int)xposition, (int)(yposition-4.5), 0), myTile);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
