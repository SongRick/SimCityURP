using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SimCity.FinalController
{
    // ���øýű���ִ��˳��Ϊ -2����ζ�������Ĭ��˳��Ľű�����ִ�У�
    // ��֤���봦���߼�����������������Ľű�֮ǰ���
    [DefaultExecutionOrder(-3)]
    public class UIInput : MonoBehaviour, PlayerControls.IUIMapActions
    {
        #region Class variables
        // �������������ʵ�������ڹ�����ҵĸ������������
        // ���������Լ�����ҵİ���������ƶ��������¼�
        public PlayerControls PlayerControls { get; private set; }

        // ����Ƿ�����
        // True     ��������������ع�꣬���ӽǿɱ�
        // False    ������������ʾ��꣬���ӽ�����
        public bool CursorLockToggledOn { get; private set; } = true;
        public bool DebugModeToggleOn { get; private set; } = true;
        public bool SelectObjectPressed { get; private set; }
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
            PlayerControls.UIMap.Enable();
            // Ϊ PlayerLocomotionMap �е����붯�����ûص�����������󶨵���ǰ�����Ӧ�����ϣ�
            // ���ж�Ӧ�����¼�����ʱ������ð󶨵ķ������д���
            PlayerControls.UIMap.SetCallbacks(this);
        }

        // ���ýű�ʵ��������ʱ���ã����ڹر�����ϵͳ
        private void OnDisable()
        {
            // ���� PlayerLocomotionMap ����ӳ�䣬ֹͣ������ӳ���µ������¼�
            PlayerControls.UIMap.Disable();
            // �Ƴ� PlayerLocomotionMap �е����붯���Ļص����������ⲻ��Ҫ�ĵ���
            PlayerControls.UIMap.RemoveCallbacks(this);
        }
        #endregion

        #region Input Callbacks
        public void OnCursorLock(InputAction.CallbackContext context)
        {
            // ��������¼��Ƿ���ִ�У������������£�
            if (context.performed)
            {
                // ������������£��л��������״̬
                CursorLockToggledOn = !CursorLockToggledOn;
            }
        }

        public void OnDebugMode(InputAction.CallbackContext context)
        {
            // ��������¼��Ƿ���ִ�У������������£�
            if (context.performed)
            {
                // ������������£��л�debugģʽ
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