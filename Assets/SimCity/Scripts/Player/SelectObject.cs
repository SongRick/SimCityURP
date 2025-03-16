// 引入 SimCity.FinalController 命名空间，可能包含与游戏最终控制器相关的类和功能
using SimCity.FinalController;
// 引入 System.Collections 命名空间，该命名空间提供了用于处理集合的接口和类
using System.Collections;
// 引入 System.Collections.Generic 命名空间，提供了泛型集合相关的接口和类
using System.Collections.Generic;
// 引入 UnityEngine 命名空间，这是 Unity 引擎的核心命名空间，包含了大量基础类和功能
using UnityEngine;
// 引入 UnityEngine.InputSystem 命名空间，用于处理输入系统相关的功能
using UnityEngine.InputSystem;

// 定义一个名为 SelectObject 的公共类，继承自 MonoBehaviour，这意味着它可以作为一个组件挂载到游戏对象上
public class SelectObject : MonoBehaviour
{
    // 使用 [SerializeField] 特性，使得该私有字段可以在 Unity 编辑器的 Inspector 面板中进行赋值
    [SerializeField]
    // 用于放置物体的对象引用
    private ObjectPlacer objectPlacer;
    // 放置系统的引用，负责处理物体的放置逻辑
    private PlacementSystem placementSystem;
    // 当前选中物体在 objectPlacer.placedGameObjects 列表中的索引，初始化为 -1 表示未选中任何物体
    private int selectedIndex = -1;
    // 选择模式开关，用于控制是否开启选择物体的功能
    public bool SelectModeToggleOn = false;
    // 新增字段：保存原始材质和颜色
    private Material originalMaterial;
    private Color originalColor;

    // 在脚本实例被加载时调用，常用于初始化操作
    private void Awake()
    {
        // 在场景中查找 ObjectPlacer 组件并赋值给 objectPlacer 变量
        objectPlacer = FindObjectOfType<ObjectPlacer>();
        // 如果未找到 ObjectPlacer 组件，输出错误日志
        if (objectPlacer == null)
        {
            Debug.LogError("未找到ObjectPlacer组件！");
        }
        // 在场景中查找 PlacementSystem 组件并赋值给 placementSystem 变量
        placementSystem = FindObjectOfType<PlacementSystem>();
        // 如果未找到 PlacementSystem 组件，输出错误日志
        if (placementSystem == null)
        {
            Debug.LogError("未找到PlacementSystem组件！");
        }
    }

    // 用于开启选择模式的方法
    public void toggleSelectModeOn()
    {
        // 将选择模式开关设置为 true，开启选择模式
        SelectModeToggleOn = true;
        // 调用 PlacementSystem 组件的 initState 方法，可能用于初始化放置系统的状态
        placementSystem.initState();
    }

    // 用于选择建筑的方法
    public void selectBuilding()
    {
        // 检查鼠标左键是否在当前帧被按下
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // 从主摄像机的屏幕点发射一条射线，射线的起点为鼠标当前位置
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            // 用于存储射线碰撞信息的结构体
            RaycastHit hit;
            // 发射射线并检查是否有碰撞
            if (Physics.Raycast(ray, out hit))
            {
                // 获取射线碰撞到的物体的游戏对象
                GameObject clickedObject = hit.collider.gameObject;
                // 临时变量，用于遍历父对象链
                GameObject current = clickedObject;
                // 标记是否为已放置的物体
                bool isPlacedObject = false;

                // 遍历父对象链，检查是否在 objectPlacer.placedGameObjects 列表中
                while (current != null && !isPlacedObject)
                {
                    // 遍历 objectPlacer.placedGameObjects 列表中的每个物体
                    foreach (var obj in objectPlacer.placedGameObjects)
                    {
                        // 检查当前物体是否在列表中
                        if (obj != null && obj == current)
                        {
                            // 如果在列表中，标记为已放置的物体
                            isPlacedObject = true;
                            break;
                        }
                    }
                    // 如果不是已放置的物体，继续检查其父对象
                    if (!isPlacedObject)
                    {
                        // 获取当前物体的父对象
                        current = current.transform.parent?.gameObject;
                    }
                }

                // 修改后的选中逻辑
                if (isPlacedObject)
                {
                    // 恢复上一个选中对象的颜色（改进版）
                    if (selectedIndex != -1 && selectedIndex < objectPlacer.placedGameObjects.Count)
                    {
                        GameObject prevObj = objectPlacer.placedGameObjects[selectedIndex];
                        if (prevObj != null)
                        {
                            // 关键修改：获取子对象Renderer（同名情况适用）
                            Renderer prevChildRenderer = prevObj.transform.Find("BB-080")?.GetComponent<Renderer>();
                            if (prevChildRenderer != null && originalMaterial != null)
                            {
                                // 还原原始材质和颜色
                                prevChildRenderer.material = originalMaterial;
                                prevChildRenderer.material.color = originalColor;
                            }
                        }
                    }

                    // 设置新选中对象为红色
                    Renderer currentRenderer = current.GetComponentInChildren<Renderer>();
                    if (currentRenderer != null)
                    {
                        // 保存原始材质和颜色
                        originalMaterial = currentRenderer.material;
                        originalColor = originalMaterial.color;

                        // 创建新材质实例避免污染原始材质
                        Material highlightMat = new Material(originalMaterial);
                        highlightMat.color = Color.red;
                        currentRenderer.material = highlightMat;

                        selectedIndex = objectPlacer.placedGameObjects.IndexOf(current);
                    }
                }
            }
        }
    }
}