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

        public void Setup(DateTime date, bool isFuture, bool isSelected, Color selectColor, bool bComp, bool lComp, bool dComp, bool isFadedMonth, Action<DateTime> onClick)
        {
            SelfWire();
            cellDate = date;
            onClickCallback = onClick;

            bool isToday = date.Date == DateTime.Today.Date;

            // Compute completed count
            int completedCount = 0;
            if (bComp) completedCount++;
            if (lComp) completedCount++;
            if (dComp) completedCount++;

            if (dateText != null)
            {
                dateText.text = date.Day.ToString();
                dateText.enabled = true;
                dateText.fontSize = 22;
                dateText.fontStyle = FontStyles.Bold;

                if (isFuture)
                {
                    dateText.color = new Color(0.682f, 0.718f, 0.698f, isFadedMonth ? 0.25f : 0.5f);
                }
                else
                {
                    dateText.color = new Color(0.973f, 0.976f, 0.98f, isFadedMonth ? 0.5f : 1.0f);
                }
            }

            if (dateImage != null)
            {
                Color cellColor;
                if (isFadedMonth)
                {
                    // Dark shadowed cell color for offset month days
                    cellColor = new Color(0.02f, 0.06f, 0.05f, 0.6f);
                }
                else if (isFuture)
                {
                    cellColor = new Color(0.09f, 0.196f, 0.161f, 0.4f); // spruce green translucent
                }
                else
                {
                    // Past/Present day states:
                    // 0 meals = Red, 1 meal = Yellow, 2 meals = Blue, 3 meals = Green
                    switch (completedCount)
                    {
                        case 0:
                            cellColor = new Color(0.9f, 0.3f, 0.3f, 1.0f);    // Red
                            break;
                        case 1:
                            cellColor = new Color(1.0f, 0.757f, 0.027f, 1.0f); // Yellow
                            break;
                        case 2:
                            cellColor = new Color(0.176f, 0.612f, 0.859f, 1.0f); // Blue
                            break;
                        case 3:
                            cellColor = new Color(0.18f, 0.8f, 0.443f, 1.0f);  // Green
                            break;
                        default:
                            cellColor = new Color(0.09f, 0.196f, 0.161f, 1.0f);
                            break;
                    }
                }
                dateImage.color = cellColor;
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
                cellButton.interactable = !isFuture;
            }

            SetupDots(isFuture, bComp, lComp, dComp, isFadedMonth);
        }

        private void SetupDots(bool isFuture, bool bComp, bool lComp, bool dComp, bool isFadedMonth)
        {
            if (dotsContainer == null || dotPrefab == null) return;

            // Clear old dots
            foreach (Transform child in dotsContainer)
            {
                if (child.gameObject == dotPrefab) continue;
                Destroy(child.gameObject);
            }

            if (isFuture || isFadedMonth) return; // Future or offset days show no dots

            // Dot colors:
            // Breakfast = Green, Lunch = Orange, Dinner = Purple
            Color breakfastColor = new Color(0.18f, 0.8f, 0.443f, 1.0f);   // `#2ECC71` Green
            Color lunchColor = new Color(1.0f, 0.647f, 0.0f, 1.0f);       // `#FFA500` Orange
            Color dinnerColor = new Color(0.6f, 0.4f, 0.9f, 1.0f);         // `#9966E6` Purple

            if (isFadedMonth)
            {
                breakfastColor.a *= 0.4f;
                lunchColor.a *= 0.4f;
                dinnerColor.a *= 0.4f;
            }

            // Create dot based on which meal was completed
            if (bComp) CreateDot(breakfastColor);
            if (lComp) CreateDot(lunchColor);
            if (dComp) CreateDot(dinnerColor);
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
