using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Windows.Forms;

public class OpenButtonControl : MonoBehaviour
{
    public RecordsControl records;
    public void OpenFile()
    {
        string fileName = null;
        using(var dialog = new OpenFileDialog())
        {
            dialog.ShowDialog();
            if(string.IsNullOrEmpty(fileName = dialog.FileName)) return;
        }
        records.OpenFile(fileName);
    }
}
