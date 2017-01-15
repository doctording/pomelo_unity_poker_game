using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardsRules {
    
    public static void SortCards(Card[] cards,int len)
    {
        if (len <= 1)
            return;
        for(int i = 1 ; i < len; i++ )
        {
            Card tc = cards[i];
            int t = cards[i].GetCardWeight;
            int j = i;
            while ((j > 0) && (cards[j - 1].GetCardWeight > t))
            {
                cards[j] = cards[j - 1];
                --j;
            }
            cards[j] = tc;
        }
    }

    /// <summary>
    /// 是否是单
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsSingle(Card[] cards)
    {
        if (cards.Length == 1)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 是否是对子
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsDouble(Card[] cards)
    {
        if (cards.Length == 2)
        {
            if (cards[0].GetCardWeight == cards[1].GetCardWeight)
                return true;
        }

        return false;
    }

    /// <summary>
    /// 是否是顺子
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsStraight(Card[] cards)
    {
        if (cards.Length < 5 || cards.Length > 12)
            return false;
        for (int i = 0; i < cards.Length - 1; i++)
        {
            int w = cards[i].GetCardWeight;
            if (cards[i + 1].GetCardWeight - w != 1)
                return false;

            //不能超过A
            if (w > 14 || cards[i + 1].GetCardWeight > 14)
                return false;
        }

        return true;
    }

    /// <summary>
    /// 是否是双顺子
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsDoubleStraight(Card[] cards)
    {
        if (cards.Length < 6 || cards.Length % 2 != 0)
            return false;

        for (int i = 0; i < cards.Length; i += 2)
        {
            if (cards[i + 1].GetCardWeight != cards[i].GetCardWeight)
                return false;

            if (i < cards.Length - 2)
            {
                if (cards[i + 2].GetCardWeight - cards[i].GetCardWeight != 1)
                    return false;

                //不能超过A
                if (cards[i].GetCardWeight > 14 || cards[i + 2].GetCardWeight > 14)
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 飞机不带
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsTripleStraight(Card[] cards)
    {
        if (cards.Length < 6 || cards.Length % 3 != 0)
            return false;

        for (int i = 0; i < cards.Length; i += 3)
        {
            if (cards[i + 1].GetCardWeight != cards[i].GetCardWeight)
                return false;
            if (cards[i + 2].GetCardWeight != cards[i].GetCardWeight)
                return false;
            if (cards[i + 1].GetCardWeight != cards[i + 2].GetCardWeight)
                return false;

            if (i < cards.Length - 3)
            {
                if (cards[i + 3].GetCardWeight - cards[i].GetCardWeight != 1)
                    return false;

                //不能超过A
                if (cards[i].GetCardWeight > 14 || cards[i + 3].GetCardWeight > 14)
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 三不带
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsOnlyThree(Card[] cards)
    {
        if (cards.Length != 3) // 这里有修改 ？？
            return false;
        if (cards[0].GetCardWeight != cards[1].GetCardWeight)
            return false;
        if (cards[1].GetCardWeight != cards[2].GetCardWeight)
            return false;
        if (cards[0].GetCardWeight != cards[2].GetCardWeight)
            return false;

        return true;
    }


    /// <summary>
    /// 三带一
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsThreeAndOne(Card[] cards)
    {
        if (cards.Length != 4)
            return false;

        if (cards[0].GetCardWeight == cards[1].GetCardWeight &&
            cards[1].GetCardWeight == cards[2].GetCardWeight && cards[2] != cards[3]) // 这里有修改 ？？
            return true;
        else if (cards[1].GetCardWeight == cards[2].GetCardWeight &&
            cards[2].GetCardWeight == cards[3].GetCardWeight && cards[1] != cards[0])
            return true;
        return false;
    }

    /// <summary>
    /// 三代二
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsThreeAndTwo(Card[] cards)
    {
        if (cards.Length != 5)
            return false;

        if (cards[0].GetCardWeight == cards[1].GetCardWeight &&
            cards[1].GetCardWeight == cards[2].GetCardWeight)
        {
            if (cards[3].GetCardWeight == cards[4].GetCardWeight)
                return true;
        }

        else if (cards[2].GetCardWeight == cards[3].GetCardWeight &&
            cards[3].GetCardWeight == cards[4].GetCardWeight)
        {
            if (cards[0].GetCardWeight == cards[1].GetCardWeight)
                return true;
        }

        return false;
    }

    /// <summary>
    /// 炸弹
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsBoom(Card[] cards)
    {
        if (cards.Length != 4)
            return false;

        if (cards[0].GetCardWeight != cards[1].GetCardWeight)
            return false;
        if (cards[1].GetCardWeight != cards[2].GetCardWeight)
            return false;
        if (cards[2].GetCardWeight != cards[3].GetCardWeight)
            return false;

        return true;
    }

    /// <summary>
    /// 王炸
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static bool IsJokerBoom(Card[] cards)
    {
        if (cards.Length != 2)
            return false;
        if (cards[0].GetCardWeight == 501)
        {
            if (cards[1].GetCardWeight == 502)
                return true;
            return false;
        }
        else if (cards[0].GetCardWeight == 502)
        {
            if (cards[1].GetCardWeight == 501)
                return true;
            return false;
        }

        return false;
    }

    /// <summary>
    /// 判断是否符合出牌规则
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool PopEnable(Card[] cards, out CardsType type)
    {
        type = CardsType.None;
        bool isRule = false;
        switch (cards.Length)
        {
            case 1:
                isRule = true;
                type = CardsType.Single;
                break;
            case 2:
                if (IsDouble(cards))
                {
                    isRule = true;
                    type = CardsType.Double;
                }
                else if (IsJokerBoom(cards))
                {
                    isRule = true;
                    type = CardsType.JokerBoom;
                }
                break;
            case 3:
                if (IsOnlyThree(cards))
                {
                    isRule = true;
                    type = CardsType.OnlyThree;
                }
                break;
            case 4:
                if (IsBoom(cards))
                {
                    isRule = true;
                    type = CardsType.Boom;
                }
                else if (IsThreeAndOne(cards))
                {
                    isRule = true;
                    type = CardsType.ThreeAndOne;
                }

                break;
            case 5:
                if (IsStraight(cards))
                {
                    isRule = true;
                    type = CardsType.Straight;
                }
                else if (IsThreeAndTwo(cards))
                {
                    isRule = true;
                    type = CardsType.ThreeAndTwo;
                }
                break;
            case 6:
                if (IsStraight(cards))
                {
                    isRule = true;
                    type = CardsType.Straight;
                }
                else if (IsTripleStraight(cards))
                {
                    isRule = true;
                    type = CardsType.TripleStraight;
                }
                else if (IsDoubleStraight(cards))
                {
                    isRule = true;
                    type = CardsType.DoubleStraight;
                }
                break;
            case 7:
                if (IsStraight(cards))
                {
                    isRule = true;
                    type = CardsType.Straight;
                }
                break;
            case 8:
                if (IsStraight(cards))
                {
                    isRule = true;
                    type = CardsType.Straight;
                }
                else if (IsDoubleStraight(cards))
                {
                    isRule = true;
                    type = CardsType.DoubleStraight;
                }
                //else if (IsTripleStraightAndSingle(cards))
                //{
                //    isRule = true;
                //    type = CardsType.TripleStraightAndSingle;
                //}
                break;
            case 9:
                if (IsStraight(cards))
                {
                    isRule = true;
                    type = CardsType.Straight;
                }
                else if (IsOnlyThree(cards))
                {
                    isRule = true;
                    type = CardsType.OnlyThree;
                }
                break;
            case 10:
                if (IsStraight(cards))
                {
                    isRule = true;
                    type = CardsType.Straight;
                }
                else if (IsDoubleStraight(cards))
                {
                    isRule = true;
                    type = CardsType.DoubleStraight;
                }
                //else if (IsTripleStraightAndDouble(cards))
                //{
                //    isRule = true;
                //    type = CardsType.TripleStraightAndDouble;
                //}
                break;

            case 11:
                if (IsStraight(cards))
                {
                    isRule = true;
                    type = CardsType.Straight;
                }
                break;
            case 12:
                if (IsStraight(cards))
                {
                    isRule = true;
                    type = CardsType.Straight;
                }
                else if (IsDoubleStraight(cards))
                {
                    isRule = true;
                    type = CardsType.DoubleStraight;
                }
                //else if (IsTripleStraightAndSingle(cards))
                //{
                //    isRule = true;
                //    type = CardsType.TripleStraightAndSingle;
                //}
                else if (IsOnlyThree(cards))
                {
                    isRule = true;
                    type = CardsType.OnlyThree;
                }
                break;
            case 13:
                break;
            case 14:
                if (IsDoubleStraight(cards))
                {
                    isRule = true;
                    type = CardsType.DoubleStraight;
                }
                break;
            case 15:
                if (IsOnlyThree(cards))
                {
                    isRule = true;
                    type = CardsType.OnlyThree;
                }
                //else if (IsTripleStraightAndDouble(cards))
                //{
                //    isRule = true;
                //    type = CardsType.TripleStraightAndDouble;
                //}
                break;
            case 16:
                if (IsDoubleStraight(cards))
                {
                    isRule = true;
                    type = CardsType.DoubleStraight;
                }
                //else if (IsTripleStraightAndSingle(cards))
                //{
                //    isRule = true;
                //    type = CardsType.TripleStraightAndSingle;
                //}
                break;
            case 17:
                break;
            case 18:
                if (IsDoubleStraight(cards))
                {
                    isRule = true;
                    type = CardsType.DoubleStraight;
                }
                else if (IsOnlyThree(cards))
                {
                    isRule = true;
                    type = CardsType.OnlyThree;
                }
                break;
            case 19:
                break;

            case 20:
                if (IsDoubleStraight(cards))
                {
                    isRule = true;
                    type = CardsType.DoubleStraight;
                }
                break;
            default:
                break;
        }

        return isRule;
    }
}
