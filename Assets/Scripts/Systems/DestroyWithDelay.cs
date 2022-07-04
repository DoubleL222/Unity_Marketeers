using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Systems {
    public class DestroyWithDelay : MonoBehaviour
    {
        public float delay;

        void Start()
        {
            Destroy(gameObject, delay);
        }

    }
}