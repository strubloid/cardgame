using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieImpulseAssets
{
    public class DemoSlide : MonoBehaviour
    {
        public GameObject[] Effects;
        private int index = 0;

        void Start()
        {
            Application.targetFrameRate = 60;
            index = Effects.Length - 1;
            // Initially, deactivate all effects except the first one
            for (int i = 1; i < Effects.Length; i++)
            {
                Effects[i].SetActive(false);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ChangeEffect(); // Move to the next effect
            }
        }

        void ChangeEffect()
        {
            // Deactivate the current effect
            Effects[index].SetActive(false);

            // Update the index for the next effect
            if (index == Effects.Length - 1)
            {
                index = 0;
            }
            else
            {
                index++;
            }

            // Activate the new effect
            Effects[index].SetActive(true);
        }
    }
}
