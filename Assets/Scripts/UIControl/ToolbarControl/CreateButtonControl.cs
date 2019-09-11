using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateButtonControl : MonoBehaviour
{
    public RecordsControl records;
    public void Create() => records.CreateNew();
}
