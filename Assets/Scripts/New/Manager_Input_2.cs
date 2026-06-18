using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Manager_Input_2 : MonoBehaviour
{
    public static Manager_Input_2 Instance;

    private List<Tile> movementTiles = new List<Tile>();
    private Character_World movementCharacter;

    private InputState inputState = InputState.Default;

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
        SwitchInput(InputState.Default);

        Debug.Log("Input Loaded");

        yield return null;
    }

    private void Update()
    {
        Controls();
    }

    private void ToggleInputState(InputState newInputState)
    {
        if (inputState != newInputState)
            inputState = newInputState;
        else
            inputState = InputState.Default;

        Manager_Cursor.Instance.SetCursor(inputState);
    }

    private void ResetInputState()
    {
        SwitchInput(InputState.Default);
        Manager_Cursor.Instance.ResetCursor();
    }

    private void Controls()
    {
        MovementControls();
        
        if (inputState == InputState.Movement)
        {
            return;
        }

        InitiativeControls();

        if (inputState == InputState.Initiative)
        {
            return;
        }

        EffectsControls();
    }

    private void SwitchInput(InputState inputState)
    {
        Tile.OnTileClicked -= ApplyDefaultEffect;
        Tile.OnTileClickedAlternate -= ApplyAlternateEffect;

        ToggleInputState(inputState);

        if (!IsInEffectinputState())
        {
            Tile.OnTileClicked -= ApplyDefaultEffect;
            Tile.OnTileClickedAlternate -= ApplyAlternateEffect;
            return;
        }

        Tile.OnTileClicked += ApplyDefaultEffect;
        Tile.OnTileClickedAlternate += ApplyAlternateEffect;
    }

    #region Movement

    private void MovementControls()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SwitchInput(InputState.Movement);

            Tile.OnTileClicked += OnTileClicked_Movement;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Tile.OnTileClicked -= OnTileClicked_Movement;
            ResetInputState();
            ClearMovementTiles();
        }

        if (Input.GetMouseButtonDown(1) && inputState == InputState.Movement)
        {
            ClearMovementTiles();
        }
    }

    private void OnTileClicked_Movement(Tile tile)
    {
        if (movementCharacter == null)
        {
            SetNewMovement(tile);
            return;
        }

        ActivateMovement(tile);
    }

    private void SetNewMovement(Tile tile)
    {
        if (tile.World_Entity == null)
        {
            return;
        }

        Character_World character_World = (Character_World)tile.World_Entity;

        if (character_World == null)
        {
            return;
        }

        ClearMovementTiles();

        movementCharacter = character_World;
        movementCharacter.Tiles.ForEach(t => t.HighlightSelection(true));

        List<Tile> totalTiles = new List<Tile>();

        for (int tileIndex = 0; tileIndex < character_World.Tiles.Count; tileIndex++)
        {
            List<Tile> tiles = new List<Tile>();
            tiles.Add(character_World.Tiles[tileIndex]);

            int movement = character_World.GetMovement();

            int currentRadiusCheck = 0;
            while (currentRadiusCheck < movement && currentRadiusCheck < 50)
            {
                List<Tile> tiles2 = new List<Tile>();

                for (int i = 0; i < tiles.Count; i++)
                {
                    List<Tile> surroundingTiles = Manager_Grid_2.Instance.GetSurroundingAlliedTile(tiles[i], character_World.Character.IsPlayerTeam);

                    foreach (Tile surroundingTile in surroundingTiles)
                    {
                        if (!tiles2.Contains(surroundingTile))
                        {
                            tiles2.Add(surroundingTile);
                        }
                    }
                }

                foreach (Tile tile2 in tiles2)
                {
                    if (!tiles.Contains(tile2))
                    {
                        tiles.Add(tile2);
                    }
                }

                currentRadiusCheck++;
            }

            totalTiles.AddRange(tiles);
        }

        for (int i = totalTiles.Count - 1; i > 0; i--)
        {
            if (totalTiles[i].World_Entity != null)
            {
                totalTiles.RemoveAt(i);
            }
        }

        totalTiles.ForEach(st => st.Movement(true));

        movementTiles.AddRange(totalTiles);
    }

    private void ActivateMovement(Tile tile)
    {
        if (!Manager_Grid_2.Instance.RoomAvailable(tile, movementCharacter.Character.EntitySize, movementCharacter))
        {
            return;
        }

        movementCharacter.Tiles.ForEach(t => t.HighlightSelection(false));
        movementCharacter.SetPosition(tile);

        ClearMovementTiles();
        ClearMovementCharacter();
    }

    private void ClearMovementTiles()
    {
        movementTiles.ForEach(st => st.Movement(false));
        movementTiles.Clear();
        ClearMovementCharacter();
    }

    private void ClearMovementCharacter()
    {
        if (movementCharacter == null)
        {
            return;
        }

        movementCharacter.Tiles.ForEach(t => t.HighlightSelection(false));
        movementCharacter = null;
    }

    #endregion

    #region Effects

    private void EffectsControls()
    {
        EffectsInput();
    }

    private void EffectsInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchInput(InputState.Armor);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchInput(InputState.Barrier);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchInput(InputState.Bleed);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchInput(InputState.Blinded);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchInput(InputState.Burn);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SwitchInput(InputState.Reload);
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SwitchInput(InputState.Root);
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SwitchInput(InputState.Slowed);
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SwitchInput(InputState.Stun);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SwitchInput(InputState.Venom);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchInput(InputState.TrueDamage);
        }
    }

    private bool IsInEffectinputState()
    {
        switch (inputState)
        {
            case InputState.Default:
                return true;
            case InputState.Movement:
                return false;
            case InputState.Armor:
                return true;
            case InputState.Barrier:
                return true;
            case InputState.Bleed:
                return true;
            case InputState.Blinded:
                return true;
            case InputState.Burn:
                return true;
            case InputState.Reload:
                return true;
            case InputState.Root:
                return true;
            case InputState.Slowed:
                return true;
            case InputState.Stun:
                return true;
            case InputState.Venom:
                return true;
            case InputState.Initiative:
                return false;
            case InputState.Spawning:
                return false;
            case InputState.TrueDamage:
                return true;
            default:
                return false;
        }
    }

    private void ApplyDefaultEffect(Tile tile)
    {
        Character_World character_World = tile.World_Entity as Character_World;

        if (character_World == null)
            return;

        switch (inputState)
        {
            case InputState.Default:
                int damageValue = Input.GetKey(KeyCode.LeftControl) ? 5 : 1;
                character_World.Damage(damageValue);
                break;
            case InputState.Armor:
                character_World.ApplyArmor();
                break;
            case InputState.Barrier:
                character_World.ApplyBarrier();
                break;
            case InputState.Bleed:
                character_World.ApplyBleed();
                break;
            case InputState.Blinded:
                character_World.ApplyBlinded();
                break;
            case InputState.Burn:
                character_World.ApplyBurn();
                break;
            case InputState.Reload:
                character_World.ApplyReloading();
                break;
            case InputState.Root:
                character_World.ApplyRooted();
                break;
            case InputState.Slowed:
                character_World.ApplySlowed();
                break;
            case InputState.Stun:
                character_World.ApplyStunned();
                break;
            case InputState.Venom:
                character_World.ApplyVenom();
                break;
            case InputState.TrueDamage:
                int trueDamageValue = Input.GetKey(KeyCode.LeftControl) ? 5 : 1;
                character_World.Damage(trueDamageValue, true, true);
                break;
            default:
                break;
        }
    }

    private void ApplyAlternateEffect(Tile tile)
    {
        Character_World character_World = tile.World_Entity as Character_World;

        if (character_World == null)
            return;

        switch (inputState)
        {
            case InputState.Default:
                int healingValue = Input.GetKey(KeyCode.LeftControl) ? 5 : 1;
                character_World.Heal(healingValue);
                break;
            case InputState.Bleed:
                character_World.CleanseBleed();
                break;
            case InputState.Blinded:
                character_World.CleanseBlinded();
                break;
            case InputState.Burn:
                character_World.CleanseBurn();
                break;
            case InputState.Reload:
                character_World.CleanseReload();
                break;
            case InputState.Root:
                character_World.CleanseRoot();
                break;
            case InputState.Slowed:
                character_World.CleanseSlowed();
                break;
            case InputState.Stun:
                character_World.CleanseStunned();
                break;
            case InputState.Venom:
                character_World.CleanseVenom();
                break;
            default:
                break;
        }
    }

    #endregion

    #region Spawning

    #endregion

    #region Initiative

    private void InitiativeControls()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            SwitchInput(InputState.Initiative);

            Tile.OnTileClicked += OnTileClicked_Initiative;
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            Tile.OnTileClicked -= OnTileClicked_Initiative;
            ResetInputState();
        }
    }

    private void OnTileClicked_Initiative(Tile tile)
    {
        Character_World character_World = tile.World_Entity as Character_World;

        if (character_World == null)
            return;

        character_World.SetInitiative(false);
    }

    #endregion


}