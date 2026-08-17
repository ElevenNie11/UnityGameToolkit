using UnityEngine;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;
    private string savePath;
    void Awake()
    {
        Instance = this;
        savePath = Application.persistentDataPath + "/savefile.json";
    }

    void Start()
    {
        StartCoroutine(LoadNextFrame());
    }

    System.Collections.IEnumerator LoadNextFrame()
    {
        yield return null;    //等一帧，让所有系统Awake完
        Load();               //游戏启动时自动读档
    }

    //OnApplicationQuit函数：打包后在正式游戏里退出是会触发的，但编辑器里不稳定
    void OnAlicationQuit()
    {
        Save();
    }

    //保存游戏
    public void Save()
    {
        SaveData data = new SaveData();
        //从各个系统读取当前数据
        data.currentHealth = HealthSystem.Instance.currentHealth;
        data.coins = ShopManager.Instance.coins;
        data.questIndex = QuestManager.Instance.currentQuestIndex;
        data.questCompleted = QuestManager.Instance.questCompleted;

        //转成JSON并写入文件
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log("游戏已保存到：" + savePath);
    }

    //读取存档
    public void Load()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("没有找到存档文件");
            return;
        }

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        //恢复数据到各个系统
        HealthSystem.Instance.currentHealth = data.currentHealth;
        HealthSystem.Instance.UpdateHealthBar();   //刷新血条显示

        ShopManager.Instance.coins = data.coins;
        ShopUI.Instance.UpdateCoinsDisplay();      //刷新金币显示

        QuestManager.Instance.currentQuestIndex = data.questIndex;
        QuestManager.Instance.questCompleted = data.questCompleted;

        Debug.Log("存档已加载完毕");
    }

    //重置游戏
    public void ResetGame()
    {
        //重置数据到初始状态
        HealthSystem.Instance.currentHealth = 5;
        ShopManager.Instance.coins = 100;
        QuestManager.Instance.currentQuestIndex = 0;
        QuestManager.Instance.questCompleted = false;
        //立刻刷新UI显示
        HealthSystem.Instance.UpdateHealthBar();
        ShopUI.Instance.UpdateCoinsDisplay();
        //删除存档
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        Debug.Log("所有数据已重置");
    }
}
