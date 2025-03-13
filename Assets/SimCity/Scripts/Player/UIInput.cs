using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SimCity.FinalController
{
    [DefaultExecutionOrder(-3)]
    public class UIInput : MonoBehaviour, PlayerControls.IUIMapActions
    {
        #region Class variables
        public PlayerControls PlayerControls { get; private set; }
        public bool ToggleCursorLock { get; private set; } = false;
        public bool ToggleEditMode { get; private set; } = false;
        public bool ToggleDebugMode { get; private set; } = true;
        public bool SelectObjectPressed { get; private set; }

        private PlacementSystem placementSystem;
        private ScrpUI scrpUI;
        private InputManager inputManager; // 新增InputManager引用
        #endregion

        #region Startup
        private void OnEnable()
        {
            // 初始化InputManager
            inputManager = FindObjectOfType<InputManager>();
            if (inputManager == null)
            {
                Debug.LogError("未找到InputManager组件！");
            }

            PlayerControls = new PlayerControls();
            PlayerControls.Enable();
            PlayerControls.UIMap.Enable();
            PlayerControls.UIMap.SetCallbacks(this);

            placementSystem = FindObjectOfType<PlacementSystem>();
            if (placementSystem == null)
            {
                Debug.LogError("未找到PlacementSystem组件！");
            }

            scrpUI = FindObjectOfType<ScrpUI>();
            if (scrpUI == null)
            {
                Debug.LogError("未找到scrpUI组件！");
            }
        }

        private void OnDisable()
        {
            PlayerControls.UIMap.Disable();
            PlayerControls.UIMap.RemoveCallbacks(this);
        }
        #endregion

        #region Input Callbacks
        public void OnToggleEditMode(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                bool newState = !ToggleEditMode;
                if (!newState)
                {
                    // 触发OnExit事件
                    inputManager.TriggerOnExit();
                }

                ToggleEditMode = newState;
                // 编辑模式下，应屏幕锁定，CursorLock解锁
                ToggleCursorLock = !ToggleEditMode;

                if (placementSystem != null)
                {
                    placementSystem.updateEditMode();
                }
                else
                {
                    Debug.LogError("placementSystem为null，无法调用updateEditMode方法！");
                }

                if (scrpUI != null)
                {
                    scrpUI.updateEditMode(ToggleEditMode);
                }
                else
                {
                    Debug.LogError("scrpUI为null，无法调用updateEditMode方法！");
                }
            }
        }

        public void OnToggleDebugMode(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                ToggleDebugMode = !ToggleDebugMode;
            }
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // 触发鼠标左键点击事件
                inputManager.TriggerOnClicked();
            }
        }

        public void OnToggleNewDebugMode(InputAction.CallbackContext context)
        {
            Time.timeScale = 0f;
        }
        #endregion
    }
}