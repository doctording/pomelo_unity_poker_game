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

public class RoomCon : MonoBehaviour
{
    public static JsonObject userMes;

    public static int index; // 进入房间的顺序

    // 直接从login 获取userName,rooms,PomeloClient
    private string userName = LoginGUI.userName;
    private JsonObject roomsObjHave = LoginGUI.roomsObj;

    private PomeloClient pclient = LoginGUI.pomeloClient;

    public ScrollRect Scroll_View_roomList;
    public InputField InputField_roomid;
    public Button btn_create;
    public Button btn_choose;

    public GameObject tip;
    public GameObject mycanvas;

    protected bool _bNeedLoadScene = false;

    private ArrayList roomList = null;
    Dictionary<string, string> hashMap = new Dictionary<string, string>();

    public static ArrayList userListInARoom = new ArrayList();
    public static Dictionary<string, int> mapIndex = new Dictionary<string, int>(); // 名字与其进入房间的index对应
    public static Dictionary<int, string> mapIndex2 = new Dictionary<int, string>(); // 名字与index对应

    public static string roomid = null; // 用户的roomId

    public Text text_hello;

    void Start()
    {
        Application.runInBackground = true;

        roomList = new ArrayList();

        // 找到各个控件
        InputField_roomid = GameObject.FindGameObjectWithTag("channel").GetComponent<InputField>();
        btn_create = GameObject.FindGameObjectWithTag("btn_create").GetComponent<Button>();
        btn_choose = GameObject.FindGameObjectWithTag("btn_choose").GetComponent<Button>();

        // 添加按钮的事件监听方法
        btn_choose.onClick.AddListener(chooseRoom);
        btn_create.onClick.AddListener(createRoom);

        // 刚进入场景得到的房间列表
        string s = roomsObjHave.ToString();
        JsonData jd = JsonMapper.ToObject(s);
        if (jd.Keys.Count >= 1)
        {
            int len = jd["rooms"].Count;
            if (len >= 1)
            {
                ICollection<string> it = jd["rooms"].Keys;
                foreach (string c in it)
                {
                    // 具体的一个房间的情况
                    string aobj = jd["rooms"][c].ToJson();
                    JsonData jdobj = JsonMapper.ToObject(aobj);
                    string createUser = jdobj["createUser"].ToString();
                    string num = jdobj["number"].ToString(); // 人数

                    int intnum = int.Parse(num);
                    if (intnum == 0)
                    {
                        if (roomList.Contains(c))
                            roomList.Remove(c);
                    }
                    else
                    {
                        if (hashMap.ContainsKey(c))
                        {
                            hashMap.Remove(c);
                            hashMap.Add(c, num);
                        }
                        else
                        {
                            hashMap.Add(c, num);
                        }
                        if (!roomList.Contains(c))
                            roomList.Add(c);
                    }
                }
            }
        }

        // 有新的房间，接收服务器push过来的消息
        pclient.on("onAddRoom", (data) =>
        {
            string tmp = data.ToString();
            JsonData jdtmp = JsonMapper.ToObject(tmp);

            string newRid = jdtmp["rid"].ToString();
            string num = jdtmp["num"].ToString(); // 该房间的人数

            if (hashMap.ContainsKey(newRid))
            {
                hashMap.Remove(newRid);
                hashMap.Add(newRid, num);
            }
            else
            {
                hashMap.Add(newRid, num);
            }

            if (!roomList.Contains(newRid))
                roomList.Add(newRid);
        });

        // 某个房间添加了一个新用户
        pclient.on("onAddCommonUser", (data) =>
        {
            string tmp = data.ToString();
            JsonData jdtmp = JsonMapper.ToObject(tmp);

            string newRid = jdtmp["rid"].ToString();
            string num = jdtmp["num"].ToString(); // 该房间的人数

            if (hashMap.ContainsKey(newRid))
            {
                hashMap.Remove(newRid);
                hashMap.Add(newRid, num);
            }
            else
            {
                hashMap.Add(newRid, num);
            }

            if (!roomList.Contains(newRid))
                roomList.Add(newRid);
        });

        // 某个房间删除了一个普通用户
        pclient.on("onDelCommonUser", (data) =>
        {
            string tmp = data.ToString();
            JsonData jdtmp = JsonMapper.ToObject(tmp);

            string newRid = jdtmp["rid"].ToString();
            string num = jdtmp["num"].ToString(); // 该房间的人数

            int intnum = int.Parse(num);
            if (intnum == 0)
            {
                if (roomList.Contains(newRid))
                    roomList.Remove(newRid);
            }
            else
            {
                if (hashMap.ContainsKey(newRid))
                {
                    hashMap.Remove(newRid);
                    hashMap.Add(newRid, num);
                }
                else
                {
                    hashMap.Add(newRid, num);
                }

                if (!roomList.Contains(newRid))
                    roomList.Add(newRid);
            }
        });

    }

