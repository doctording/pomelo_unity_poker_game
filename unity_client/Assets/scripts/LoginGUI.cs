using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using Pomelo.DotNetClient;
using System.Threading;
using UnityEngine.UI;
using LitJson;

public class LoginGUI : MonoBehaviour
{
    public static string userName = "";
    public static JsonObject roomsObj = null;

    public static PomeloClient pomeloClient = null;

    protected bool _bNeedLoadScene = false;

    private Button btn_login;
    private InputField infield_username;

    public Canvas mycanvas; // 为显示提示弹框
    public GameObject tipPrefab;
    public bool tipFlag;

    void Start()
    {
        tipFlag = false;

        // 找到各个控件
        infield_username = GameObject.FindGameObjectWithTag("username").GetComponent<InputField>();
        btn_login = GameObject.FindGameObjectWithTag("btn_login").GetComponent<Button>();

        // 添加按钮的事件监听方法
        btn_login.onClick.AddListener(Login);
    }

    //When quit, release resource
    void OnApplicationQuit()
    {
        if (pomeloClient != null)
        {
            pomeloClient.disconnect();
        }
    }

    //Login the chat application and new PomeloClient.
    void Login()
    {
        userName = infield_username.text; // 获取输入框中的信息
      
        if (userName == "" )
            return;

        string host = "127.0.0.1"; // gate的host和port
        int port = 3014;

        pomeloClient = new PomeloClient();

        //listen on network state changed event
        pomeloClient.NetWorkStateChangedEvent += (state) =>
        {
            Debug.logger.Log("CurrentState is:" + state);
        };
        
        // 请求gate服务器，得到connector服务器的host和clientPort
        pomeloClient.initClient(host, port, () =>
        {
            // user 消息传递给 gate.gateHandler.queryEntry
            JsonObject user = new JsonObject();
            user["uid"] = userName;
            pomeloClient.connect(user, data =>
            {
                //process handshake call back data
                JsonObject msg = new JsonObject();
                msg["uid"] = userName;
                pomeloClient.request("gate.gateHandler.queryEntry", msg, OnQuery);
            });
        });
    }

    void OnQuery(JsonObject result)
    {
        if (Convert.ToInt32(result["code"]) == 200)
        {
            pomeloClient.disconnect();

            string connectorHost = (string)result["host"];
            int connectorPort = Convert.ToInt32(result["port"]);

            pomeloClient = new PomeloClient();

            pomeloClient.initClient(connectorHost, connectorPort, () =>
            {
                //The user data is the handshake user params
                JsonObject user = new JsonObject();
                pomeloClient.connect(user, data =>
                {
                    Entry();
                });
            });

            // 请求到了connector数据后，跳转场景
            //_bNeedLoadScene = true;
        }
    }

    void Entry()
    {
        JsonObject userMessage = new JsonObject();
        userMessage["uid"] = userName;
        pomeloClient.request("connector.entryHandler.enterFirst", userMessage, (data) =>
        {
            // 用户名重复，或者服务器内部错误
            string tmp = data.ToString();
            JsonData jd = JsonMapper.ToObject(tmp);
            if (jd.Keys.Contains("code"))
            {
                tipFlag = true;
                return;
            }
            // 用户登录成功
            roomsObj = data;
            _bNeedLoadScene = true;
        });
    }

    //When quit, release resource
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (pomeloClient != null)
            {
                pomeloClient.disconnect();
            }
            Application.Quit();
        }

        if (_bNeedLoadScene)
        {
            // 场景切换
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        if (tipFlag)
        {
            tipFlag = false;
            GameObject newTip = Instantiate(tipPrefab);
            newTip.transform.FindChild("Text").GetComponent<Text>().text = "user alreay exist";
            newTip.transform.parent = mycanvas.transform;
            newTip.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            Destroy(newTip, 2.0f);
        }
    }
}