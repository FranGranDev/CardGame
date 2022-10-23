using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ResolutionHandler : MonoBehaviour
    {
        public static ResolutionHandler Active;

        private readonly Vector2 ScreenMatchXTablet = new Vector2(1500, 1920);
        private readonly Vector2 ScreenMatchXPhone = new Vector2(1080, 1920);

        public ResolutionHandler()
        {
            Active = this;
        }

        public void SetFieldOfView(CanvasScaler canvasScaler)
        {
            float screenRatio = ((float)Screen.height) / ((float)Screen.width);
            if (screenRatio < 1.5f)
            {
                canvasScaler.referenceResolution = ScreenMatchXTablet;
            }
            else
            {
                canvasScaler.referenceResolution = ScreenMatchXPhone;
            }
        }
    }
}