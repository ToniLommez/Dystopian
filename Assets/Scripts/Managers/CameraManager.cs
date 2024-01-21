using System;
using UnityEngine;

namespace WorldBuilder
{
    public class CameraManager : MonoBehaviour
    {
        public Transform target;
        public float smoothing = 5f;

        void Update()
        {
            if (target == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
                else
                {
                    return;
                }
            }

            
            Vector3 targetPosition = target.position;
            targetPosition.z = transform.position.z;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothing);


        }
    }
}