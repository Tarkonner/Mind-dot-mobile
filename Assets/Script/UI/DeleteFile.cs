using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteFile : MonoBehaviour
{
    public void DeleteSaveFile()
    {
        FindAnyObjectByType<SaveSystem>().DeleteFile();
    }
}
