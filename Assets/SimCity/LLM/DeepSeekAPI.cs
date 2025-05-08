using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;
using UnityEngine.Networking;
using Unity.VisualScripting.FullSerializer;
using System.Text.RegularExpressions;

// Unity 脚本，用于向 DeepSeek API 发送聊天请求
public class DeepSeekAPI : MonoBehaviour
{
    [Header("API Settings")]
    // DeepSeek API 的密钥，用于身份验证
    private string apiKey = "sk-50b19a4873f447d5ba0e50c11a836765";
    // 使用的模型名称，这里指定为 deepseek-chat
    private string modelName = "deepseek-chat";
    // DeepSeek API 的请求地址
    private string apiUrl = "https://api.deepseek.com/v1/chat/completions";
    // 备用的 API 请求地址，当前注释掉未使用
    //private string apiUrl = "https://api.siliconflow.cn/v1/chat/completions";

    // 对话参数
    [Header("Dialogue Settings")]
    // 控制生成文本的随机性，取值范围在 0 到 1 之间，值越高生成的文本越随机
    [Range(0, 1)] public float temperature = 0.5f;
    // 生成的最大令牌数，用于控制回复的长度
    [Range(1, 1000)] public int maxTokens = 1000;

    // 角色设定
    [System.Serializable]
    public class NPCCharacter
    {
        // NPC 的名称
        public string name = "城市建筑规划专家";
        // 角色设定提示词，用于向模型描述 NPC 的角色特点
        [TextArea(3, 10)]
        public string personalityPrompt = "现有3*3区域，从左上到右下依次编号为0-8；现有6种建筑类型（生活、工作、商业、医疗、教育、娱乐），编号依次为0-5；用户将提供给你城市规划的要求，请你分析并给出每块区域合理的建筑类型，不要给多余的回复，以 JSON 的形式输出，输出的 JSON 需遵守以下的格式：\r\n{\r\n\t\"areaID\":<区域编号>,\r\n\t\"buildingTypeID\":<建筑类型编号>\r\n}";
    }
    // 序列化字段，用于在 Unity 编辑器中显示和设置 NPC 角色信息
    [SerializeField]
    public NPCCharacter npcCharacter;
    // 定义一个委托，用于处理对话响应的回调函数
    public delegate void DialogueCallback(string content, bool isSuccess);

    void Start()
    {
        // 调用 SendMessageToDeepSeek 方法，发送消息 "你好啊"，不使用回调函数
        //SendMessageToDeepSeek("你好啊", null);
    }

    /// <summary>
    /// 向 DeepSeek API 发送聊天消息的公共方法
    /// </summary>
    /// <param name="message">要发送的聊天消息内容</param>
    /// <param name="callback">请求完成后的回调函数，用于处理响应结果</param>
    public void SendMessageToDeepSeek(string message, DialogueCallback callback)
    {
        // 启动协程，处理向 API 发送请求的异步操作
        StartCoroutine(PostRequest(message, callback));
    }

