using System.Collections.Generic;
using UnityEngine;

public class HighlightOnHover : MonoBehaviour
{

    [SerializeField] public Color highlightEmission = Color.yellow;
    [SerializeField] public float intensity = 2f;

    private List<Material> materials = new List<Material>();
    private List<Color> originalEmissions = new List<Color>();

    void Start()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(includeInactive: true);

        if (renderers.Length == 0)
        {
            Debug.LogWarning($"{name}: no renderers found in children.");
            return;
        }

        foreach (Renderer r in renderers)
        {
            Material mat = r.material;

            if (mat.HasProperty("_EmissionColor"))
            {
                materials.Add(mat);
                originalEmissions.Add(mat.GetColor("_EmissionColor"));
            }
        }
    }

    private void OnMouseEnter()
    {
        for (int i = 0; i < materials.Count; i++)
        {
            Material mat = materials[i];
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", highlightEmission * intensity);
        }
    }

    private void OnMouseExit()
    {
        for (int i = 0; i < materials.Count; i++)
        {
            Material mat = materials[i];
            mat.SetColor("_EmissionColor", originalEmissions[i]);
            mat.DisableKeyword("_EMISSION");
        }
    }
}