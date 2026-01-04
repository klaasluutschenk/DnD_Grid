using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Initiative_UI : MonoBehaviour
{
    public Character_Initiative Character => character;

    [SerializeField] private bool useCurve = default;
    [SerializeField] private GameObject gameobject_Initative = default;
    [SerializeField] private Transform container;
    [SerializeField] private float curveDuration;
    [SerializeField] private AnimationCurve growCurve;

    [SerializeField] private Image image_Initiative;
    [SerializeField] private Image image_CharacterSprite;

    [SerializeField] private TextMeshProUGUI text_CharacterName;
    [SerializeField] private TextMeshProUGUI text_Initative;
    [SerializeField] private TextMeshProUGUI text_UnitCount;

    private Character_Initiative character;
    private bool hasInitative = false;

    public void Setup(Character_Initiative character_Initative)
    {
        character = character_Initative;

        image_Initiative.color = character.InitativeColor;
        image_CharacterSprite.sprite = character.Character.Sprite;

        text_CharacterName.text = character.Character.Name;

        if (text_Initative != null)
            text_Initative.text = character_Initative.Initiative.ToString();

        if (text_UnitCount != null)
            text_UnitCount.text = character_Initative.unitCount.ToString();

        Manager_Initative.OnInitiativeUpdated += OnInitiativeUpdated;
    }

    private void OnDestroy()
    {
        Manager_Initative.OnInitiativeUpdated -= OnInitiativeUpdated;
    }

    private void OnInitiativeUpdated(List<Character_Initiative> activeCharacters)
    {
        if (text_UnitCount != null)
            text_UnitCount.text = character.unitCount.ToString();
    }

    public void SetInitative(bool isActive)
    {
        if (hasInitative == isActive)
            return;

        hasInitative = isActive;

        if (!useCurve)
        {
            if (gameobject_Initative != null)
                gameobject_Initative.SetActive(hasInitative);
            return;
        }

        if (hasInitative)
            StartCoroutine(Grow());
        else
            StartCoroutine(Shrink());
    }

    private IEnumerator Grow()
    {
        float scale;
        float timer = 0;

        while (timer < 0.3f)
        {
            scale = growCurve.Evaluate(timer);
            SetScale(scale);

            timer += Time.deltaTime;
            yield return null;
        }

        timer = curveDuration;
        scale = growCurve.Evaluate(timer);
        SetScale(scale);
    }

    private IEnumerator Shrink()
    {
        float scale;
        float timer = curveDuration;

        while (timer > 0)
        {
            scale = growCurve.Evaluate(timer);
            SetScale(scale);

            timer -= Time.deltaTime;
            yield return null;
        }

        timer = 0;;
        scale = growCurve.Evaluate(timer);
        SetScale(scale);
    }

    private void SetScale(float scale)
    {
        container.localScale = new Vector3(scale, scale, scale);
    }
}
