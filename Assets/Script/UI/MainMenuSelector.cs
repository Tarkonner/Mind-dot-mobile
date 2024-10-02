using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSelector : MonoBehaviour
{
    [SerializeField] GameObject[] menuSections;

    [SerializeField] GameObject levelsSections;

    private void Start()
    {
        if(SceneController.Instance != null && SceneController.Instance.showLevels)
        {
            SectionsSelect(levelsSections);
            SceneController.Instance.HasLoadedLevel();
        }
    }

    public void SectionsSelect(GameObject openSection)
    {
        foreach (GameObject section in menuSections)
        {
            if (openSection != section)
                section.SetActive(false);
            else
                section.SetActive(true);
        }
    }
}
