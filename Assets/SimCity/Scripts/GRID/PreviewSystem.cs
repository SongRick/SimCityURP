// 整体简介：
// 此代码定义了一个名为 PreviewSystem 的类，该类继承自 MonoBehaviour，主要用于管理游戏中物体放置和移除操作的预览效果。
// 通过创建预览对象和光标指示器，它能在玩家进行放置或移除操作时，实时显示操作的位置和可行性，为玩家提供直观的视觉反馈。
// 该系统会根据操作的有效性改变预览对象和光标指示器的颜色，帮助玩家判断操作是否可行，提升游戏的交互性和用户体验。

using UnityEngine;

// PreviewSystem 类用于管理放置系统中的预览效果，包括预览对象和光标指示器的显示、位置更新以及反馈效果
public class PreviewSystem : MonoBehaviour
{
    // 可在 Unity 编辑器中设置的预览对象在 Y 轴上的偏移量，用于调整预览对象的垂直位置
    [SerializeField]
    private float previewYOffset = 0.06f;

    // 可在 Unity 编辑器中设置的光标指示器的游戏对象，用于标记放置或移除操作的位置
    [SerializeField]
    private GameObject cellIndicator;

    // 存储预览对象的游戏对象引用，预览对象是玩家准备放置的物体的临时副本
    private GameObject previewObject;

    // 可在 Unity 编辑器中设置的预览材质的预制体，用于改变预览对象的外观
    [SerializeField]
    private Material previewMaterialPrefab;

    // 预览材质的实例，应用到预览对象上，避免直接修改预制体材质
    private Material previewMaterialInstance;

    // 光标指示器的渲染器组件引用，用于修改光标指示器的外观属性
    private Renderer cellIndicatorRenderer;

    // 在脚本实例被启用时调用，进行初始化操作
    private void Start()
    {
        // 基于预览材质预制体创建一个新的材质实例
        previewMaterialInstance = new Material(previewMaterialPrefab);

        // 初始时禁用光标指示器，避免干扰玩家
        cellIndicator.SetActive(false);

        // 获取光标指示器子对象的渲染器组件，以便后续修改其外观
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    // 开始显示放置预览效果
    // 参数 prefab: 要放置的物体的预制体
    // 参数 size: 物体的尺寸，用于调整光标指示器的大小
    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        // 实例化预制体作为预览对象
        previewObject = Instantiate(prefab);

        // 为预览对象应用预览材质
        PreparePreview(previewObject);

        // 根据物体尺寸调整光标指示器的大小和纹理缩放
        PrepareCursor(size);

        // 启用光标指示器，开始显示操作位置
        cellIndicator.SetActive(true);
    }

    // 根据传入的尺寸调整光标指示器的大小和纹理缩放
    // 参数 size: 物体的尺寸
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

    // 将预览材质应用到预览对象的所有渲染器上
    // 参数 previewObject: 预览对象
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
        {
            Destroy(previewObject);
        }
    }

    // 更新预览对象和光标指示器的位置，并根据有效性应用反馈效果
    // 参数 position: 要更新到的位置
    // 参数 validity: 操作的有效性，用于决定反馈颜色
    public void UpdatePosition(Vector3 position, bool validity)
    {
        // 如果预览对象存在
        if (previewObject != null)
        {
            // 移动预览对象到指定位置，并加上 Y 轴偏移量
            MovePreview(position);

            // 根据有效性为预览对象应用反馈效果，改变其颜色
            ApplyFeedbackToPreview(validity);
        }

        // 移动光标指示器到指定位置
        MoveCursor(position);

        // 根据有效性为光标指示器应用反馈效果，改变其颜色
        ApplyFeedbackToCursor(validity);
    }

    // 根据操作的有效性为预览对象应用反馈效果，主要是改变预览材质的颜色
    // 参数 validity: 操作的有效性
    private void ApplyFeedbackToPreview(bool validity)
    {
        // 如果操作有效，颜色为绿色；否则为红色
        Color c = validity ? Color.green : Color.red;

        // 设置颜色的透明度为 0.8
        c.a = 0.8f;

        // 将修改后的颜色应用到预览材质实例上
        previewMaterialInstance.color = c;
    }

    // 根据操作的有效性为光标指示器应用反馈效果，主要是改变光标指示器渲染材质的颜色
    // 参数 validity: 操作的有效性
    private void ApplyFeedbackToCursor(bool validity)
    {
        // 如果操作有效，颜色为白色；否则为红色
        Color c = validity ? Color.white : Color.red;

        // 设置颜色的透明度为 0.5
        c.a = 0.5f;

        // 将修改后的颜色应用到光标指示器的渲染材质上
        cellIndicatorRenderer.material.color = c;
    }

    // 移动光标指示器到指定位置
    // 参数 position: 要移动到的位置
    private void MoveCursor(Vector3 position)
    {
        // 设置光标指示器的位置为指定位置
        cellIndicator.transform.position = position;
    }

    // 移动预览对象到指定位置，并加上 Y 轴偏移量
    // 参数 position: 要移动到的位置
    private void MovePreview(Vector3 position)
    {
        // 设置预览对象的位置，在指定位置的 Y 轴上加上偏移量
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

        // 准备光标指示器，将尺寸设置为 (1, 1)
        PrepareCursor(Vector2Int.one);

        // 为光标指示器应用无效反馈效果，将其颜色设为红色
        ApplyFeedbackToCursor(false);
    }
}