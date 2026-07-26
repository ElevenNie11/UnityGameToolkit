using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;      //音量滑动条涉及到UI组件
using UnityEngine.Audio;   //Audio-音量
public class VolumeController : MonoBehaviour
{
    [Header("UI组件")]
    public Slider volumeSlider; //在Inspector中拖入VolumeSlider，脚本通过它读写滑条的值
    private const string VOLUME_KEY = "MasterVolume";  ////PlayerPrefs键名:存档用的键名,用于PlayerPrefs的读写
    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 0.75f); //从本地存档读取上次音量，如果没有存档就默认0.75
        volumeSlider.value = savedVolume;                            //把滑条的位置同步到存档的值
        ApplyVolume(savedVolume);                                    //立即应用音量，确保游戏启动时音量正确
        volumeSlider.onValueChanged.AddListener(VolumeChanged);      //注册监听——滑条一变动就自动调用OnVolumeChanged
    }
    //函数1:滑条变动时触发:每次用户拖动滑条，Unity自动传入当前值value
    void VolumeChanged(float value)
    {
        ApplyVolume(value);
        PlayerPrefs.SetFloat(VOLUME_KEY, volumeSlider.value);       ////保存设置 下次启动游戏仍然生效
    }
    //函数2：实际应用音量
    void ApplyVolume(float value)
    {
        //AudioListener是Unity全局音频监听器
        AudioListener.volume = value;
    }
}
