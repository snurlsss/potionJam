using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Contract : MonoBehaviour
{
    public Ability ability;
    public TMP_Text displayName;
    public SpriteRenderer back;
    public SpriteRenderer front;
    public SpriteRenderer logo;

    public void Initialize(ContractSO data, AbilityFactory abilityFactory)
    {
        //displayName.text = data.companyName;

        front.color = data.frontColor;
        back.color = data.backColor;
        logo.sprite = data.logo;

        abilityFactory.TryGetAbilityFromID(data.abilityId, out ability);
        ability?.Activate();
    }

    private void OnEnable()
    {
        ability?.Activate();
    }

    private void OnDisable()
    {
        ability?.Deactivate();
    }
}
