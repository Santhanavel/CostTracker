using TMPro;
using UnityEngine;

public class SummaryItem : MonoBehaviour
{
    [SerializeField] private TMP_Text headingText;
    [SerializeField] private TMP_Text valueText;

    public void SetData(string heading, string value)
    {
        headingText.text = heading;
        valueText.text = value;
    }
}