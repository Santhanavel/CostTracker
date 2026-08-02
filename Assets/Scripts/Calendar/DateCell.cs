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
        [SerializeField] private Image borderImage; // Selected outline
        [SerializeField] private Image todayImage;  // Today outline or dot
        [SerializeField] private Button cellButton;

        [Header("Indicator Dots")]
        [SerializeField] private Transform dotsContainer;
        [SerializeField] private GameObject dotPrefab;
        [SerializeField] private Sprite dotSprite; // White circle sprite

        private DateTime cellDate;
        private Action<DateTime> onClickCallback;

        private void Awake()
        {
            SelfWire();
        }

        public void SelfWire()
        {
            if (dateText == null) dateText = transform.Find("DateText")?.GetComponent<TMP_Text>();
            if (dateImage == null) dateImage = GetComponent<Image>();
            if (borderImage == null) borderImage = transform.Find("Border")?.GetComponent<Image>();
            if (todayImage == null) todayImage = transform.Find("TodayHighlight")?.GetComponent<Image>();
            if (cellButton == null) cellButton = GetComponent<Button>();
            if (dotsContainer == null) dotsContainer = transform.Find("DotsContainer");
            
            if (dotPrefab == null)
            {
                // Create a clean dot template
                GameObject dotObj = new GameObject("DotTemplate", typeof(RectTransform), typeof(CanvasRenderer));
                dotObj.transform.SetParent(transform, false);
                dotObj.SetActive(false);
                Image img = dotObj.AddComponent<Image>();
                if (dotSprite != null) img.sprite = dotSprite;
                img.color = Color.white;
                dotObj.GetComponent<RectTransform>().sizeDelta = new Vector2(14, 14);
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
            SelfWire();
            cellDate = date;
            onClickCallback = onClick;

            bool isToday = date.Date == DateTime.Today.Date;

            if (dateText != null)
            {
                dateText.text = date.Day.ToString();
                dateText.enabled = true;
                dateText.fontSize = 22;
                dateText.fontStyle = FontStyles.Bold;

                if ((date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) && completedCount == 0 && !isFutureOrNoData)
                {
                    dateText.color = new Color(0.96f, 0.969f, 0.98f, 1.0f); // Bright Text `#F5F7FA`
                }
                else
                {
                    dateText.color = isFutureOrNoData ? new Color(0.655f, 0.718f, 0.694f, 0.5f) : new Color(0.96f, 0.969f, 0.98f, 1.0f);
                }
            }

            if (dateImage != null)
            {
                if ((date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) && completedCount == 0 && !isFutureOrNoData)
                {
                    dateImage.color = new Color(1.0f, 0.357f, 0.357f, 1.0f); // Failed red `#FF5B5B`
                }
                else
                {
                    // If future, apply lower opacity
                    Color finalColor = bgColor;
                    if (isFutureOrNoData)
                    {
                        finalColor.a = 0.4f;
                    }
                    dateImage.color = finalColor;
                }
            }

            if (borderImage != null)
            {
                borderImage.gameObject.SetActive(isSelected);
                borderImage.color = selectColor;
            }

            if (todayImage != null)
            {
                todayImage.gameObject.SetActive(isToday);
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

            Color greenColor = new Color(0.18f, 0.8f, 0.443f, 1.0f);   // `#2ECC71`
            Color yellowColor = new Color(1.0f, 0.757f, 0.027f, 1.0f); // `#FFC107`
            Color redColor = new Color(1.0f, 0.357f, 0.357f, 1.0f);    // `#FF5B5B`
            Color grayColor = new Color(0.655f, 0.718f, 0.694f, 0.4f);  // `#A7B7B1` muted

            if (isWeekend && completedCount == 0 && !isFutureOrNoData)
            {
                GameObject xObj = new GameObject("MissedText", typeof(RectTransform));
                xObj.transform.SetParent(dotsContainer, false);
                TMP_Text t = xObj.AddComponent<TextMeshProUGUI>();
                t.text = "×";
                t.fontSize = 22;
                t.fontStyle = FontStyles.Bold;
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
            if (img != null)
            {
                if (dotSprite != null) img.sprite = dotSprite;
                img.color = c;
            }
        }

        public void SetEmpty(Color emptyColor)
        {
            SelfWire();
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

            if (todayImage != null)
            {
                todayImage.gameObject.SetActive(false);
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
