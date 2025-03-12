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
        public bool CursorLockToggledOn { get; private set; } = true;
        public bool DebugModeToggleOn { get; private set; } = true;
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
        public void OnCursorLock(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                bool newState = !CursorLockToggledOn;
                if (newState)
                {
                    // 触发OnExit事件
                    inputManager.TriggerOnExit();
                }

                CursorLockToggledOn = newState;

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
                    scrpUI.updateEditMode(CursorLockToggledOn);
                }
                else
                {
                    Debug.LogError("scrpUI为null，无法调用updateEditMode方法！");
                }
            }
        }

        public void OnDebugMode(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                DebugModeToggleOn = !DebugModeToggleOn;
            }
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            // 空实现
        }
        #endregion
    }
}