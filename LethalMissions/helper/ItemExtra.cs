using UnityEngine;
using System.Collections.Generic;

public class ItemExtra : MonoBehaviour
{
    public string uniqueID;


    private void Awake()
    {
        uniqueID = System.Guid.NewGuid().ToString();
    }
}