using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoolTimeSlot : MonoBehaviour
{
    private Image progressImage;
    private TextMeshProUGUI coolTimeText;
    GameObject selected;

    private void Awake()
    {
        progressImage = transform.GetChild(1).GetComponent<Image>();
        coolTimeText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        selected = transform.GetChild(3).gameObject;
    }

    public void RefreshUI(float current, float max)
    {
        if(current < 0)
        {
            current = 0;
        }
        coolTimeText.text = $"{current:f1}";
        progressImage.fillAmount = current / max;
    }

    public void SetSelected(bool show)
    {
        selected.SetActive(show);
    }

    public void SetDurationMode(bool duration)
    {
        if (duration)
        {
            progressImage.color = new Color(0,1,1,103.0f);
        }
        else
        {
            //progressImage.color = new Color(0.9339623f, 0.8883526f, 0.2070577f);
            progressImage.color = new Color(255,0,0,103.0f);
        }
    }
}
