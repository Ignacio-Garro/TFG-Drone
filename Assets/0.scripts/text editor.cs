using UnityEngine;
using TMPro;

public class texteditor : MonoBehaviour
{
    [SerializeField] private TMP_Text textMeshPro; 
    
    public void SetText(string newText)
    {
        if (textMeshPro != null)
            textMeshPro.text = newText;
        else
            Debug.LogWarning("TextMeshPro reference not set in " + gameObject.name);
    }
}
