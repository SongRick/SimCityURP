// 整体简介：
// 此代码定义了一个名为 InputManager 的类，该类继承自 MonoBehaviour，主要用于处理游戏中的用户输入。
// 它负责监听鼠标点击和键盘按键事件，同时提供方法判断鼠标指针是否在 UI 上，以及获取鼠标点击位置对应的地图坐标。
// 通过事件机制，它可以通知其他系统（如建筑系统）用户的操作，实现了输入处理与其他功能模块的解耦，提高了代码的可维护性和可扩展性。

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// InputManager 类继承自 MonoBehaviour，用于管理游戏中的用户输入
public class InputManager : MonoBehaviour
{
    // 可在 Unity 编辑器中设置的场景相机，用于将屏幕坐标转换为世界坐标
    [SerializeField]
    private Camera sceneCamera;

    // 记录最后一次点击的世界坐标位置
    private Vector3 lastPosition;

    // 可在 Unity 编辑器中设置的层掩码，用于指定射线检测的层
    [SerializeField]
    private LayerMask placementLayermask;

    // 定义两个事件，分别在鼠标左键点击和按下 Esc 键时触发
    // 其他类可以订阅这些事件来响应相应的输入操作
    public event Action OnClicked, OnExit;

    // 每帧更新时调用，用于检测用户输入
    private void Update()
    {
        // 检测鼠标左键是否按下
        if (Input.GetMouseButtonDown(0))
            // 如果按下，触发 OnClicked 事件，若有订阅者则执行其相应逻辑
            OnClicked?.Invoke();
        // 检测是否按下 Esc 键
        if (Input.GetKeyDown(KeyCode.Escape))
            // 如果按下，触发 OnExit 事件，若有订阅者则执行其相应逻辑
            OnExit?.Invoke();
    }

    // 判断鼠标指针是否在 UI 元素上的方法
    public bool IsPointerOverUI()
        // 调用 EventSystem 的方法判断当前鼠标指针是否在 UI 元素上
        => EventSystem.current.IsPointerOverGameObject();

    // 获取鼠标点击位置对应的地图坐标的方法
    public Vector3 GetSelectedMapPosition()
    {
        // 获取鼠标在屏幕上的位置
        Vector3 mousePos = Input.mousePosition;
        // 设置鼠标位置的 z 坐标为相机的近裁剪面距离
        mousePos.z = sceneCamera.nearClipPlane;
        // 从相机位置向鼠标位置发射一条射线
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        // 用于存储射线检测的结果
        RaycastHit hit;
        // 进行射线检测，射线长度设置为 1000，只检测指定层的物体
        // 原代码中射线长度 100 可能太小，视野远处会失效，这里修改为 1000
        if (Physics.Raycast(ray, out hit, 1000, placementLayermask))
        {
            // 如果射线检测到物体，记录该物体的碰撞点位置
            lastPosition = hit.point;
        }
        // 返回最后一次点击的位置
        return lastPosition;
    }
}