using UnityEngine;
using UnityEngine.UI;

public class WebCamDisplay2 : MonoBehaviour
{
    // Si usas un RawImage para mostrar el feed:
    public RawImage display;

    private WebCamTexture webcamTexture;

    void Start()
    {
        // Obtiene la lista de dispositivos de cámara disponibles
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            // Selecciona la primera cámara (puedes modificar este índice o crear una UI para elegir)
            string camName = devices[0].name;
            webcamTexture = new WebCamTexture(camName);

            // Si el objeto tiene un componente Renderer (para un plano/quad)
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.mainTexture = webcamTexture;
            }
            // Si se usa UI (RawImage), asigna la textura de la webcam:
            if (display != null)
            {
                display.texture = webcamTexture;
            }

            // Inicia la captura de la cámara
            webcamTexture.Play();
        }
        else
        {
            Debug.LogError("No se encontró ninguna cámara conectada.");
        }
    }

    void OnDestroy()
    {
        // Detén la cámara al destruir el objeto para liberar recursos
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }
}
