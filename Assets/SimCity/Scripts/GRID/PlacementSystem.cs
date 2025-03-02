// 整体简介：
// 该代码定义了一个名为 PlacementSystem 的类，它继承自 MonoBehaviour，用于管理游戏中物体的放置和移除系统。
// 该系统会处理玩家的输入事件，根据不同的操作（如开始放置、开始移除）创建相应的状态对象，并在游戏过程中更新状态。
// 同时，它还负责管理网格可视化、预览效果、声音反馈等，确保玩家在进行放置和移除操作时能获得良好的交互体验。

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// PlacementSystem 类用于管理游戏中物体的放置和移除操作，继承自 MonoBehaviour 以便在 Unity 中使用
public class PlacementSystem : MonoBehaviour
{
    // 可在 Unity 编辑器中设置的输入管理器，用于处理玩家的输入事件
    [SerializeField]
    private InputManager inputManager;

    // 可在 Unity 编辑器中设置的网格对象，用于将世界坐标转换为网格坐标
    [SerializeField]
    private Grid grid;

    // 可在 Unity 编辑器中设置的物体数据库，存储了所有可放置物体的信息
    [SerializeField]
    private ObjectsDatabaseSO database;

    // 可在 Unity 编辑器中设置的网格可视化对象，用于显示网格效果
    [SerializeField]
    private GameObject gridVisualization;

    // 可在 Unity 编辑器中设置的错误放置音效剪辑
    [SerializeField]
    private AudioClip wrongPlacementClip;

    // 可在 Unity 编辑器中设置的音频源，用于播放音效
    [SerializeField]
    private AudioSource source;

    // 统一使用的网格数据，用于管理物体的放置信息
    private GridData gridData;

    // 可在 Unity 编辑器中设置的预览系统，用于显示物体放置的预览效果
    [SerializeField]
    private PreviewSystem preview;

    // 记录上一次检测到的网格位置，初始化为 (0, 0, 0)
    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    // 可在 Unity 编辑器中设置的物体放置器，用于实际放置物体
    [SerializeField]
    private ObjectPlacer objectPlacer;

    // 当前的建筑状态对象，用于管理不同的操作状态
    IBuildingState buildingState;

    // 可在 Unity 编辑器中设置的声音反馈系统，用于播放不同操作的音效
    [SerializeField]
    private SoundFeedback soundFeedback;

    // 在脚本实例被启用时调用，进行初始化操作
    private void Start()
    {
        // 初始时禁用网格可视化
        gridVisualization.SetActive(false);

        // 初始化网格数据
        gridData = new();
    }

    // 开始物体放置操作
    // 参数 ID: 要放置物体的 ID
    public void StartPlacement(int ID)
    {
        // 停止当前可能正在进行的放置操作
        StopPlacement();

        // 启用网格可视化
        gridVisualization.SetActive(true);

        // 创建一个新的放置状态对象
        buildingState = new PlacementState(ID,
                                           grid,
                                           preview,
                                           database,
                                           gridData,
                                           objectPlacer,
                                           soundFeedback);

        // 注册点击事件，点击时调用 PlaceStructure 方法
        inputManager.OnClicked += PlaceStructure;

        // 注册退出事件，退出时调用 StopPlacement 方法
        inputManager.OnExit += StopPlacement;
    }

    // 开始物体移除操作
    public void StartRemoving()
    {
        // 停止当前可能正在进行的放置操作
        StopPlacement();

        // 启用网格可视化
        gridVisualization.SetActive(true);

        // 创建一个新的移除状态对象
        buildingState = new RemovingState(grid, preview, gridData, objectPlacer, soundFeedback);

        // 注册点击事件，点击时调用 PlaceStructure 方法
        inputManager.OnClicked += PlaceStructure;

        // 注册退出事件，退出时调用 StopPlacement 方法
        inputManager.OnExit += StopPlacement;
    }

    // 处理放置物体的操作
    private void PlaceStructure()
    {
        // 如果鼠标指针在 UI 上，则不进行放置操作
        if (inputManager.IsPointerOverUI())
        {
            return;
        }

        // 获取鼠标在地图上的位置
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();

        // 将鼠标的世界坐标转换为网格坐标
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        // 调用当前建筑状态的 OnAction 方法，处理放置操作
        buildingState.OnAction(gridPosition);
    }

    // 停止当前的放置或移除操作
    private void StopPlacement()
    {
        // 播放点击音效
        soundFeedback.PlaySound(SoundType.Click);

        // 如果当前没有建筑状态，则直接返回
        if (buildingState == null)
            return;

        // 禁用网格可视化
        gridVisualization.SetActive(false);

        // 结束当前建筑状态
        buildingState.EndState();

        // 取消点击事件的注册
        inputManager.OnClicked -= PlaceStructure;

        // 取消退出事件的注册
        inputManager.OnExit -= StopPlacement;

        // 重置上一次检测到的网格位置
        lastDetectedPosition = Vector3Int.zero;

        // 清空当前建筑状态
        buildingState = null;
    }

    // 每帧更新时调用，处理状态更新
    private void Update()
    {
        // 如果当前没有建筑状态，则不进行更新
        if (buildingState == null)
            return;

        // 获取鼠标在地图上的位置
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();

        // 将鼠标的世界坐标转换为网格坐标
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        // 如果当前网格位置与上一次检测到的位置不同
        if (lastDetectedPosition != gridPosition)
        {
            // 调用当前建筑状态的 UpdateState 方法，更新状态
            buildingState.UpdateState(gridPosition);

            // 更新上一次检测到的网格位置
            lastDetectedPosition = gridPosition;
        }
    }
}