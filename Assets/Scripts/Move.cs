using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Move : MonoBehaviour
    {
        private float moveSpeed;
        private float rotateSpeed;

        void Start()
        {
            moveSpeed = 2f;
            rotateSpeed = 40f;
        }
        
        void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.forward * Time.deltaTime * moveSpeed;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                transform.position -= transform.forward * Time.deltaTime * moveSpeed;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                transform.position -= transform.right * Time.deltaTime * moveSpeed;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.position += transform.right * Time.deltaTime * moveSpeed;
            }

            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                transform.Rotate(-Vector3.up * rotateSpeed * Time.deltaTime);
            }
        }
    }
}