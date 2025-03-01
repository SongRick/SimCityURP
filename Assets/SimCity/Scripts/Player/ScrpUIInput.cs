using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SimCity.FinalController
{
    // 设置该脚本的执行顺序为 -2，意味着它会比默认顺序的脚本更早执行，
    // 保证输入处理逻辑能在其他依赖输入的脚本之前完成
    [DefaultExecutionOrder(-3)]
    public class UIInput : MonoBehaviour, PlayerControls.IUIMapActions
    {
        #region Class variables
        // 玩家输入控制类的实例，用于管理玩家的各种输入操作，
        // 借助它可以监听玩家的按键、鼠标移动等输入事件
        public PlayerControls PlayerControls { get; private set; }

        // 光标是否锁定，默认解锁
        // true     光标锁定，即隐藏光标，且视角可变
        // false    光标解锁，即显示光标，且视角锁定
        public bool CursorLockToggledOn { get; private set; } = false;
        public bool DebugModeToggleOn { get; private set; } = true;
        public bool SelectObjectPressed { get; private set; }
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
            PlayerControls.UIMap.Enable();
            // 为 PlayerLocomotionMap 中的输入动作设置回调函数，将其绑定到当前类的相应方法上，
            // 当有对应输入事件发生时，会调用绑定的方法进行处理
            PlayerControls.UIMap.SetCallbacks(this);
        }

        // 当该脚本实例被禁用时调用，用于关闭输入系统
        private void OnDisable()
        {
            // 禁用 PlayerLocomotionMap 动作映射，停止监听该映射下的输入事件
            PlayerControls.UIMap.Disable();
            // 移除 PlayerLocomotionMap 中的输入动作的回调函数，避免不必要的调用
            PlayerControls.UIMap.RemoveCallbacks(this);
        }
        #endregion

        #region Input Callbacks
        public void OnCursorLock(InputAction.CallbackContext context)
        {
            //// 检查输入事件是否已执行（即按键被按下）
            //if (context.performed)
            //{
            //    // 如果按键被按下，切换光标锁定状态
            //    CursorLockToggledOn = !CursorLockToggledOn;
            //}
        }

        public void OnDebugMode(InputAction.CallbackContext context)
        {
            // 检查输入事件是否已执行（即按键被按下）
            if (context.performed)
            {
                // 如果按键被按下，切换debug模式
                DebugModeToggleOn = !DebugModeToggleOn;
            }
        }

        public void OnSelectObject(InputAction.CallbackContext context)
        {
            //if (context.performed)
            //{
            //    SelectObjectPressed = true;
            //}
            //else if(context.canceled)
            //{
            //    SelectObjectPressed = false;
            //}
        }
        #endregion
    }
}