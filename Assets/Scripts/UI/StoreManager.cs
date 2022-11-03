using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;

public class StoreManager : MonoBehaviour
{
    public Button attackUpgradeButton;
    public Button closeButton;
    public TextMeshProUGUI moneyNotEnough;
    public TextMeshProUGUI moneyText;

    int attackUpgradePrice = 1;
    int attackUpgradeDamage = 10;



    PlayerTank player;
    Shell shell;

    public Action<float> onMoneyChange { get => ((IMoney)player).onMoneyChange; set => ((IMoney)player).onMoneyChange = value; }

    private void Start()
    {
        attackUpgradeButton = GetComponent<Button>();      
        player = GameManager.Instance.Player.GetComponent<PlayerTank>();
        shell = GameManager.Instance.Shell.GetComponent<Shell>();
        moneyNotEnough.color = new Color(moneyNotEnough.color.r, moneyNotEnough.color.g, moneyNotEnough.color.b, 0);
        moneyText.text = $"{player.money}";
        onMoneyChange += (money) =>
        {
            moneyText.text = $"{money}";
        };
    }
    public void OnClickAttackUpgrade()
    {
        if(player.money != 0)
        {
            player.Money -= attackUpgradePrice;
            player.shellDamage += attackUpgradeDamage;
        }
        else
        {
            Debug.Log("성공");
            StartCoroutine(FadeTextToFullAlpha());
        }

        
        Debug.Log(shell.Data.damage);
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
        public void OnClickClose()
        {
            this.gameObject.SetActive(false);
        }
}
