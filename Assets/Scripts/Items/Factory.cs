using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour
{
    StoreManager store;
    private void Start()
    {
        store = GameManager.Instance.Store.GetComponent<StoreManager>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            store.OpenStore();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        store.OnClickClose();
    }
}
