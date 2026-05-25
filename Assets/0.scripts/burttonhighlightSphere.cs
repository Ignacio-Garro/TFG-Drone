using UnityEngine;

public class burttonhighlightSphere : MonoBehaviour
{
    [SerializeField] private GameObject target;

    public void Show()   => target.SetActive(true);
    public void Hide()   => target.SetActive(false);
}
