using System.Collections.Generic;
using UnityEngine;

public class HighlightOnHover : MonoBehaviour
{

    public Color highlightEmission = Color.yellow;
    public float intensity = 2f;

    private readonly List<Material> materials = new();
    private readonly List<Color> originalEmissions = new();

    private static readonly string EMISSION_COLOR = "_EmissionColor";
    private static readonly string EMISSION_KEYWORD = "_EMISSION";

    void Start()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(includeInactive: true);

        foreach (Renderer renderer in renderers)
        {
            Material[] rendererMaterials = renderer.materials;

            foreach(Material material in rendererMaterials)
            {
                if (material.HasProperty(EMISSION_COLOR))
                {
                    materials.Add(material);
                    originalEmissions.Add(material.GetColor(EMISSION_COLOR));
                }

            }
        }
    }

    private void OnMouseEnter()
    {
        HandleOnHover(true);
    }

    private void OnMouseExit()
    {
        HandleOnHover(false);
    }

    private void HandleOnHover(bool onEnter)
    {
        for (int i = 0; i < materials.Count; i++)
        {
            Material mat = materials[i];
            mat.SetColor(EMISSION_COLOR, onEnter ? highlightEmission * intensity : originalEmissions[i]);

            if (onEnter)
                mat.EnableKeyword(EMISSION_KEYWORD);
            else
                mat.DisableKeyword(EMISSION_KEYWORD);
        }
    }
}