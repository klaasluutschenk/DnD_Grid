using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Manager_Input : MonoBehaviour
{
    public static Manager_Input Instance;

    [SerializeField] private Button button_Selection = default;
    [SerializeField] private TMP_InputField inputField_Selection = default;

    [SerializeField] private Button button_Damage = default;
    [SerializeField] private TMP_InputField inputField_Damage = default;

    [SerializeField] private Button button_Heal= default;
    [SerializeField] private TMP_InputField inputField_Heal = default;

    [SerializeField] private Button button_Movement = default;

    [SerializeField] private Button button_Spawn = default;

    private InputState inputState = InputState.None;

    private int selectionRadius = 1;
    private int damageValue = 1;
    private int healValue = 1;

    private void Awake()
    {
        Instance = this;

        button_Selection.onClick.AddListener(OnSelectionClicked);
        inputField_Selection.onValueChanged.AddListener(OnSelectionChanged);

        button_Damage.onClick.AddListener(OnDamageClicked);
        inputField_Damage.onValueChanged.AddListener(OnDamageChanged);

        button_Heal.onClick.AddListener(OnHealClicked);
        inputField_Heal.onValueChanged.AddListener(OnHealChanged);

        button_Movement.onClick.AddListener(OnMovementClicked);

        button_Spawn.onClick.AddListener(OnSpawnedClicked);
    }

    #region UI Interaction

    private void OnSelectionClicked()
    {
        inputState = InputState.Selection;
    }

    private void OnSelectionChanged(string value)
    {
        selectionRadius = int.Parse(value);
    }

    private void OnDamageClicked()
    {
        inputState = InputState.Damage;
    }

    private void OnDamageChanged(string value)
    {
        damageValue = int.Parse(value);
    }

    private void OnHealClicked()
    {
        inputState = InputState.Heal;
    }

    private void OnHealChanged(string value)
    {
        healValue = int.Parse(value);
    }

    private void OnMovementClicked()
    {
        inputState = InputState.Movement;
    }

    private void OnSpawnedClicked()
    {
        inputState = InputState.Spawn;
    }

    #endregion

    private void Update()
    {
        switch (inputState)
        {
            case InputState.None:
                break;
            case InputState.Selection:
                break;
            case InputState.Damage:
                break;
            case InputState.Heal:
                break;
            case InputState.Movement:
                break;
            case InputState.Spawn:
                Spawn();
                break;
            default:
                break;
        }
    }

    #region Spawning

    private void Spawn()
    {

    }

    #endregion
}

public enum InputState
{
    None = 0,
    Selection = 1,
    Damage = 2,
    Heal = 3,
    Movement = 4,
    Spawn = 5
}