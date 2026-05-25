using UnityEngine;

public class cambiarmaterialguias : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;

    [SerializeField] private Material guia1;
    [SerializeField] private Material guia2;
    [SerializeField] private Material guia3;
    [SerializeField] private Material guia4;

    public void SetGuia1() => targetRenderer.material = guia1;
    public void SetGuia2() => targetRenderer.material = guia2;
    public void SetGuia3() => targetRenderer.material = guia3;
    public void SetGuia4() => targetRenderer.material = guia4;
}

