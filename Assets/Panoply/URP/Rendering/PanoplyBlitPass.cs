using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Opertoon.Panoply;

internal class PanoplyBlitPass : ScriptableRenderPass
{
    ProfilingSampler m_ProfilingSampler = new ProfilingSampler("PanoplyBlit");
    Material m_Material;
    RTHandle m_CameraColorTarget;


    private Panel[] _panels;
    private SimplePanel[] _simplePanels;
    private Camera _backgroundCamera;
    private float[] _borderSize;
    private float[] _offscreenLeftEdge;
    private float[] _offscreenRightEdge;
    private float[] _offscreenTopEdge;
    private float[] _offscreenBottomEdge;
    private Color[] _borderColor;
    private Color[] _tintColor;
    private float[] _cameraPos;
    private Matrix4x4[] _projectionMatrix;
    PanoplyCore _core;


    public PanoplyBlitPass(Material material)
    {
        m_Material = material;
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

        _panels = Object.FindObjectsByType<Panel>(FindObjectsSortMode.None) as Panel[];
        _simplePanels = Object.FindObjectsByType<SimplePanel>(FindObjectsSortMode.None) as SimplePanel[];
        if (_backgroundCamera == null)
        {
            _core = Object.FindAnyObjectByType<PanoplyCore>();
            if (_core != null)
            {
                _backgroundCamera = _core.transform.Find("Background Camera").GetComponent<Camera>();
            }
        }

        _borderSize = new float[24];
        _borderColor = new Color[24];
        _offscreenLeftEdge = new float[24];
        _offscreenRightEdge = new float[24];
        _offscreenTopEdge = new float[24];
        _offscreenBottomEdge = new float[24];
        _tintColor = new Color[24];
        _projectionMatrix = new Matrix4x4[24];
        _cameraPos = new float[72];
    }

