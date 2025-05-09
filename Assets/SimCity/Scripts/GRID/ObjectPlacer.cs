﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ObjectPlacer 类继承自 MonoBehaviour，用于管理游戏对象的放置和移除操作
public class ObjectPlacer : MonoBehaviour
{
    // 可在 Unity 编辑器中设置的序列化字段，用于存储已经放置的游戏对象列表
    [SerializeField]
    public List<GameObject> placedGameObjects = new();

    // 用于存储每个已放置游戏对象对应的 category
    private List<string> categoryList = new List<string>();

    // 用于存储每个 category 及其对应的建筑数量
    private Dictionary<string, int> categoryCount = new Dictionary<string, int>();

    // 该方法用于在指定位置放置一个游戏对象，并返回该对象在列表中的索引
    // 参数 prefab: 要放置的游戏对象的预制体
    // 参数 position: 游戏对象要放置的位置
    // 参数 parent: 指定的父 GameObject，若传入 null 则不设置父对象
    public int PlaceObject(GameObject prefab, Vector3 position, string category, GameObject parent = null)
    {
        // 实例化传入的预制体，创建一个新的游戏对象
        GameObject newObject = Instantiate(prefab);

        // 设置新创建的游戏对象的位置为传入的位置，略高于原位置，避免体积碰撞
        newObject.transform.position = new Vector3(position.x, position.y + 0.01f, position.z);

        // 如果指定了父对象，将新创建的对象设置为该父对象的子对象
        if (parent != null)
        {
            newObject.transform.SetParent(parent.transform);
        }

        // 将新创建的游戏对象添加到已放置游戏对象列表中
        placedGameObjects.Add(newObject);

        // 将对应的 category 添加到 categoryList 中
        categoryList.Add(category);

        // 更新 category 计数
        if (categoryCount.ContainsKey(category))
        {
            categoryCount[category]++;
        }
        else
        {
            categoryCount[category] = 1;
        }

        // 返回新对象在列表中的索引，由于列表索引从 0 开始，所以使用列表的当前数量减 1
        return placedGameObjects.Count - 1;
    }

    // 该方法用于移除指定索引位置的游戏对象
    // 参数 gameObjectIndex: 要移除的游戏对象在列表中的索引
    internal void RemoveObjectAt(int gameObjectIndex)
    {
        // 检查传入的索引是否超出了已放置游戏对象列表的范围
        // 或者该索引位置的游戏对象是否为空
        if (placedGameObjects.Count <= gameObjectIndex
            || placedGameObjects[gameObjectIndex] == null)
        {
            // 如果上述条件满足，则直接返回，不进行移除操作
            return;
        }

        // 获取要移除对象的 category
        string category = categoryList[gameObjectIndex];

        // 如果索引有效且对象不为空，则销毁该游戏对象
        Destroy(placedGameObjects[gameObjectIndex]);

        // 将该索引位置的列表元素置为 null，以便后续处理
        placedGameObjects[gameObjectIndex] = null;

        // 移除对应的 category 记录
        categoryList[gameObjectIndex] = "0"; // 用 0 表示已移除

        // 更新 category 计数
        if (categoryCount.ContainsKey(category))
        {
            categoryCount[category]--;
            if (categoryCount[category] == 0)
            {
                categoryCount.Remove(category);
            }
        }
    }

    // 提供一个公共方法，用于获取 category 统计信息
    public Dictionary<string, int> GetCategoryCount()
    {
        return categoryCount;
    }

    // ObjectPlacer.cs 新增方法
    public void UpdateObjectPosition(int index, Vector3 newPosition)
    {
        if (index < 0 || index >= placedGameObjects.Count || placedGameObjects[index] == null)
            return;

        placedGameObjects[index].transform.position = newPosition;
    }
}