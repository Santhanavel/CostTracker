using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodTracker.UI
{
    public class DateCell : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text dateText;
        [SerializeField] private Image dateImage;
        [SerializeField] private Image borderImage; // Selected day green outline
        [SerializeField] private Button cellButton;

        [Header("Indicator Dots")]
        [SerializeField] private Transform dotsContainer;
        [SerializeField] private GameObject dotPrefab;

        private DateTime cellDate;
        private Action<DateTime> onClickCallback;

        private void Awake()
        {
            if (dateText == null) dateText = transform.Find("DateText")?.GetComponent<TMP_Text>();
            if (dateImage == null) dateImage = GetComponent<Image>();
            if (borderImage == null) borderImage = transform.Find("Border")?.GetComponent<Image>();
            if (cellButton == null) cellButton = GetComponent<Button>();
            if (dotsContainer == null) dotsContainer = transform.Find("DotsContainer");
            
            if (dotPrefab == null)
            {
                // Fallback: create dot template dynamically if not assigned
                GameObject dotObj = new GameObject("DotTemplate", typeof(RectTransform), typeof(CanvasRenderer));
                dotObj.transform.SetParent(transform, false);
                dotObj.SetActive(false);
                Image img = dotObj.AddComponent<Image>();
                img.color = Color.white;
                dotObj.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 10);
                dotPrefab = dotObj;
            }
        }

        private void Start()
        {
            if (cellButton != null)
            {
                cellButton.onClick.RemoveAllListeners();
                cellButton.onClick.AddListener(OnCellClicked);
            }
        }

        public void Setup(DateTime date, Color bgColor, bool isSelected, Color selectColor, int completedCount, bool isFutureOrNoData, Action<DateTime> onClick)
        {
            cellDate = date;
            onClickCallback = onClick;

            if (dateText != null)
            {
                dateText.text = date.Day.ToString();
                dateText.enabled = true;
                if ((date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) && completedCount == 0 && !isFutureOrNoData)
                {
                    dateText.color = Color.white;
                }
                else
                {
                    dateText.color = isFutureOrNoData ? new Color(0.4f, 0.4f, 0.4f, 1.0f) : Color.white;
                }
            }

            if (dateImage != null)
            {
                if ((date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) && completedCount == 0 && !isFutureOrNoData)
                {
                    dateImage.color = new Color(0.75f, 0.15f, 0.15f, 1.0f);
                }
                else
                {
                    dateImage.color = bgColor;
                }
            }

            if (borderImage != null)
            {
                borderImage.gameObject.SetActive(isSelected);
                borderImage.color = selectColor;
            }

            if (cellButton != null)
            {
                cellButton.interactable = true;
            }

            SetupDots(completedCount, isFutureOrNoData, date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);
        }

        private void SetupDots(int completedCount, bool isFutureOrNoData, bool isWeekend)
        {
            if (dotsContainer == null || dotPrefab == null) return;

            // Clear old dots
            foreach (Transform child in dotsContainer)
            {
                if (child.gameObject == dotPrefab) continue;
                Destroy(child.gameObject);
            }

            Color greenColor = new Color(0.18f, 0.74f, 0.46f, 1.0f);
            Color yellowColor = new Color(1.0f, 0.75f, 0.03f, 1.0f);
            Color redColor = new Color(0.91f, 0.22f, 0.22f, 1.0f);
            Color grayColor = new Color(0.4f, 0.4f, 0.4f, 0.5f);

            if (isWeekend && completedCount == 0 && !isFutureOrNoData)
            {
                GameObject xObj = new GameObject("MissedText", typeof(RectTransform));
                xObj.transform.SetParent(dotsContainer, false);
                TMP_Text t = xObj.AddComponent<TextMeshProUGUI>();
                t.text = "×";
                t.fontSize = 18;
                t.color = Color.white;
                t.alignment = TextAlignmentOptions.Center;
                return;
            }

            if (isFutureOrNoData)
            {
                CreateDot(grayColor);
                CreateDot(grayColor);
            }
            else if (completedCount == 2)
            {
                CreateDot(greenColor);
                CreateDot(greenColor);
            }
            else if (completedCount == 1)
            {
                CreateDot(yellowColor);
            }
            else
            {
                CreateDot(redColor);
            }
        }

        private void CreateDot(Color c)
        {
            GameObject d = Instantiate(dotPrefab, dotsContainer);
            d.SetActive(true);
            Image img = d.GetComponent<Image>();
            if (img != null) img.color = c;
        }

        public void SetEmpty(Color emptyColor)
        {
            if (dateText != null)
            {
                dateText.text = "";
                dateText.enabled = false;
            }

            if (dateImage != null)
            {
                dateImage.color = emptyColor;
            }

            if (borderImage != null)
            {
                borderImage.gameObject.SetActive(false);
            }

            if (cellButton != null)
            {
                cellButton.interactable = false;
            }

            if (dotsContainer != null)
            {
                foreach (Transform child in dotsContainer)
                {
                    if (child.gameObject == dotPrefab) continue;
                    Destroy(child.gameObject);
                }
            }
        }

        private void OnCellClicked()
        {
            onClickCallback?.Invoke(cellDate);
        }
    }
}
