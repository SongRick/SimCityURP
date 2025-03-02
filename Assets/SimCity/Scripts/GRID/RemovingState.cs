// 整体简介：
// 此代码定义了一个名为 RemovingState 的类，该类实现了 IBuildingState 接口。RemovingState 类用于管理游戏中物体的移除操作状态。
// 在该状态下，系统会根据玩家的鼠标位置，判断是否可以移除对应网格位置的物体，并提供相应的视觉和声音反馈。
// 它与网格系统、预览系统、数据管理系统、物体放置器以及声音反馈系统进行交互，确保移除操作的顺利进行和良好的用户体验。

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// RemovingState 类实现了 IBuildingState 接口，用于处理物体移除状态下的相关操作
public class RemovingState : IBuildingState
{
    // 存储要移除的游戏对象的索引，初始值为 -1 表示未选中任何对象
    private int gameObjectIndex = -1;

    // 网格对象，用于将世界坐标和网格坐标进行转换
    Grid grid;

    // 预览系统，用于显示移除操作的预览效果
    PreviewSystem previewSystem;

    // 统一使用的网格数据管理对象，用于管理物体的放置信息
    GridData gridData;

    // 物体放置器，用于实际移除游戏对象
    ObjectPlacer objectPlacer;

    // 声音反馈系统，用于播放移除操作相关的音效
    SoundFeedback soundFeedback;

    // 构造函数，初始化移除状态所需的各种对象，并启动移除预览
    public RemovingState(Grid grid,
                         PreviewSystem previewSystem,
                         GridData gridData,
                         ObjectPlacer objectPlacer,
                         SoundFeedback soundFeedback)
    {
        // 将传入的网格对象赋值给类的成员变量
        this.grid = grid;
        // 将传入的预览系统对象赋值给类的成员变量
        this.previewSystem = previewSystem;
        // 将传入的网格数据管理对象赋值给类的成员变量
        this.gridData = gridData;
        // 将传入的物体放置器对象赋值给类的成员变量
        this.objectPlacer = objectPlacer;
        // 将传入的声音反馈系统对象赋值给类的成员变量
        this.soundFeedback = soundFeedback;
        // 调用预览系统的方法，开始显示移除预览效果
        previewSystem.StartShowingRemovePreview();
    }

    // 结束移除状态的方法，停止显示预览效果
    public void EndState()
    {
        // 调用预览系统的方法，停止显示预览效果
        previewSystem.StopShowingPreview();
    }

    // 当玩家执行移除操作时调用的方法，处理移除逻辑
    public void OnAction(Vector3Int gridPosition)
    {
        // 检查网格数据中该网格位置是否有物体
        if (gridData.CanPlaceObejctAt(gridPosition, Vector2Int.one) == false)
        {
            // 调用声音反馈系统，播放移除物体的音效
            soundFeedback.PlaySound(SoundType.Remove);
            // 获取该网格位置物体的索引
            gameObjectIndex = gridData.GetRepresentationIndex(gridPosition);
            // 如果索引为 -1，说明该位置没有有效物体，直接返回
            if (gameObjectIndex == -1)
                return;
            // 调用数据管理对象的方法，移除该网格位置的物体信息
            gridData.RemoveObjectAt(gridPosition);
            // 调用物体放置器的方法，移除对应的游戏对象
            objectPlacer.RemoveObjectAt(gameObjectIndex);
        }
        else
        {
            // 调用声音反馈系统，播放错误放置的音效
            soundFeedback.PlaySound(SoundType.wrongPlacement);
        }

        // 将网格坐标转换为世界坐标
        Vector3 cellPosition = grid.CellToWorld(gridPosition);
        // 调用预览系统的方法，更新预览位置，并根据该位置是否有效提供反馈
        previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition));
    }

    // 检查所选网格位置是否有可移除物体的方法
    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        // 如果该位置可以放置物体，说明没有可移除的物体，返回 false；否则返回 true
        return !gridData.CanPlaceObejctAt(gridPosition, Vector2Int.one);
    }

    // 当网格位置更新时调用的方法，更新预览效果
    public void UpdateState(Vector3Int gridPosition)
    {
        // 检查该网格位置是否有可移除物体
        bool validity = CheckIfSelectionIsValid(gridPosition);
        // 将网格坐标转换为世界坐标
        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        // 调用预览系统的方法，更新预览位置，并根据有效性提供反馈
        previewSystem.UpdatePosition(worldPosition, validity);
    }
}