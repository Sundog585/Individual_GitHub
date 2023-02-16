using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour,IMoney
{
    TextMeshProUGUI attackDamage;
    TextMeshProUGUI defenceValue;
    TextMeshProUGUI speedValue;
    TextMeshProUGUI moneyNotEnough;
    Shell shell;
    PlayerTank player;

    public Action<float> onMoneyChange { get; set; }
    public Action<float> onDamageChange { get => player.onDamageChange; set => player.onDamageChange = value; }
    public Action<float> onDefenceChange { get => player.onDefenceChange; set => player.onDefenceChange = value; }
    public Action<float> onSpeedChange { get => player.onSpeedChange; set => player.onSpeedChange = value; }
    //public Action<float> onDamageChange { get => ((IMoney)player).onDamageChange; set => ((IMoney)player).onDamageChange = value; }

    private void Awake()
    {

        attackDamage = transform.Find("ShellDamage").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        defenceValue = transform.Find("DefenceValue").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        speedValue = transform.Find("SpeedValue").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        moneyNotEnough = transform.Find("MoneyEmpty").GetComponent<TextMeshProUGUI>();
        //shell = GameManager.Instance.Shell.GetComponent<Shell>();
    }

    private void Start()
    {       
        player = GameManager.Instance.Player.GetComponent<PlayerTank>();
        shell = GameManager.Instance.Shell.GetComponent<Shell>();
        moneyNotEnough.color = new Color(moneyNotEnough.color.r, moneyNotEnough.color.g, moneyNotEnough.color.b, 0);
        ShellDamageTextChange();
        DefenceValueTextChange();
        SpeedValueTextChange();
    }

    void ShellDamageTextChange()
    {
        attackDamage.text = $"Shell Damge : {player.shellDamage}";
        onDamageChange += (damage) =>
        {
            attackDamage.text = $"Shell Damge : {damage}";
        };
    }

    void DefenceValueTextChange()
    {
        defenceValue.text = $"Defence Value : {player.defencePower}";
        onDefenceChange += (defence) =>
        {
            defenceValue.text = $"Defence Value : {defence}";
        };
    }

    void SpeedValueTextChange()
    {
        speedValue.text = $"Speed Value : {player.moveSpeed}";
        onSpeedChange += (speed) =>
        {
            speedValue.text = $"Speed Value : {speed}";
        };
    }

    public void MoneyNotEnough()
    {
        StartCoroutine(FadeTextToFullAlpha());
    }

    public IEnumerator FadeTextToFullAlpha() // 알파값 0에서 1로 전환
    {
        moneyNotEnough.color = new Color(moneyNotEnough.color.r, moneyNotEnough.color.g, moneyNotEnough.color.b, 0);
        while (moneyNotEnough.color.a < 1.0f)
        {
            moneyNotEnough.color = new Color(moneyNotEnough.color.r, moneyNotEnough.color.g, moneyNotEnough.color.b, moneyNotEnough.color.a + (Time.deltaTime / 2.0f));
            yield return null;
        }
        StartCoroutine(FadeTextToZero());
    }

    public IEnumerator FadeTextToZero()  // 알파값 1에서 0으로 전환
    {
        moneyNotEnough.color = new Color(moneyNotEnough.color.r, moneyNotEnough.color.g, moneyNotEnough.color.b, 1);
        while (moneyNotEnough.color.a > 0.0f)
        {
            moneyNotEnough.color = new Color(moneyNotEnough.color.r, moneyNotEnough.color.g, moneyNotEnough.color.b, moneyNotEnough.color.a - (Time.deltaTime / 2.0f));
            yield return null;
        }
    }
}
