using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

// 脚本功能说明：
// 此脚本定义了一个名为 PlacementState 的类，该类实现了 IBuildingState 接口。
// 其主要功能是处理物体放置状态下的一系列逻辑，包括初始化放置状态、显示放置预览、
// 检查放置的有效性、执行放置操作、结束放置状态以及更新放置预览等。

// PlacementState 类，用于管理物体放置状态下的相关逻辑
public class PlacementState : IBuildingState
{
    // 所选物体在数据库中的索引，初始值 -1 表示尚未选择任何物体
    private int selectedObjectIndex = -1;
    // 所选物体的唯一标识符
    int ID;
    // 场景中的网格对象，用于进行世界坐标和网格坐标之间的转换
    Grid grid;
    // 预览系统，用于在场景中显示物体放置的预览效果
    PreviewSystem previewSystem;
    // 物体数据库，存储了所有可放置物体的详细信息
    ObjectsDatabaseSO database;
    // 地面数据对象，用于管理地面物体的放置信息
    GridData floorData;
    // 家具数据对象，用于管理家具物体的放置信息
    GridData furnitureData;
    // 物体放置器，负责将物体实际放置到场景中
    ObjectPlacer objectPlacer;
    // 声音反馈系统，根据不同的操作播放相应的声音
    SoundFeedback soundFeedback;
    // 指定的新建建筑的父 GameObject，若传入 null 则不设置父对象
    GameObject parentObject = GameObject.Find("Buildings");

    // 构造函数，用于初始化 PlacementState 类的各个参数
    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabaseSO database,
                          GridData floorData,
                          GridData furnitureData,
                          ObjectPlacer objectPlacer,
                          SoundFeedback soundFeedback)
    {
        // 为所选物体的 ID 赋值
        ID = iD;
        // 保存传入的网格对象
        this.grid = grid;
        // 保存传入的预览系统
        this.previewSystem = previewSystem;
        // 保存传入的物体数据库
        this.database = database;
        // 保存传入的地面数据
        this.floorData = floorData;
        // 保存传入的家具数据
        this.furnitureData = furnitureData;
        // 保存传入的物体放置器
        this.objectPlacer = objectPlacer;
        // 保存传入的声音反馈系统
        this.soundFeedback = soundFeedback;

        // 在数据库中查找指定 ID 的物体的索引
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        // 如果找到了对应的物体
        if (selectedObjectIndex > -1)
        {
            // 调用预览系统开始显示该物体的放置预览
            previewSystem.StartShowingPlacementPreview(
                database.objectsData[selectedObjectIndex].Prefab,
                database.objectsData[selectedObjectIndex].Size);
        }
        else
        {
            // 如果未找到对应的物体，抛出异常提示
            throw new System.Exception($"No object with ID {iD}");
        }
    }

    // 结束当前放置状态时调用的方法
    public void EndState()
    {
        // 调用预览系统停止显示物体放置的预览效果
        previewSystem.StopShowingPreview();
    }

    // 当执行放置操作时调用的方法，参数为网格位置
    public void OnAction(Vector3Int gridPosition)
    {
        // 检查在指定网格位置放置所选物体是否有效
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        // 如果放置无效
        if (placementValidity == false)
        {
            // 播放放置错误的声音
            soundFeedback.PlaySound(SoundType.wrongPlacement);
            return;
        }
        // 如果放置有效，播放放置成功的声音
        soundFeedback.PlaySound(SoundType.Place);
        // 调用物体放置器将所选物体放置到指定的世界坐标位置，并获取该物体的索引，指定父gameobject
        int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab,
            grid.CellToWorld(gridPosition), parentObject);

        // 根据所选物体的 ID 判断是地面物体还是家具物体，选择对应的网格数据
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
            floorData :
            furnitureData;
        // 在对应的网格数据中记录该物体的放置信息
        selectedData.AddObjectAt(gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            index);

        // 更新预览系统的位置，将其移动到放置位置，并设置为无效状态（可能是逻辑需要）
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    // 检查在指定网格位置放置所选物体是否有效的私有方法
    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        // 根据所选物体的 ID 判断是地面物体还是家具物体，选择对应的网格数据
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
            floorData :
            furnitureData;

        // 调用所选网格数据的方法检查是否可以在指定位置放置该物体
        return selectedData.CanPlaceObejctAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    // 当状态更新时调用的方法，通常在鼠标移动时更新预览位置
    public void UpdateState(Vector3Int gridPosition)
    {
        // 检查在指定网格位置放置所选物体是否有效
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        // 更新预览系统的位置和有效性状态
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}