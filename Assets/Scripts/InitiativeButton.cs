using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class InitiativeButton : MonoBehaviour
{
    public Action<int> OnClicked;

    [SerializeField] private Button button = default;
    [SerializeField] private TextMeshProUGUI text = default;

    private int index;

    public void Setup(int index)
    {
        this.index = index;

        text.text = index.ToString();

        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        OnClicked?.Invoke(index);
    }
}
