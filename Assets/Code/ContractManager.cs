using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class ContractManager : MonoBehaviour
{
    public List<Contract> spawnedContracts;
    public int maxContrats = 5;
    [Header("References")]
    public AbilityFactory abilityFactory;
    public Transform contractParent;

    [Header("Testing")]
    public List<ContractSO> contractSOs;

    private void Start()
    {
        foreach(var c in contractSOs)
        {
            CreateContract(c);
        }
    }

    public void CreateContract(ContractSO data)
    {
        if (spawnedContracts.Count >= maxContrats)
            return;

        var spawnedContract = Instantiate(data.visual, contractParent);
        spawnedContract.Initialize(data, abilityFactory);

        spawnedContract.transform.position = contractParent.transform.position + (Vector3.right * (2 * spawnedContracts.Count));

        spawnedContracts.Add(spawnedContract);
    }
}
