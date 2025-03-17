// 整体简介：
// 这段代码主要定义了两个类，用于管理游戏中可放置物体的数据。`ObjectsDatabaseSO` 类继承自 `ScriptableObject`，它作为一个数据容器，能够在 Unity 编辑器中被创建为一个独立的资源文件，用于存储一系列 `ObjectData` 对象。`ObjectData` 类则是具体的物体数据模型，包含了物体的名称、唯一标识符、尺寸和预制体等信息。通过这种方式，开发者可以方便地在编辑器中管理和配置游戏中的各种可放置物体。

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 使用 [CreateAssetMenu] 特性，允许在 Unity 编辑器中通过菜单创建 ObjectsDatabaseSO 类型的资源文件
// 这个类继承自 ScriptableObject，用于存储一系列物体的数据
[CreateAssetMenu]
public class ObjectsDatabaseSO : ScriptableObject
{
    // 定义一个公共的 List，用于存储 ObjectData 类型的元素
    // 这个列表包含了所有可放置物体的详细数据
    public List<ObjectData> objectsData;
}

// 使用 [Serializable] 特性，使得该类可以被序列化，能够在 Unity 编辑器中显示和保存其数据
// 这个类用于存储单个物体的详细信息
[Serializable]
public class ObjectData
{
    // 使用 [field: SerializeField] 特性，将属性标记为可序列化，允许在 Unity 编辑器中设置其值
    // Name 属性用于存储物体的名称，并且只能在初始化时赋值，之后不能被修改
    [field: SerializeField]
    public string Name { get; private set; }

    // ID 属性用于存储物体的唯一标识符，同样只能在初始化时赋值，之后不能被修改
    [field: SerializeField]
    public int ID { get; private set; }

    // Size 属性用于存储物体的尺寸，类型为 Vector2Int，默认值为 (1, 1)
    // 该属性也只能在初始化时赋值，之后不能被修改
    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;

    // Prefab 属性用于存储物体对应的预制体，在 Unity 中预制体是一种可重复使用的游戏对象模板
    // 该属性同样只能在初始化时赋值，之后不能被修改
    [field: SerializeField]
    public GameObject Prefab { get; private set; }
    [field: SerializeField]
    public string category { get; private set; }


}