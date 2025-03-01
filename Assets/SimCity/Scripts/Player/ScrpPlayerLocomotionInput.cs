using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SimCity.FinalController
{
    // 设置该脚本的执行顺序为 -2，意味着它会比默认顺序的脚本更早执行，
    // 保证输入处理逻辑能在其他依赖输入的脚本之前完成
    [DefaultExecutionOrder(-2)]
    public class PlayerLocomotionInput : MonoBehaviour, PlayerControls.IPlayerLocomotionMapActions
    {
        #region Class variables
        // 玩家输入控制类的实例，用于管理玩家的各种输入操作，
        // 借助它可以监听玩家的按键、鼠标移动等输入事件
        public PlayerControls PlayerControls { get; private set; }

        // 存储玩家的移动输入向量，将其改为三维向量以支持上下移动，
        // 分别对应 X（左右）、Y（上下）、Z（前后）方向的输入
        public Vector3 MovementInput { get; private set; }

        // 存储玩家的视角输入向量，用于控制相机的旋转，
        // 通常 X 分量控制水平视角旋转，Y 分量控制垂直视角旋转
        public Vector2 LookInput { get; private set; }

        // 标记玩家是否按下了跳跃键，用于判断是否触发跳跃动作
        public bool JumpPressed { get; private set; }

        // 标记玩家是否开启了冲刺模式，默认未开启
        public bool SprintToggledOn { get; private set; } = false;

        // 标记玩家是否开启了飞行模式，默认开启
        public bool FlightModeToggleOn { get; private set; } = true;
        #endregion

        #region Startup
        // 当该脚本实例被启用时调用，用于初始化输入系统
        private void OnEnable()
        {
            // 创建一个新的 PlayerControls 实例，用于管理输入操作
            PlayerControls = new PlayerControls();
            // 启用 PlayerControls 实例，使其开始监听输入事件
            PlayerControls.Enable();

            // 启用 PlayerLocomotionMap 动作映射，该映射包含了移动、视角和跳跃等输入动作
            PlayerControls.PlayerLocomotionMap.Enable();
            // 为 PlayerLocomotionMap 中的输入动作设置回调函数，将其绑定到当前类的相应方法上，
            // 当有对应输入事件发生时，会调用绑定的方法进行处理
            PlayerControls.PlayerLocomotionMap.SetCallbacks(this);
        }

        // 当该脚本实例被禁用时调用，用于关闭输入系统
        private void OnDisable()
        {
            // 禁用 PlayerLocomotionMap 动作映射，停止监听该映射下的输入事件
            PlayerControls.PlayerLocomotionMap.Disable();
            // 移除 PlayerLocomotionMap 中的输入动作的回调函数，避免不必要的调用
            PlayerControls.PlayerLocomotionMap.RemoveCallbacks(this);
        }
        #endregion

        #region Late Update Logic
        // 在每帧的 LateUpdate 阶段调用，用于重置跳跃按键状态
        private void LateUpdate()
        {
            // 将跳跃按键状态标记为未按下，确保每次按键事件只处理一次，
            // 避免持续触发跳跃动作
            JumpPressed = false;
        }
        #endregion

        #region Input Callbacks
        // 当玩家进行移动输入时调用的回调函数
        public void OnMovement(InputAction.CallbackContext context)
        {
            // 读取三维移动输入向量，将其赋值给 MovementInput，
            // 此向量包含了玩家在 X、Y、Z 方向上的移动输入
            MovementInput = context.ReadValue<Vector3>();
        }

        // 当玩家进行视角输入时调用的回调函数
        public void OnLook(InputAction.CallbackContext context)
        {
            // 从输入事件的上下文中读取视角输入向量，并将其赋值给 LookInput，
            // 用于控制相机的旋转视角
            LookInput = context.ReadValue<Vector2>();
        }

        // 当玩家进行跳跃输入时调用的回调函数
        public void OnJump(InputAction.CallbackContext context)
        {
            // 检查输入事件是否已执行（即按键被按下）
            if (context.performed)
            {
                // 如果按键被按下，将跳跃按键状态标记为已按下，
                // 后续可根据此标记触发跳跃动作
                JumpPressed = true;
            }
        }

        // 当玩家进行冲刺模式切换输入时调用的回调函数
        public void OnToggleSprint(InputAction.CallbackContext context)
        {
            // 检查输入事件是否已执行（即按键被按下）
            if (context.performed)
            {
                // 如果按键被按下，切换冲刺模式的开启状态
                SprintToggledOn = !SprintToggledOn;
            }
        }

        // 当玩家进行飞行模式切换输入时调用的回调函数
        public void OnEnableFlightMode(InputAction.CallbackContext context)
        {
            // 切换飞行模式的开启状态
            FlightModeToggleOn = !FlightModeToggleOn;
        }
        #endregion

        #region Other Methods
        // 禁用冲刺模式的方法
        public void DisableSprint()
        {
            // 将冲刺模式标记为未开启
            SprintToggledOn = false;
        }
        // 启用飞行模式的方法
        public void EnableFlightMode()
        {
            // 进入飞行模式
            FlightModeToggleOn = true;
        }
        #endregion
    }
}