using UnityEngine;
using UnityEngine.UI;

public class WebCamDisplay : MonoBehaviour
{
    public RawImage display; // Referencia al UI RawImage para mostrar el feed de la webcam
    private WebCamTexture webcamTexture;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            string camName = devices[0].name;
            webcamTexture = new WebCamTexture(camName);
            display.texture = webcamTexture;
            webcamTexture.Play();
        }
        else
        {
            Debug.LogError("No se encontró ninguna cámara conectada.");
        }
    }

    void OnDestroy()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }
}