//纯数据类：不能继承MonoBehaviour, 否则会Json反序列化报错
[System.Serializable]
public class SaveData
{
    public int currentHealth;  //血量
    public int coins;          //金币
    public int questIndex;     //任务索引
    public bool questCompleted;//任务是否完成
}
