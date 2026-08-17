//纯数据类
using UnityEngine;
[System.Serializable]
public class SaveData : MonoBehaviour
{
    public int currentHealth;  //血量
    public int coins;          //金币
    public int questIndex;     //任务索引
    public bool questCompleted;//任务是否完成
}
