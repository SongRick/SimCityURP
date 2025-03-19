using SimCity.FinalController;
using System;
using UnityEngine;

// PlacementSystem 类负责管理游戏中物体的放置和移除操作
public class PlacementSystem : MonoBehaviour
{
    // 序列化字段，用于在 Unity 编辑器中赋值
    // 输入管理器，处理用户输入
    [SerializeField]
    private InputManager inputManager;

    // 网格对象，用于定位和布局
    [SerializeField]
    private Grid grid;

    // 物体数据库，存储可放置的物体信息
    [SerializeField]
    private ObjectsDatabaseSO database;

    // 网格可视化对象，用于显示网格辅助
    [SerializeField]
    public GameObject gridVisualization;

    // 错误放置音效剪辑
    [SerializeField]
    private AudioClip wrongPlacementClip;

    // 音频源，用于播放音效
    [SerializeField]
    private AudioSource source;

    // 网格数据对象，存储网格相关信息
    private GridData gridData;

    // 预览系统，用于显示物体放置的预览效果
    [SerializeField]
    private PreviewSystem preview;

    // 上一次检测到的网格位置
    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    // 物体放置器，负责实际放置物体
    [SerializeField]
    private ObjectPlacer objectPlacer;

    // 当前的建筑状态接口
    public IBuildingState buildingState;

    // 音效反馈系统，用于播放不同类型的音效
    [SerializeField]
    private SoundFeedback soundFeedback;

    // UI 输入组件，用于获取用户的 UI 输入
    private UIInput _UIInput;
    private SelectObject selectObject;
    // 目标游戏对象，通常是玩家对象
    GameObject targetGameObject;

    // 脚本开始时调用的方法
    private void Start()
    {
        // 查找名为 "Player" 的游戏对象
        targetGameObject = GameObject.Find("Player");
        // 更新编辑模式状态
        updateEditMode();
        // 初始化网格数据对象
        gridData = new();

        selectObject = FindObjectOfType<SelectObject>();
        if (selectObject == null)
        {
            Debug.LogError("未找到SelectObject组件！");
        }
    }

    // 开始物体放置操作
    public void StartPlacement(int ID)
    {
        selectObject.SelectModeToggleOn = false;
        initState();
        // 创建一个新的放置状态对象
        buildingState = new PlacementState(ID, grid, preview, database, gridData, objectPlacer, soundFeedback);
        // 注册点击事件处理方法
        inputManager.OnClicked += PlaceStructure;
        // 注册退出事件处理方法
        inputManager.OnExit += StopPlacement;
    }
    public void initState()
    {
        // 停止当前的放置或移除操作
        StopPlacement();
        // 根据编辑模式开关激活或禁用网格可视化
        gridVisualization.SetActive(_UIInput.EditModeToggleOn);
    }

    // 开始物体移除操作
    public void StartRemoving()
    {
        selectObject.SelectModeToggleOn = false;
        initState();
        // 创建一个新的移除状态对象
        buildingState = new RemovingState(grid, preview, gridData, objectPlacer, soundFeedback);
        // 注册点击事件处理方法
        inputManager.OnClicked += PlaceStructure;
        // 注册退出事件处理方法
        inputManager.OnExit += StopPlacement;
    }
    // 处理物体放置或移除的方法
    public void PlaceStructure()
    {
        // 如果鼠标指针在 UI 上，则不进行操作
        if (inputManager.IsPointerOverUI())
        {
            return;
        }

        // 获取鼠标在地图上的位置
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        // 将世界坐标转换为网格坐标
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        // 调用当前建筑状态的操作方法
        buildingState.OnAction(gridPosition);
    }
    // 停止当前的放置或移除操作
    public void StopPlacement()
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
        // 取消注册点击事件处理方法
        inputManager.OnClicked -= PlaceStructure;
        // 取消注册退出事件处理方法
        inputManager.OnExit -= StopPlacement;
        // 重置上一次检测到的位置
        lastDetectedPosition = Vector3Int.zero;
        // 清空当前建筑状态
        buildingState = null;
    }
    // 更新编辑模式状态
    public void updateEditMode()
    {
        // 如果找到了目标游戏对象
        if (targetGameObject != null)
        {
            // 获取目标游戏对象上的 UI 输入组件
            _UIInput = targetGameObject.GetComponent<UIInput>();
        }
        else
        {
            // 若未找到目标游戏对象，输出错误信息
            Debug.LogError("未找到指定名称的游戏对象");
        }
        // 根据编辑模式开关激活或禁用网格可视化
        gridVisualization.SetActive(_UIInput.EditModeToggleOn);
    }



    // 每帧更新时调用的方法
    private void Update()
    {
        // 如果当前没有建筑状态，则不进行操作
        if (buildingState == null)
            return;

        // 获取鼠标在地图上的位置
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        // 将世界坐标转换为网格坐标
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        // 如果当前网格位置与上一次检测到的位置不同
        if (lastDetectedPosition != gridPosition)
        {
            // 更新当前建筑状态
            buildingState.UpdateState(gridPosition);
            // 更新上一次检测到的位置
            lastDetectedPosition = gridPosition;
        }
    }
    // 在 PlacementSystem 类中添加
    public void StartMoving(int index, Vector3Int gridPosition, Vector2Int size, int ID)
    {
        initState();
        buildingState = new MovingState(
            index,
            gridPosition,
            size,
            ID,
            grid,
            preview,
            gridData,
            objectPlacer,
            soundFeedback
        );
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }
}