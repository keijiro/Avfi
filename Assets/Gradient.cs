using UnityEngine;

sealed class Gradient : MonoBehaviour
{
    [SerializeField, HideInInspector] Shader _shader = null;

    Material _material;

    void Start()
      => _material = new Material(_shader);

    void OnRenderImage(RenderTexture source, RenderTexture destination)
      => Graphics.Blit(source, destination, _material, 0);
}
