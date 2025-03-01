using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SimCity.FinalController
{
    // ���øýű���ִ��˳��Ϊ -2����ζ�������Ĭ��˳��Ľű�����ִ�У�
    // ��֤���봦���߼�����������������Ľű�֮ǰ���
    [DefaultExecutionOrder(-2)]
    public class PlayerLocomotionInput : MonoBehaviour, PlayerControls.IPlayerLocomotionMapActions
    {
        #region Class variables
        // �������������ʵ�������ڹ�����ҵĸ������������
        // ���������Լ�����ҵİ���������ƶ��������¼�
        public PlayerControls PlayerControls { get; private set; }

        // �洢��ҵ��ƶ����������������Ϊ��ά������֧�������ƶ���
        // �ֱ��Ӧ X�����ң���Y�����£���Z��ǰ�󣩷��������
        public Vector3 MovementInput { get; private set; }

        // �洢��ҵ��ӽ��������������ڿ����������ת��
        // ͨ�� X ��������ˮƽ�ӽ���ת��Y �������ƴ�ֱ�ӽ���ת
        public Vector2 LookInput { get; private set; }

        // �������Ƿ�������Ծ���������ж��Ƿ񴥷���Ծ����
        public bool JumpPressed { get; private set; }

        // �������Ƿ����˳��ģʽ��Ĭ��δ����
        public bool SprintToggledOn { get; private set; } = false;

        // �������Ƿ����˷���ģʽ��Ĭ�Ͽ���
        public bool FlightModeToggleOn { get; private set; } = true;
        #endregion

        #region Startup
        // ���ýű�ʵ��������ʱ���ã����ڳ�ʼ������ϵͳ
        private void OnEnable()
        {
            // ����һ���µ� PlayerControls ʵ�������ڹ����������
            PlayerControls = new PlayerControls();
            // ���� PlayerControls ʵ����ʹ�俪ʼ���������¼�
            PlayerControls.Enable();

            // ���� PlayerLocomotionMap ����ӳ�䣬��ӳ��������ƶ����ӽǺ���Ծ�����붯��
            PlayerControls.PlayerLocomotionMap.Enable();
            // Ϊ PlayerLocomotionMap �е����붯�����ûص�����������󶨵���ǰ�����Ӧ�����ϣ�
            // ���ж�Ӧ�����¼�����ʱ������ð󶨵ķ������д���
            PlayerControls.PlayerLocomotionMap.SetCallbacks(this);
        }

        // ���ýű�ʵ��������ʱ���ã����ڹر�����ϵͳ
        private void OnDisable()
        {
            // ���� PlayerLocomotionMap ����ӳ�䣬ֹͣ������ӳ���µ������¼�
            PlayerControls.PlayerLocomotionMap.Disable();
            // �Ƴ� PlayerLocomotionMap �е����붯���Ļص����������ⲻ��Ҫ�ĵ���
            PlayerControls.PlayerLocomotionMap.RemoveCallbacks(this);
        }
        #endregion

        #region Late Update Logic
        // ��ÿ֡�� LateUpdate �׶ε��ã�����������Ծ����״̬
        private void LateUpdate()
        {
            // ����Ծ����״̬���Ϊδ���£�ȷ��ÿ�ΰ����¼�ֻ����һ�Σ�
            // �������������Ծ����
            JumpPressed = false;
        }
        #endregion

        #region Input Callbacks
        // ����ҽ����ƶ�����ʱ���õĻص�����
        public void OnMovement(InputAction.CallbackContext context)
        {
            // ��ȡ��ά�ƶ��������������丳ֵ�� MovementInput��
            // ����������������� X��Y��Z �����ϵ��ƶ�����
            MovementInput = context.ReadValue<Vector3>();
        }

        // ����ҽ����ӽ�����ʱ���õĻص�����
        public void OnLook(InputAction.CallbackContext context)
        {
            // �������¼����������ж�ȡ�ӽ����������������丳ֵ�� LookInput��
            // ���ڿ����������ת�ӽ�
            LookInput = context.ReadValue<Vector2>();
        }

        // ����ҽ�����Ծ����ʱ���õĻص�����
        public void OnJump(InputAction.CallbackContext context)
        {
            // ��������¼��Ƿ���ִ�У������������£�
            if (context.performed)
            {
                // ������������£�����Ծ����״̬���Ϊ�Ѱ��£�
                // �����ɸ��ݴ˱�Ǵ�����Ծ����
                JumpPressed = true;
            }
        }

        // ����ҽ��г��ģʽ�л�����ʱ���õĻص�����
        public void OnToggleSprint(InputAction.CallbackContext context)
        {
            // ��������¼��Ƿ���ִ�У������������£�
            if (context.performed)
            {
                // ������������£��л����ģʽ�Ŀ���״̬
                SprintToggledOn = !SprintToggledOn;
            }
        }

        // ����ҽ��з���ģʽ�л�����ʱ���õĻص�����
        public void OnEnableFlightMode(InputAction.CallbackContext context)
        {
            // �л�����ģʽ�Ŀ���״̬
            FlightModeToggleOn = !FlightModeToggleOn;
        }
        #endregion

        #region Other Methods
        // ���ó��ģʽ�ķ���
        public void DisableSprint()
        {
            // �����ģʽ���Ϊδ����
            SprintToggledOn = false;
        }
        // ���÷���ģʽ�ķ���
        public void EnableFlightMode()
        {
            // �������ģʽ
            FlightModeToggleOn = true;
        }
        #endregion
    }
}