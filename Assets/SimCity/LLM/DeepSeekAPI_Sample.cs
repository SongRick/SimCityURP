using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;
using UnityEngine.Networking;

// Unity 脚本，用于向 DeepSeek API 发送聊天请求
public class DeepSeekAPI_Sample : MonoBehaviour
{
    // DeepSeek API 的访问密钥，用于身份验证
    private string apiKey = "sk-50b19a4873f447d5ba0e50c11a836765";
    // DeepSeek API 的聊天完成接口的 URL
    private string apiUrl = "https://api.deepseek.com/v1/chat/completions";

    // 当脚本实例被启用并激活时调用此方法
    void Start()
    {
        // 调用 SendMessageToDeepSeek 方法，发送消息 "你好啊"，不使用回调函数
        SendMessageToDeepSeek("你好啊", null);
    }

    /// <summary>
    /// 向 DeepSeek API 发送聊天消息的公共方法
    /// </summary>
    /// <param name="message">要发送的聊天消息内容</param>
    /// <param name="callback">请求完成后的回调函数，用于处理响应结果</param>
    public void SendMessageToDeepSeek(string message, UnityAction<string> callback)
    {
        // 启动一个协程来执行 PostRequest 方法，进行异步网络请求
        StartCoroutine(PostRequest(message, callback));
    }

    /// <summary>
    /// 执行 POST 请求到 DeepSeek API 的协程方法
    /// </summary>
    /// <param name="message">要发送的聊天消息内容</param>
    /// <param name="callback">请求完成后的回调函数，用于处理响应结果</param>
    /// <returns>用于异步操作的 IEnumerator</returns>
    IEnumerator PostRequest(string message, UnityAction<string> callback)
    {
        // 创建一个匿名类型的对象作为请求体
        var requestBody = new
        {
            // 指定要使用的模型名称
            model = "deepseek-chat",
            // 消息列表，包含用户发送的消息
            messages = new[]
            {
                // 定义用户消息，角色为 "user"，内容为传入的消息
                new { role = "user", content = message }
            }
        };

        // 使用 Newtonsoft.Json 库将请求体对象序列化为 JSON 字符串
        string jsonBody = JsonConvert.SerializeObject(requestBody);
        // 在控制台打印序列化后的 JSON 请求体，方便调试
        Debug.Log(jsonBody);

        // 暂停协程执行，等待下一帧
        yield return null;

        // 创建一个 UnityWebRequest 对象，指定请求的 URL 和请求方法为 POST
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        // 将 JSON 请求体字符串转换为 UTF-8 编码的字节数组
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        // 设置请求的上传处理器，将字节数组作为请求体上传
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        // 设置请求的下载处理器，用于接收服务器的响应数据
        request.downloadHandler = new DownloadHandlerBuffer();
        // 设置请求头，指定请求体的内容类型为 JSON
        request.SetRequestHeader("Content-Type", "application/json");
        // 设置请求头，添加 API 访问密钥进行身份验证
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // 发送请求并等待请求完成
        yield return request.SendWebRequest();

        // 检查请求结果，如果出现连接错误或协议错误
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            // 在控制台打印错误信息
            Debug.LogError("Error: " + request.error);
            // 在控制台打印服务器返回的详细错误响应信息
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
        else
        {
            // 若请求成功，获取服务器返回的响应数据并转换为字符串
            string responseJson = request.downloadHandler.text;
            // 在控制台打印服务器的响应信息
            Debug.Log("Response: " + responseJson);
        }
        request.Dispose();
        yield break;
    }
}