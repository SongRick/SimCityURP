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
    private int lastSelectedIndex = -1;
    private Renderer selectedRenderer;
    private Renderer lastSelectedRenderer;
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
                selectedIndex = getIndexInBuildinglist(clickedObject);
                // 找到了点击的对象，但不在列表中
                if (selectedIndex == -1) 
                {
                    Debug.Log("点击的对象是" + clickedObject + "，不在建筑列表中！");
                }
                // 找到了点击的对象，且在列表中
                else
                {
                    Debug.Log("点击的对象是" + clickedObject + "，在建筑列表中的序号是：" + selectedIndex);
                    changeSelectedState(selectedIndex);
                }
            }
        }
    }
    // 返回鼠标点击的对象在建筑列表里的序号，若不在列表中，则返回-1
    public int getIndexInBuildinglist(GameObject gameObject)
    {
        // 处理空对象或无效组件的情况
        if (gameObject == null || objectPlacer == null || objectPlacer.placedGameObjects == null)
            return -1;

        GameObject current = gameObject;

        // 向上遍历父对象链
        while (current != null)
        {
            // 遍历已放置对象列表
            for (int i = 0; i < objectPlacer.placedGameObjects.Count; i++)
            {
                // 找到匹配的父对象时返回索引
                if (objectPlacer.placedGameObjects[i] == current)
                {
                    return i;
                }
            }

            // 移动到上一级父对象（使用空值传播避免NullReference）
            current = current.transform.parent?.gameObject;
        }

        // 遍历完所有父对象仍未找到
        return -1;
    }

    public void changeSelectedState(int index)
    {
        // 验证索引有效性
        if (objectPlacer == null ||
            objectPlacer.placedGameObjects == null ||
            index < 0 ||
            index >= objectPlacer.placedGameObjects.Count)
        {
            Debug.LogError("无效的索引或组件未初始化");
            return;
        }

        // 获取目标对象A
        GameObject objA = objectPlacer.placedGameObjects[index].transform.GetChild(0).gameObject;
        if (objA == null)
        {
            Debug.LogError($"索引 {index} 对应的对象不存在");
            return;
        }

        // 寻找同名子对象B
        GameObject objB = null;
        foreach (Transform child in objA.transform)
        {
            if (child.gameObject.name == objA.name)
            {
                objB = child.gameObject;
                break;
            }
        }
        // 如果存在同名子对象
        if (objB)
        {
            Debug.Log("objB:" + objB.name);
            selectedRenderer = objA.GetComponent<Renderer>();
        }
        // 如果不存在同名子对象
        else
        {
            Debug.Log("objA:" + objA.name);
            selectedRenderer = objA.GetComponent<Renderer>();
        }
        if(selectedRenderer)
        {
            selectedRenderer.material.color = Color.red;
        }
    }
}