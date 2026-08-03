using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using FoodTracker.Managers;
using FoodTracker.Persistence;

namespace FoodTracker.UI
{
    public class OnboardingController : MonoBehaviour
    {
        [Header("Slides")]
        [SerializeField] private List<GameObject> slides = new List<GameObject>();

        [Header("Dot Indicators")]
        [SerializeField] private List<Image> dots = new List<Image>();
        [SerializeField] private Sprite activeDotSprite;
        [SerializeField] private Sprite inactiveDotSprite;
        [SerializeField] private Color activeDotColor = Color.white;
        [SerializeField] private Color inactiveDotColor = new Color(1, 1, 1, 0.4f);

        [Header("Navigation Buttons")]
        [SerializeField] private Button nextButton;
        [SerializeField] private Button skipButton;
        [SerializeField] private Button getStartedButton;

        private int currentSlideIndex = 0;
        private GameObject nameInputPopup = null;

        private void Start()
        {
            if (nextButton != null) nextButton.onClick.AddListener(OnNextClicked);
            if (skipButton != null) skipButton.onClick.AddListener(OnSkipClicked);
            if (getStartedButton != null) getStartedButton.onClick.AddListener(OnGetStartedClicked);

            UpdateSlidesDisplay();
        }

        private void OnNextClicked()
        {
            if (currentSlideIndex < slides.Count - 1)
            {
                currentSlideIndex++;
                UpdateSlidesDisplay();
            }
        }

        private void OnSkipClicked()
        {
            PromptNameEntry();
        }

        private void OnGetStartedClicked()
        {
            PromptNameEntry();
        }

        private void PromptNameEntry()
        {
            // If the user already has a name saved, bypass name entry
            if (SaveManager.Instance != null && SaveManager.Instance.AppData != null &&
                !string.IsNullOrEmpty(SaveManager.Instance.AppData.profile.name))
            {
                CompleteOnboarding();
                return;
            }

            // Create a premium Welcome Name Entry Popup dynamically
            if (nameInputPopup != null)
            {
                nameInputPopup.SetActive(true);
                return;
            }

            Canvas canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                CompleteOnboarding();
                return;
            }

            // Tint Overlay Background
            nameInputPopup = new GameObject("Name Entry Overlay", typeof(RectTransform));
            nameInputPopup.transform.SetParent(canvas.transform, false);
            RectTransform popRect = nameInputPopup.GetComponent<RectTransform>();
            popRect.anchorMin = Vector2.zero;
            popRect.anchorMax = Vector2.one;
            popRect.sizeDelta = Vector2.zero;

            Image bg = nameInputPopup.AddComponent<Image>();
            bg.color = new Color(0.027f, 0.098f, 0.09f, 0.95f); // Rich dark green overlay matching theme

            // Center Card Box
            GameObject cardBox = new GameObject("CardBox", typeof(RectTransform));
            cardBox.transform.SetParent(nameInputPopup.transform, false);
            RectTransform boxRect = cardBox.GetComponent<RectTransform>();
            boxRect.sizeDelta = new Vector2(500, 420);
            boxRect.anchorMin = new Vector2(0.5f, 0.5f);
            boxRect.anchorMax = new Vector2(0.5f, 0.5f);
            boxRect.pivot = new Vector2(0.5f, 0.5f);

            Image cardImg = cardBox.AddComponent<Image>();
            cardImg.color = new Color(0.059f, 0.141f, 0.125f, 1.0f); // Card dark green

            // Shadow border highlight
            GameObject border = new GameObject("Border", typeof(RectTransform));
            border.transform.SetParent(cardBox.transform, false);
            Image bImg = border.AddComponent<Image>();
            bImg.color = new Color(0.18f, 0.8f, 0.443f, 1.0f); // Accent green border
            border.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            border.GetComponent<RectTransform>().anchorMax = Vector2.one;
            border.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);

            // Vertical Layout for Card content
            VerticalLayoutGroup vl = cardBox.AddComponent<VerticalLayoutGroup>();
            vl.padding = new RectOffset(30, 30, 40, 40);
            vl.spacing = 20;
            vl.childAlignment = TextAnchor.MiddleCenter;

            // Title
            GameObject titleObj = new GameObject("Title", typeof(RectTransform));
            titleObj.transform.SetParent(cardBox.transform, false);
            TMP_Text title = titleObj.AddComponent<TextMeshProUGUI>();
            title.text = "Welcome!";
            title.fontSize = 32;
            title.fontStyle = FontStyles.Bold;
            title.color = Color.white;
            title.alignment = TextAlignmentOptions.Center;

            // Subtitle
            GameObject subObj = new GameObject("Subtitle", typeof(RectTransform));
            subObj.transform.SetParent(cardBox.transform, false);
            TMP_Text sub = subObj.AddComponent<TextMeshProUGUI>();
            sub.text = "Enter your full name to personalize your dashboard experience";
            sub.fontSize = 18;
            sub.color = new Color(0.682f, 0.718f, 0.698f, 1.0f);
            sub.alignment = TextAlignmentOptions.Center;

