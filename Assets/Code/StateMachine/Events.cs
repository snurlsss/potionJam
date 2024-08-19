using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class ActivationContext
{
    /// <summary>
    /// Who activated the ability.
    /// </summary>
    public GameObject Source { get; }


    /// <summary>
    /// Who owns the ability.
    /// </summary>
    public GameObject Target { get; }

    public ActivationContext(GameObject source, GameObject target)
    {
        Source = source;
        Target = target;
    }
}

public class PotionCollectEvent
{
    public GameObject CollectedObject { get; }

    public PotionCollectEvent(GameObject collectedObject)
    {
        CollectedObject = collectedObject;
    }
}

public class PotionChangeBottleEvent
{
    public Bottle PrevType { get; }
    public Bottle NewType { get; }
    public Potion Potion { get; }

    public PotionChangeBottleEvent(Bottle prevType, Bottle newType, Potion potion)
    {
        PrevType = prevType;
        NewType = newType;
        Potion = potion;
    }
}

public class PotionChangeFlavorEvent
{
    public Flavor AddedFlavor { get; }
    public Potion Potion { get; }

    public PotionChangeFlavorEvent(Flavor addedFlavor, Potion potion)
    {
        AddedFlavor = addedFlavor;
        Potion = potion;
    }
}

public class PotionShiftEvent
{
    public int OldBeltIndex { get; }
    public int NewBeltIndex { get; }
    public Potion Potion { get; }

    public PotionShiftEvent(int oldBeltIndex, int newBeltIndex, Potion potion)
    {
        OldBeltIndex = oldBeltIndex;
        NewBeltIndex = newBeltIndex;
        Potion = potion;
    }

}



public class PassedEvent
{
    public GameObject PassingObject { get; }
    public Tile TilePassed { get; }

    public PassedEvent(GameObject passingObject, Tile tilePassed)
    {
        PassingObject = passingObject;
        TilePassed = tilePassed;
    }
}
