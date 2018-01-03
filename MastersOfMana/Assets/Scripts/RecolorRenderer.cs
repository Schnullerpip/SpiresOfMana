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
            rendererProperties[i] = new RendererProperty
            {
                renderer = renderers[i]
            };
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

    [System.Serializable]
    public class RendererProperty
    {
        public Renderer renderer;
        public string propertyName = "_Color";

        public int propertyID
        {
            get
            {
                if(mPropertyID == null)
                {
                    mPropertyID = Shader.PropertyToID(propertyName);
                }
                return mPropertyID.Value;
            }
        }

        private int? mPropertyID;

        public Color GetColor()
        {
            return renderer.material.GetColor(propertyID);
        }

        public void SetColor(Color col)
        {
            renderer.material.SetColor(propertyID, col);
        }
    }
}
