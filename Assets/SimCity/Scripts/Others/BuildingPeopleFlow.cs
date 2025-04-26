using UnityEngine;
using UnityEngine.UI;

// BuildingPeopleFlow 类用于管理建筑人流量文本的显示和更新
public class BuildingPeopleFlow : MonoBehaviour
{
    // 用于存储所有建筑的 Transform 组件，方便遍历建筑
    public Transform buildings;
    // 用于显示人流量文本的 Canvas
    public Canvas canvas;
    // 标记人流量文本是否可见
    private bool isTextVisible = false;
    // 用于存储所有创建的文本对象
    private GameObject[] textObjects;

    // Update 方法每帧都会被调用
    void Update()
    {
        // 检测用户是否按下了 P 键
        if (Input.GetKeyDown(KeyCode.P))
        {
            // 调用 ToggleTextVisibility 方法切换文本的可见性
            ToggleTextVisibility();
        }

        // 如果文本是可见的
        if (isTextVisible)
        {
            // 调用 UpdateTextPositions 方法更新文本的位置
            UpdateTextPositions();
        }
    }

    // 切换人流量文本的可见性
    void ToggleTextVisibility()
    {
        // 取反 isTextVisible 的值
        isTextVisible = !isTextVisible;

        // 如果文本现在应该可见
        if (isTextVisible)
        {
            // 调用 CreatePeopleFlowTexts 方法创建人流量文本
            CreatePeopleFlowTexts();
        }
        else
        {
            // 调用 DestroyTextObjects 方法销毁所有文本对象
            DestroyTextObjects();
        }
    }

    // 创建人流量文本对象
    void CreatePeopleFlowTexts()
    {
        // 初始化 textObjects 数组，数组大小为 buildings 的子对象数量
        textObjects = new GameObject[buildings.childCount];
        int index = 0;
        // 遍历 buildings 下的所有子对象（即所有建筑）
        foreach (Transform building in buildings)
        {
            // 创建一个新的 GameObject 用于显示人流量文本
            GameObject textObject = new GameObject("PeopleFlowText");
            // 将文本对象的父对象设置为 canvas
            textObject.transform.SetParent(canvas.transform);
            // 给文本对象添加 Text 组件
            Text peopleFlowText = textObject.AddComponent<Text>();

            // 设置 Text 组件的字体为 Unity 内置的 LegacyRuntime.ttf 字体
            peopleFlowText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            // 设置字体大小为 20
            peopleFlowText.fontSize = 25;
            // 设置文本对齐方式为居中
            peopleFlowText.alignment = TextAnchor.MiddleCenter;

            // 随机生成一个 0 到 99 之间的整数作为人流量数值
            int peopleFlow = Random.Range(0, 100);
            // 将人流量数值转换为字符串并赋值给文本组件的 text 属性
            peopleFlowText.text = peopleFlow.ToString();

            // 根据人流量数值设置文本的颜色
            if (peopleFlow < 50)
            {
                // 人流量小于 50 时，文本颜色为绿色
                peopleFlowText.color = Color.green;
            }
            else if (peopleFlow < 80)
            {
                // 人流量在 50 到 79 之间时，文本颜色为黄色
                peopleFlowText.color = Color.yellow;
            }
            else if (peopleFlow <= 100)
            {
                // 人流量在 80 到 100 之间时，文本颜色为红色
                peopleFlowText.color = Color.red;
            }

            // 将创建的文本对象存储到 textObjects 数组中
            textObjects[index] = textObject;
            index++;
        }
        // 调用 UpdateTextPositions 方法更新文本的位置
        UpdateTextPositions();
    }

    // 更新所有文本对象的位置，使其位于对应建筑的顶部
    void UpdateTextPositions()
    {
        int index = 0;
        // 遍历 buildings 下的所有子对象（即所有建筑）
        foreach (Transform building in buildings)
        {
            // 获取建筑的 MeshRenderer 组件
            MeshRenderer renderer = building.GetComponent<MeshRenderer>();
            // 如果建筑有 MeshRenderer 组件
            if (renderer != null)
            {
                // 获取建筑的边界框
                Bounds bounds = renderer.bounds;
                // 获取建筑的位置
                Vector3 buildingPosition = building.position;
                // 计算文本应该显示的位置，位于建筑顶部上方 5 个单位的位置，并将其转换为屏幕坐标
                Vector3 textPosition = Camera.main.WorldToScreenPoint(buildingPosition + new Vector3(0, bounds.size.y + 5, 0));
                // 获取文本对象的 RectTransform 组件
                RectTransform rectTransform = textObjects[index].GetComponent<RectTransform>();
                // 将文本对象的位置设置为计算得到的位置
                rectTransform.position = textPosition;
            }
            index++;
        }
    }

    // 销毁所有文本对象
    void DestroyTextObjects()
    {
        // 如果 textObjects 数组不为空
        if (textObjects != null)
        {
            // 遍历 textObjects 数组中的所有文本对象
            foreach (GameObject textObject in textObjects)
            {
                // 销毁文本对象
                Destroy(textObject);
            }
            // 将 textObjects 数组置为 null
            textObjects = null;
        }
    }
}