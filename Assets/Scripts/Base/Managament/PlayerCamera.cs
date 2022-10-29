using System.Collections;
using UnityEngine;

namespace Managament
{
    public class PlayerCamera : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private new Camera camera;

        [SerializeField] private Transform point_0;
        [SerializeField] private Transform point_1;

        private bool isFirst = true;

        public void Initilize()
        {

        }

        private void Switch()
        {
            isFirst = !isFirst;

            Transform point = point_0;
            if(isFirst)
            {
                point = point_0;
            }
            else
            {
                point = point_1;
            }

            camera.transform.position = point.transform.position;
            camera.transform.rotation = point.transform.rotation;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.S))
            {
                Switch();
            }
        }
    }
}