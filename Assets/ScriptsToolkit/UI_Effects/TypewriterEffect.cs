using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//一行数据的内容：文本格式为：说话人(speaker)|说话内容(content) 
public class DialogueLine
{
    public string speaker;
    public string content;
    public DialogueLine(string speaker, string content)
    {
        this.speaker = speaker;
        this.content = content;
    }
}

public class TypewriterEffect : MonoBehaviour
{
    [Header("文字TMPro")]
    public TMP_Text contentText;
    [Header("打字机效果的速度：数值越小越快")]
    public float typeSpeed = 0.03f;
    //协程 IEnumerator
    private IEnumerator TypeOneLine(DialogueLine line)
    {
        contentText.text = " ";          //先清空文本框
        foreach (char c in line.content)
        {
            contentText.text += c;
            yield return new WaitForSeconds(typeSpeed);   //等待一段时间再显示下一个字符
        }
    }
}

/*-------------*/
//TMP_Text 是 TextMeshPro 系统中的核心抽象文本组件，在Unity里左UI文字工具包的时候，应该优先围绕它设计展开
//TMP_Text 的关键API：
//1. [设置文字]: contentText.text = "你好";
//2. [获取文字]：string value = contentText.text;
//3. [清空文字]：contentText.text = "";