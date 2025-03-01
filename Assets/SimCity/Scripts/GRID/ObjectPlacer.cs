// 整体简介：
// 此代码定义了一个名为 ObjectPlacer 的类，它继承自 MonoBehaviour。该类主要负责管理游戏中对象的放置和移除操作。
// 通过维护一个已放置游戏对象的列表，它能够在指定位置实例化新的游戏对象，并为其分配一个索引，方便后续管理。
// 同时，也能根据对象的索引从游戏场景中移除相应的游戏对象，确保场景的整洁和资源的合理利用。

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ObjectPlacer 类继承自 MonoBehaviour，用于管理游戏对象的放置和移除操作
public class ObjectPlacer : MonoBehaviour
{
    // 可在 Unity 编辑器中设置的序列化字段，用于存储已经放置的游戏对象列表
    [SerializeField]
    private List<GameObject> placedGameObjects = new();

    // 该方法用于在指定位置放置一个游戏对象，并返回该对象在列表中的索引
    // 参数 prefab: 要放置的游戏对象的预制体
    // 参数 position: 游戏对象要放置的位置
    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        // 实例化传入的预制体，创建一个新的游戏对象
        GameObject newObject = Instantiate(prefab);

        // 设置新创建的游戏对象的位置为传入的位置，略高于原位置，避免体积碰撞
        newObject.transform.position = new Vector3(position.x, position.y + 0.01f, position.z);

        // 将新创建的游戏对象添加到已放置游戏对象列表中
        placedGameObjects.Add(newObject);

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

        // 如果索引有效且对象不为空，则销毁该游戏对象
        Destroy(placedGameObjects[gameObjectIndex]);

        // 将该索引位置的列表元素置为 null，以便后续处理
        placedGameObjects[gameObjectIndex] = null;
    }
}