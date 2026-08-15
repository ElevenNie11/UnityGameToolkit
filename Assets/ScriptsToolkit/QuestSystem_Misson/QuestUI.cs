using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestUI : MonoBehaviour
{
    [Header("图标icon")]
    public GameObject closedBook;
    public GameObject openBook;
    [Header("任务面板")]
    public GameObject questPanel;
    public TextMeshProUGUI questText;

    private bool isPanelOpen = false;

    void Start()
    {
        //绑定任务图标点击事件
        GetComponent<Button>().onClick.AddListener(OnQuestIconClicked);
        closedBook.SetActive(true);
        openBook.SetActive(false);
        questPanel.SetActive(false);
    }

    //点击任务图标：打开/关闭面板
    private void OnQuestIconClicked()
    {
        if (isPanelOpen)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
    }

    //打开面板
    public void OpenPanel()
    {
        //如果当前任务已完成，就推进到下一个任务
        if(QuestManager.Instance.questCompleted)
        {
            QuestManager.Instance.AdvanceToNextQuest();
        }
        questText.text = QuestManager.Instance.GetCurrentQuest(); //更新任务文本

        closedBook.SetActive(false);
        openBook.SetActive(true);
        questPanel.SetActive(true);
        isPanelOpen = true;
    }

    //关闭面板
    public void ClosePanel()
    {
        closedBook.SetActive(true);
        openBook.SetActive(false);
        questPanel.SetActive(false);
        isPanelOpen = false;
    }
}
