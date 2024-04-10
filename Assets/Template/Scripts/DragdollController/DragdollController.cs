using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames{
    public class DragdollController : MonoBehaviour
    {
        [SerializeField] private Rigidbody pelvis = null;
        [SerializeField] private Rigidbody lHip = null;
        [SerializeField] private Rigidbody lKnee = null;
        [SerializeField] private Rigidbody rHip = null;
        [SerializeField] private Rigidbody rKnee = null;
        [SerializeField] private Rigidbody lArm = null;
        [SerializeField] private Rigidbody lElbow = null;
        [SerializeField] private Rigidbody rArm = null;
        [SerializeField] private Rigidbody rElbow = null;
        [SerializeField] private Rigidbody mSpine = null;
        [SerializeField] private Rigidbody head = null;

        public Rigidbody RootRb => pelvis;

        private Rigidbody[] rigidbodies => new Rigidbody[]{
            pelvis,
            lHip,
            lKnee,
            rHip,
            rKnee,
            lArm,
            lElbow,
            rArm,
            rElbow,
            mSpine,
            head,
        };

        public void AddForce(Vector3 force, Vector3 position, float falloff, ForceMode mode)
        {
            foreach (var rb in rigidbodies)
                rb.AddForceAtPosition(force * Mathf.Clamp01(1 - (rb.position - position).magnitude/falloff), position, mode);
        }

        private void OnEnable() {
            foreach (var rb in rigidbodies)
            {
                rb.isKinematic = false;
                rb.GetComponent<Collider>().enabled = true;
            }
        }

        private void OnDisable() {
            foreach (var rb in rigidbodies)
            {
                rb.isKinematic = true;
                rb.GetComponent<Collider>().enabled = false;
            }
        }

        private void OnValidate() {
            if(enabled)
                OnEnable();
            else
                OnDisable();
        }
    }
}