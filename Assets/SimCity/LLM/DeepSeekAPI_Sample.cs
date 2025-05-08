using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;
using UnityEngine.Networking;

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
        // 创建UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");// 设置上传处理器
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);// 设置下载处理器
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text); // 打印详细错误信息
        }
        else
        {
            // 处理响应
            string responseJson = request.downloadHandler.text;
            Debug.Log("Response: " + responseJson);
        }
    }
}
