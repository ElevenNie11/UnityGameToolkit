using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("任务列表")]
    public List<string>quests = new List<string>
    {
        "亲爱的冒险者, 请将体力恢复到100%吧！",
        "接下来, 请前往里奥列斯先生的书店~"
        //以后继续往后加任务......
    };

    [Header("状态")]
    public int currentQuestIndex = 0;    //当前任务索引
    public bool questCompleted = false;  //当前任务是否完成

    void Awake()
    {
        Instance = this;
    }

    //获取当前任务文本
    public string GetCurrentQuest()
    {
        if(currentQuestIndex < quests.Count)
        {
            return quests[currentQuestIndex];
        }
        else
        {
            return "所有任务已完成！";
        }
    }

    //完成当前任务
    public void CompleteQuest()
    {
        if(questCompleted) return; //如果当前任务已经完成，则不做任何操作
        questCompleted = true;
        Debug.Log("任务完成: " + GetCurrentQuest());
    }

    //推进到下一个任务
    public void AdvanceToNextQuest()
    {
        if(!questCompleted) return; //如果当前任务未完成，则不推进到下一个任务
        currentQuestIndex++;
        questCompleted = false;     //重置任务完成状态

        if(currentQuestIndex >= quests.Count)
        {
            Debug.Log("所有任务已完成！");
        }
    }   
}
