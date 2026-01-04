using UnityEngine;
using System;

public class PhysicsObject_Destructable : PhysicsObject
{
    public static Action<PhysicsObject_Destructable> OnHighlighted;
    public static Action<PhysicsObject_Destructable> OnStopHighlighted;

    [SerializeField] private int healthPoints = default;

    [SerializeField] private Material defaultMaterial = default;
    [SerializeField] private Material highLightMaterial = default;
    
    protected override bool RemoveTiles => false;
    protected override bool ShowVisual => true;

    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnMouseOver()
    {
        OnHighlighted?.Invoke(this);
    }

    private void OnMouseExit()
    {
        OnStopHighlighted?.Invoke(this);
    }

    public void SetDefault()
    {
        meshRenderer.material = defaultMaterial;
    }

    public void HighLight()
    {
        meshRenderer.material = highLightMaterial;
    }

    public bool Damage(int damage)
    {
        healthPoints -= damage;

        if (healthPoints <= 0)
        {
            Destroy(this.gameObject);
            return true;
        }

        return false;
    }
}
