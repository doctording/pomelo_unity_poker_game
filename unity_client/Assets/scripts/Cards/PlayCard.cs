using UnityEngine;
using System.Collections;

public class PlayCard
{
    //card1：上一轮的牌；card2：这一轮的牌

    //上一轮为单支的出牌逻辑
    public static bool isSingleCompare(Card[] card1, Card[] card2, CardsType type1, CardsType type2)
    {
        //CardsType type1;
        //CardsRules.PopEnable(card1, out type1);

        //CardsType type2;
        //CardsRules.PopEnable(card2, out type2);

        if (type1 == CardsType.Single)
        {
            if (type2 == CardsType.Single)
            {
                if (card2[0].GetCardWeight > card1[0].GetCardWeight)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (type2 == CardsType.Boom || type2 == CardsType.JokerBoom)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    //上一轮为对子的出牌逻辑
    public static bool isDoubleCompare(Card[] card1, Card[] card2, CardsType type1, CardsType type2)
    {
        if (type1 == CardsType.Double)
        {
            if (type2 == CardsType.Double)
            {
                if (card2[0].GetCardWeight > card1[0].GetCardWeight)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (type2 == CardsType.Boom || type2 == CardsType.JokerBoom)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    //上一轮为三不带的出牌逻辑
    public static bool isOnlyThreeCompare(Card[] card1, Card[] card2, CardsType type1, CardsType type2)
    {
        if (type1 == CardsType.OnlyThree)
        {
            if (type2 == CardsType.OnlyThree)
            {
                if (card2[0].GetCardWeight > card1[0].GetCardWeight)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (type2 == CardsType.Boom || type2 == CardsType.JokerBoom)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    //上一轮为三带一的出牌逻辑
    public static bool isThreeAndOneCompare(Card[] card1, Card[] card2, CardsType type1, CardsType type2)
    {
        if (type1 == CardsType.ThreeAndOne)
        {
            if (type2 == CardsType.ThreeAndOne)
            {
                bool isThreeFirstCard1;//判断三个连续数是不是靠前
                bool isThreeFirstCard2;
                if (card1[0].GetCardWeight == card1[1].GetCardWeight)
                {
                    isThreeFirstCard1 = true;
                }
                else
                {
                    isThreeFirstCard1 = false;
                }

                if (card2[0].GetCardWeight == card2[1].GetCardWeight)
                {
                    isThreeFirstCard2 = true;
                }
                else
                {
                    isThreeFirstCard2 = false;
                }

                if (isThreeFirstCard2)
                {
                    if (isThreeFirstCard1)
                    {
                        if (card2[0].GetCardWeight > card1[0].GetCardWeight)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (card2[0].GetCardWeight > card1[3].GetCardWeight)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (isThreeFirstCard1)
                    {
                        if (card2[3].GetCardWeight > card1[0].GetCardWeight)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (card2[3].GetCardWeight > card1[3].GetCardWeight)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            else if (type2 == CardsType.Boom || type2 == CardsType.JokerBoom)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    //上一轮为三带二的逻辑
    public static bool isThreeAndTwoCompare(Card[] card1, Card[] card2, CardsType type1, CardsType type2)
    {
        if (type1 == CardsType.ThreeAndTwo)
        {
            if (type2 == CardsType.ThreeAndTwo)
            {
                bool isThreeFirstCard1;//判断三个连续数是不是靠前
                bool isThreeFirstCard2;
                if (card1[0].GetCardWeight == card1[2].GetCardWeight)
                {
                    isThreeFirstCard1 = true;
                }
                else
                {
                    isThreeFirstCard1 = false;
                }

                if (card2[0].GetCardWeight == card2[2].GetCardWeight)
                {
                    isThreeFirstCard2 = true;
                }
                else
                {
                    isThreeFirstCard2 = false;
                }

                if (isThreeFirstCard2)
                {
                    if (isThreeFirstCard1)
                    {
                        if (card2[0].GetCardWeight > card1[0].GetCardWeight)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (card2[0].GetCardWeight > card1[4].GetCardWeight)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (isThreeFirstCard1)
                    {
                        if (card2[4].GetCardWeight > card1[0].GetCardWeight)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (card2[4].GetCardWeight > card1[4].GetCardWeight)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            else if (type2 == CardsType.Boom || type2 == CardsType.JokerBoom)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    //上一轮为顺子的出牌逻辑
    public static bool isStraightCompare(Card[] card1, Card[] card2, CardsType type1, CardsType type2)
    {
        if (type1 == CardsType.Straight)
        {
            if (type2 == CardsType.Straight)
            {
                //如果两者都是顺子，但是牌的数量不同
                if (card1.Length != card2.Length)
                {
                    return false;
                }
                else
                {
                    // isReasonable = true;
                    if (card2[0].GetCardWeight > card1[0].GetCardWeight)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            else if (type2 == CardsType.Boom || type2 == CardsType.JokerBoom)
            {
                // isReasonable = true;
                return true;
            }
            else
            {
                // isReasonable = true;
                return false;
            }
        }
        else
        {
            //isReasonable = true;
            return false;
        }
    }

    //上一轮为对子顺子的出牌逻辑
    public static bool isDoubleStraightCompare(Card[] card1, Card[] card2, CardsType type1, CardsType type2)
    {
        if (type1 == CardsType.DoubleStraight)
        {
            if (type2 == CardsType.DoubleStraight)
            {
                //如果两者都是对子顺子，但是牌的数量不同
                if (card1.Length != card2.Length)
                {
                    //出牌不合理                    
                    return false;
                }
                else
                {

                    if (card2[0].GetCardWeight > card1[0].GetCardWeight)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            else if (type2 == CardsType.Boom || type2 == CardsType.JokerBoom)
            {

                return true;
            }
            else
            {

                return false;
            }
        }
        else
        {
            return false;
        }
    }

    //上一轮三顺子的出牌逻辑
    public static bool isTripleStraightCompare(Card[] card1, Card[] card2, CardsType type1, CardsType type2)
    {
        if (type1 == CardsType.TripleStraight)
        {
            if (type2 == CardsType.TripleStraight)
            {
                //如果两者都是对子顺子，但是牌的数量不同
                if (card1.Length != card2.Length)
                {
                    //出牌不合理                    
                    return false;
                }
                else
                {
                    if (card2[0].GetCardWeight > card1[0].GetCardWeight)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            else if (type2 == CardsType.Boom || type2 == CardsType.JokerBoom)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    // 上一轮为炸弹的出牌逻辑
    public static bool isBoomCompare(Card[] card1, Card[] card2, CardsType type1, CardsType type2)
    {
        if (type1 == CardsType.Boom)
        {
            if (type2 == CardsType.Boom)
            {
                if (card2[0].GetCardWeight > card1[0].GetCardWeight)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (type2 == CardsType.JokerBoom)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    // 上一轮为王炸的出牌逻辑
    public static bool isJokerBoomCompare(Card[] card1, Card[] card2, CardsType type1, CardsType type2)
    {
        return false;
    }

    /**
     * 整个出牌判断
     */ 
    public static bool isCanOutCard(Card[] card1, Card[] card2, CardsType type1, CardsType type2)
    {
        if (isSingleCompare(card1, card2, type1, type2))
            return true;
        else if (isDoubleCompare(card1, card2, type1, type2))
            return true;
        else if (isOnlyThreeCompare(card1, card2, type1, type2))
            return true;
        else if (isThreeAndOneCompare(card1, card2, type1, type2))
            return true;
        else if (isStraightCompare(card1, card2, type1, type2))
            return true;
        else if (isDoubleStraightCompare(card1, card2, type1, type2))
            return true;
        else if (isTripleStraightCompare(card1, card2, type1, type2))
            return true;
        else if (isBoomCompare(card1, card2, type1, type2))
            return true;
        else if (isJokerBoomCompare(card1, card2, type1, type2))
            return true;
        else
            return false;
    }
}
