using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFactory : MonoBehaviour
{
    public EventHubSO hub;
    public GameManager gameManager;
    Dictionary<string, Func<Ability>> builders = new();
    Ability a;

    private void Awake()
    {
        InitializeBuilders();
        a = TestAbility();
        a.Activate();
    }

    public bool TryGetAbilityFromID(string abilityId, out Ability newAbility)
    {
        if (builders.ContainsKey(abilityId))
        {
            newAbility = builders[abilityId]();
            newAbility.Name = abilityId;
            return true;
        }

        Debug.LogWarning($"No key ability found for ID: {abilityId}");

        newAbility = null;
        return false;
    }

    void InitializeBuilders()
    {
        builders.Add("0", TestAbility);

        builders.Add("ChangeColorRed", ChangePotionFlavorRed);
        builders.Add("ChangeColorYellow", ChangePotionFlavorYellow);
        builders.Add("ChangeColorBlue", ChangePotionFlavorBlue);
        builders.Add("ChangeColorGreen", ChangePotionFlavorGreen);

        builders.Add("testContract", TestContract);
        builders.Add("PotionCollect", PotionCollect);

        builders.Add("likesGreen", LikesGreen);

        builders.Add("BottleSquare", BottleSquare);
        builders.Add("BottleOval", BottleOval);
        builders.Add("BottleDiamond", BottleDiamond);

        builders.Add("ShiftPotionUp", ShiftPotionUp);
        builders.Add("ShiftPotionDown", ShiftPotionDown);
        builders.Add("ShiftPotionFlip", ShiftPotionFlip);

        builders.Add("ShiftPotionDownRed", ShiftPotionDownRed);
    }

    Ability TestAbility()
    {
        var ability = new Ability();
        ability.AddTrigger<PotionCollectEvent>(hub.Instance, (args, context) =>
        {
            var currentValue = args.TryGet("Count", out var countValue) ? (int)countValue : 0;
            Debug.Log($"I heard that {context.CollectedObject.name} passed {currentValue} tiles.");
        });

        ability.AddTrigger<PassedEvent>(hub.Instance, (args, context) =>
        {
            var currentValue = args.TryGet("Count", out var countValue) ? (int)countValue : 0;
            args.Set("Count", currentValue + 1);
        });

        return ability;
    }

    Ability ChangePotionFlavorRed()
    {
        var ability = new Ability();
        ability.AddActive((args, context) => { PotionBottleFlavorChange(context.Source, Flavor.Red); });

        return ability;
    }


    Ability ChangePotionFlavorBlue()
    {
        var ability = new Ability();
        ability.AddActive((args, context) => { PotionBottleFlavorChange(context.Source, Flavor.Blue); });

        return ability;
    }

    Ability ChangePotionFlavorYellow()
    {
        var ability = new Ability();
        ability.AddActive((args, context) => { PotionBottleFlavorChange(context.Source, Flavor.Yellow); });

        return ability;
    }

    Ability ChangePotionFlavorGreen()
    {
        var ability = new Ability();
        ability.AddActive((args, context) => { PotionBottleFlavorChange(context.Source, Flavor.Green);});

        return ability;
    }

    Ability TestContract()
    {
        var ability = new Ability();
        ability.AddTrigger<PotionCollectEvent>(hub.Instance, (args, context) =>
        {
            if(context.CollectedObject.TryGetComponent(out Potion potion) && potion.Flavors == Flavor.Red)
            {
                gameManager.ChangeScore(50);
            }
        });

        return ability;
    }

    Ability PotionCollect()
    {
        var ability = new Ability();
        ability.AddActive((args, context) =>
        {
            if(context.Source.TryGetComponent(out Potion potion))
            {
                int total = GameManager.CalculateScoreFromPotion(potion);
                gameManager.ChangeScore(total);
            }
        });

        return ability;
    }

    Ability LikesGreen()
    {
        var ability = new Ability();
        ability.AddTrigger<PotionCollectEvent>(hub.Instance, (args, context) =>
        {
            if (context.CollectedObject.TryGetComponent(out Potion potion))
            {
                if(potion.Flavors == Flavor.Green)
                {
                    gameManager.ChangeScore(5);
                }
            }
        });
        return ability;
    }


    Ability BottleSquare()
    {
        var ability = new Ability();
        ability.AddActive((args, context) => { PotionBottleTypeChange(context.Source, Bottle.Square); });
        return ability;
    }

    Ability BottleOval()
    {
        var ability = new Ability();
        ability.AddActive((args, context) => { PotionBottleTypeChange(context.Source, Bottle.Oval); });
        return ability;
    }

    Ability BottleDiamond()
    {
        var ability = new Ability();
        ability.AddActive((args, context) => { PotionBottleTypeChange(context.Source, Bottle.Diamond); });
        return ability;
    }

    private Ability ShiftPotionUp()
    {
        var ability = new Ability();
        ability.AddActive((args, context) =>
        {
            if(context.Source.TryGetComponent(out Potion potion))
            {
                gameManager.ShiftPotion(potion, -1);
            }
        });
        return ability;
    }

    private Ability ShiftPotionDown()
    {
        var ability = new Ability();
        ability.AddActive((args, context) =>
        {
            if (context.Source.TryGetComponent(out Potion potion))
            {
                gameManager.ShiftPotion(potion, 1);
            }
        });
        return ability;
    }

    private Ability ShiftPotionFlip()
    {
        var ability = new Ability();
        ability.AddActive((args, context) =>
        {
            if (context.Source.TryGetComponent(out Potion potion))
            {
                var key = "FlipState";
                var flipState = args.TryGet(key, out var state) ? (bool)state : false;

                var flipValue = flipState ? 1 : -1;

                gameManager.ShiftPotion(potion, flipValue);

                args.Set(key, !flipState);
            }
        });
        return ability;
    }

    private Ability ShiftPotionDownRed()
    {
        var ability = new Ability();
        ability.AddActive((args, context) =>
        {
            if (context.Source.TryGetComponent(out Potion potion) && potion.Flavors == Flavor.Red)
            {
                gameManager.ShiftPotion(potion, 1);
            }
        });
        return ability;
    }


    void PotionBottleFlavorChange(GameObject source, Flavor flavor)
    {
        if (source.TryGetComponent(out Potion potion))
            gameManager.ChangePotionFlavor(potion, flavor);
    }

    void PotionBottleTypeChange(GameObject source, Bottle bottle)
    {
        if (source.TryGetComponent(out Potion potion))
            gameManager.ChangePotionBottle(potion, bottle);
    }
}
