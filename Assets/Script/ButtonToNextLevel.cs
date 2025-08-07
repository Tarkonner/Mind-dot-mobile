using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonToNextLevel : MonoBehaviour
{
    

    [Header("Animation")]
    // Animation settings
    [SerializeField] float scaleAnimatiomTime = 1.2f;

    

    private void Start()
    {
        
        transform.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        StartCoroutine(ScaleUP());
    }

    IEnumerator ScaleUP()
    {
        yield return new WaitForSeconds(.1f);
        transform.DOScale(Vector3.one, scaleAnimatiomTime);
    }

    public void ScaleDown()
    {
        transform.DOScale(Vector3.zero, scaleAnimatiomTime)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
