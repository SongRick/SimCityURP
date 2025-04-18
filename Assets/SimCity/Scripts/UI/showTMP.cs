using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class showTMP : MonoBehaviour
{
    public TextMeshProUGUI textLabel;
    private void Start()
    {
        textLabel = GetComponent<TextMeshProUGUI>();
        if (textLabel == null)
        {
            Debug.LogError("未找到指定名称的TextMeshPro组件");
        }
    }
    public void ShowTMPSiteSelection(int category)
    {
        if (category == 1)
        {
            textLabel.text = "**餐饮类**\r\n    - **快餐连锁品牌**：如肯德基（KFC）、麦当劳（McDonald’s）、汉堡王（Burger King）、必胜客（Pizza Hut），这些是全球知名的快餐品牌，在城市选址时会综合考虑多种因素，例如商业区域、周边客流量以及竞争对手分布等。";
        }
        else if (category == 2)
        {
            textLabel.text = "**零售类**：便利店连锁品牌便利蜂（Bianlifeng）和7 - 11，这类品牌在选址时会考虑周边居民的生活便利性、交通枢纽的位置以及竞争对手的分布情况等。";
        }
        else if (category == 3)
        {
            textLabel.text = "**金融类**：中国工商银行（ICBC）、中国建设银行（CCB）、中国银行（BOC），这些国有商业银行在全国开设众多分支机构和ATM，选址时会关注区域的经济活跃度、企业和居民的金融服务需求以及交通便利性等。";
        }
        else if (category == 4)
        {
            textLabel.text = "**休闲运动类**：耐克（Nike）、阿迪达斯（Adidas）、李宁（Li - Ning），这些休闲运动品牌在选址时会考虑商业区域的人流量、消费群体的运动偏好以及周边配套设施等。";
        }
        else if (category == 5)
        {
            textLabel.text = "**住宿类**：速8酒店（Super 8 Hotel）、汉庭酒店（Hanting Hotel）、99旅馆（99 Inn）、如家酒店（Home Inn），作为中国较大的酒店连锁品牌，选址时会关注区域的旅游资源、商务活动频繁程度以及交通便利性等。 ";
        }
    }
}
