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
    Shell shell;
    PlayerTank player;

    public Action<float> onMoneyChange { get; set; }
    public Action<float> onDamageChange { get; set; }
    public Action<float> onDefenceChange { get; set; }

    private void Awake()
    {

        attackDamage = transform.Find("ShellDamage").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        defenceValue = transform.Find("DefenceValue").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        //shell = GameManager.Instance.Shell.GetComponent<Shell>();
    }

    private void Start()
    {       
        player = GameManager.Instance.Player.GetComponent<PlayerTank>();
        shell = GameManager.Instance.Shell.GetComponent<Shell>();
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
}
