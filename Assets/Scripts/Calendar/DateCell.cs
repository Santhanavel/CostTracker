using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class DateCell : MonoBehaviour
{
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private Image dateImage;

    public void SetState(Color color , string date)
    {
        dateImage.color = color;
        dateText.text = date;
        dateText.enabled = true;

    }
    public void SetEmpty(Color color)
    {
        dateImage.color = color;
        dateText.text = "";
        dateText.enabled = false;
    }
}
