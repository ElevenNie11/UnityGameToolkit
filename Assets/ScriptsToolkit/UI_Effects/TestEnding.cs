using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//当点击结束按钮以后就会触发片尾动画
public class TestEnding : MonoBehaviour
{
    public EndingCutscene_ver1 ending;
    public void OnclickTestButton()
    {
        ending.PlayEnding();
    }
}
