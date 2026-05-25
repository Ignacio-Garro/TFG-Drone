using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class buttonbackgroundchanger : MonoBehaviour
{
    [SerializeField] private Image background;

    private static readonly Color darkUnselected = new Color(0x4B / 255f, 0x4B / 255f, 0x4B / 255f);
    private static readonly Color lightSelected   = new Color(0x93 / 255f, 0x00 / 255f, 0x02 / 255f);

    private Color targetColor;
    private bool enforceColor = false;

    void Awake() => targetColor = darkUnselected;

    void LateUpdate()
    {
        if (enforceColor)
            background.color = targetColor;
    }

    public void SetDarkUnselected() => StartCoroutine(SetColorDelayed(darkUnselected));

    public void SetLightSelected() => StartCoroutine(SetColorDelayed(lightSelected));

    private IEnumerator SetColorDelayed(Color color)
    {
        yield return new WaitForSeconds(0.5f);
        targetColor = color;
        enforceColor = true;
    }
}
