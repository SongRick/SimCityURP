using SimCity.FinalController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    private UIInput _UIInput;
    GameObject targetGameObject;
    private bool flagEditMode = false;
    // 在脚本实例被启用时调用，进行初始化操作
    private void Start()
    {
        targetGameObject = GameObject.Find("Player");
        if (targetGameObject != null)
        {
            // 获取目标游戏对象上的 UIInput 组件
            _UIInput = targetGameObject.GetComponent<UIInput>();
        }
        else
        {
            Debug.LogError("未找到指定名称的游戏对象");
        }
    }
    public void updateEditMode()
    {
        flagEditMode = _UIInput.CursorLockToggledOn;
        Debug.Log("flagEditMode=" + flagEditMode);
        gameObject.SetActive(!_UIInput.CursorLockToggledOn);
    }

}
