using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public float progress;
    public Flavor Flavors;
    public int CurrentScore = 0;
    public int CurrentMult = 1;
    public Bottle Bottle;

    [Header("References")]
    public FlavorMapping flavorMapping;
    public BottleMapping bottleMapping;
    public SpriteRenderer bottleSprite;
    public SpriteRenderer liquidSprite;

    public Ability onCollectAbility;

    public void UpdateVisual()
    {
        if (flavorMapping.TryGet(Flavors, out var flavorData))
            liquidSprite.color = flavorData.color;

        if(bottleMapping.TryGet(Bottle, out var bottleData))
        {
            bottleSprite.sprite = bottleData.bottleSprite;
            liquidSprite.sprite = bottleData.liquidSprite;
        }
    }

    public void Initialize(Ability ability)
    {
        if (onCollectAbility != null)
            onCollectAbility.Deactivate();

        onCollectAbility = ability;
        onCollectAbility.Activate();
    }

    private void OnDestroy()
    {
        if (onCollectAbility != null)
            onCollectAbility.Deactivate();
    }
}
