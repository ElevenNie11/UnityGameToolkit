using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldown : MonoBehaviour
{
    int manager = 1;           //manager是一个“管理员”：当manager值为1的时候代表：技能正在冷却中，玩家此时无法点击按钮
    [Header("UI组件")]
    public Image IMG;          //填充量
    public Button SkillButton; //技能按钮
    [Header("填充量的清空速度")]
    public float cleanSpeed = 0.5f;

    public void startCoolDown()
    {
        IMG.fillAmount = 1f;    //填充量
        manager = 1;
    }

    void Update()
    {
        if(manager == 1)
        {
            if(IMG.fillAmount > 0)
            {
                IMG.fillAmount -= cleanSpeed * Time.deltaTime;
            }
            else  //冷却结束（填充量变为0）：玩家又可以点击按钮 释放技能
            {
                SkillButton.enabled = true;
                manager = 0;              //避免在Update里持续执行
            }
        }
    }
}
