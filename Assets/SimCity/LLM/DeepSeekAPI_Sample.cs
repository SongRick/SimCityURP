using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;

// Unity 脚本10 个引用
public class DeepSeekAPI_Sample : MonoBehaviour
{
    private string apiKey = "sk-50b19a4873f447d5ba0e50c11a836765";
    private string apiUrl = "https://api.deepseek.com/v1/chat/completions";

    void Start()
    {
        SendMessageToDeepSeek("你好啊", null);
    }
    public void SendMessageToDeepSeek(string message, UnityAction<string> callback)
    {
        StartCoroutine(PostRequest(message, callback));
    }
    IEnumerator PostRequest(string message, UnityAction<string> callback)
    {
        // 创建匿名类型请求体
        var requestBody = new
        {
            model = "deepseek-chat",
            messages = new[]
            {
                new { role = "user", content = message }
            }
        };

        // 使用Newtonsoft.Json序列化
        string jsonBody = JsonConvert.SerializeObject(requestBody);
        Debug.Log(jsonBody);
        yield return null;
    }
}
