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
    TextMeshProUGUI moneyNotEnough;
    Shell shell;
    PlayerTank player;

    public Action<float> onMoneyChange { get; set; }
    public Action<float> onDamageChange { get; set; }
    public Action<float> onDefenceChange { get; set; }

    private void Awake()
    {

        attackDamage = transform.Find("ShellDamage").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        defenceValue = transform.Find("DefenceValue").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        moneyNotEnough = transform.Find("MoneyEmpty").GetComponent<TextMeshProUGUI>();
        //shell = GameManager.Instance.Shell.GetComponent<Shell>();
    }

    private void Start()
    {       
        player = GameManager.Instance.Player.GetComponent<PlayerTank>();
        shell = GameManager.Instance.Shell.GetComponent<Shell>();
        moneyNotEnough.color = new Color(moneyNotEnough.color.r, moneyNotEnough.color.g, moneyNotEnough.color.b, 0);
    }

    private void Update()
    {
        //attackDamage.text = $"Shell Damge : {shell.Data.damage}";
        ShellDamageTextChange();
        DefenceValueTextChange();
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
            attackDamage.text = $"Shell Damge : {defence}";
        };
    }

    public void MoneyNotEnough()
    {
        StartCoroutine(FadeTextToFullAlpha());
    }

    public IEnumerator FadeTextToFullAlpha() // ���İ� 0���� 1�� ��ȯ
    {
        moneyNotEnough.color = new Color(moneyNotEnough.color.r, moneyNotEnough.color.g, moneyNotEnough.color.b, 0);
        while (moneyNotEnough.color.a < 1.0f)
        {
            moneyNotEnough.color = new Color(moneyNotEnough.color.r, moneyNotEnough.color.g, moneyNotEnough.color.b, moneyNotEnough.color.a + (Time.deltaTime / 2.0f));
            yield return null;
        }
        StartCoroutine(FadeTextToZero());
    }

    public IEnumerator FadeTextToZero()  // ���İ� 1���� 0���� ��ȯ
    {
        moneyNotEnough.color = new Color(moneyNotEnough.color.r, moneyNotEnough.color.g, moneyNotEnough.color.b, 1);
        while (moneyNotEnough.color.a > 0.0f)
        {
            moneyNotEnough.color = new Color(moneyNotEnough.color.r, moneyNotEnough.color.g, moneyNotEnough.color.b, moneyNotEnough.color.a - (Time.deltaTime / 2.0f));
            yield return null;
        }
    }
}
