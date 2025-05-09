using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Initiative_UI : MonoBehaviour
{
    public character_Initative Character => character;

    [SerializeField] private bool useCurve = default;
    [SerializeField] private GameObject gameobject_Initative = default;
    [SerializeField] private Transform container;
    [SerializeField] private float curveDuration;
    [SerializeField] private AnimationCurve growCurve;

    [SerializeField] private Image image_Initiative;
    [SerializeField] private Image image_CharacterSprite;

    [SerializeField] private TextMeshProUGUI text_CharacterName;
    [SerializeField] private TextMeshProUGUI text_Initative;

    private character_Initative character;
    private bool hasInitative = false;

    public void Setup(character_Initative character_Initative)
    {
        character = character_Initative;

        image_Initiative.color = character.InitativeColor;
        image_CharacterSprite.sprite = character.Sprite;

        text_CharacterName.text = character.Name;

        if (text_Initative != null)
            text_Initative.text = character_Initative.Initiative.ToString();
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
