using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    //引用和配置
    [Header("References")]
    [SerializeField] private DeepSeekAPI deepSeekAPI; //对话管理器
    [SerializeField] private InputField inputField; //玩家问题输入框
    [SerializeField] private Text dialogueText; //角色回复的文本内容
    private string characterName;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f; // 打字机效果的字符显示速度
    [SerializeField] private GameObject loadingIndicator;
    // Start is called before the first frame update
    void Start()
    {
        characterName = deepSeekAPI.npcCharacter.name; //角色姓名赋值
        inputField.onSubmit.AddListener((text) => {
            if (string.IsNullOrEmpty(text))
            {
                Debug.LogWarning("输入内容为空，请重新输入。");
                return;
            }
            inputField.text = ""; // 清空输入框
            loadingIndicator.SetActive(true);
            deepSeekAPI.SendMessageToDeepSeek(text, HandleAIResponse); //发送对话请求到DeepSeek AI
        });
    }

    /// <summary>
    /// 处理AI的响应
    /// </summary>
    /// <param name="content">AI的回复内容</param>
    /// <param name="isSuccess">请求是否成功</param>
    private void HandleAIResponse(string content, bool isSuccess)
    {
        StopAllCoroutines();
        string message = content;
        StartCoroutine(TypewriterEffect(isSuccess ? characterName + "：" + message : characterName + "：(通讯中断)")); //启动打字机效果协程
    }

    /// <summary>
    /// 打字机效果协程
    /// </summary>
    /// <param name="text">角色的回复内容</param>
    /// <returns></returns>
    private IEnumerator TypewriterEffect(string text)
    {
        loadingIndicator.SetActive(false);

        StringBuilder sb = new StringBuilder();
        foreach (char c in text)//遍历每个字符
        {
            sb.Append(c);
            dialogueText.text = sb.ToString();
            yield return new WaitForSeconds(typingSpeed); //等待一定时间
        }
    }


}
