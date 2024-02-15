using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ListViewContainer : VisualElement
{
    private ListView listView;

    public ListViewContainer()
    {
        listView = new ListView();
        Add(listView);
    }
}