    //  主动选择一个房间
    void chooseRoom()
    {
        roomid = InputField_roomid.text; // 获取输入框中的信息
        if (roomid == null)
            return;

        if (!roomList.Contains(roomid))
        {
            GameObject newTip = Instantiate(tip);
            newTip.transform.FindChild("Text").GetComponent<Text>().text = "此房间不存在！";
            newTip.transform.parent = mycanvas.transform;
            newTip.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            Destroy(newTip, 2.0f);

            return;
        }

        if (int.Parse(hashMap[roomid]) == 3)
        {
            GameObject newTip = Instantiate(tip);
            newTip.transform.FindChild("Text").GetComponent<Text>().text = "此房间已满！";
            newTip.transform.parent = mycanvas.transform;
            newTip.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            Destroy(newTip, 2.0f);

            return;
        }

        JsonObject userMessage = new JsonObject();
        userMessage.Add("uid", userName);
        userMessage.Add("rid", roomid);

        if (pclient != null)
        {
            pclient.request("connector.entryHandler.enter2", userMessage, (data) =>
            {
                userMes = data;

                string s = data.ToString();
                JsonData jd = JsonMapper.ToObject(s);
                //int len = jd["rooms"].Count;
                ICollection<string> it = jd["rooms"].Keys;
                foreach (string c in it)
                {
                    // 具体的一个房间的情况
                    string aobj = jd["rooms"][c].ToJson();
                    JsonData jdobj = JsonMapper.ToObject(aobj);
                    string createUser = jdobj["createUser"].ToString();
                    string num = jdobj["number"].ToString(); // 人数

                    int intnum = int.Parse(num);

                    index = intnum;

                    // 当前房间的所有用户
                    if (roomid != null && c == roomid && intnum >= 1)
                    {
                        string alluser = jdobj["users"].ToJson();
                        JsonData jdUser = JsonMapper.ToObject(alluser);
                        ICollection<string> it2 = jdUser.Keys;
                        foreach (string uname in it2)
                        {
                            string tmp2 = jdUser[uname].ToJson();
                            JsonData tmpObj2 = JsonMapper.ToObject(tmp2);
                            string indexStr = tmpObj2["index"].ToString();
                            int indexInt = int.Parse(indexStr);

                            // 用户，及其 进入房间的index改变
                            if (!userListInARoom.Contains(uname))
                            {
                                userListInARoom.Add(uname);
                            }
                            if (mapIndex.ContainsKey(uname))
                                mapIndex[uname] = indexInt;
                            else
                                mapIndex.Add(uname, indexInt);

                            if (mapIndex2.ContainsKey(indexInt))
                                mapIndex2[indexInt] = uname;
                            else
                                mapIndex2.Add(indexInt, uname);

                            //if (!userListInARoom.Contains(uname))
                            //{
                            //    userListInARoom.Add(uname);
                            //}
                        }
                    }

                    if (hashMap.ContainsKey(c))
                    {
                        hashMap.Remove(c);
                        hashMap.Add(c, num);
                    }
                    else
                    {
                        hashMap.Add(c, num);
                    }
                    if (!roomList.Contains(c))
                        roomList.Add(c);
                }

                _bNeedLoadScene = true;
            });
        }
    }

