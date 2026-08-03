using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using FoodTracker.Persistence;
using FoodTracker.Data;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

namespace FoodTracker.Managers
{
    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager Instance { get; private set; }

        [Header("Toast UI Settings")]
        [SerializeField] private GameObject toastPanelPrefab; // We'll build a sliding toast popup
        private Canvas parentCanvas;

        private HashSet<string> triggeredRemindersToday = new HashSet<string>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitChannels();
        }

        private void Start()
        {
            parentCanvas = FindAnyObjectByType<Canvas>();
            StartCoroutine(TimeCheckLoop());
        }

        private void InitChannels()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var channel = new AndroidNotificationChannel()
            {
                Id = "meal_reminders",
                Name = "Meal Reminders",
                Importance = Importance.High,
                Description = "Channel for daily meal time reminders",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
#endif
        }

        public void ScheduleAllNotifications()
        {
            if (SaveManager.Instance == null || SaveManager.Instance.AppData == null) return;

#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidNotificationCenter.CancelAllNotifications();
#endif

            List<ReminderData> reminders = SaveManager.Instance.AppData.reminderRecords;
            foreach (var reminder in reminders)
            {
                if (!reminder.enabled) continue;

                if (DateTime.TryParse(reminder.timeString, out DateTime timeParsed))
                {
                    DateTime triggerTime = DateTime.Today.AddHours(timeParsed.Hour).AddMinutes(timeParsed.Minute);
                    if (triggerTime < DateTime.Now)
                    {
                        triggerTime = triggerTime.AddDays(1); // Schedule for tomorrow
                    }

                    string title = reminder.mealType;
                    string message = "Time to log your " + reminder.mealType + "!";

#if UNITY_ANDROID && !UNITY_EDITOR
                    var notification = new AndroidNotification
                    {
                        Title = title,
                        Text = message,
                        FireTime = triggerTime,
                        SmallIcon = "icon_0",
                        LargeIcon = "icon_1"
                    };
                    AndroidNotificationCenter.SendNotification(notification, "meal_reminders");
#endif
                    Debug.Log($"[NotificationManager] Scheduled remote local notification for {reminder.mealType} at {triggerTime}");
                }
            }
        }

        private IEnumerator TimeCheckLoop()
        {
            while (true)
            {
                if (SaveManager.Instance != null && SaveManager.Instance.AppData != null)
                {
                    List<ReminderData> reminders = SaveManager.Instance.AppData.reminderRecords;
                    string todayKey = DateTime.Today.ToString("yyyy-MM-dd");

                    foreach (var reminder in reminders)
                    {
                        if (!reminder.enabled) continue;

                        if (DateTime.TryParse(reminder.timeString, out DateTime timeParsed))
                        {
                            DateTime now = DateTime.Now;
                            // Check if current hour and minute match
                            if (now.Hour == timeParsed.Hour && now.Minute == timeParsed.Minute)
                            {
                                string triggerId = $"{reminder.id}_{todayKey}";
                                if (!triggeredRemindersToday.Contains(triggerId))
                                {
                                    triggeredRemindersToday.Add(triggerId);
                                    TriggerOnScreenNotification(reminder.mealType);
                                }
                            }
                        }
                    }
                }
                yield return new WaitForSeconds(10f); // Check every 10 seconds
            }
        }

        // Expose a public function to trigger notification simulation instantly (e.g. for testing)
        public void TriggerOnScreenNotification(string mealName)
        {
            Debug.Log($"[NotificationManager] Alert! {mealName} time starts!");
            StartCoroutine(ShowToastPopup(mealName));
        }

        private IEnumerator ShowToastPopup(string mealName)
        {
            if (parentCanvas == null) parentCanvas = FindAnyObjectByType<Canvas>();
            if (parentCanvas == null) yield break;

            // Colors matching our dark green palette
            Color cardCol = new Color(0.059f, 0.141f, 0.125f, 1.0f);    // `#0F2420` Dark Green
            Color activeCol = new Color(0.18f, 0.8f, 0.443f, 1.0f);     // `#2ECC71` Accent Green
            Color textCol = Color.white;

            // Dynamically build a premium sliding Toast Popup UI
            GameObject toast = new GameObject("Sliding Toast Notification", typeof(RectTransform));
            toast.transform.SetParent(parentCanvas.transform, false);
            RectTransform tr = toast.GetComponent<RectTransform>();
            
            // Anchored to Top
            tr.anchorMin = new Vector2(0.5f, 1f);
            tr.anchorMax = new Vector2(0.5f, 1f);
            tr.pivot = new Vector2(0.5f, 1f);
            tr.anchoredPosition = new Vector2(0, 150); // Start off-screen top
            tr.sizeDelta = new Vector2(400, 100);

            UnityEngine.UI.Image img = toast.AddComponent<UnityEngine.UI.Image>();
            img.color = cardCol;

            // Border Outline
            GameObject border = new GameObject("Border", typeof(RectTransform));
            border.transform.SetParent(toast.transform, false);
            UnityEngine.UI.Image bImg = border.AddComponent<UnityEngine.UI.Image>();
            bImg.color = activeCol;
            border.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            border.GetComponent<RectTransform>().anchorMax = Vector2.one;
            border.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);

            // Horizontal layout for layout padding
            HorizontalLayoutGroup hl = toast.AddComponent<HorizontalLayoutGroup>();
            hl.padding = new RectOffset(20, 20, 10, 10);
            hl.spacing = 15;
            hl.childAlignment = TextAnchor.MiddleCenter;

            // Left bell icon
            GameObject iconObj = new GameObject("Icon", typeof(RectTransform));
            iconObj.transform.SetParent(toast.transform, false);
            UnityEngine.UI.Image icon = iconObj.AddComponent<UnityEngine.UI.Image>();
            Sprite bellSprite = null;
