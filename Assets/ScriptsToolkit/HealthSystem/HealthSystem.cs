using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HealthSystem : MonoBehaviour
{
    public static HealthSystem Instance;
    [Header("血量数据")]
    public int maxHealth = 100;
    public int currentHealth = 10;

    [Header("血量UI Slider引用")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;   //显示百分比

    void Awake()
    {
        Instance = this;
    }
     void Start()
    {
        UpdateHealthBar();
    }

    //更新血量UI
    public void UpdateHealthBar()
    {
       float percent = (float)currentHealth / maxHealth;
       healthSlider.value = percent * 100;
       if(healthText != null)
       {
           //显示百分比：RoudToInt表示四舍五入取整
           healthText.text = Mathf.RoundToInt(percent * 100) + "%";
       }
    }

    //1. 回血:percent 百分比（传10就是+10%）
    public void Heal(int percent)
    {
        int healAmount = Mathf.RoundToInt(maxHealth * percent / 100f);
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        UpdateHealthBar();
    }
    //2. 掉血
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        UpdateHealthBar();
    }
}
