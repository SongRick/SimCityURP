using SimCity.FinalController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectObject : MonoBehaviour, PlayerControls.IUIMapActions
{
    [SerializeField]
    private ObjectPlacer objectPlacer;
    private int selectedIndex = -1;// 当前选中物体的序号
    #region 无用回调函数
    public void OnReturn(InputAction.CallbackContext context)
    {

    }
    public void OnToggleConsoleMode(InputAction.CallbackContext context)
    {

    }
    public void OnToggleDebugMode(InputAction.CallbackContext context)
    {

    }
    public void OnToggleEditMode(InputAction.CallbackContext context)
    {

    }
    #endregion
    public void OnSelect(InputAction.CallbackContext context)
    {
        Debug.Log("OnSelect");
        selectBuilding();
    }
    

    private void Awake()
    {
        objectPlacer = FindObjectOfType<ObjectPlacer>();
        if (objectPlacer == null)
        {
            Debug.LogError("未找到objectPlacer组件！");
        }
        else
            Debug.Log("找到objectPlacer组件！");
    }
    public void selectBuilding()
    {
        Debug.Log("new onselect");

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // 获取点击对象及其层级链
                GameObject clickedObject = hit.collider.gameObject;
                GameObject current = clickedObject;
                bool isPlacedObject = false;

                // 遍历父对象链，检查是否在 placedGameObjects 列表中
                while (current != null && !isPlacedObject)
                {
                    foreach (var obj in objectPlacer.placedGameObjects)
                    {
                        if (obj != null && obj == current)
                        {
                            isPlacedObject = true;
                            break;
                        }
                    }
                    if (!isPlacedObject)
                    {
                        current = current.transform.parent?.gameObject;
                    }
                }

                if (isPlacedObject)
                {
                    // 恢复上一个选中对象的颜色
                    if (selectedIndex != -1 && selectedIndex < objectPlacer.placedGameObjects.Count)
                    {
                        GameObject prevObj = objectPlacer.placedGameObjects[selectedIndex];
                        if (prevObj != null)
                        {
                            Renderer prevRenderer = prevObj.GetComponent<Renderer>();
                            if (prevRenderer != null)
                            {
                                prevRenderer.material.color = Color.white; // 假设原始颜色为白色
                            }
                        }
                    }

                    // 设置新选中对象为红色
                    Renderer renderer = current.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = Color.red;
                        selectedIndex = objectPlacer.placedGameObjects.IndexOf(current);
                    }
                    else
                    {
                        Debug.LogWarning("选中对象无 Renderer 组件");
                    }
                }
            }
        }
    }

}
