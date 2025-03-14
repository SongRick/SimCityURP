using SimCity.FinalController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectObject : MonoBehaviour, PlayerControls.IUIMapActions
{
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
    public void test()
    {
        Debug.Log("new onselect");
    }

}
