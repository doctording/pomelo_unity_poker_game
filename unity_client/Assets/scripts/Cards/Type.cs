using UnityEngine;
using System.Collections;

/// <summary>
/// 出牌类型
/// </summary>
public enum CardsType
{
    //未知类型
    None,
    //王炸
    JokerBoom,
    //炸弹
    Boom,
    //三个不带
    OnlyThree,
    //三个带一
    ThreeAndOne,
    //三个带二
    ThreeAndTwo,
    //顺子 五张或更多的连续单牌
    Straight,
    //双顺 三对或更多的连续对牌
    DoubleStraight,
    //三顺 二个或更多的连续三张牌
    TripleStraight,
    //对子
    Double,
    //单个
    Single
}