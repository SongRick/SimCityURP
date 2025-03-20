// 引入泛型集合命名空间，用于使用如 List、Dictionary 等数据结构
using System;
using System.Collections.Generic;
// 引入 Unity 引擎核心命名空间，包含了游戏对象、组件等核心功能
using UnityEngine;
// 引入 Unity 输入系统命名空间，用于处理输入事件
using UnityEngine.InputSystem;

// 定义一个名为 SelectObject 的公共类，继承自 MonoBehaviour，意味着该类可以作为组件挂载到游戏对象上
public class SelectObject : MonoBehaviour
{
    // 使用 [SerializeField] 特性，使得该私有字段可以在 Unity 编辑器的 Inspector 面板中进行赋值
    [SerializeField]
    // 用于放置物体的对象引用，通过该对象可以访问已放置的游戏对象列表
    private ObjectPlacer objectPlacer;
    // 放置系统的引用，负责处理物体的放置逻辑
    private PlacementSystem placementSystem;
    // 当前选中物体在 objectPlacer.placedGameObjects 列表中的索引，初始化为 -1 表示未选中任何物体
    private int selectedIndex = -1;
    // 上一次选中物体在 objectPlacer.placedGameObjects 列表中的索引，初始化为 -1 表示还没有上一次选中的物体
    private int lastSelectedIndex = -1;
    // 存储当前选中物体的材质列表
    private List<Material> selectedMaterials = new();
    // 存储上一次选中物体的材质列表
    private List<Material> lastSelectedMaterials = new();
    // 选择模式开关，用于控制是否开启选择物体的功能
    public bool SelectModeToggleOn = false;
    // 用于保存材质的原始颜色，键为材质，值为该材质的原始颜色
    private Dictionary<Material, Color> originalColors = new Dictionary<Material, Color>();

