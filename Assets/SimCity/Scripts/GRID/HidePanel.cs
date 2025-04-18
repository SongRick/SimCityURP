using SimCity.FinalController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidePanel : MonoBehaviour
{
    private UIInput _UIInput;
    private GameObject targetGameObject;
    private GameObject gameObjectPanelCreate;
    // 在脚本实例被启用时调用，进行初始化操作
    private void Start()
    {
        gameObject.SetActive(false);
        targetGameObject = GameObject.Find("Player");
        gameObjectPanelCreate = GameObject.Find("panel_create");
        gameObjectPanelCreate.SetActive(false);
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
    public void updateEditMode(bool flagEditMode)
    {
        gameObject.SetActive(flagEditMode);
    }
    public void showPanelCreate()
    {
        if(gameObjectPanelCreate==null)
        {
            Debug.LogError("未找到panel_create");
            return;
        }
        if(gameObjectPanelCreate.activeSelf)
        {
            gameObjectPanelCreate.SetActive(false);
        }
        else
        {
            gameObjectPanelCreate.SetActive(true);
        }
    }
}