    public void SetTarget(RTHandle colorHandle)
    {
        m_CameraColorTarget = colorHandle;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        ConfigureTarget(m_CameraColorTarget);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var cameraData = renderingData.cameraData;
        if (cameraData.camera.cameraType != CameraType.Game)
            return;

        if (m_Material == null)
            return;


        int n;
        n = Mathf.Min(_panels.Length, 24);
        Panel panel;
        int pc = 0;
        for (int i = 0; i < n; i++)
        {
            panel = _panels[i];
            if (panel.camera)
            {
                _borderSize[pc] = (panel.borderSize * 2) * PanoplyCore.resolutionScale;
                _borderColor[pc] = panel.borderColor.linear;
                _tintColor[pc] = panel.matteColor.linear;
                _projectionMatrix[i] = GL.GetGPUProjectionMatrix(panel.camera.projectionMatrix, false);
                _cameraPos[pc * 3] = panel.transform.position.x;
                _cameraPos[pc * 3 + 1] = panel.transform.position.y;
                _cameraPos[pc * 3 + 2] = panel.transform.position.z;
                if (PanoplyCore.panoplyRenderer != null)
                {
                    _offscreenLeftEdge[pc] = Mathf.Abs(Mathf.Max(PanoplyCore.panoplyRenderer.screenRect.xMin - Mathf.Max(panel.frameRect.xMin, 0), 0));
                    _offscreenRightEdge[pc] = (panel.frameRect.width + Mathf.Min(panel.frameRect.xMin, 0)) - Mathf.Max(panel.frameRect.xMax - PanoplyCore.panoplyRenderer.screenRect.xMax, 0);
                    _offscreenBottomEdge[pc] = Mathf.Abs(Mathf.Max(PanoplyCore.panoplyRenderer.screenRect.yMin - Mathf.Max(panel.frameRect.yMin, 0), 0));
                    _offscreenTopEdge[pc] = panel.frameRect.height - Mathf.Max(panel.frameRect.yMax - PanoplyCore.panoplyRenderer.screenRect.yMax, 0);
                }
                pc++;
            }
        }

        n = Mathf.Min(_simplePanels.Length, 24 - _panels.Length);
        SimplePanel simplePanel;
        for (int i = 0; i < n; i++)
        {
            simplePanel = _simplePanels[i];
            if (simplePanel.camera)
            {
                _borderSize[pc] = (simplePanel.borderSize * 2) * PanoplyCore.resolutionScale;
                _borderColor[pc] = simplePanel.borderColor.linear;
                _tintColor[pc] = simplePanel.matteColor.linear;
                _projectionMatrix[pc] = GL.GetGPUProjectionMatrix(simplePanel.camera.projectionMatrix, false);
                _cameraPos[pc * 3] = simplePanel.transform.position.x;
                _cameraPos[pc * 3 + 1] = simplePanel.transform.position.y;
                _cameraPos[pc * 3 + 2] = simplePanel.transform.position.z;
                if (PanoplyCore.panoplyRenderer != null)
                {
                    _offscreenLeftEdge[pc] = Mathf.Abs(Mathf.Max(PanoplyCore.panoplyRenderer.screenRect.xMin - Mathf.Max(simplePanel.frameRect.xMin, 0), 0));
                    _offscreenRightEdge[pc] = (simplePanel.frameRect.width + Mathf.Min(simplePanel.frameRect.xMin, 0)) - Mathf.Max(simplePanel.frameRect.xMax - PanoplyCore.panoplyRenderer.screenRect.xMax, 0);
                    _offscreenBottomEdge[pc] = Mathf.Abs(Mathf.Max(PanoplyCore.panoplyRenderer.screenRect.yMin - Mathf.Max(simplePanel.frameRect.yMin, 0), 0));
                    _offscreenTopEdge[pc] = simplePanel.frameRect.height - Mathf.Max(simplePanel.frameRect.yMax - PanoplyCore.panoplyRenderer.screenRect.yMax, 0);
                }
                pc++;
            }
        }

        m_Material.SetColorArray("_TintColor", _tintColor);
        if (_backgroundCamera != null)
        {
            m_Material.SetColor("_BackgroundColor", _backgroundCamera.backgroundColor.linear);
        }

        m_Material.SetFloatArray("_BorderSize", _borderSize);
        m_Material.SetColorArray("_BorderColor", _borderColor);
        m_Material.SetFloatArray("_OffscreenLeftEdge", _offscreenLeftEdge);
        m_Material.SetFloatArray("_OffscreenRightEdge", _offscreenRightEdge);
        m_Material.SetFloatArray("_OffscreenBottomEdge", _offscreenBottomEdge);
        m_Material.SetFloatArray("_OffscreenTopEdge", _offscreenTopEdge);
        m_Material.SetMatrixArray("_ProjectionMatrix", _projectionMatrix);
        m_Material.SetFloatArray("_CameraPos", _cameraPos);

        if (PanoplyCore.panoplyRenderer != null)
        {
            m_Material.SetFloat("_ScreenLeft", PanoplyCore.panoplyRenderer.screenRect.xMin);
            m_Material.SetFloat("_ScreenRight", PanoplyCore.panoplyRenderer.screenRect.xMax);
            m_Material.SetFloat("_ScreenBottomInverted", Screen.height - PanoplyCore.panoplyRenderer.screenRect.yMin);
            m_Material.SetFloat("_ScreenTopInverted", Screen.height - PanoplyCore.panoplyRenderer.screenRect.yMax);
            m_Material.SetFloat("_ScreenBottom", PanoplyCore.panoplyRenderer.screenRect.yMin);
            m_Material.SetFloat("_ScreenTop", PanoplyCore.panoplyRenderer.screenRect.yMax);
        }

        CommandBuffer cmd = CommandBufferPool.Get();

        Blitter.BlitCameraTexture(cmd, m_CameraColorTarget, m_CameraColorTarget, m_Material, 0);

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        CommandBufferPool.Release(cmd);
    }
}