#if UNITY_EDITOR
            bellSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Icons/bell.png");
#endif
            if (bellSprite != null) icon.sprite = bellSprite;
            icon.color = activeCol;
            iconObj.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);

            // Right text wrap
            GameObject textWrap = new GameObject("TextWrap", typeof(RectTransform));
            textWrap.transform.SetParent(toast.transform, false);
            VerticalLayoutGroup vl = textWrap.AddComponent<VerticalLayoutGroup>();
            vl.spacing = 4;

            GameObject titleObj = new GameObject("Title", typeof(RectTransform));
            titleObj.transform.SetParent(textWrap.transform, false);
            TMP_Text title = titleObj.AddComponent<TextMeshProUGUI>();
            title.text = "Meal Reminder";
            title.fontSize = 20;
            title.fontStyle = FontStyles.Bold;
            title.color = textCol;

            GameObject descObj = new GameObject("Desc", typeof(RectTransform));
            descObj.transform.SetParent(textWrap.transform, false);
            TMP_Text desc = descObj.AddComponent<TextMeshProUGUI>();
            desc.text = $"{mealName} time starts now!";
            desc.fontSize = 16;
            desc.color = Color.gray;

            // Slide Down Animation
            float elapsed = 0f;
            float duration = 0.4f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float ratio = elapsed / duration;
                float currentY = Mathf.Lerp(150f, -40f, ratio); // Slide down to Y = -40
                tr.anchoredPosition = new Vector2(0, currentY);
                yield return null;
            }
            tr.anchoredPosition = new Vector2(0, -40f);

            // Hold open for 3 seconds
            yield return new WaitForSeconds(3.5f);

            // Slide Up Animation
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float ratio = elapsed / duration;
                float currentY = Mathf.Lerp(-40f, 150f, ratio);
                tr.anchoredPosition = new Vector2(0, currentY);
                yield return null;
            }

            Destroy(toast);
        }
    }
}
