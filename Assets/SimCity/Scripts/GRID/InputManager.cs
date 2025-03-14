using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;

    public event Action OnClicked, OnExit;

    // 新增触发方法
    public void TriggerOnExit()
    {
        OnExit?.Invoke();
    }

    // 新增触发鼠标左键点击事件的方法
    public void TriggerOnClicked()
    {
        OnClicked?.Invoke();
        Debug.Log("TriggerOnClicked()");
    }

    public bool IsPointerOverUI()
    {
        // 获取 EventSystem
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            Debug.LogError("未找到 EventSystem！");
            return false;
        }

        // 使用新输入系统获取鼠标位置
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        // 创建 PointerEventData
        PointerEventData pointerEventData = new PointerEventData(eventSystem)
        {
            position = mousePosition
        };

        // 执行 Raycast
        System.Collections.Generic.List<RaycastResult> results = new System.Collections.Generic.List<RaycastResult>();
        eventSystem.RaycastAll(pointerEventData, results);

        // 如果有命中 UI 元素，则返回 true
        return results.Count > 0;
    }

    public Vector3 GetSelectedMapPosition()
    {
        // 使用新输入系统获取鼠标位置
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }
}