            // Input Field
            GameObject inputObj = new GameObject("InputField", typeof(RectTransform));
            inputObj.transform.SetParent(cardBox.transform, false);
            Image inputBg = inputObj.AddComponent<Image>();
            inputBg.color = new Color(0.09f, 0.196f, 0.161f, 1.0f); // Spruce cell green
            inputObj.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 60);

            GameObject ta = new GameObject("TextArea", typeof(RectTransform));
            ta.transform.SetParent(inputObj.transform, false);
            RectTransform taRect = ta.GetComponent<RectTransform>();
            taRect.anchorMin = Vector2.zero;
            taRect.anchorMax = Vector2.one;
            taRect.sizeDelta = new Vector2(-20, -10);

            GameObject placeholder = new GameObject("Placeholder", typeof(RectTransform));
            placeholder.transform.SetParent(ta.transform, false);
            TMP_Text pTxt = placeholder.AddComponent<TextMeshProUGUI>();
            pTxt.text = "Your Full Name...";
            pTxt.fontSize = 18;
            pTxt.color = new Color(0.682f, 0.718f, 0.698f, 0.5f);
            RectTransform pRect = placeholder.GetComponent<RectTransform>();
            pRect.anchorMin = Vector2.zero;
            pRect.anchorMax = Vector2.one;
            pRect.sizeDelta = Vector2.zero;

            GameObject textObj = new GameObject("Text", typeof(RectTransform));
            textObj.transform.SetParent(ta.transform, false);
            TMP_Text tTxt = textObj.AddComponent<TextMeshProUGUI>();
            tTxt.fontSize = 18;
            tTxt.color = Color.white;
            RectTransform tRect = textObj.GetComponent<RectTransform>();
            tRect.anchorMin = Vector2.zero;
            tRect.anchorMax = Vector2.one;
            tRect.sizeDelta = Vector2.zero;

            TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();
            inputField.textViewport = taRect;
            inputField.textComponent = tTxt;
            inputField.placeholder = pTxt;

            // Continue Button
            GameObject btnObj = new GameObject("Continue Button", typeof(RectTransform), typeof(CanvasRenderer));
            btnObj.transform.SetParent(cardBox.transform, false);
            btnObj.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 60);

            Image btnImg = btnObj.AddComponent<Image>();
            btnImg.color = new Color(0.18f, 0.8f, 0.443f, 1.0f); // Accent green

            GameObject btnLbl = new GameObject("LabelText", typeof(RectTransform));
            btnLbl.transform.SetParent(btnObj.transform, false);
            TMP_Text bl = btnLbl.AddComponent<TextMeshProUGUI>();
            bl.text = "Get Started";
            bl.fontSize = 20;
            bl.fontStyle = FontStyles.Bold;
            bl.color = Color.white;
            bl.alignment = TextAlignmentOptions.Center;
            btnLbl.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            btnLbl.GetComponent<RectTransform>().anchorMax = Vector2.one;
            btnLbl.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            Button button = btnObj.AddComponent<Button>();
            button.onClick.AddListener(() => {
                string enteredName = inputField.text.Trim();
                if (!string.IsNullOrEmpty(enteredName))
                {
                    if (SaveManager.Instance != null && SaveManager.Instance.AppData != null)
                    {
                        SaveManager.Instance.AppData.profile.name = enteredName;
                    }
                    nameInputPopup.SetActive(false);
                    CompleteOnboarding();
                }
                else
                {
                    // Visual warning feedback
                    inputBg.color = new Color(0.9f, 0.3f, 0.3f, 1.0f);
                }
            });

            // Focus name input box
            inputField.ActivateInputField();
        }

        private void CompleteOnboarding()
        {
            if (SaveManager.Instance != null && SaveManager.Instance.AppData != null)
            {
                SaveManager.Instance.AppData.firstLaunch = false;
                SaveManager.Instance.Save();
            }

            if (NavigationManager.Instance != null)
            {
                NavigationManager.Instance.NavigateTo(PageType.Home);
            }
        }

        private void UpdateSlidesDisplay()
        {
            for (int i = 0; i < slides.Count; i++)
            {
                if (slides[i] != null)
                {
                    slides[i].SetActive(i == currentSlideIndex);
                }
            }

            for (int i = 0; i < dots.Count; i++)
            {
                if (dots[i] != null)
                {
                    dots[i].color = (i == currentSlideIndex) ? activeDotColor : inactiveDotColor;
                    if (activeDotSprite != null && inactiveDotSprite != null)
                    {
                        dots[i].sprite = (i == currentSlideIndex) ? activeDotSprite : inactiveDotSprite;
                    }
                }
            }

            bool isLastSlide = (currentSlideIndex == slides.Count - 1);
            if (nextButton != null) nextButton.gameObject.SetActive(!isLastSlide);
            if (skipButton != null) skipButton.gameObject.SetActive(!isLastSlide);
            if (getStartedButton != null) getStartedButton.gameObject.SetActive(isLastSlide);
        }
    }
}
