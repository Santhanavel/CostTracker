using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonthlySummary : MonoBehaviour
{
    [Header("UI Header")]
    [SerializeField] private TMP_Text headingText;

    [Header("Card Texts")]
    [SerializeField] private TMP_Text breakfastValueTxt;
    [SerializeField] private TMP_Text lunchValueTxt;
    [SerializeField] private TMP_Text dinnerValueTxt;
    [SerializeField] private TMP_Text mealsValueTxt;
    [SerializeField] private TMP_Text costValueTxt;

    private void Awake()
    {
        SelfWire();
    }

    public void SelfWire()
    {
        if (headingText == null) headingText = transform.Find("Heading Text")?.GetComponent<TMP_Text>();
        if (breakfastValueTxt == null) breakfastValueTxt = transform.Find("Grid/Breakfast Card/ValueText")?.GetComponent<TMP_Text>();
        if (lunchValueTxt == null) lunchValueTxt = transform.Find("Grid/Lunch Card/ValueText")?.GetComponent<TMP_Text>();
        if (dinnerValueTxt == null) dinnerValueTxt = transform.Find("Grid/Dinner Card/ValueText")?.GetComponent<TMP_Text>();
        if (mealsValueTxt == null) mealsValueTxt = transform.Find("Grid/Meals Card/ValueText")?.GetComponent<TMP_Text>();
        if (costValueTxt == null) costValueTxt = transform.Find("Grid/Cost Card/ValueText")?.GetComponent<TMP_Text>();
    }

    public void UpdateSummary(
        int year,
        int month,
        int breakfast,
        int lunch,
        int dinner,
        int meals,
        int cost)
    {
        SelfWire();

        if (headingText != null)
        {
            headingText.text = $"{new System.DateTime(year, month, 1):MMMM} Summary";
        }

        if (breakfastValueTxt != null) breakfastValueTxt.text = breakfast.ToString();
        if (lunchValueTxt != null) lunchValueTxt.text = lunch.ToString();
        if (dinnerValueTxt != null) dinnerValueTxt.text = dinner.ToString();
        if (mealsValueTxt != null) mealsValueTxt.text = meals.ToString();
        if (costValueTxt != null) costValueTxt.text = $"₹{cost:N0}";
    }
}