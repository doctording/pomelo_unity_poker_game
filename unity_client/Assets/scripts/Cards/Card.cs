using UnityEngine;
using System.Collections;

public class Card {

    private int val;
    private int weight;

    public Card(int _val)
    {
        val = _val;
        
        if(val == 501 || val == 502)
        {
            weight = val;
        }else
        {
            int we = _val % 100;
            if(we == 1 || we == 2)
            {
                weight = 13 + we;
            }else
            {
                weight = we;
            }
        }  
    }

    // 返回数值
    public int GetIntVal {
           get { return val; }
    }

    // 返回权值
    public int GetCardWeight
    {
        get { return weight; }
    }
}
