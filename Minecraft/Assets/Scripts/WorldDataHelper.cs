using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public static List<Vector3Int> GetChunkPositionsAroundPlayer(World world, Vector3Int playerPosition)
    {
        int startX = playerPosition.x - (world.chunkDrawingRange) * world.chunkSize;
        int startZ = playerPosition.z - (world.chunkDrawingRange) * world.chunkSize;
        int endX = playerPosition.x + (world.chunkDrawingRange) * world.chunkSize;
        int endZ = playerPosition.z + (world.chunkDrawingRange) * world.chunkSize;

        List<Vector3Int> chunkPositionsToCreate = new List<Vector3Int>();
        for (int x = startX; x <= endX; x += world.chunkSize)
        {
            for (int z = startZ; z <= endZ; z += world.chunkSize)
            {
                Vector3Int chunkPos = ChunkPositionFromBlockCoords(world, new Vector3Int(x, 0, z));
                chunkPositionsToCreate.Add(chunkPos);
                if (x >= playerPosition.x - world.chunkSize
                    && x <= playerPosition.x + world.chunkSize
                    && z >= playerPosition.z - world.chunkSize
                    && z <= playerPosition.z + world.chunkSize)
                {
                    for (int y = -world.chunkHeight; y >= playerPosition.y - world.chunkHeight * 2; y -= world.chunkHeight)
                    {
                        chunkPos = ChunkPositionFromBlockCoords(world, new Vector3Int(x, y, z));
                        chunkPositionsToCreate.Add(chunkPos);
                    }
                }
            }
        }

        return chunkPositionsToCreate;
    }

    public static List<Vector3Int> GetDataPositionsAroundPlayer(World world, Vector3Int playerPosition)
    {
        int startX = playerPosition.x - (world.chunkDrawingRange + 1) * world.chunkSize;
        int startZ = playerPosition.z - (world.chunkDrawingRange + 1) * world.chunkSize;
        int endX = playerPosition.x + (world.chunkDrawingRange + 1) * world.chunkSize;
        int endZ = playerPosition.z + (world.chunkDrawingRange + 1) * world.chunkSize;

        List<Vector3Int> chunkDataPositionsToCreate = new List<Vector3Int>();
        for (int x = startX; x <= endX; x += world.chunkSize)
        {
            for (int z = startZ; z <= endZ; z += world.chunkSize)
            {
                Vector3Int chunkPos = ChunkPositionFromBlockCoords(world, new Vector3Int(x, 0, z));
                chunkDataPositionsToCreate.Add(chunkPos);
                if (x >= playerPosition.x - world.chunkSize
                    && x <= playerPosition.x + world.chunkSize
                    && z >= playerPosition.z - world.chunkSize
                    && z <= playerPosition.z + world.chunkSize)
                {
                    for (int y = -world.chunkHeight; y >= playerPosition.y - world.chunkHeight * 2; y -= world.chunkHeight)
                    {
                        chunkPos = ChunkPositionFromBlockCoords(world, new Vector3Int(x, y, z));
                        chunkDataPositionsToCreate.Add(chunkPos);
                    }
                }
            }
        }

        return chunkDataPositionsToCreate;
    }

    public static List<Vector3Int> GetUnneededChunks(World.WorldData worldData, List<Vector3Int> allChunkPositionsNeeded)
    {
        List<Vector3Int> positionsToRemove = new List<Vector3Int>();
        foreach (var pos in worldData.chunkDictionary.Keys
                     .Where(pos => allChunkPositionsNeeded.Contains(pos) == false))
        {
            if (worldData.chunkDictionary.ContainsKey(pos))
            {
                positionsToRemove.Add(pos);
            }
        }

        return positionsToRemove;
    }

    public static List<Vector3Int> GetUnneededData(World.WorldData worldData, List<Vector3Int> allChunkDataPositionsNeeded)
    {
        return worldData.chunkDataDictionary.Keys
            .Where(pos => allChunkDataPositionsNeeded.Contains(pos) == false && worldData.chunkDataDictionary[pos].modifiedByThePlayer == false)
            .ToList();
    }
    
    public static List<Vector3Int> SelectPositionsToCreate(World.WorldData worldData, List<Vector3Int> allChunkPositionsNeeded, Vector3Int playerPosition)
    {
        return allChunkPositionsNeeded
            .Where(pos => worldData.chunkDictionary.ContainsKey(pos) == false)
            .OrderBy(pos => Vector3.Distance(playerPosition, pos))
            .ToList();
    }

    public static List<Vector3Int> SelectDataPositonsToCreate(World.WorldData worldData, List<Vector3Int> allChunkDataPositionsNeeded, Vector3Int playerPosition)
    {
        return allChunkDataPositionsNeeded
            .Where(pos => worldData.chunkDataDictionary.ContainsKey(pos) == false)
            .OrderBy(pos => Vector3.Distance(playerPosition, pos))
            .ToList();
    }

    public static void RemoveChunk(World world, Vector3Int pos)
    {
        ChunkRenderer chunk = null;
        
        if (world.worldData.chunkDictionary.TryGetValue(pos, out chunk))
        {
            world.RemoveChunk(chunk);
            world.worldData.chunkDictionary.Remove(pos);
        }
    }

    public static void RemoveChunkData(World world, Vector3Int pos)
    {
        world.worldData.chunkDataDictionary.Remove(pos);
    }

    public static void SetBlock(World worldReference, Vector3Int pos, BlockType blockType)
    {
        ChunkData chunkData = GetChunkData(worldReference, pos);

        if (chunkData != null)
        {
            Vector3Int localPosition = Chunk.GetBlockInChunkCoordinates(chunkData, pos);
            Chunk.SetBlock(chunkData, localPosition, blockType);
        }
    }

    public static ChunkData GetChunkData(World worldReference, Vector3Int pos)
    {
        Vector3Int chunkPos = ChunkPositionFromBlockCoords(worldReference, pos);
        ChunkData containerChunk = null;
        
        worldReference.worldData.chunkDataDictionary.TryGetValue(chunkPos, out containerChunk);
        
        return containerChunk;
    }

    public static ChunkRenderer GetChunk(World worldReference, Vector3Int worldPosition)
    {
        if (worldReference.worldData.chunkDictionary.ContainsKey(worldPosition))
            return worldReference.worldData.chunkDictionary[worldPosition];

        return null;
    }
}
