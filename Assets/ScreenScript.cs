using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ScreenScript : MonoBehaviour
{
    private const string fileName = "image.png";
    private RenderTexture originalTexture;
    private RenderTexture[] textures;
    private RenderTexture thumbnailTexture;

    private MeshRenderer meshRenderer;
    private Material meshMaterial;
    private bool dirty;

    private ScreenModeType screenMode;
    private float verticalDistotion;
    private float horizontalDistotion;
    private bool softFilter;
    private bool sharpFilter;

    [SerializeField]
    private Material sharpenMaterial = null;
    [SerializeField]
    private Material softMaterial = null;
    [SerializeField]
    private Material distortionMaterial = null;

    public enum ScreenModeType
    {
        Normal8K,
        Distotion4K,
        Normal4K,
        Distotion2K,
        Normal2K,
    };

    public ScreenModeType ScreenMode
    {
        get
        {
            return screenMode;
        }
        set
        {
            screenMode = value;
            dirty = true;
        }
    }
    public float VerticalDistotion
    {
        get
        {
            return verticalDistotion;
        }
        set
        {
            verticalDistotion = value;
            dirty = true;
        }
    }
    public float HorizontalDistotion
    {
        get
        {
            return horizontalDistotion;
        }
        set
        {
            horizontalDistotion = value;
            dirty = true;
        }
    }
    public bool SoftFilter
    {
        get
        {
            return softFilter;
        }
        set
        {
            softFilter = value;
            dirty = true;
        }
    }
    public bool SharpFilter
    {
        get
        {
            return sharpFilter;
        }
        set
        {
            sharpFilter = value;
            dirty = true;
        }
    }
    public Texture Thumbnail
    {
        get
        {
            return thumbnailTexture;
        }
    }
    public bool Updated
    {
        get;
        private set;
    }
    private void OnEnable()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshMaterial = new Material(meshRenderer.sharedMaterial);
        meshRenderer.sharedMaterial = meshMaterial;
        verticalDistotion = 1.5f;
        horizontalDistotion = 1.7f;
        meshMaterial.SetFloat("_VDst", verticalDistotion);
        meshMaterial.SetFloat("_HDst", horizontalDistotion);
        screenMode = ScreenModeType.Normal8K;
        textures = new RenderTexture[6];
        textures[0] = new RenderTexture(8192, 4096, 0);
        textures[1] = new RenderTexture(8192, 4096, 0);
        textures[2] = new RenderTexture(4096, 2048, 0);
        textures[3] = new RenderTexture(4096, 2048, 0);
        textures[4] = new RenderTexture(2048, 1024, 0);
        textures[5] = new RenderTexture(2048, 1024, 0);
        thumbnailTexture = new RenderTexture(256, 128, 0);
        if (!string.IsNullOrEmpty(fileName))
        {
            StartCoroutine(LoadTexture());
        }
    }
    private void OnDisable()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material.SetTexture("_MainTex", null);
        }
        meshRenderer = null;
        if (originalTexture != null)
        {
            Destroy(originalTexture);
        }
        originalTexture = null;
        if (textures != null)
        {
            foreach (var tex in textures)
            {
                if (tex != null)
                {
                    Destroy(tex);
                }
            }
        }
        textures = null;
    }
    private void Swap<T>(ref T v0, ref T v1)
    {
        T vt = v0;
        v0 = v1;
        v1 = vt;
    }
    private void Blits(RenderTexture tex0, RenderTexture tex1, Material mat)
    {
        if (mat != null)
        {
            Graphics.Blit(originalTexture, tex0, mat);
            Graphics.Blit(originalTexture, thumbnailTexture, mat);
        }
        else
        {
            Graphics.Blit(originalTexture, tex0);
            Graphics.Blit(originalTexture, thumbnailTexture);
        }
        if (SoftFilter && (softMaterial != null))
        {
            Graphics.Blit(tex0, tex1, softMaterial);
            Swap(ref tex0, ref tex1);
        }
        if (SharpFilter && (sharpenMaterial != null))
        {
            Graphics.Blit(tex0, tex1, sharpenMaterial);
            Swap(ref tex0, ref tex1);
        }
        meshMaterial.SetTexture("_MainTex", tex0);
    }
    private void Update()
    {
        if (originalTexture == null)
        {
            return;
        }
        if (dirty)
        {
            dirty = false;
            if (meshRenderer != null)
            {
                distortionMaterial.SetFloat("_VDist", verticalDistotion);
                distortionMaterial.SetFloat("_HDist", horizontalDistotion);
                var hdst = horizontalDistotion;
                var vdst = verticalDistotion;
                switch (screenMode)
                {
                    case ScreenModeType.Normal8K:
                        Blits(textures[0], textures[1], null);
                        hdst = 1.0f;
                        vdst = 1.0f;
                        break;
                    case ScreenModeType.Distotion4K:
                        Blits(textures[2], textures[3], distortionMaterial);
                        break;
                    case ScreenModeType.Normal4K:
                        Blits(textures[2], textures[3], null);
                        hdst = 1.0f;
                        vdst = 1.0f;
                        break;
                    case ScreenModeType.Distotion2K:
                        Blits(textures[4], textures[5], distortionMaterial);
                        break;
                    case ScreenModeType.Normal2K:
                        Blits(textures[4], textures[5], null);
                        hdst = 1.0f;
                        vdst = 1.0f;
                        break;
                }
                meshMaterial.SetFloat("_VDist", vdst);
                meshMaterial.SetFloat("_HDist", hdst);
            }
            Updated = true;
        }
        else
        {
            Updated = false;
        }
    }

    IEnumerator LoadTexture()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        var path = "jar:file://" + Application.dataPath + "!/assets/";
#else
        var path = Application.dataPath + "/StreamingAssets/";
#endif
        var url = path + fileName;
        using (var request = UnityWebRequestTexture.GetTexture(url, true))
        {
            yield return request.SendWebRequest();

            if (request.isDone)
            {
                var handler = request.downloadHandler as DownloadHandlerTexture;
                var texture = handler.texture;
                originalTexture = new RenderTexture(texture.width, texture.height, 0);
                originalTexture.useMipMap = false;
                originalTexture.filterMode = FilterMode.Bilinear;
                Graphics.Blit(texture, originalTexture);
                dirty = true;
            }
        }
    }
}