    /// <summary>
    /// 处理对话请求的协程
    /// </summary>
    /// <param name="message">玩家的输入内容</param>
    /// <param name="callback">回调函数，用于处理API响应</param>
    IEnumerator PostRequest(string message, DialogueCallback callback)
    {
        // 构建消息列表，包含系统提示和用户输入
        List<Message> messages = new List<Message>
        {
            // 系统角色设定，向模型提供 NPC 的角色特点信息
            new Message { role = "system", content = npcCharacter.personalityPrompt },
            // 用户输入的消息
            new Message { role = "user", content = message }
        };
        // 构建请求体
        ChatRequest requestBody = new ChatRequest
        {
            // 模型名称
            model = modelName,
            // 消息列表
            messages = messages,
            // 温度参数，控制生成文本的随机性
            temperature = temperature,
            // 最大令牌数，控制回复长度
            max_tokens = maxTokens
        };

        // 使用 Newtonsoft.Json 序列化请求体
        string jsonBody = JsonConvert.SerializeObject(requestBody);
        // 使用 Unity 的 JsonUtility 序列化请求体，仅支持自定义的类，不支持匿名传递的类
        //string jsonBody = JsonUtility.ToJson(requestBody);

        // 在控制台打印序列化后的 JSON 请求体，方便调试
        Debug.Log(jsonBody);

        // 暂停协程执行，等待下一帧
        yield return null;

        // 创建 UnityWebRequest 对象，用于发送 HTTP 请求
        UnityWebRequest request = CreateWebRequest(jsonBody);

        // 发送请求并等待请求完成
        yield return request.SendWebRequest();

        // 检查请求是否出错
        if (IsRequestError(request))
        {
            // 如果响应码为 429，表示达到速率限制
            if (request.responseCode == 429)
            {
                // 打印警告信息，提示速率限制达到，正在延迟重试
                Debug.LogWarning("速率限制达到，延迟重试中...");
                // 延迟 5 秒后重试请求
                yield return new WaitForSeconds(5);
                // 重新启动协程，再次发送请求
                StartCoroutine(PostRequest(message, callback));
                // 结束当前协程
                yield break;
            }
            else
            {
                // 打印错误信息，包含响应码和错误内容
                Debug.LogError($"API Error: {request.responseCode}\n{request.downloadHandler.text}");
                // 调用回调函数，通知请求失败
                callback?.Invoke($"API请求失败: {request.downloadHandler.text}", false);
                // 结束当前协程
                yield break;
            }
        }

        // 打印 API 响应内容
        Debug.Log(request.downloadHandler.text);
        // 解析 API 响应
        DeepSeekResponse response = ParseResponse(request.downloadHandler.text);
        // 检查响应是否有效，并且包含至少一个选择
        if (response != null && response.choices.Length > 0)
        {
            // 打印完整的响应信息
            Debug.Log("Reply " + request.downloadHandler.text);
            // 获取第一个选择的消息内容
            string npcReply = response.choices[0].message.content;
            // 打印 NPC 的回复内容
            Debug.Log(npcReply);
            // 调用解析方法
            ParseNpcReply(npcReply);
            // 调用回调函数，通知请求成功并传递回复内容
            callback?.Invoke(npcReply, true);
        }
        else
        {
            // 调用回调函数，通知请求失败，并提示 NPC 陷入沉默
            callback?.Invoke(name + " (陷入沉默)", false);
        }
        // 释放 UnityWebRequest 对象，避免资源泄漏
        request.Dispose();
        // 结束当前协程
        yield break;
    }
    // 定义一个类来表示 JSON 中的每个对象
    public class BuildingInfo
    {
        public int areaID;
        public int buildingTypeID;
    }
    // 解析 npcReply 的方法
    // 解析 npcReply 的方法
    // 解析 npcReply 的方法
    // 解析 npcReply 的方法
    public void ParseNpcReply(string npcReply)
    {
        // 打印原始的 npcReply 内容，方便调试
        Debug.Log($"原始 npcReply 内容: {npcReply}");

        // 移除前后的空白字符
        npcReply = npcReply.Trim();

        // 移除 Markdown 代码块标识
        if (npcReply.StartsWith("```json"))
        {
            npcReply = npcReply.Substring(9).TrimStart(); // 移除 ```json 并去除前导空格
        }
        if (npcReply.EndsWith("```"))
        {
            npcReply = npcReply.Substring(0, npcReply.Length - 3).TrimEnd(); // 移除 ``` 并去除尾随空格
        }

        // 确保 JSON 以数组开头和结尾
        if (!npcReply.StartsWith("["))
        {
            npcReply = "[" + npcReply;
        }
        if (!npcReply.EndsWith("]"))
        {
            npcReply = npcReply + "]";
        }

        // 打印清理后的 npcReply 内容，方便调试
        Debug.Log($"清理后的 npcReply 内容: {npcReply}");

        try
        {
            // 反序列化 JSON 字符串为 BuildingInfo 列表
            List<BuildingInfo> buildingInfos = JsonConvert.DeserializeObject<List<BuildingInfo>>(npcReply);

            // 遍历列表，获取每个区域的编号和建筑类型编号
            foreach (BuildingInfo info in buildingInfos)
            {
                int areaNumber = info.areaID;
                int buildingType = info.buildingTypeID;

                // 打印获取到的信息
                Debug.Log($"区域编号: {areaNumber}, 建筑类型编号: {buildingType}");
            }
        }
        catch (JsonException e)
        {
            // 处理 JSON 解析错误
            Debug.LogError($"JSON 解析错误: {e.Message}");
        }
    }

