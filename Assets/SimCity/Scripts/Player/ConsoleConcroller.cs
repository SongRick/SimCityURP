using System;
using System.Collections.Generic;
using UnityEngine;
//参考教程https://www.bilibili.com/video/BV1AG4y1G7jz时间01162222 T1不好用

namespace SimCity.FinalController
{
    [DefaultExecutionOrder(-4)]
    #region ConsoleCommandBase
    public class ConsoleCommandBase
    {
        private string _commandID;
        private string _commandDescription;
        private string _commandFormat;

        public string commandID { get { return _commandID; } }
        public string commandDescription { get { return _commandDescription; } }
        public string commandFormat { get { return _commandFormat; } }
        public ConsoleCommandBase(string id, string description, string format)
        {
            _commandID = id;
            _commandDescription = description;
            _commandFormat = format;
        }
    }
    #endregion
    #region ConsoleCommand
    public class ConsoleCommand : ConsoleCommandBase
    {
        private Action command;
        public ConsoleCommand(string id, string description, string format, Action command) : base(id, description, format)
        {
            this.command = command;
        }
        public void Invoke()
        {
            command.Invoke();
        }
    }
    #endregion
    #region ConsoleCommand<T1>
    public class ConsoleCommand<T1> : ConsoleCommandBase
    {
        private Action<T1> command;
        public ConsoleCommand(string id, string description, string format, Action<T1> command) : base(id, description, format)
        {
            this.command = command;
        }
        public void Invoke(T1 value)
        {
            command.Invoke(value);
        }
    }
    #endregion
    #region ConsoleConcroller
    public class ConsoleConcroller : MonoBehaviour
    {
        public static bool showConsole;//显示控制台
        private bool showHelp;//显示帮助

        public string input;
        public string printContent;

        public Vector2 scrollPosition;

        public static ConsoleCommand HELP;
        public static ConsoleCommand SHOW_INFO;
        public static ConsoleCommand REMOVE;
        public static ConsoleCommand<float> SET_HEIGHT;
        public static ConsoleCommand COMMAND1;
        public static ConsoleCommand COMMAND2;


        public List<object> commandList;


        private void Awake()
        {
            HELP = new ConsoleCommand("help", "shows a list of commands", "help", () =>
            {
                printContent = "";
                for (int i = 0; i < commandList.Count; i++)
                {
                    ConsoleCommandBase command = commandList[i] as ConsoleCommandBase;
                    printContent += $"{command.commandFormat}\t-{command.commandDescription}\n";
                }
            });
            SHOW_INFO = new ConsoleCommand("showinfo", "display information of the selected object", "showinfo", () =>
            {

            });
            REMOVE = new ConsoleCommand("remove", "remove the selected object", "remove", () =>
            {

            });
            SET_HEIGHT = new ConsoleCommand<float>("setheight", "set the height of the selected object to x times the default height.", "setheight <x>", (x) =>
            {

            });
            COMMAND1 = new ConsoleCommand("command1", "command1", "command1", () =>
            {

            });
            COMMAND2 = new ConsoleCommand("command2", "command2", "command2", () =>
            {

            });
            commandList = new List<object>
            {
                HELP,
                SHOW_INFO,
                REMOVE,
                SET_HEIGHT,
                COMMAND1,
                COMMAND2
            };


        }

        private void Start()
        {
            showConsole = false;
        }

        private void Update()
        {

        }

        public void OnGUI()
        {
            if (!showConsole)
                return;
            //定义GUIStyle
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.yellow;
            labelStyle.fontSize = 40;
            float widthTextField = 60f;//文本输入框的高度
            float widthScroll = 400;//滚动条的高度
            float margin = 5f;//文本输入框或滚动条 与各自box的边距
            float maxScrollContentRectHeight = 1000;//滚动条内容高度

            //绘制放置文本输入框的box
            Rect rectBoxForTextField = new Rect(0f, Screen.height - widthTextField, Screen.width, widthTextField);
            GUI.Box(rectBoxForTextField, "");
            // 确保输入框可以获得焦点
            GUI.SetNextControlName("MyTextField");
            //绘制输入框，与box上下左右的边距均为margin
            Rect rectTextField = new Rect
                (rectBoxForTextField.x + margin,
                rectBoxForTextField.y + margin,
                rectBoxForTextField.width - 2 * margin,
                rectBoxForTextField.height - 2 * margin);
            input = GUI.TextField(rectTextField, input, labelStyle);
            // 让输入框自动获得焦点
            if (Event.current.type == EventType.Repaint)
            {
                GUI.FocusControl("MyTextField");
            }

            //绘制放置滚动条的box
            Rect rectBoxForScroll = new Rect
                (rectBoxForTextField.x,
                rectBoxForTextField.y - widthScroll,
                Screen.width,
                widthScroll);
            GUI.Box(rectBoxForScroll, "");
            // 绘制滚动条
            // 第一个参数 rectScroll 表示滚动条的位置和大小
            // 第二个参数 scrollPosition 表示滚动位置，存储用户滚动的位置信息
            // 第三个参数 rectScrollView 表示滚动视图中内容的位置（相对于滚动视图而非屏幕原点）和大小
            Rect rectScroll = new Rect
                (rectBoxForScroll.x + 3 * margin,// 滚动条内容缩进3*margin，且刚好隐藏右侧的滚动滑块？哈哈
                rectBoxForScroll.y,
                rectBoxForScroll.width,
                rectBoxForScroll.height);
            Rect rectScrollView = new Rect
                (margin,
                margin,
                0,// 将宽度设为极小值，似乎可以隐藏底部的滚动滑块？
                maxScrollContentRectHeight);
            scrollPosition = GUI.BeginScrollView(rectScroll, scrollPosition, rectScrollView);
            GUI.Label(rectScrollView, printContent, labelStyle);
            GUI.EndScrollView();
            // 处理 GUI 事件，确保不拦截鼠标点击事件
            // 如果不加这行代码，调试时无问题，但是打包生成游戏，运行时会发现，调出控制台时无法单击鼠标以选中对象
            if (Event.current.type == EventType.MouseDown)
            {
                Event.current.Use();
            }
        }

        public void HandleInput()
        {
            string[] properties = input.Split(' ');
            bool isCommandValued = false;// 判断输入的命令是否有效
            string strCommandIncorrect = "The syntax of the command is incorrect, please try again!";
            for (int i = 0; i < commandList.Count; i++)
            {
                ConsoleCommandBase commandBase = commandList[i] as ConsoleCommandBase;
                if (input.Contains(commandBase.commandID))
                {
                    if (commandList[i] as ConsoleCommand != null)
                    {
                        (commandList[i] as ConsoleCommand).Invoke();
                    }
                    else if (commandList[i] as ConsoleCommand<float> != null)
                    {
                        if (properties.Length > 1)
                        {
                            (commandList[i] as ConsoleCommand<float>).Invoke(float.Parse(properties[1]));
                        }
                        else
                        {
                            Debug.Log("The syntax of the command is incorrect, please try again!");
                        }
                    }
                    isCommandValued = true;
                }
            }
            if (!isCommandValued)
            {
                printContent = $"{input}\n{strCommandIncorrect}";
            }
        }








    }
    #endregion

}