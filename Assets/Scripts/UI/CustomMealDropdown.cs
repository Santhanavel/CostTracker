using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

namespace FoodTracker.UI
{
    public class CustomMealDropdown : MonoBehaviour
    {
        [Header("UI Bindings")]
        [SerializeField] private TMP_Text labelText;
        [SerializeField] private GameObject listPanel;
        [SerializeField] private GameObject spaceObj;
        [SerializeField] private Button mainButton;
        [SerializeField] private Button breakfastBtn;
        [SerializeField] private Button lunchBtn;
        [SerializeField] private Button dinnerBtn;

        [System.Serializable]
        public struct OptionData
        {
            public string text;
            public OptionData(string t) { text = t; }
        }

        public List<OptionData> options = new List<OptionData>()
        {
            new OptionData("Breakfast"),
            new OptionData("Lunch"),
            new OptionData("Dinner")
        };

        private int _value = 0;
        public int value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp(value, 0, 2);
                if (labelText != null)
                {
                    labelText.text = options[_value].text;
                }
            }
        }

        private void Start()
        {
            if (mainButton != null)
            {
                mainButton.onClick.RemoveAllListeners();
                mainButton.onClick.AddListener(ToggleList);
            }
            if (breakfastBtn != null)
            {
                breakfastBtn.onClick.RemoveAllListeners();
                breakfastBtn.onClick.AddListener(() => SelectIndex(0));
            }
            if (lunchBtn != null)
            {
                lunchBtn.onClick.RemoveAllListeners();
                lunchBtn.onClick.AddListener(() => SelectIndex(1));
            }
            if (dinnerBtn != null)
            {
                dinnerBtn.onClick.RemoveAllListeners();
                dinnerBtn.onClick.AddListener(() => SelectIndex(2));
            }
            
            SelectIndex(value);
            if (listPanel != null) listPanel.SetActive(false);
        }

        private void ToggleList()
        {
            if (listPanel != null) listPanel.SetActive(!listPanel.activeSelf);
            if (spaceObj != null) spaceObj.SetActive(!spaceObj.activeSelf);
        }

        public void SelectIndex(int index)
        {
            _value = Mathf.Clamp(index, 0, 2);
            if (labelText != null)
            {
                labelText.text = options[_value].text;
            }
            if (listPanel != null) listPanel.SetActive(false);
        }
    }
}
