using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchInput
{
    public class SelectRaycast
    {
        public static bool TryGet<T>(ref T unit, Vector2 screenPoint)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(screenPoint);
            if (Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("Selectable")))
            {
                if(hit.transform.TryGetComponent(out unit))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool GetPoint(ref Vector3 point, Vector2 screenPoint)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(screenPoint);
            if (Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("Ground")))
            {
                point = hit.point;
                return true;
            }
            return false;
        }

        public enum FindType { Simple, Parent}

    }
}
