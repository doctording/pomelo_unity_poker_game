using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class Utils
{
    /**
     * 根据时间戳 得到 DateTime 对象
     *  
     */
    public static DateTime GetTime(string timeStamp)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(timeStamp + "0000000");
        TimeSpan toNow = new TimeSpan(lTime);
        return dtStart.Add(toNow);
    }

    /**  
     * 根据 DateTime 对象 得到 时间戳 
     * 
     */
    public static int ConvertDateTimeInt(System.DateTime time)
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        return (int)(time - startTime).TotalSeconds;
    }

    /**  
     * 获取当前的时间戳
     * 
     */
    public static string GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }

    /**  
     * 用户名加上时间戳构成用户的id
     * 
     */
    public static string getUidByName(string name)
    {
        return name + "_" + GetTimeStamp();
    }

    /**  
     * 根据用户的id 获取用户名字
     * 
     */
    public static string getNameByUid(string uid)
    {
        return uid.Substring(0, uid.LastIndexOf("_"));
    }
}