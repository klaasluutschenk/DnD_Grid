using UnityEngine;
using UnityEngine.UI;

public class Manager_Background : MonoBehaviour
{
    public static Manager_Background Instance;

    [SerializeField] private SpriteRenderer spriteRenderer = default;
    [SerializeField] private RawImage dynamicBackground = default;
    [SerializeField] private RectTransform dynamicMaskBackgroundRect = default;

    private float horizontalSpeed;
    private float verticalSpeed;

    private void Awake()
    {
        Instance = this;

        Manager_Combat.OnCombatEncounterEnded += OnCombatEncounterEnded;
        Manager_Combat.OnCombatEncounterLoaded += OnCombatEncounterLoaded;
    }

    private void Update()
    {
        UpdateDynamicBackground();

        if (Input.GetKey(KeyCode.N))
            ControlDynamicBackground(0, 0);

        if (Input.GetKey(KeyCode.M))
            ControlDynamicBackground(0.03f, 0);

        if (Input.GetKey(KeyCode.B))
            ControlDynamicBackground(0.01f, 0);
    }

    private void OnCombatEncounterEnded()
    {
        spriteRenderer.sprite = null;
        spriteRenderer.enabled = false;
    }

    private void OnCombatEncounterLoaded (CombatEncounter combatEncounter)
    {
        spriteRenderer.sprite = combatEncounter.Background;
        spriteRenderer.enabled = true;

        if (combatEncounter.DynamicBackground != null)
        {
            dynamicBackground.enabled = true;

            Vector2 maskSize = new Vector2(combatEncounter.Background.rect.width, combatEncounter.Background.rect.height);
            dynamicMaskBackgroundRect.sizeDelta = maskSize;

            SetDynamicBackground(combatEncounter.DynamicBackground);
            ControlDynamicBackground(combatEncounter.defaultDynamicBackgroundSpeed.x, combatEncounter.defaultDynamicBackgroundSpeed.y);
        }
        else
        {
            dynamicBackground.enabled = false;
            dynamicBackground.texture = null;
            ControlDynamicBackground(0, 0);
        }
    }

    private void SetDynamicBackground(Texture texture)
    {
        dynamicBackground.texture = texture;
    }

    public void ControlDynamicBackground(float horizontalSpeed, float verticalSpeed)
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
}
