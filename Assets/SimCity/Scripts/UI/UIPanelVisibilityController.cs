using SimCity.FinalController;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIPanelVisibilityController : MonoBehaviour
{
    private UIInput _UIInput; 
    private GameObject targetGameObject;
    private GameObject gameObjectPanelCreate;
    private GameObject gameObjectPanelSiteSelection;
    private GameObject gameObjectPanelText;
    private List<GameObject> gameObjectPanelList = new List<GameObject>();
    // 在脚本实例被启用时调用，进行初始化操作
    private void Start()
    {
        gameObject.SetActive(false);
        targetGameObject = GameObject.Find("Player");
        gameObjectPanelCreate = GameObject.Find("panel_create");
        gameObjectPanelCreate.SetActive(false);
        gameObjectPanelText = GameObject.Find("panel_text");
        gameObjectPanelText.SetActive(false);
        gameObjectPanelSiteSelection = GameObject.Find("panel_site_selection");
        gameObjectPanelSiteSelection.SetActive(false);
        gameObjectPanelList.Add(gameObjectPanelCreate);
        gameObjectPanelList.Add(gameObjectPanelSiteSelection);
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
        showTargetPanel(gameObjectPanelCreate);
    }
    public void showPanelSiteSelection()
    {
        showTargetPanel(gameObjectPanelSiteSelection);
    }
    public void showTargetPanel(GameObject obj)
    {
        if(obj==null)
        {
            Debug.LogError("未找到panel:"+obj.name);
            return;
        }
        // 如果目标panel已经激活，则将其关闭
        if(obj.activeSelf)
        {
            obj.SetActive(false);
        }
        else
        {
            // 否则将其激活
            obj.SetActive(true);
            // 遍历gameObjectPanelList，将其他panel关闭
            for (int i = 0; i < gameObjectPanelList.Count; i++)
            {
                if(gameObjectPanelList[i]!=obj)
                {
                    gameObjectPanelList[i].SetActive(false);
                }
            }
        }
    }
    public void showPanelText()
    {
        gameObjectPanelText.SetActive(true);
    }
}
