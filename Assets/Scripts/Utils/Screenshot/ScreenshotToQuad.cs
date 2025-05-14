using System.Collections;
using UnityEngine;

public class ScreenshotToQuad : MonoBehaviour
{
    public GameObject quad; 
    
    private Material quadMaterial;

    void Start()
    {
        quadMaterial = new Material(quad.GetComponent<Renderer>().material);
    }

    public void CaptureScreenshot()
    {
        StartCoroutine(CaptureAndApply());
    }

    IEnumerator CaptureAndApply()
    {
        yield return new WaitForEndOfFrame();

        Texture2D screenshotTexture = ScreenCapture.CaptureScreenshotAsTexture();

        Texture2D highQualityTexture = new Texture2D(screenshotTexture.width, screenshotTexture.height, TextureFormat.RGBA32, false);
        highQualityTexture.SetPixels(screenshotTexture.GetPixels());
        highQualityTexture.Apply();
        quadMaterial.mainTexture = highQualityTexture;
        quad.GetComponent<MeshRenderer>().material = quadMaterial;
        yield return new WaitForEndOfFrame();

        QuadShatter.Instance.Shatter(quad.GetComponent<MeshRenderer>());
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CaptureScreenshot();
        }
    }
}
