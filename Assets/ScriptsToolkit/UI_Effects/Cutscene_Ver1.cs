using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;     //引入命名空间：PlayableDirector属于Timeline/Playables系统

public class Cutscene_ver1 : MonoBehaviour
{   
    [Header("拖引用过来")]
    public PlayableDirector director;  //拖带PlayableDirector的对象（也就是IntroTimeline对象）
    public GameObject Canvas_Cutscene;
    private void Awake()
    {
        if (director != null)       //防止空引用报错
        {
            // += 表示[添加监听]
            director.stopped += OnTimelineStopped;  //当Timeline停止播放时，调用OnTimelineStopped函数
        }
    }
    private void OnDestroy()        //OnDestory是Unity内置的函数
    {
        if(director != null)
        {
            // -= 表示[移除监听]
            director.stopped -= OnTimelineStopped;  //当这个脚本对象被销毁时，取消事件绑定
        }     
    }
    private void OnTimelineStopped(PlayableDirector obj)
    {
        if(Canvas_Cutscene != null)
        {
            Canvas_Cutscene.SetActive(false);
        }
    }
}

//PlayableDirector 是 Unity Timeline 的控制核心。它负责：
//1. 播放Timeline
//2. 暂停Timeline
//3. 停止Timeline
//4. 监听Timeline状态