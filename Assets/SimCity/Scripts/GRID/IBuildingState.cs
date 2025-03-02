// 整体简介：
// 此代码定义了一个名为 IBuildingState 的接口，该接口在游戏开发中用于规范建筑状态相关的操作。
// 在游戏里，建筑状态可能包含放置、移除等不同的操作阶段，每个阶段都需要有对应的处理逻辑。
// 通过定义这个接口，开发者可以让不同的建筑状态类实现这些统一的方法，从而实现代码的模块化和可扩展性，
// 方便管理和切换不同的建筑状态。

using UnityEngine;

// 定义一个名为 IBuildingState 的接口，用于规范建筑状态的操作
public interface IBuildingState
{
    // 结束当前建筑状态的方法
    // 当建筑状态结束时，调用此方法进行清理或其他结束操作
    void EndState();

    // 在建筑状态下执行具体动作的方法
    // 参数 gridPosition 表示动作发生的网格位置
    // 例如在放置物体时，该位置就是要放置物体的网格位置
    void OnAction(Vector3Int gridPosition);

    // 更新建筑状态的方法
    // 参数 gridPosition 表示当前关注的网格位置
    // 当网格位置发生变化时，调用此方法更新状态，可能会更新预览效果等
    void UpdateState(Vector3Int gridPosition);
}