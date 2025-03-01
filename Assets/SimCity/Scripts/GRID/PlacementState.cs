using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 此引用语句可能是多余的，因为这里没有实际使用 Unity.VisualScripting.Member 中的静态成员
using static Unity.VisualScripting.Member;

// PlacementState 类实现了 IBuildingState 接口，用于处理物体放置状态下的相关逻辑
public class PlacementState : IBuildingState
{
    // 所选物体在数据库中的索引，初始化为 -1 表示未选择
    private int selectedObjectIndex = -1;
    // 所选物体的 ID
    int ID;
    // 场景中的网格对象，用于将世界坐标和网格坐标进行转换
    Grid grid;
    // 预览系统，用于显示物体放置的预览效果
    PreviewSystem previewSystem;
    // 物体数据库，存储了所有可放置物体的信息
    ObjectsDatabaseSO database;
    // 地面数据，用于管理地面物体的放置信息
    GridData floorData;
    // 家具数据，用于管理家具物体的放置信息
    GridData furnitureData;
    // 物体放置器，负责实际放置物体到场景中
    ObjectPlacer objectPlacer;
    // 声音反馈系统，用于播放不同操作的声音
    SoundFeedback soundFeedback;

    // 构造函数，用于初始化放置状态所需的参数
    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabaseSO database,
                          GridData floorData,
                          GridData furnitureData,
                          ObjectPlacer objectPlacer,
                          SoundFeedback soundFeedback)
    {
        // 赋值所选物体的 ID
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;

        // 在数据库中查找指定 ID 的物体索引
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
        // 调用预览系统停止显示预览
        previewSystem.StopShowingPreview();
    }

    // 当执行放置操作时调用的方法
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
        // 调用物体放置器将所选物体放置到指定的世界坐标位置，并获取该物体的索引
        int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab,
            grid.CellToWorld(gridPosition));

        // 根据所选物体的 ID 判断是地面物体还是家具物体，选择对应的网格数据
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
            floorData :
            furnitureData;
        // 在对应的网格数据中记录该物体的放置信息
        selectedData.AddObjectAt(gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            index);

        // 更新预览系统的位置，将其移动到放置位置，并设置为无效状态（这里可能是逻辑需要，根据实际情况）
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

    // 当状态更新时调用的方法，通常是鼠标移动时更新预览位置
    public void UpdateState(Vector3Int gridPosition)
    {
        // 检查在指定网格位置放置所选物体是否有效
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        // 更新预览系统的位置和有效性状态
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}