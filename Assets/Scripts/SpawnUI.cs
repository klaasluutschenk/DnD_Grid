using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SpawnUI : MonoBehaviour
{
    public Action<Character> OnClicked;

    [SerializeField] private TextMeshProUGUI text_Name = default;
    [SerializeField] private Image image_Sprite = default;
    [SerializeField] private Button button = default;

    private Character character;

    public void Setup(Character character)
    {
        this.character = character;

        text_Name.text = character.Name;
        image_Sprite.sprite = character.Sprite;

        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        OnClicked?.Invoke(character);
    }
}
