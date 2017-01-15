using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SimpleJson;
using Pomelo.DotNetClient;
using System.Threading;
using LitJson;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class puke : MonoBehaviour
{
    // 直接从login 获取userName,PomeloClient，
    private string userName = LoginGUI.userName;
    private PomeloClient pclient = LoginGUI.pomeloClient;
    // 从 RoomCon 获取 房间号，同房间号的人列表，自己加入房间的序号等
    private ArrayList userList = RoomCon.userListInARoom; 
    private string roomid = RoomCon.roomid;
    private int index = RoomCon.index;

    //
    public ArrayList userCardNumberGO; //玩家的牌GameObject 
    public ArrayList userCardNumber; //玩家的牌
    Dictionary<int, bool> hashMap;
    public int lastOrderNum; // 上一的出牌玩家的Id,用来判断哪个玩家可以出牌
    public int lastCardNum; // 现规定每次只能出一张牌， 表示 上一次 某个玩家出的牌

    public ScrollRect Scroll_View_cards; // 自己出牌的Scroll_View, 出牌相关的几个按钮
    public Button btn_Pass; // 不出牌，让下一个人出牌
    public Button btn_Out; // 打出自己选出的牌
    public Button btn_Reset; // 重新选择自己牌
    public Button btn_start; // 准备按钮

    // 桌面上出的牌的相关信息
    public ScrollRect Scroll_View_cards_lastOut;
    public Text lastIndex;
    public string lastIndexTextContent;

    // 3个用户名字显示的Text
    public Text text_me;
    public Text text_le;
    public Text text_ri;
    Dictionary<string, int> mapIndex = RoomCon.mapIndex; // 名字与index对应
    Dictionary<int, string> mapIndex2 = RoomCon.mapIndex2; // 名字与index对应
   
    public Text isReadyCnt;
    private int isReadyCount;
    public string isReadyUidText;

    public Canvas mycanvas; // canvas
    public GameObject tipPrefab; // 弹框的prefab
    public GameObject msgPrefab; // 牌的信息prefab

    public int GameState; // -1 游戏未开始， 0 游戏开始了， 1 游戏已经结束（也可以说游戏未开始）
    bool isFapai; // 是否需要发牌的flag标志
    public int whosTurn; // 这一轮轮到谁出牌

    bool btnStart_Flag; // 准备按钮的标志（主要是按钮需要变化背景，是否可点击的设置）

    public Text Text_LeCnt; // 自己界面上用户的牌的数量 Text
    public Text Text_RiCnt;
    Dictionary<int, int> index_CardsCnt; //某个用户牌的数量 hash结构

    public ArrayList deleteCards; // 用来存储上一次出过的牌（可以选择不出牌）

    void Start()
    {
        Application.runInBackground = true;
        //////////////////////////////////////////////////////
        index_CardsCnt = new Dictionary<int, int>();
        btnStart_Flag = false;
        GameState = -1;
        deleteCards = new ArrayList();
        lastIndexTextContent = "还未开始";
        isFapai = false;
        userCardNumberGO = null; //玩家的牌GameObject 
        userCardNumber = new ArrayList(); //玩家的牌
        hashMap = new Dictionary<int, bool>();

        isReadyUidText = "";
        isReadyCount = 0;
        isReadyCnt.text = "Cnt";
        msgPrefab = Resources.Load<GameObject>("Prefabs/GameObject_1");
       
        // 添加按钮的事件监听方法
        btn_Out.onClick.AddListener(Btn_OutCards);
        btn_Reset.onClick.AddListener(Btn_ResetCards);
        btn_start.onClick.AddListener(Btn_StartGame);
        btn_Pass.onClick.AddListener(Btn_BuChuPai);

        //mapIndex2.Add(index, userName);
        //mapIndex.Add(userName, index);
        if (mapIndex2.ContainsKey(index))
            mapIndex2[index] = userName;
        else
            mapIndex2.Add(index, userName);

        if (mapIndex.ContainsKey(userName))
            mapIndex[userName] = index;
        else
            mapIndex[userName] = index;
        
        // pomelo消息监听
        if (pclient != null)
        {
            // 该房间有一个新的的用户加入了
            pclient.on("onCommonUserAddToGame", userAdd);

            // 3个用户都准备好了，开始游戏（发牌，初始数据等等）
            pclient.on("onStartGame", getPuke);

            // 有用户点击了准备按钮
            pclient.on("onReady", readyPuke);

            // 某个用户出牌
            pclient.on("onChupai", Chupai);

            // 某个用户选择了不出牌，服务器也推送消息
            pclient.on("onBuchupai", BuchupaiUI);

            // 游戏结束
            pclient.on("onOver", gameOverUI);
        }

        btnStart_Flag = true;

        if(index == 3) // 如果自己是最后一个进入的，那么准备按钮初始 就是可点击的
        {
            btnStart_Flag = false;
        }

        // 玩家中途退出了,情况有多种情况 ???
        //pclient.on("onDelCommonUserInRoom", (data) =>
        //{
        //    string tmp = data.ToString();
        //    JsonData jdtmp = JsonMapper.ToObject(tmp);

        //    string newRid = jdtmp["rid"].ToString();
        //    string uid = jdtmp["uid"].ToString();

        //});
    }

    void userAdd(JsonObject data)
    {
        string tmp = data.ToString();
        JsonData jdtmp = JsonMapper.ToObject(tmp);

        string newRid = jdtmp["rid"].ToString();
        string uid = jdtmp["uid"].ToString();
        string strIndex = jdtmp["index"].ToString();

        if(int.Parse(strIndex) == 3)
        {
            btnStart_Flag = false;
        }

        if (!userList.Contains(uid))
            userList.Add(uid);

        if (mapIndex.ContainsKey(uid))
            mapIndex[uid] = int.Parse(strIndex);
        else
            mapIndex.Add(uid, int.Parse(strIndex));

        if (mapIndex2.ContainsKey(int.Parse(strIndex)))
            mapIndex2[int.Parse(strIndex)] = uid;
        else
            mapIndex2.Add(int.Parse(strIndex), uid);
    }

    void getPuke(JsonObject data)
    {
        whosTurn = 1;
        //isReadyUidText = "Already:game start";
        //isReadyUidText = "Already," + "whosTurn: " + mapIndex2[whosTurn];
        isReadyUidText = "当前出牌者：" + mapIndex2[whosTurn];

        //Debug.Log("onStartGame----------");
        string tmp = data.ToString();
        JsonData jdtmp = JsonMapper.ToObject(tmp);

        string tmp2 = jdtmp["arrayPuke"].ToJson();
        JsonData jdtmp2 = JsonMapper.ToObject(tmp2);

        string userIndex = "u" + index.ToString();
        JsonData jdtmpU1 = JsonMapper.ToObject(jdtmp2[userIndex].ToJson());

        /**
         * 这里需要对服务端随机发过来的牌进行排序，然后再显示
         */
        Card[] cardsTmp = new Card[jdtmpU1.Count];

        for (int i = 0; i < jdtmpU1.Count; i++)
        {
            string numStr = jdtmpU1[i].ToString();
            int numInt = int.Parse(numStr);
            cardsTmp[i] = new Card(numInt);
            // if(!userCardNumber.Contains(numInt))
            //userCardNumber.Add(numInt);
        }
        CardsRules.SortCards(cardsTmp, cardsTmp.Length);

        for (int i = 0; i < jdtmpU1.Count; i++)
            userCardNumber.Add(cardsTmp[i].GetIntVal);


        foreach (int val in userCardNumber)
        {
            if (!hashMap.ContainsKey(val))
                hashMap.Add(val, false);
        }

        lastIndexTextContent = "就绪";
        isFapai = true;
        userCardNumberGO = new ArrayList();
        GameState = 0;

        // 每个玩家对应的剩余扑克牌数
        index_CardsCnt.Add(1, 17);
        index_CardsCnt.Add(2, 17);
        index_CardsCnt.Add(3, 17);
    }

    void readyPuke(JsonObject data)
    {
        isReadyCount++;

        string tmp = data.ToString();
        JsonData jdtmp = JsonMapper.ToObject(tmp);
        string uid = jdtmp["uid"].ToString();

        isReadyUidText = isReadyUidText + "," + uid;
    }

    void Chupai(JsonObject data)
    {
        //Debug.Log("有人出了牌");
        string tmp = data.ToString();
        JsonData jdtmp = JsonMapper.ToObject(tmp);

        string rid = jdtmp["rid"].ToString();
        string uid = jdtmp["uid"].ToString();
        string strIndex = jdtmp["index"].ToString();

        lastIndexTextContent = uid; // 记录牌局中 显示出的牌的人的uid；

        string tmp2 = jdtmp["deleteCards"].ToJson();
        JsonData jdtmp2 = JsonMapper.ToObject(tmp2);

        deleteCards.Clear();
        for (int i = 0; i < jdtmp2.Count; i++)
        {
            string numStr = jdtmp2[i].ToString();
            deleteCards.Add(numStr);
        }
        //
        whosTurn = getNextIndex(int.Parse(strIndex));
        //isReadyUidText = "Already." + "whosTurn: " + mapIndex2[whosTurn];
        isReadyUidText = "当前出牌者：" + mapIndex2[whosTurn];

        // 某个玩家牌数量的改变
        index_CardsCnt[int.Parse(strIndex)] = index_CardsCnt[int.Parse(strIndex)] - jdtmp2.Count;
       
    }

    void BuchupaiUI(JsonObject data)
    {
        // Debug.Log("有人直接pass出牌");
        string tmp = data.ToString();
        JsonData jdtmp = JsonMapper.ToObject(tmp);

        string rid = jdtmp["rid"].ToString();
        string uid = jdtmp["uid"].ToString();
        string strIndex = jdtmp["index"].ToString();

        whosTurn = getNextIndex(int.Parse(strIndex));
        //isReadyUidText = "Already." + "whosTurn: " + mapIndex2[whosTurn];
        isReadyUidText = "当前出牌者：" + mapIndex2[whosTurn];
    }

    void gameOverUI(JsonObject data)
    {
        userCardNumber.Clear();
        userCardNumberGO.Clear();
        deleteCards.Clear();
        hashMap.Clear();

        GameState = 1;
        isReadyCount = 0;
        isFapai = false;

        isReadyUidText = "";
        lastIndexTextContent = "未开始";

        btnStart_Flag = false;
    }

    // 每一轮都按照顺序出牌，根据当前用户的index,得到他下一个用户的index
    int getNextIndex(int index)
    {
        if (index < 3)
            return index + 1;
        else if (index == 3)
            return 1;
        return -1;
    }

    // 准备按钮的点击事件处理
    void Btn_StartGame()
    {
        if (btnStart_Flag) // 已经点过了,或者人还没有凑成3个人
        {
            return;
        }
        btnStart_Flag = true;

        GameState = 0;

        deleteCards.Clear(); // 清空deleteCards的数据

        isReadyCount++;

        JsonObject message = new JsonObject();
        message.Add("uid", userName);
        message.Add("rid", roomid);
        message.Add("index", index);

        if (isReadyCount < 3)
        {
            pclient.request("chat.chatHandler.isready", message, (data) =>
            {
                //
                Debug.Log("已准备好");
            });
        }
        if (isReadyCount == 3)
        {
            pclient.request("chat.chatHandler.fapai", message, (data) =>
            {
                Debug.Log("fapai");
            });
            //pclient.request("chat.chatHandler.fapai", message, getPuke);
            //pclient.request("chat.chatHandler.fapai", message, (data) =>
            //{
            //    string tmp = data.ToString();
            //    JsonData jdtmp = JsonMapper.ToObject(tmp);

            //    string tmp2 = jdtmp["arrayPuke"].ToJson();
            //    JsonData jdtmp2 = JsonMapper.ToObject(tmp2);

            //    string userIndex = "u" + index.ToString();
            //    JsonData jdtmpU1 = JsonMapper.ToObject(jdtmp2[userIndex].ToJson());
            //    for (int i = 0; i < jdtmpU1.Count; i++)
            //    {
            //        string numStr = jdtmpU1[i].ToString();
            //        int numInt = int.Parse(numStr);
            //        if(!userCardNumber.Contains(numInt))
            //            userCardNumber.Add(numInt);
            //    }
            //});
        }
    }

    // 不出牌按钮的点击事件
    void Btn_BuChuPai()
    {
        if (!isGameStart())
            return;

        if (whosTurn != index)
        {
            //Debug.Log("还未轮到自己出牌");
            GameObject newTip = Instantiate(tipPrefab);
            newTip.transform.FindChild("Text").GetComponent<Text>().text = "还未轮到自己出牌！";
            newTip.transform.parent = mycanvas.transform;
            newTip.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            Destroy(newTip, 2.0f);

            return;
        }

        // 给服务发送出牌的消息
        JsonObject message = new JsonObject();
        message.Add("uid", userName);
        message.Add("rid", roomid);
        message.Add("index", index);
        pclient.request("chat.chatHandler.buchupai", message, (data) =>
        {
            Debug.Log("不出牌");
        });

        Btn_ResetCards();
    }

    // 出牌按钮的点击事件
    void Btn_OutCards()
    {
        if (!isGameStart())
            return;

        if (whosTurn != index)
        {
            //Debug.Log("还未轮到自己出牌");
            GameObject newTip = Instantiate(tipPrefab);
            newTip.transform.FindChild("Text").GetComponent<Text>().text = "还未轮到自己出牌！";
            newTip.transform.parent = mycanvas.transform;
            newTip.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            Destroy(newTip, 2.0f);

            return;
        }
        //whosTurn = getNextIndex(whosTurn);// 改变下一个出牌者
        //isReadyUidText = "Already." + "whosTurn: " + mapIndex2[whosTurn];

        ArrayList deleteObj = new ArrayList();
        foreach (GameObject obj in userCardNumberGO)
        {
            string strVal = obj.transform.FindChild("Text").gameObject.GetComponent<Text>().text;
            int val = int.Parse(strVal);
            bool flag = hashMap[val];
            if (flag == true)
            {
                deleteObj.Add(obj);

                //Destroy(obj);
                //userCardNumber.Remove(val);
                //userCardNumberGO.Remove(obj);

                //GameObject t2 = (GameObject)Instantiate(msgPrefab);
                //t2.transform.FindChild("Text").gameObject.GetComponent<Text>().text = val.ToString();
                //t2.transform.FindChild("Image").gameObject.GetComponent<Image>().GetComponent<Image>().overrideSprite =
                //    Resources.Load("puke_prefabs/" + val.ToString(), typeof(Sprite)) as Sprite;
                //t2.transform.SetParent(Scroll_View_cards_lastOut.transform.FindChild("Viewport").FindChild("Content").transform);
            }
        }

        Card[] cards = new Card[deleteObj.Count];
        int i = 0;
        //ArrayList deleteCardsNum = new ArrayList();
        // deleteCards.Clear();
        foreach (GameObject obj in deleteObj)
        {
            string strVal = obj.transform.FindChild("Text").gameObject.GetComponent<Text>().text;
            //deleteCards.Add(strVal);
            //deleteCardsNum.Add(strVal);

            Card c = new Card(int.Parse(strVal));
            cards[i++] = c;
        }
        CardsRules.SortCards(cards, cards.Length);
        CardsType type2;
        if (!CardsRules.PopEnable(cards, out type2)) // 出牌不合理，或者没有出牌（点击pass按钮），都是无效操作
        {
            deleteCards.Clear();

            GameObject newTip = Instantiate(tipPrefab);
            newTip.transform.FindChild("Text").GetComponent<Text>().text = "出牌不合理！";
            newTip.transform.parent = mycanvas.transform;
            newTip.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            Destroy(newTip, 2.0f);

            return;
        }
        // lastIndex == index ,随便出
        if (lastIndexTextContent == "就绪" || int.Parse(lastIndexTextContent) == index)
        {

        }
        else // 出的牌 必须大于上一个玩家的牌，才能出，否则不能出牌
        {
            // cards 1，上（上）一个玩家出的牌
            int ii = 0;
            Card[] c1 = new Card[deleteCards.Count];
            foreach (string val in deleteCards)
            {
                c1[ii++] = new Card(int.Parse(val));
            }
            CardsRules.SortCards(c1, deleteCards.Count);
            // cards 2 就是 cards（当前要出的牌）
            CardsType type1;
            CardsRules.PopEnable(c1, out type1);

            // 不合理的话，弹框提醒，并退出
            if (!PlayCard.isCanOutCard(c1, cards, type1, type2))
            {
                GameObject newTip = Instantiate(tipPrefab);
                newTip.transform.FindChild("Text").GetComponent<Text>().text = "出的牌不够大！";
                newTip.transform.parent = mycanvas.transform;
                newTip.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                Destroy(newTip, 2.0f);

                return;
            }
        }

        ArrayList deleteCardsNum = new ArrayList();
        deleteCards.Clear();
        for (int j = 0; j < i; j++)
        {
            string tmp = cards[j].GetIntVal.ToString();
            deleteCards.Add(tmp);
            deleteCardsNum.Add(tmp);
        }

        if (deleteCardsNum.Count == userCardNumberGO.Count)
        {
            // TODO 发送游戏结束的消息 
            //isReadyUidText = "Game Over, Start new Game";
            JsonObject messageOver = new JsonObject();
            messageOver.Add("uid", userName);
            messageOver.Add("rid", roomid);
            messageOver.Add("index", index);
            pclient.request("chat.chatHandler.over", messageOver, (data) =>
            {
                Debug.Log("游戏结束");
            });
            return; // 结束后直接return,下面的出牌消息直接不发送
            //
        }
        // 给服务发送出牌的消息
        JsonObject message = new JsonObject();
        message.Add("uid", userName);
        message.Add("rid", roomid);
        message.Add("index", index);
        message.Add("delete_cards", deleteCardsNum);
        pclient.request("chat.chatHandler.chupai", message, (data) =>
        {
            Debug.Log("出牌");
        });
    }

    // 重置按钮的点击事件
    void Btn_ResetCards()
    {
        if (!isGameStart())
            return;

        foreach (GameObject obj in userCardNumberGO)
        {
            string strVal = obj.transform.FindChild("Text").gameObject.GetComponent<Text>().text;
            int val = int.Parse(strVal);
            bool flag = hashMap[val];
            if (flag == true)
            {
                hashMap[val] = false;
                RectTransform rectTransform2 = obj.GetComponent<RectTransform>();
                rectTransform2.localPosition = new Vector3(rectTransform2.localPosition.x,
                        rectTransform2.localPosition.y - 10,
                        rectTransform2.localPosition.z);
            }
        }
    }

    // 用户每张牌有一个点击事件，这里处理用户点击牌后，牌往上移，再次点击 则往下移
    private void onClickButtonHandler(GameObject obj)
    {
        //Debug.Log("你点击了一张牌");
        string strVal = obj.transform.FindChild("Text").gameObject.GetComponent<Text>().text;
        int val = int.Parse(strVal);
        bool flag = hashMap[val];

        GameObject t3 = obj.transform.FindChild("Image").gameObject;
        RectTransform rectTransform2 = t3.GetComponent<RectTransform>();
        if (flag == false)
        {
            rectTransform2.localPosition = new Vector3(rectTransform2.localPosition.x,
                rectTransform2.localPosition.y + 10,
                rectTransform2.localPosition.z);
        }
        else
        {
            rectTransform2.localPosition = new Vector3(rectTransform2.localPosition.x,
               rectTransform2.localPosition.y - 10,
               rectTransform2.localPosition.z);
        }
        hashMap[val] = !flag;
    }

    // 判断游戏是否开始，并给出弹框提示
    bool isGameStart()
    {
        if(GameState != 0) // 未开始， 弹出提示框
        {
            GameObject newTip = Instantiate(tipPrefab);
            newTip.transform.FindChild("Text").GetComponent<Text>().text = "游戏还未开始";
            newTip.transform.parent = mycanvas.transform;
            newTip.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            Destroy(newTip, 2.0f);

            return false;
        }
        return true;
    }

    void Update()
    {
        // 开始按钮的图片显示
        if (btnStart_Flag)
        {
            btn_start.GetComponent<Image>().sprite =
               Resources.Load("NewUI/btnStartNegative", typeof(Sprite)) as Sprite;
        }else
        {
            btn_start.GetComponent<Image>().sprite =
              Resources.Load("NewUI/准备", typeof(Sprite)) as Sprite;
        }

        isReadyCnt.text = isReadyUidText;

        lastIndex.text = lastIndexTextContent;

        // 游戏结束时的弹框提醒
        if (GameState == 1)
        {
            foreach (Transform child in Scroll_View_cards_lastOut.transform.FindChild("Viewport").FindChild("Content").transform)
            {
                //child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }

            deleteCards.Clear();

            foreach (Transform child in Scroll_View_cards.transform.FindChild("Viewport").FindChild("Content").transform)
                Destroy(child.gameObject);

            GameObject newTip = Instantiate(tipPrefab);
            newTip.transform.FindChild("Text").GetComponent<Text>().text = "Game Over！";
            newTip.transform.parent = mycanvas.transform;
            newTip.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            Destroy(newTip, 2.0f);

            GameState = -1;
            //return;
        }

        // 某个用户出的牌 显示出来
        if (deleteCards != null)
        {
            if (deleteCards.Count >= 1)
            {
                foreach (Transform child in Scroll_View_cards_lastOut.transform.FindChild("Viewport").FindChild("Content").transform)
                    if (child != null)
                    {
                        Destroy(child.gameObject);
                    }

                foreach (string val in deleteCards)
                {
                    GameObject t2 = (GameObject)Instantiate(msgPrefab);
                    t2.transform.FindChild("Text").gameObject.GetComponent<Text>().text = val;
                    t2.transform.FindChild("Image").gameObject.GetComponent<Image>().GetComponent<Image>().overrideSprite =
                        Resources.Load("puke_prefabs/" + val.ToString(), typeof(Sprite)) as Sprite;
                    t2.transform.SetParent(Scroll_View_cards_lastOut.transform.FindChild("Viewport").FindChild("Content").transform);

                    // 
                    foreach (Transform child in Scroll_View_cards.transform.FindChild("Viewport").FindChild("Content").transform)
                    {
                        if (child.gameObject.name == val)
                        {
                            Destroy(child.gameObject);
                            if (userCardNumberGO.Contains(child.gameObject))
                                userCardNumberGO.Remove(child.gameObject);
                        }
                    }
                }
                // deleteCards.Clear();
            }
        }

        // 3个用户都准备好后，发牌的GameObject new出来
        if (isFapai)
        {
            isFapai = false;

            foreach (Transform child in Scroll_View_cards.transform.FindChild("Viewport").FindChild("Content").transform)
            {
                Destroy(child.gameObject);
            }
            if (userCardNumberGO == null)
                userCardNumberGO = new ArrayList();
            else
                userCardNumberGO.Clear();
            foreach (int val in userCardNumber)
            {
                GameObject t2 = (GameObject)Instantiate(msgPrefab);
                t2.name = val.ToString();
                t2.transform.FindChild("Text").gameObject.GetComponent<Text>().text = val.ToString();
                //Texture2D _tex = (Texture2D)Resources.Load("puke_prefabs/" + val.ToString());
                t2.transform.FindChild("Image").gameObject.GetComponent<Image>().GetComponent<Image>().overrideSprite =
                    Resources.Load("puke_prefabs/" + val.ToString(), typeof(Sprite)) as Sprite;
                //t2.transform.parent = Scroll_View_cards.transform.FindChild("Viewport").FindChild("Content");
                // t2.transform.SetParent(Scroll_View_cards.transform.FindChild("Viewport").FindChild("Content").transform);
                if (!userCardNumberGO.Contains(t2))
                {
                    userCardNumberGO.Add(t2);
                    EventTriggerListener.Get(t2).onClick = onClickButtonHandler;
                }

                RectTransform rectTransform2 = t2.GetComponent<RectTransform>();
                string strVal = t2.transform.FindChild("Text").gameObject.GetComponent<Text>().text;
                bool flag = false;
                if (hashMap.ContainsKey(int.Parse(strVal)))
                    flag = hashMap[int.Parse(strVal)];
                if (flag == false)
                {
                    rectTransform2.localPosition = new Vector3(rectTransform2.localPosition.x,
                        rectTransform2.localPosition.y + 10,
                        rectTransform2.localPosition.z);
                }
                else
                {
                    rectTransform2.localPosition = new Vector3(rectTransform2.localPosition.x,
                       rectTransform2.localPosition.y - 10,
                       rectTransform2.localPosition.z);
                }
            }
            if (userCardNumberGO != null && userCardNumberGO.Count >= 1)
                foreach (GameObject go in userCardNumberGO)
                {
                    go.transform.SetParent(Scroll_View_cards.transform.FindChild("Viewport").FindChild("Content").transform);
                }
        }

        // 界面上，其他用户的显示 ，其他用户剩余牌数量的显示
        if (userList != null && userList.Count >= 1)
        {
            int userlistLen = userList.Count;
            if (userlistLen == 1)
            {
                text_me.text = userName;
            }
            else if (userlistLen == 2)
            {
                if (index == 1)
                {
                    text_me.text = mapIndex2[1];
                    text_ri.text = mapIndex2[2];

                    
                }
                else if (index == 2)
                {
                    text_me.text = mapIndex2[2];
                    text_le.text = mapIndex2[1];
                }
            }
            else if (userlistLen == 3)
            {
                if (index == 1)
                {
                    text_me.text = mapIndex2[1];
                    text_ri.text = mapIndex2[2];
                    text_le.text = mapIndex2[3];

                    // 牌数量
                    //Text_RiCnt.text = index_CardsCnt[2].ToString();
                    //Text_LeCnt.text = index_CardsCnt[3].ToString();

                    if (index_CardsCnt != null && index_CardsCnt.Count >= 3)
                    {
                        Text_RiCnt.text = index_CardsCnt[2].ToString();
                        Text_LeCnt.text = index_CardsCnt[3].ToString();
                    }
                }
                else if (index == 2)
                {
                    text_me.text = mapIndex2[2];
                    text_ri.text = mapIndex2[3];
                    text_le.text = mapIndex2[1];

                    // 牌数量
                    //Text_RiCnt.text = index_CardsCnt[3].ToString();
                    //Text_LeCnt.text = index_CardsCnt[1].ToString();

                    if (index_CardsCnt != null && index_CardsCnt.Count >= 3)
                    {
                        Text_RiCnt.text = index_CardsCnt[3].ToString();
                        Text_LeCnt.text = index_CardsCnt[1].ToString();
                    }
                }
                else if (index == 3)
                {
                    text_me.text = mapIndex2[3];
                    text_ri.text = mapIndex2[1];
                    text_le.text = mapIndex2[2];

                    // 牌数量
                    //Text_RiCnt.text = index_CardsCnt[1].ToString();
                    //Text_LeCnt.text = index_CardsCnt[2].ToString();

                    if (index_CardsCnt != null && index_CardsCnt.Count >= 3)
                    {
                        Text_RiCnt.text = index_CardsCnt[1].ToString();
                        Text_LeCnt.text = index_CardsCnt[2].ToString();
                    }
                }
            }
        } // end userList
    }
    
}