    /// <summary>
    /// 创建UnityWebRequest对象
    /// </summary>
    /// <param name="jsonBody">请求体的JSON字符串</param>
    /// <returns>配置好的UnityWebRequest对象</returns>
    private UnityWebRequest CreateWebRequest(string jsonBody)
    {
        // 将 JSON 请求体转换为字节数组
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        // 创建一个 POST 请求的 UnityWebRequest 对象
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        // 设置上传处理器，用于发送请求体
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        // 设置下载处理器，用于接收响应内容
        request.downloadHandler = new DownloadHandlerBuffer();
        // 设置请求头，指定请求内容类型为 JSON
        request.SetRequestHeader("Content-type", "application/json");
        // 设置认证头，使用 API 密钥进行身份验证
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
        // 设置接受类型为 JSON
        request.SetRequestHeader("Accept", "application/json");
        return request;
    }

    /// <summary>
    /// 检查请求是否出错
    /// </summary>
    /// <param name="request">UnityWebRequest对象</param>
    /// <returns>如果请求出错返回true，否则返回false</returns>
    private bool IsRequestError(UnityWebRequest request)
    {
        // 检查请求结果是否为连接错误、协议错误或数据处理错误
        return request.result == UnityWebRequest.Result.ConnectionError ||
               request.result == UnityWebRequest.Result.ProtocolError ||
               request.result == UnityWebRequest.Result.DataProcessingError;
    }

    /// <summary>
    /// 解析API响应
    /// </summary>
    /// <param name="jsonResponse">API响应的JSON字符串</param>
    /// <returns>解析后的DeepSeekResponse对象</returns>
    private DeepSeekResponse ParseResponse(string jsonResponse)
    {
        try
        {
            // 使用 JsonUtility 解析 JSON 响应为 DeepSeekResponse 对象
            DeepSeekResponse response = JsonUtility.FromJson<DeepSeekResponse>(jsonResponse);
            // 检查解析结果是否有效，或者是否包含有效的选择
            if (response == null || response.choices == null || response.choices.Length == 0)
            {
                // 打印错误信息，提示 API 响应格式错误或未包含有效数据
                Debug.LogError("API响应格式错误或未包含有效数据。");
                return null;
            }
            return response;
        }
        catch (System.Exception e)
        {
            // 打印错误信息，包含异常消息和响应内容
            Debug.LogError($"JSON解析失败：{e.Message}\n响应内容：{jsonResponse}");
            return null;
        }
    }

    // 可序列化数据结构
    [System.Serializable]
    private class ChatRequest
    {
        // 模型名称
        public string model;
        // 消息列表
        public List<Message> messages;
        // 温度参数，控制生成文本的随机性
        public float temperature;
        // 最大令牌数，控制回复长度
        public int max_tokens;
    }

    [System.Serializable]
    public class Message
    {
        // 角色 (system/user/assistant)
        public string role;
        // 消息内容
        public string content;
    }

    [System.Serializable]
    private class Choice
    {
        // 生成的消息
        public Message message;
    }

    [System.Serializable]
    private class DeepSeekResponse
    {
        // 生成的选择列表
        public Choice[] choices;
    }
}