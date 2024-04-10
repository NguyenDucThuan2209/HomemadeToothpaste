using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _GemstonesAttachment
{
    public class AttachableSlot : MonoBehaviour
    {
        public void Hide()
        {
            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
        }

        [ContextMenu("AttachOnTeeth")]
        void AttachOnTeeth()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.parent.forward, out hit))
            {
                transform.rotation = Quaternion.LookRotation(hit.normal);
                transform.position = hit.point + hit.normal * 0.002f;
            }
        }
    }
}