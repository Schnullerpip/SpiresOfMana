using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolorRenderer : ARecolorSingleColor
{
    public RendererProperty[] rendererProperties;

    protected override void Init()
    {
        mOriginalColors = new Color[rendererProperties.Length];
        for (int i = 0; i < mOriginalColors.Length; ++i)
        {
            mOriginalColors[i] = rendererProperties[i].GetColor();
        }
    }

    [ContextMenu("GetRendererInChildren")]
    private void GetRendererInChildren()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        rendererProperties = new RendererProperty[renderers.Length];

        for (int i = 0; i < rendererProperties.Length; ++i)
        {
            rendererProperties[i] = new RendererProperty();
            rendererProperties[i].renderer = renderers[i];

            rendererProperties[i].Validate();
        }
    }

    public override void RevertColor()
    {
        for (int i = 0; i < rendererProperties.Length; ++i)
        {
            rendererProperties[i].SetColor(mOriginalColors[i]);
        }
    }

    public override void RevertColor(float t)
    {
        for (int i = 0; i < rendererProperties.Length; ++i)
        {
            rendererProperties[i].SetColor(Color.Lerp(rendererProperties[i].GetColor(), mOriginalColors[i], t));
        }
    }

    public override void ChangeColorTo(Color col)
    {
        for (int i = 0; i < rendererProperties.Length; ++i)
        {
            rendererProperties[i].SetColor(col);
        }
    }

    public override void ChangeColorTo(Color col, float t)
    {
        for (int i = 0; i < rendererProperties.Length; ++i)
        {
            rendererProperties[i].SetColor(Color.Lerp(rendererProperties[i].GetColor(), col, t));
        }
    }

    private void OnValidate()
    {
        for (int i = 0; i < rendererProperties.Length; ++i)
        {
            rendererProperties[i].Validate();
        }
    }

    [System.Serializable]
    public class RendererProperty
    {
        public Renderer renderer;
        public string propertyName;

        [ReadOnly]
        public int propertyId;

        internal void Validate()
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                propertyName = "_Color";
            }
            propertyId = Shader.PropertyToID(propertyName);
        }

        public Color GetColor()
        {
            return renderer.material.GetColor(propertyId);
        }

        public void SetColor(Color col)
        {
            renderer.material.SetColor(propertyId, col);
        }
    }
}
