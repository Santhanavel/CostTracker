using TMPro;
using UnityEngine;

public class MonthlySummary : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text headingText;
    [SerializeField] private Transform contentParent;
    [SerializeField] private SummaryItem summaryPrefab;

    public void UpdateSummary(
        int year,
        int month,
        int lunch,
        int dinner,
        int meals,
        int cost)
    {
        headingText.text =
            $"{new System.DateTime(year, month, 1):MMMM} Summary";

        Clear();

        CreateItem("Lunch", lunch.ToString());
        CreateItem("Dinner", dinner.ToString());
        CreateItem("Meals", meals.ToString());
        CreateItem("Cost", $"₹{cost:N0}");
    }

    private void CreateItem(string heading, string value)
    {
        SummaryItem item =
            Instantiate(summaryPrefab, contentParent);

        item.SetData(heading, value);
    }

    private void Clear()
    {
        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(contentParent.GetChild(i).gameObject);
        }
    }
}