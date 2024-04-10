using System;
using System.Collections;
using System.Collections.Generic;
using LatteGames;
using MoreMountains;
using Unity.Mathematics;
using UnityEngine;

namespace _GemstonesAttachment
{
    public class AttachmentTool : MonoBehaviour
    {
        public Action OnGemAttached;
        [SerializeField] GameObject cameraPivot;
        [SerializeField] GameObject yawPivot;
        [SerializeField] GameObject pitchPivot;
        [SerializeField] GameObject toolPivot;
        [SerializeField] GameObject head;
        [SerializeField] Vector2 headMoveRange;
        [SerializeField] AnimationCurve headMoveCurve;
        [SerializeField] float headMoveDuration = 0.8f;
        [SerializeField] float yawSensitivity, pitchSensitivity;
        [SerializeField] Vector2 yawRange;
        [SerializeField] Vector2 pitchRange;
        [SerializeField] float maxRotatableAnglePerFrame = 1;
        [SerializeField] float differenceAngleBetweenToolAndYawPivot = 30f;
        [SerializeField] float camFollowMultiplier = 3f;
        [SerializeField] float toolFollowMultiplier = 8f;
        [SerializeField] GameObject attachmentDirectionObject;
        [SerializeField] GameObject gemStonePrefab;
        [SerializeField] float attachGemStoneSpeed = 10f;
        [SerializeField] AudioSource audioSource;
        [SerializeField] ReferentialSoundClip blingSound;
        [SerializeField] GameObject crosshairPrefab;

        Vector3 currentMousePos;
        float yawAngle, pitchAngle, limitedYawAngle;
        bool isActive = true;

        GameObject crosshair;
        private void Start()
        {
            crosshair = Instantiate(crosshairPrefab);
            crosshair.transform.parent = transform.parent;
            crosshair.transform.position = Vector3.zero;
        }

        // Update is called once per frame
        void Update()
        {
            if (isActive)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    currentMousePos = Input.mousePosition;
                }
                else if (Input.GetMouseButton(0))
                {
                    var delta = currentMousePos - Input.mousePosition;
                    currentMousePos = Input.mousePosition;
                    yawAngle += delta.x * yawSensitivity;
                    yawAngle = Mathf.Clamp(yawAngle, yawRange.x, yawRange.y);
                    limitedYawAngle = yawAngle >= 0 ? math.remap(0, yawRange.y, differenceAngleBetweenToolAndYawPivot, yawRange.y + differenceAngleBetweenToolAndYawPivot, yawAngle) :
                        math.remap(0, yawRange.x, -differenceAngleBetweenToolAndYawPivot, yawRange.x - differenceAngleBetweenToolAndYawPivot, yawAngle);
                    yawPivot.transform.rotation = Quaternion.AngleAxis(yawAngle, Vector3.up);
                    pitchAngle -= delta.y * pitchSensitivity;
                    pitchAngle = Mathf.Clamp(pitchAngle, pitchRange.x, pitchRange.y);
                    pitchPivot.transform.localRotation = Quaternion.AngleAxis(pitchAngle, Vector3.right);
                    CheckToAttachGemStone();
                }
                toolPivot.transform.rotation = Quaternion.Lerp(toolPivot.transform.rotation, Quaternion.AngleAxis(limitedYawAngle, Vector3.up) * Quaternion.AngleAxis(pitchAngle, Vector3.right), Time.deltaTime * toolFollowMultiplier);
            }
            cameraPivot.transform.rotation = Quaternion.Lerp(cameraPivot.transform.rotation, Quaternion.AngleAxis(yawAngle, Vector3.up), Time.deltaTime * camFollowMultiplier);
        }

        void CheckToAttachGemStone()
        {
            RaycastHit teethHit;
            if (Physics.SphereCast(attachmentDirectionObject.transform.position, 0.01f, Vector3.forward + attachmentDirectionObject.transform.forward * 2, out teethHit, 1f, LayerMask.GetMask("AttachableGem")))
            {
                crosshair.gameObject.SetActive(true);
                crosshair.transform.position = teethHit.point + (0.01f * teethHit.transform.forward);
                crosshair.transform.forward = teethHit.normal;
            }
            else
            {
                crosshair.gameObject.SetActive(false);
            }


            RaycastHit hit;
            if (Physics.SphereCast(attachmentDirectionObject.transform.position, 0.01f, Vector3.forward + attachmentDirectionObject.transform.forward * 2, out hit, 10))
            {
                var attachableSlot = hit.collider.GetComponent<AttachableSlot>();
                if (attachableSlot != null && !isAttaching)
                {
                    attachableSlot.Hide();
                    Debug.Log(isAttaching);
                    StartCoroutine(AttachGemStone(hit));
                }
            }
        }

        bool isAttaching = false;
        IEnumerator AttachGemStone(RaycastHit hit)
        {
            isAttaching = true;
            float t = 0;
            var startLocalPos = head.transform.localPosition;
            var gemStone = Instantiate(gemStonePrefab);
            gemStone.transform.SetParent(hit.collider.transform);
            gemStone.transform.position = attachmentDirectionObject.transform.position;
            gemStone.transform.rotation = hit.collider.transform.rotation;
            var targetPos = hit.collider.transform.position;
            bool isAttached = false;
            while (t < 1)
            {
                t += Time.deltaTime / headMoveDuration;
                startLocalPos.y = Mathf.Lerp(headMoveRange.x, headMoveRange.y, headMoveCurve.Evaluate(t));
                head.transform.localPosition = startLocalPos;
                if (!isAttached)
                {
                    gemStone.transform.position = head.transform.position;
                }
                if (t > 0.5f && !isAttached)
                {
                    isAttached = true;
                    gemStone.transform.position = targetPos;
                }
                yield return null;
            }
            isAttaching = false;
            // while ((gemStone.transform.position - hit.collider.transform.position).magnitude > 0.01f) {
            //     gemStone.transform.position = Vector3.MoveTowards (gemStone.transform.position, targetPos, attachGemStoneSpeed * Time.deltaTime);
            //     yield return null;
            // }
            HarpticManager.Instance.triggerContinousHarptic = false;
            HarpticManager.Instance.TriggerHaptics();
            var novaFX = gemStone.GetComponentInChildren<ParticleSystem>();
            novaFX.Play();
            PlaySound(blingSound);
            OnGemAttached?.Invoke();
        }
        public void DisableTooth()
        {
            crosshair.gameObject.SetActive(false);
            toolPivot.SetActive(false);
            isActive = false;
            yawAngle = 0;
        }
        void PlaySound(ReferentialSoundClip clip)
        {
            audioSource.clip = clip.Clip;
            audioSource.Play();
        }
    }
}