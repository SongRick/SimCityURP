using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TerrainTools;
using UnityEngine.Windows;

namespace SimCity.FinalController
{
    [DefaultExecutionOrder(-3)]
    public class UIInput : MonoBehaviour, PlayerControls.IUIMapActions
    {
        #region Class variables
        public PlayerControls PlayerControls { get; private set; }
        public bool CursorLockToggleOn { get; private set; } = false;
        public bool lastCursorLockToggleOn { get; private set; } = false;
        public bool EditModeToggleOn { get; private set; } = false;
        public bool DebugModeToggleOn { get; private set; } = true;
        public bool ConsoleModeToggleOn { get; private set; } = false;

        private PlacementSystem placementSystem;
        private HidePanel scrpUI;
        private InputManager inputManager; // 新增InputManager引用
        private ConsoleConcroller consoleConcroller;// 新增ConsoleConcroller引用
        private SelectObject selectObject;// 新增SelectObject引用
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
            consoleConcroller = FindObjectOfType<ConsoleConcroller>();
            if (consoleConcroller == null)
            {
                Debug.LogError("未找到ConsoleConcroller组件！");
            }
            selectObject = FindObjectOfType<SelectObject>();
            if (selectObject == null)
            {
                Debug.LogError("未找到selectObject组件！");
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

            scrpUI = FindObjectOfType<HidePanel>();
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
                bool newState = !EditModeToggleOn;
                if (!newState)
                {
                    // 触发OnExit事件
                    inputManager.TriggerOnExit();
                }

                EditModeToggleOn = newState;
                // 编辑模式下，应屏幕锁定，CursorLock解锁
                CursorLockToggleOn = !EditModeToggleOn;

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
                    scrpUI.updateEditMode(EditModeToggleOn);
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
                DebugModeToggleOn = !DebugModeToggleOn;
            }
        }

        // UIInput.cs 修改 OnSelect 方法
        public void OnSelect(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (selectObject.SelectModeToggleOn)
                {
                    placementSystem.StopPlacement();
                    placementSystem.gridVisualization.SetActive(EditModeToggleOn);
                    selectObject.selectBuilding();
                }
                else
                {
                    // 仅在非选择模式下触发点击
                    if (!selectObject.isDragging)
                    {
                        inputManager.TriggerOnClicked();
                    }
                }
            }
        }

        public void OnToggleConsoleMode(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // 控制台模式
                if (!ConsoleModeToggleOn)
                {
                    ConsoleModeToggleOn = !ConsoleModeToggleOn;
                    Time.timeScale = 0f;
                    lastCursorLockToggleOn = CursorLockToggleOn;
                    // 视角锁定
                    CursorLockToggleOn = false;
                    consoleConcroller.input = "";
                }
                // 退出控制台模式
                else
                {
                    ConsoleModeToggleOn = !ConsoleModeToggleOn;
                    Time.timeScale = 1f;
                    // 退出控制台模式时，视角锁定恢复进入控制台之前的状态
                    CursorLockToggleOn = lastCursorLockToggleOn;
                    consoleConcroller.printContent = "";
                }
            }
        }

        public void OnReturn(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Debug.Log("OnReturn");
                if (ConsoleModeToggleOn)
                {
                    consoleConcroller.HandleInput();
                    consoleConcroller.input = "";
                }

            }
        }
        #endregion

        #region GUI
        public void OnGUI()
        {
            if (!ConsoleModeToggleOn)
                return;
            //定义GUIStyle
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.yellow;
            labelStyle.fontSize = 40;
            float widthTextField = 60f;//文本输入框的高度
            float widthScroll = 400;//滚动条的高度
            float margin = 5f;//文本输入框或滚动条 与各自box的边距
            float maxScrollContentRectHeight = 1000;//滚动条内容高度

            //绘制放置文本输入框的box
            Rect rectBoxForTextField = new Rect(0f, Screen.height - widthTextField, Screen.width, widthTextField);
            GUI.Box(rectBoxForTextField, "");
            // 确保输入框可以获得焦点
            GUI.SetNextControlName("MyTextField");
            //绘制输入框，与box上下左右的边距均为margin
            Rect rectTextField = new Rect
                (rectBoxForTextField.x + margin,
                rectBoxForTextField.y + margin,
                rectBoxForTextField.width - 2 * margin,
                rectBoxForTextField.height - 2 * margin);
            consoleConcroller.input = GUI.TextField(rectTextField, consoleConcroller.input, labelStyle);
            // 让输入框自动获得焦点
            if (Event.current.type == EventType.Repaint)
            {
                GUI.FocusControl("MyTextField");
            }

            //绘制放置滚动条的box
            Rect rectBoxForScroll = new Rect
                (rectBoxForTextField.x,
                rectBoxForTextField.y - widthScroll,
                Screen.width,
                widthScroll);
            GUI.Box(rectBoxForScroll, "");
            // 绘制滚动条
            // 第一个参数 rectScroll 表示滚动条的位置和大小
            // 第二个参数 scrollPosition 表示滚动位置，存储用户滚动的位置信息
            // 第三个参数 rectScrollView 表示滚动视图中内容的位置（相对于滚动视图而非屏幕原点）和大小
            Rect rectScroll = new Rect
                (rectBoxForScroll.x + 3 * margin,// 滚动条内容缩进3*margin，且刚好隐藏右侧的滚动滑块？哈哈
                rectBoxForScroll.y,
                rectBoxForScroll.width,
                rectBoxForScroll.height);
            Rect rectScrollView = new Rect
                (margin,
                margin,
                0,// 将宽度设为极小值，似乎可以隐藏底部的滚动滑块？
                maxScrollContentRectHeight);
            consoleConcroller.scrollPosition = GUI.BeginScrollView(rectScroll, consoleConcroller.scrollPosition, rectScrollView);
            GUI.Label(rectScrollView, consoleConcroller.printContent, labelStyle);
            GUI.EndScrollView();
            // 处理 GUI 事件，确保不拦截鼠标点击事件
            // 如果不加这行代码，调试时无问题，但是打包生成游戏，运行时会发现，调出控制台时无法单击鼠标以选中对象
            if (Event.current.type == EventType.MouseDown)
            {
                Event.current.Use();
            }
        }
        #endregion

    }
}