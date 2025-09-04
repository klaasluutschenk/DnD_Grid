using UnityEngine;

public class PhysicsObject_Destructable : PhysicsObject
{
    [SerializeField] private int healthPoints = default;
    
    protected override bool RemoveTiles => false;
    protected override bool ShowVisual => true;

    private Manager_Input manager_Input;

    private void OnMouseOver()
    {
        if (manager_Input == null)
        {
            if (Manager_Input.Instance != null)
            {
                manager_Input = Manager_Input.Instance;
            }
            else
                return;
        }

        if (Manager_Input.Instance.InputState != InputState.Damage)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            healthPoints -= manager_Input.DamageValue;

            if (healthPoints <= 0)
                Destroy(this.gameObject);
        }
    }
}
