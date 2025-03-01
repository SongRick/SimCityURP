using UnityEngine;

// 该类用于管理放置系统中的预览效果，包括预览对象和光标指示器的显示、位置更新以及反馈效果
public class PreviewSystem : MonoBehaviour
{
    // 序列化字段，可在Unity编辑器中设置预览对象在Y轴上的偏移量
    [SerializeField]
    private float previewYOffset = 0.06f;

    // 序列化字段，可在Unity编辑器中设置光标指示器的游戏对象
    [SerializeField]
    private GameObject cellIndicator;
    // 用于存储预览对象的游戏对象引用
    private GameObject previewObject;

    // 序列化字段，可在Unity编辑器中设置预览材质的预制体
    [SerializeField]
    private Material previewMaterialPrefab;
    // 预览材质的实例，用于应用到预览对象上
    private Material previewMaterialInstance;

    // 光标指示器的渲染器组件引用
    private Renderer cellIndicatorRenderer;

    // 在脚本实例被启用时调用，通常用于初始化操作
    private void Start()
    {
        // 创建一个新的材质实例，基于预览材质预制体
        previewMaterialInstance = new Material(previewMaterialPrefab);
        // 初始时禁用光标指示器
        cellIndicator.SetActive(false);
        // 获取光标指示器子对象的渲染器组件
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    // 开始显示放置预览效果
    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        // 实例化预制体作为预览对象
        previewObject = Instantiate(prefab);
        // 准备预览对象，主要是应用预览材质
        PreparePreview(previewObject);
        // 准备光标指示器，根据尺寸调整其大小和纹理缩放
        PrepareCursor(size);
        // 启用光标指示器
        cellIndicator.SetActive(true);
    }

    // 准备光标指示器，根据传入的尺寸调整其大小和纹理缩放
    private void PrepareCursor(Vector2Int size)
    {
        // 检查尺寸是否有效
        if (size.x > 0 || size.y > 0)
        {
            // 根据尺寸调整光标指示器的局部缩放
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            // 根据尺寸调整光标指示器渲染材质的主纹理缩放
            cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    // 准备预览对象，将预览材质应用到预览对象的所有渲染器上
    private void PreparePreview(GameObject previewObject)
    {
        // 获取预览对象及其子对象的所有渲染器组件
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        // 遍历所有渲染器
        foreach (Renderer renderer in renderers)
        {
            // 获取渲染器的所有材质
            Material[] materials = renderer.materials;
            // 遍历所有材质
            for (int i = 0; i < materials.Length; i++)
            {
                // 将每个材质替换为预览材质实例
                materials[i] = previewMaterialInstance;
            }
            // 将修改后的材质数组重新赋值给渲染器
            renderer.materials = materials;
        }
    }

    // 停止显示预览效果
    public void StopShowingPreview()
    {
        // 禁用光标指示器
        cellIndicator.SetActive(false);
        // 如果预览对象存在，则销毁它
        if (previewObject != null)
            Destroy(previewObject);
    }

    // 更新预览对象和光标指示器的位置，并根据有效性应用反馈效果
    public void UpdatePosition(Vector3 position, bool validity)
    {
        // 如果预览对象存在
        if (previewObject != null)
        {
            // 移动预览对象到指定位置
            MovePreview(position);
            // 根据有效性为预览对象应用反馈效果
            ApplyFeedbackToPreview(validity);
        }
        // 移动光标指示器到指定位置
        MoveCursor(position);
        // 根据有效性为光标指示器应用反馈效果
        ApplyFeedbackToCursor(validity);
    }

    // 根据有效性为预览对象应用反馈效果，主要是改变预览材质的颜色
    private void ApplyFeedbackToPreview(bool validity)
    {
        // 如果有效，颜色为白色；否则为红色
        Color c = validity ? Color.green : Color.red;
        // 设置颜色的透明度为0.5
        c.a = 0.8f;
        // 将修改后的颜色应用到预览材质实例上
        previewMaterialInstance.color = c;
    }

    // 根据有效性为光标指示器应用反馈效果，主要是改变光标指示器渲染材质的颜色
    private void ApplyFeedbackToCursor(bool validity)
    {
        // 如果有效，颜色为白色；否则为红色
        Color c = validity ? Color.white : Color.red;
        // 设置颜色的透明度为0.5
        c.a = 0.5f;
        // 将修改后的颜色应用到光标指示器的渲染材质上
        cellIndicatorRenderer.material.color = c;
    }

    // 移动光标指示器到指定位置
    private void MoveCursor(Vector3 position)
    {
        // 设置光标指示器的位置为指定位置
        cellIndicator.transform.position = position;
    }

    // 移动预览对象到指定位置，并加上Y轴偏移量
    private void MovePreview(Vector3 position)
    {
        // 设置预览对象的位置，在指定位置的Y轴上加上偏移量
        previewObject.transform.position = new Vector3(
            position.x,
            position.y + previewYOffset,
            position.z);
    }

    // 开始显示移除预览效果
    internal void StartShowingRemovePreview()
    {
        // 启用光标指示器
        cellIndicator.SetActive(true);
        // 准备光标指示器，尺寸设置为(1, 1)
        PrepareCursor(Vector2Int.one);
        // 为光标指示器应用无效反馈效果
        ApplyFeedbackToCursor(false);
    }
}