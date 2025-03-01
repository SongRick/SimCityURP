using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCity.FinalController
{
    // 设置该脚本的执行顺序为 -1，意味着它会比默认顺序的脚本更早执行
    [DefaultExecutionOrder(-1)]
    public class PlayerController : MonoBehaviour
    {
        #region Class Variables
        // 在 Inspector 面板中创建一个名为 "Components" 的标题，用于组织下面的组件字段
        [Header("Components")]
        // 序列化字段，允许在 Inspector 面板中指定角色控制器组件，用于控制角色的移动
        [SerializeField] private CharacterController _characterController;
        // 序列化字段，允许在 Inspector 面板中指定玩家相机组件，用于控制相机视角
        [SerializeField] private Camera _playerCamera;

        // 在 Inspector 面板中创建一个名为 "Base Movement" 的标题，用于组织下面的基础移动参数
        [Header("Base Movement")]
        // 新增初速度，高速起步，并解决漂移问题
        public float initialVelocity = 19f;
        // 玩家跑步时的加速度，单位为单位/秒²
        public float runAcceleration = 50f;
        // 玩家跑步的最大速度，单位为单位/秒
        public float runSpeed = 20f;
        // 玩家受到的阻力系数，用于模拟运动中的阻力
        public float drag = 200f;
        // 重力加速度，单位为单位/秒²，用于模拟角色受重力影响
        public float gravity = 25f;
        // 角色跳跃时的初始垂直速度，单位为单位/秒
        public float jumpSpeed = 10f;
        // 上升和下降的速度
        public float verticalFlightSpeed = 10f;

        // 在 Inspector 面板中创建一个名为 "Camera Settings" 的标题，用于组织下面的相机设置参数
        [Header("Camera Settings")]
        // 相机水平旋转的灵敏度，控制相机水平旋转的速度
        public float lookSenseH = 0.1f;
        // 相机垂直旋转的灵敏度，控制相机垂直旋转的速度
        public float lookSenseV = 0.1f;
        // 相机垂直旋转的最大角度限制，避免相机垂直旋转过度
        public float lookLimitV = 89f;
        // 存储玩家的移动和视角输入信息的对象
        private PlayerLocomotionInput _playerLocomotionInput;
        // 存储相机的旋转角度，x 轴表示水平旋转，y 轴表示垂直旋转
        private Vector2 _cameraRotation = Vector2.zero;
        // 存储玩家角色的目标旋转角度，x 轴表示水平旋转，y 轴表示垂直旋转
        private Vector2 _playerTargetRotation = Vector2.zero;
        // 角色的垂直速度，用于处理重力和跳跃效果
        private float _verticalVelocity = 0f;

        // 存储UI输入信息的对象
        private UIInput _UIInput;

        private Quaternion initialCameraRotation;
        private Quaternion initialPlayerRotation;
        #endregion

        #region Startup

        // 在脚本实例被加载时调用，用于初始化操作
        private void Awake()
        {
            InitPlayer();
        }

        private void InitPlayer()
        {
            // 获取当前游戏对象上的 PlayerLocomotionInput 组件，并赋值给 _playerLocomotionInput 变量
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            // 获取当前游戏对象上的 UIInput 组件，并赋值给 _UIInput 变量
            _UIInput = GetComponent<UIInput>();
            // 自动获取角色控制器组件，如果不想使用自动获取方式，可删去此代码，在编辑器面板里手动绑定
            _characterController = GetComponent<CharacterController>();
            // 自动获取主相机组件，如果不想使用自动获取方式，可删去此代码，在编辑器面板里手动绑定
            _playerCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            initialCameraRotation = _playerCamera.transform.rotation;
            initialPlayerRotation = transform.rotation;
        }
        #endregion

        #region Update Logic
        // 每帧调用一次，用于处理玩家的移动逻辑
        private void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            // 获取相机向前方向在 XZ 平面上的投影，并将其归一化，得到一个长度为 1 的向量
            // 这样做是为了确保角色的移动方向是基于相机在水平面上的朝向
            Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
            // 获取相机向右方向在 XZ 平面上的投影，并将其归一化，得到一个长度为 1 的向量
            // 用于结合玩家输入确定角色的水平移动方向
            Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;

            // 根据玩家的输入和相机的方向，计算玩家的水平移动方向向量
            // _playerLocomotionInput.MovementInput.x 表示玩家水平方向的输入，_playerLocomotionInput.MovementInput.z 表示玩家前后方向的输入
            Vector3 horizontalMovementDirection = cameraRightXZ * _playerLocomotionInput.MovementInput.x + cameraForwardXZ * _playerLocomotionInput.MovementInput.z;

            // 根据移动方向和加速度，计算当前帧的水平移动速度增量
            // Time.deltaTime 表示从上一帧到当前帧所经过的时间，确保移动速度与帧率无关
            Vector3 horizontalMovementDelta = horizontalMovementDirection * (initialVelocity + runAcceleration * Time.deltaTime);

            // 计算新的水平速度向量，将当前角色的水平速度和水平移动速度增量相加
            Vector3 newHorizontalVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z) + horizontalMovementDelta;

            // 计算当前的水平阻力向量，阻力方向与水平速度方向相反，大小与水平速度大小成正比
            Vector3 currentHorizontalDrag = newHorizontalVelocity.normalized * drag * Time.deltaTime;

            // 如果当前水平速度的大小大于水平阻力乘以时间增量，则从水平速度中减去水平阻力向量
            // 否则，将水平速度设置为零，避免出现反向移动的情况
            newHorizontalVelocity = (newHorizontalVelocity.magnitude > drag * Time.deltaTime) ? newHorizontalVelocity - currentHorizontalDrag : Vector3.zero;

            // 限制新水平速度的大小
            if (_playerLocomotionInput.SprintToggledOn)
                // 如果开启冲刺模式，将水平速度限制在最大速度的两倍
                newHorizontalVelocity = Vector3.ClampMagnitude(newHorizontalVelocity, 2 * runSpeed);
            else
                // 如果未开启冲刺模式，将水平速度限制在正常最大速度
                newHorizontalVelocity = Vector3.ClampMagnitude(newHorizontalVelocity, runSpeed);

            // 若光标解锁，则自动进入飞行模式
            if (!_UIInput.CursorLockToggledOn)
            {
                _playerLocomotionInput.EnableFlightMode();
            }

            // 处理垂直移动
            if (_playerLocomotionInput.FlightModeToggleOn)
            {
                // 飞行模式下，根据输入的 Y 分量调整垂直速度
                float baseVerticalSpeed = _playerLocomotionInput.MovementInput.y * verticalFlightSpeed;
                // 如果开启冲刺模式，垂直速度加倍
                if (_playerLocomotionInput.SprintToggledOn)
                {
                    _verticalVelocity = baseVerticalSpeed * 2;
                }
                else
                {
                    _verticalVelocity = baseVerticalSpeed;
                }
            }
            else
            {
                // 处理重力
                if (_characterController.isGrounded)
                {
                    // 当角色在地面上时，保持一个轻微的向下力，确保角色稳定在地面上
                    _verticalVelocity = -gravity * Time.deltaTime;
                }
                else
                {
                    // 当角色在空中时，应用重力，使垂直速度不断减小
                    _verticalVelocity -= gravity * Time.deltaTime;
                }

                // 处理跳跃
                if (_playerLocomotionInput.JumpPressed && _characterController.isGrounded)
                {
                    // 如果开启冲刺模式，跳跃速度加倍
                    float jumpSpeedToUse = _playerLocomotionInput.SprintToggledOn ? jumpSpeed * 2 : jumpSpeed;
                    // 当玩家按下跳跃键且角色在地面上时，赋予角色一个向上的初始垂直速度
                    _verticalVelocity = jumpSpeedToUse;
                }
            }

            // 合并垂直速度到水平速度向量中，得到最终的移动速度向量
            Vector3 newVelocity = new Vector3(newHorizontalVelocity.x, _verticalVelocity, newHorizontalVelocity.z);

            // 检查速度的大小，如果小于 1f，则关闭冲刺状态
            if (newVelocity.magnitude < 1f)
            {
                _playerLocomotionInput.DisableSprint();
            }

            // 使用 CharacterController 组件移动角色，Unity 建议每帧只调用一次该方法
            // 移动的距离为新速度乘以时间增量
            _characterController.Move(newVelocity * Time.deltaTime);
        }
        #endregion

        #region Late Update Logic
        // 在所有 Update 方法调用后调用，用于处理相机的旋转逻辑
        private void LateUpdate()
        {
            UpdateCameraRotation();
        }

        private void UpdateCameraRotation()
        {
            // 若光标锁定，则隐藏光标，此时视角可变
            if (_UIInput.CursorLockToggledOn)
            {
                //光标锁定（隐藏）
                Cursor.lockState = CursorLockMode.Locked;
                // 根据玩家的水平视角输入和相机水平旋转灵敏度，更新相机的水平旋转角度
                _cameraRotation.x += lookSenseH * _playerLocomotionInput.LookInput.x;

                // 根据玩家的垂直视角输入和相机垂直旋转灵敏度，更新相机的垂直旋转角度
                // 同时使用 Mathf.Clamp 方法将垂直旋转角度限制在 -lookLimitV 到 lookLimitV 之间
                _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookSenseV * _playerLocomotionInput.LookInput.y, -lookLimitV, lookLimitV);

                // 根据玩家的水平视角输入和相机水平旋转灵敏度，更新玩家角色的目标水平旋转角度
                _playerTargetRotation.x += lookSenseH * _playerLocomotionInput.LookInput.x;
            }
            else
            {
                //光标解锁（显示）
                Cursor.lockState = CursorLockMode.None;

            }

            // 设置玩家角色的旋转角度，只在 Y 轴上进行旋转，并考虑初始旋转
            Quaternion playerRotation = Quaternion.Euler(0f, initialPlayerRotation.eulerAngles.y + _playerTargetRotation.x, 0f);
            transform.rotation = playerRotation;

            // 设置相机的旋转角度，根据更新后的 _cameraRotation 向量进行旋转，并考虑初始旋转
            Quaternion cameraRotation = Quaternion.Euler(initialCameraRotation.eulerAngles.x + _cameraRotation.y, initialCameraRotation.eulerAngles.y + _cameraRotation.x, 0f);
            _playerCamera.transform.rotation = cameraRotation;
        }
        #endregion

        #region GUI
        void OnGUI()
        {

            GUIStyle labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.black;
            labelStyle.fontSize = 50;
            int positionGUIX = Screen.width - 300;
            int positionGUIY = 10;
            // 若未开启debug模式，则不显示所有GUI消息
            if (!_UIInput.DebugModeToggleOn)
                return;
            if (_playerLocomotionInput.FlightModeToggleOn)
            {
                GUI.Label(new Rect(positionGUIX, positionGUIY, 140, 20), "Flight Mode", labelStyle);
            }
            else
            {
                GUI.Label(new Rect(positionGUIX, positionGUIY, 140, 20), "Normal Mode", labelStyle);
            }
            if (_playerLocomotionInput.SprintToggledOn)
            {
                GUI.Label(new Rect(positionGUIX, positionGUIY + 60, 140, 20), "Sprinting", labelStyle);
            }
            if (_UIInput.CursorLockToggledOn)
            {
                GUI.Label(new Rect(positionGUIX, positionGUIY + 120, 140, 20), "CursorLock", labelStyle);
            }
            else
            {
                GUI.Label(new Rect(positionGUIX, positionGUIY + 120, 140, 20), "CursorUnlock", labelStyle);
            }
        }

        #endregion
    }
}