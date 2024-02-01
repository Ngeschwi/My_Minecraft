using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldDataHelper
{
    public static Vector3Int ChunkPositionFromBlockCoords(World world, Vector3Int position)
    {
        return new Vector3Int(
            Mathf.FloorToInt(position.x / (float)world.chunkSize) * world.chunkSize,
            Mathf.FloorToInt(position.y / (float)world.chunkHeight) * world.chunkHeight,
            Mathf.FloorToInt(position.z / (float)world.chunkSize) * world.chunkSize);
    }
}
