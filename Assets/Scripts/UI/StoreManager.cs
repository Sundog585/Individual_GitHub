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
    public TextMeshProUGUI moneyText;

    int attackUpgradePrice = 1;
    int defenceUpgradePrice = 1;
    int attackUpgradeDamage = 1;



    PlayerTank player;
    PlayerUI playerUI;
    Shell shell;
    RectTransform rectTransform;
    

    public Action<float> onMoneyChange { get => ((IMoney)player).onMoneyChange; set => ((IMoney)player).onMoneyChange = value; }

    private void Start()
    {
        attackUpgradeButton = GetComponent<Button>();
        playerUI = GameManager.Instance.PlayerUI.GetComponent<PlayerUI>();
        player = GameManager.Instance.Player.GetComponent<PlayerTank>();
        shell = GameManager.Instance.Shell.GetComponent<Shell>();
        rectTransform = transform.GetChild(0).GetComponent<RectTransform>();

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
            playerUI.MoneyNotEnough();
        }

        
        Debug.Log(shell.Data.damage);
    }

    public void OnClickDefenceUpgrade()
    {
        if (player.money != 0)
        {
            player.Money -= attackUpgradePrice;
            player.defencePower += defenceUpgradePrice;
            // 강화 금액 점점 증가하게 할지 고민해보기
        }
        else
        {
            playerUI.MoneyNotEnough();
        }


        Debug.Log(shell.Data.damage);
    }

    public void OnClickClose()
    {
        //this.gameObject.SetActive(false);
        rectTransform.anchoredPosition = new(0,900);
    }

    public void OpenStore()
    {
        //this.gameObject.SetActive(true);
        rectTransform.anchoredPosition = new(0, 0);
    }
}