    // 自己创建一个房间，然后直接进入场景
    void createRoom()
    {
        roomid = InputField_roomid.text; // 获取输入框中的信息
        if (roomid == null)
            return;

        JsonObject userMessage = new JsonObject();
        userMessage.Add("uid", userName);
        userMessage.Add("rid", roomid);

        if (roomList.Contains(roomid))
        {
            GameObject newTip = Instantiate(tip);
            newTip.transform.FindChild("Text").GetComponent<Text>().text = "此房间已存在！";
            newTip.transform.parent = mycanvas.transform;
            newTip.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            Destroy(newTip, 2.0f);

            return;
        }

        if (pclient != null)
        {
            pclient.request("connector.entryHandler.enter", userMessage, (data) =>
            {
                userMes = data;

                string s = data.ToString();
                JsonData jd = JsonMapper.ToObject(s);
                // int len = jd["rooms"].Count;
                ICollection<string> it = jd["rooms"].Keys;
                foreach (string c in it)
                {
                    // 具体的一个房间的情况
                    string aobj = jd["rooms"][c].ToJson();
                    JsonData jdobj = JsonMapper.ToObject(aobj);
                    string createUser = jdobj["createUser"].ToString();
                    string num = jdobj["number"].ToString(); // 人数

                    // 当前房间的所有用户
                    if (roomid != null && c == roomid)
                    {
                        string alluser = jdobj["users"].ToJson();
                        JsonData jdUser = JsonMapper.ToObject(alluser);
                        ICollection<string> it2 = jdUser.Keys;
                        foreach (string uname in it2)
                        {
                            string tmp2 = jdUser[uname].ToJson();
                            JsonData tmpObj2 = JsonMapper.ToObject(tmp2);
                            string indexStr = tmpObj2["index"].ToString();
                            int indexInt = int.Parse(indexStr);

                            // 用户，及其 进入房间的index改变
                            if (!userListInARoom.Contains(uname))
                            {
                                userListInARoom.Add(uname);
                            }
                            if (mapIndex.ContainsKey(uname))
                                mapIndex[uname] = indexInt;
                            else
                                mapIndex.Add(uname, indexInt);

                            if (mapIndex2.ContainsKey(indexInt))
                                mapIndex2[indexInt] = uname;
                            else
                                mapIndex2.Add(indexInt, uname);
                        }
                    }

                    if (hashMap.ContainsKey(c))
                    {
                        hashMap.Remove(c);
                        hashMap.Add(c, num);
                    }
                    else
                    {
                        hashMap.Add(c, num);
                    }
                    if (!roomList.Contains(c))
                        roomList.Add(c);
                }

                index = 1;
                _bNeedLoadScene = true;
            });
        }
    }

    // 转换成显示的字符串
    string showstr(string rid, string num)
    {
        string rs = "房间:" + rid + ", 人数: " + num;
        return rs;
    }

    //When quit, release resource
    void Update()
    {
        text_hello.text = "Hello:" + userName;

        if (Input.GetKey(KeyCode.Escape) || Input.GetKey("escape"))
        {
            if (pclient != null)
            {
                pclient.disconnect();
            }
            Application.Quit();
        }
        if (_bNeedLoadScene)    // 选好房间后直接跳转
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
        }
        else
        {
            // msglist
            foreach (Transform child in Scroll_View_roomList.transform.FindChild("Viewport").FindChild("Content").transform)
            {
                Destroy(child.gameObject);
            }

            if (roomList != null && roomList.Count >= 1)
                foreach (string roomid in roomList)
                {
                    GameObject t3 = new GameObject();
                    t3.transform.localPosition = new Vector3(0, 0, 0);
                    t3.AddComponent<Text>();
                    t3.GetComponent<Text>().text = showstr(roomid, hashMap[roomid]);
                    t3.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                    t3.GetComponent<Text>().fontSize = 20;
                    t3.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                    RectTransform rectTransform2 = t3.GetComponent<RectTransform>();
                    rectTransform2.localPosition = new Vector3(0, 0, 0);
                    t3.transform.parent = Scroll_View_roomList.transform.FindChild("Viewport").FindChild("Content");

                }
        }
    }

    //When quit, release resource
    void OnApplicationQuit()
    {
        if (pclient != null)
        {
            pclient.disconnect();
        }
    }

}