    // 在脚本实例被加载时调用，常用于初始化操作
    private void Awake()
    {
        // 在场景中查找 ObjectPlacer 组件并赋值给 objectPlacer 变量
        objectPlacer = FindObjectOfType<ObjectPlacer>();
        // 如果未找到 ObjectPlacer 组件，输出错误日志
        if (objectPlacer == null) Debug.LogError("未找到ObjectPlacer组件！");

        // 在场景中查找 PlacementSystem 组件并赋值给 placementSystem 变量
        placementSystem = FindObjectOfType<PlacementSystem>();
        // 如果未找到 PlacementSystem 组件，输出错误日志
        if (placementSystem == null) Debug.LogError("未找到PlacementSystem组件！");
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
                // 获取点击对象在建筑列表中的索引
                selectedIndex = getIndexInBuildinglist(clickedObject);
                // 如果索引为 -1，说明点击的对象不在建筑列表中
                if (selectedIndex == -1)
                    Debug.Log("点击的对象是" + clickedObject + "，不在建筑列表中！");
                else
                {
                    // 输出点击对象的信息和其在建筑列表中的索引
                    Debug.Log("点击的对象是" + clickedObject + "，在建筑列表中的序号是：" + selectedIndex);

                    // 判断当前点击的建筑是否已经处于选中状态
                    if (selectedIndex == lastSelectedIndex)
                    {
                        // 如果已经选中，恢复其初始颜色
                        restoreOriginalColor(lastSelectedMaterials);
                        lastSelectedIndex = -1;
                        selectedIndex = -1;
                        lastSelectedMaterials.Clear();
                        originalColors.Clear();
                    }
                    else
                    {
                        // 调用方法改变选中对象的状态（颜色）
                        changeSelectedState(selectedIndex);
                    }
                }
            }
        }
    }

    // 返回鼠标点击的对象在建筑列表里的序号，若不在列表中，则返回 -1
    public int getIndexInBuildinglist(GameObject gameObject)
    {
        // 处理空对象或无效组件的情况，如果游戏对象为空或者 objectPlacer 或其放置的游戏对象列表为空，则返回 -1
        if (gameObject == null || objectPlacer?.placedGameObjects == null) return -1;

        // 临时变量，用于遍历父对象链
        GameObject current = gameObject;
        // 向上遍历父对象链
        while (current != null)
        {
            // 遍历已放置对象列表
            for (int i = 0; i < objectPlacer.placedGameObjects.Count; i++)
            {
                // 找到匹配的父对象时返回索引
                if (objectPlacer.placedGameObjects[i] == current) return i;
            }
            // 移动到上一级父对象（使用空值传播避免 NullReference）
            current = current.transform.parent?.gameObject;
        }
        // 遍历完所有父对象仍未找到，返回 -1
        return -1;
    }

    // 改变选中建筑的状态（颜色）
    public void changeSelectedState(int index)
    {
        // 验证索引有效性，如果 objectPlacer 为空或者索引超出范围，则输出错误日志并返回
        if (objectPlacer == null || index < 0 || index >= objectPlacer.placedGameObjects.Count)
        {
            Debug.LogError("无效的索引或组件未初始化");
            return;
        }

        // 存储当前选中对象的材质列表
        List<Material> currentMaterials = new List<Material>();
        // 获取目标对象 A
        GameObject objA = objectPlacer.placedGameObjects[index].transform.GetChild(0).gameObject;
        // 如果目标对象 A 为空，输出错误日志并返回
        if (objA == null)
        {
            Debug.LogError($"索引 {index} 对应的对象不存在");
            return;
        }

        // 获取主对象的渲染器组件
        Renderer mainRenderer = objA.GetComponent<Renderer>();
        // 如果渲染器组件不为空，将其材质添加到当前材质列表中
        if (mainRenderer != null) currentMaterials.Add(mainRenderer.material);

        // 遍历主对象的子对象
        foreach (Transform child in objA.transform)
        {
            // 如果子对象的名称与主对象相同
            if (child.gameObject.name == objA.name)
            {
                // 获取子对象的渲染器组件
                Renderer childRenderer = child.GetComponent<Renderer>();
                if (childRenderer != null)
                {
                    // 遍历子对象的材质列表
                    foreach (Material mat in childRenderer.materials)
                    {
                        // 如果材质名称包含 "Atlas-1"
                        if (mat.name.Contains("Atlas-1"))
                        {
                            // 将该材质添加到当前材质列表中
                            currentMaterials.Add(mat);
                            // 跳出循环
                            break;
                        }
                    }
                }
            }
        }

        // 如果上一次选中的材质列表不为空
        if (lastSelectedMaterials.Count > 0)
        {
            // 遍历上一次选中的材质列表
            foreach (Material mat in lastSelectedMaterials)
            {
                // 尝试从原始颜色字典中获取该材质的原始颜色
                if (originalColors.TryGetValue(mat, out Color originalColor))
                {
                    // 将材质颜色恢复为原始颜色
                    mat.color = originalColor;
                }
            }
            // 清空原始颜色字典
            originalColors.Clear();
            // 清空上一次选中的材质列表
            lastSelectedMaterials.Clear();
        }

        // 遍历当前选中的材质列表
        foreach (Material mat in currentMaterials)
        {
            // 保存材质的原始颜色到原始颜色字典中
            originalColors[mat] = mat.color;
            // 将材质颜色设置为红色
            mat.color = Color.green;
        }

        // 更新上一次选中的材质列表为当前选中的材质列表
        lastSelectedMaterials = new List<Material>(currentMaterials);
        // 更新当前选中对象的索引
        lastSelectedIndex = index;
    }

    // 恢复材质的初始颜色
    private void restoreOriginalColor(List<Material> materials)
    {
        foreach (Material mat in materials)
        {
            if (originalColors.TryGetValue(mat, out Color originalColor))
            {
                mat.color = originalColor;
            }
        }
    }

    private string debugNoObjectSelected = "No object selected, please select an object first!";
    public string showObjectInfo()
    {
        if (selectedIndex == -1)
        {
            return debugNoObjectSelected;
        }
        else
        {
            Transform transform = objectPlacer.placedGameObjects[selectedIndex].transform;
            return $"Name:\t{transform.name}\n" +
                $"Position:\t{transform.position}\n" +
                $"Rotation:\t{transform.rotation}\n" +
                $"Scale:\t{transform.localScale}\n";
        }
    }
    public string removeObject()
    {
        if (selectedIndex == -1)
        {
            return debugNoObjectSelected;
        }
        else
        {
            Transform transform = objectPlacer.placedGameObjects[selectedIndex].transform;
            string strRemoveObject = $"{transform.name} has been removed!";
            objectPlacer.RemoveObjectAt(selectedIndex);
            return strRemoveObject;
        }
    }
    public string setObjectHeight(float x)
    {
        if (selectedIndex == -1)
        {
            return debugNoObjectSelected;
        }
        else
        {
            Transform transform = objectPlacer.placedGameObjects[selectedIndex].transform;
            string strSetObjectHeight = $"The height of {transform.name} has changed to {x} times";
            transform.localScale = new Vector3(1, x, 1);
            return strSetObjectHeight;
        }
    }
    public string countBuildingTypes()
    {
        Dictionary<string, int> categoryCount = objectPlacer.GetCategoryCount();
        string strCountBuildingTypes = "category\tcount\n";
        foreach (KeyValuePair<string, int> pair in categoryCount)
        {
            strCountBuildingTypes += $"{pair.Key}\t{pair.Value}\n";
        }

        return strCountBuildingTypes;
    }

}