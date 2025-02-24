using UnityEngine;
using OpenCvSharp;

public class WebCamCupTracker : MonoBehaviour
{
    public Renderer displayRenderer; // Plano para mostrar la webcam
    public Transform cupVirtual; // Objeto 3D del vaso en Unity
    public float smoothFactor = 0.1f; // Suavizado del movimiento

    private WebCamTexture webcamTexture;
    private Mat frameMat;

    void Start()
    {
        // Inicializar la webcam
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            webcamTexture = new WebCamTexture(devices[0].name);
            displayRenderer.material.mainTexture = webcamTexture;
            webcamTexture.Play();
        }
        else
        {
            Debug.LogError("No se encontró ninguna cámara conectada.");
        }
    }

    void Update()
    {
        if (webcamTexture.didUpdateThisFrame)
        {
            frameMat = OpenCvSharp.Unity.TextureToMat(webcamTexture);

            // Convertir a HSV para detectar color amarillo
            Mat hsv = new Mat();
            Cv2.CvtColor(frameMat, hsv, ColorConversionCodes.BGR2HSV);

            // Rango de color para detectar el vaso amarillo
            Scalar lowerColor = new Scalar(14, 50, 50);
            Scalar upperColor = new Scalar(34, 255, 255);

            Mat mask = new Mat();
            Cv2.InRange(hsv, lowerColor, upperColor, mask);

            // Encontrar contornos del vaso
            Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(mask, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            if (contours.Length > 0)
            {
                // Seleccionar el contorno más grande (asumimos que es el vaso)
                Point[] largestContour = contours[0];
                foreach (var contour in contours)
                {
                    if (Cv2.ContourArea(contour) > Cv2.ContourArea(largestContour))
                    {
                        largestContour = contour;
                    }
                }

                // Obtener centro del vaso
                Moments moments = Cv2.Moments(largestContour);
                int cx = (int)(moments.M10 / (moments.M00 + 1e-5));
                int cy = (int)(moments.M01 / (moments.M00 + 1e-5));

                // **INVERTIR EL MOVIMIENTO** (Ejemplo: Invertir eje X)
                cx = webcamTexture.width - cx; // Espejo horizontal
                cy = webcamTexture.height - cy; // Espejo vertical (si se necesita)

                // Convertir coordenadas de píxeles a Unity
                Vector3 targetPos = Camera.main.ScreenToWorldPoint(new Vector3(cx, cy, 2.0f));

                // Aplicar SUAVIZADO con Lerp
                cupVirtual.position = Vector3.Lerp(cupVirtual.position, new Vector3(targetPos.x, targetPos.y, cupVirtual.position.z), smoothFactor);
            }

            // Mostrar la textura procesada en el Quad
            displayRenderer.material.mainTexture = OpenCvSharp.Unity.MatToTexture(mask);
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
