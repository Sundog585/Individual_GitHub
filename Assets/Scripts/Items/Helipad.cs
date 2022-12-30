using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helipad : MonoBehaviour
{
    public float healingHP = 1.0f;
    PlayerTank player;

    private void Start()
    {
        player = GameManager.Instance.Player.GetComponent<PlayerTank>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.HP += healingHP * Time.deltaTime;
            return;
        }
    }
}
