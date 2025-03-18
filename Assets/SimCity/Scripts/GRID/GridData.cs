// 整体简介：
// 这段代码主要实现了一个网格数据管理系统，用于管理物体在网格上的放置、移除和位置检查等操作。
// 包含两个核心类：GridData 类和 PlacementData 类。GridData 类负责管理整个网格数据，处理物体的添加、移除、检查是否可放置以及获取物体索引等操作；
// PlacementData 类用于存储单个物体在网格上的放置信息，如物体占据的网格位置、物体的 ID 和放置物体的索引。

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GridData 类：管理网格数据，提供对物体在网格上操作的功能
public class GridData
{
    // 字典，用于存储已放置物体的信息
    // 键为网格位置（Vector3Int 类型），值为 PlacementData 类型的对象，包含物体的详细放置信息
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    // 方法：在指定网格位置添加一个物体
    // 参数说明：
    // gridPosition：物体放置的起始网格位置
    // objectSize：物体在网格上占据的大小，x 表示宽度，y 表示长度
    // ID：物体的唯一标识符
    // placedObjectIndex：放置物体的索引，用于标识该物体在某个集合中的位置
    public void AddObjectAt(Vector3Int gridPosition,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex)
    {
        // 计算物体在网格上要占据的所有位置
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        // 创建一个新的 PlacementData 对象，存储物体的放置信息
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
        // 遍历物体要占据的每个网格位置
        foreach (var pos in positionToOccupy)
        {
            // 检查该位置是否已经有物体放置
            if (placedObjects.ContainsKey(pos))
                // 如果已有物体，抛出异常提示该位置已被占用
                throw new Exception($"Dictionary already contains this cell positiojn {pos}");
            // 如果该位置未被占用，将该位置和对应的放置信息添加到字典中
            placedObjects[pos] = data;
        }
    }

    // 私有方法：计算物体在网格上要占据的所有位置
    // 参数说明：
    // gridPosition：物体放置的起始网格位置
    // objectSize：物体在网格上占据的大小，x 表示宽度，y 表示长度
    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        // 用于存储物体要占据的所有网格位置的列表
        List<Vector3Int> returnVal = new();
        // 遍历物体的宽度方向
        for (int x = 0; x < objectSize.x; x++)
        {
            // 遍历物体的长度方向
            for (int y = 0; y < objectSize.y; y++)
            {
                // 计算当前位置相对于起始位置的偏移量，并添加到列表中
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        // 返回存储所有占据位置的列表
        return returnVal;
    }

    // 方法：检查是否可以在指定网格位置放置指定大小的物体
    // 参数说明：
    // gridPosition：物体尝试放置的起始网格位置
    // objectSize：物体在网格上占据的大小，x 表示宽度，y 表示长度
    public bool CanPlaceObejctAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        // 计算物体在网格上要占据的所有位置
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        // 遍历物体要占据的每个网格位置
        foreach (var pos in positionToOccupy)
        {
            // 检查该位置是否已经有物体放置
            if (placedObjects.ContainsKey(pos))
                // 如果已有物体，返回 false 表示不能放置
                return false;
        }
        // 如果所有位置都未被占用，返回 true 表示可以放置
        return true;
    }

    // 内部方法：获取指定网格位置上放置物体的表示索引
    // 参数说明：
    // gridPosition：要查询的网格位置
    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        // 检查该网格位置是否有物体放置
        if (placedObjects.ContainsKey(gridPosition) == false)
            // 如果没有物体，返回 -1 表示该位置无物体
            return -1;
        // 如果有物体，返回该物体的放置索引
        return placedObjects[gridPosition].PlacedObjectIndex;
    }

    // 内部方法：移除指定网格位置上的物体
    // 参数说明：
    // gridPosition：要移除物体的起始网格位置
    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        // 遍历该物体占据的所有网格位置
        foreach (var pos in placedObjects[gridPosition].occupiedPositions)
        {
            // 从字典中移除该位置及其对应的放置信息
            placedObjects.Remove(pos);
        }
    }
    // GridData.cs 新增方法
    public PlacementData GetPlacementDataByIndex(int index)
    {
        foreach (var data in placedObjects.Values)
        {
            if (data.PlacedObjectIndex == index)
                return data;
        }
        return null;
    }

    public void MoveObject(Vector3Int newGridPosition, int index)
    {
        PlacementData data = GetPlacementDataByIndex(index);
        if (data == null) return;

        // 移除旧位置
        foreach (var pos in data.occupiedPositions)
        {
            placedObjects.Remove(pos);
        }

        // 计算新位置
        List<Vector3Int> newPositions = CalculatePositions(newGridPosition, data.Size);

        // 添加新位置
        foreach (var pos in newPositions)
        {
            placedObjects[pos] = data;
        }
        data.occupiedPositions = newPositions;
    }
}

// PlacementData 类：存储单个物体在网格上的放置信息
public class PlacementData
{
    // 物体占据的所有网格位置的列表
    public List<Vector3Int> occupiedPositions;
    // 物体的唯一标识符，使用私有设置器，确保只能在构造函数中赋值
    public int ID { get; private set; }
    // 放置物体的索引，使用私有设置器，确保只能在构造函数中赋值
    public int PlacedObjectIndex { get; private set; }

    // 构造函数：初始化 PlacementData 对象
    // 参数说明：
    // occupiedPositions：物体占据的所有网格位置的列表
    // iD：物体的唯一标识符
    // placedObjectIndex：放置物体的索引
    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        // 初始化物体占据的位置列表
        this.occupiedPositions = occupiedPositions;
        // 初始化物体的 ID
        ID = iD;
        // 初始化放置物体的索引
        PlacedObjectIndex = placedObjectIndex;
    }
}