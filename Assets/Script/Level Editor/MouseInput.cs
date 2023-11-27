using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseInput : MonoBehaviour
{
    GraphicRaycaster canvasRaycast;
    PointerEventData pointerEventData;    

    //[SerializeField] LevelEditor levelEditor;

    [SerializeField] GameObject cellSpawner;

    [SerializeField] EditBoard boardEditor;

    void Start()
    {
        canvasRaycast = GetComponent<GraphicRaycaster>();
        pointerEventData = new PointerEventData(EventSystem.current);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Create a pointer event data with the current input position        
            pointerEventData.position = Input.mousePosition;
            // Create a list to store the raycast results
            List<RaycastResult> results = new List<RaycastResult>();

            // Raycast using the GraphicRaycaster
            EventSystem.current.RaycastAll(pointerEventData, results);

            foreach (var item in results)
            {
                if(item.gameObject.TryGetComponent(out CellSpawner spawner))
                {
                    boardEditor.grid[spawner.gridPosition.x, spawner.gridPosition.y].gameObject.SetActive(true);
                    boardEditor.spawnerGrid[spawner.gridPosition.x, spawner.gridPosition.y].gameObject.SetActive(false);
                }
                else if (item.gameObject.TryGetComponent(out Cell cell))
                {
                    boardEditor.grid[cell.gridPos.x, cell.gridPos.y].gameObject.SetActive(false);
                    boardEditor.spawnerGrid[cell.gridPos.x, cell.gridPos.y].gameObject.SetActive(true);
                }
            }
        }
    }
}
