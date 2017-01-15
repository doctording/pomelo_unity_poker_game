using UnityEngine;
using System.Collections;

public class RoomRecord { 
    public string rid { get; set; }
    public string num { get; set; }

    public RoomRecord()
    {

    }
    //Chat record
    public RoomRecord(string userName, string userDialog)
    {
        this.rid = userName;
        this.num = userDialog;
    }
}