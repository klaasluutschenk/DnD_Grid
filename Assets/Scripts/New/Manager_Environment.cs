using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Manager_Environment : MonoBehaviour
{
    public static Manager_Environment Instance;

    [SerializeField] private SpriteRenderer spriteRenderer = default;
    [SerializeField] private RawImage dynamicBackground = default;
    [SerializeField] private RectTransform dynamicMaskBackgroundRect = default;

    private float horizontalSpeed;
    private float verticalSpeed;

    private void Awake()
    {
        Instance = this;
    }

    public Coroutine Initialize()
    {
        return StartCoroutine(InitializeRoutine());
    }

    private IEnumerator InitializeRoutine()
    {
        CombatEncounter combatEncounter = Manager_Encounter.Instance.CombatEncounter;

        spriteRenderer.sprite = combatEncounter.Background;

        SetupDynamicBackground(combatEncounter.Background, combatEncounter.DynamicBackground, combatEncounter.defaultDynamicBackgroundSpeed);

        yield return null;

        Debug.Log("Environment Loaded");
    }


    private void Update()
    {
        UpdateDynamicBackground();
        UpdateDynamicMovement();
    }


    #region Dynamic Background

    private void UpdateDynamicMovement()
    {
        if (Input.GetKey(KeyCode.B))
            ControlDynamicBackground(0, 0);

        if (Input.GetKey(KeyCode.N))
            ControlDynamicBackground(0.01f, 0);

        if (Input.GetKey(KeyCode.M))
            ControlDynamicBackground(0.03f, 0);
    }

    private void ControlDynamicBackground(float horizontalSpeed, float verticalSpeed)
    {
        this.horizontalSpeed = horizontalSpeed;
        this.verticalSpeed = verticalSpeed;
    }

    private void UpdateDynamicBackground()
    {
        Rect currentUvRect = dynamicBackground.uvRect;

        currentUvRect.x += horizontalSpeed * Time.deltaTime;
        currentUvRect.y += verticalSpeed * Time.deltaTime;

        dynamicBackground.uvRect = currentUvRect;
    }

    private void SetupDynamicBackground(Sprite backgroundSprite, Texture dynamicBackgroundTexture, Vector2 dynamicBackgroundMovement)
    {
        if (dynamicBackgroundTexture == null)
        {
            dynamicBackground.enabled = false;
            dynamicBackground.texture = null;
            ControlDynamicBackground(0, 0);
            return;
        }

        dynamicBackground.enabled = true;

        Vector2 maskSize = new Vector2(backgroundSprite.rect.width, backgroundSprite.rect.height);
        dynamicMaskBackgroundRect.sizeDelta = maskSize;

        dynamicBackground.texture = dynamicBackgroundTexture;
        ControlDynamicBackground(dynamicBackgroundMovement.x, dynamicBackgroundMovement.y);
    }

    #endregion
}
