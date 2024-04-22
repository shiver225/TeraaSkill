using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Camera camera;
    [SerializeField] private Transform targert;
    [SerializeField] private Vector3 offset;

    public void UpdateHealthBar(float currValue, float maxValue) 
    {
        slider.value = currValue / maxValue;
    }
    // Update is called once per frame
    void Update()
    {
        transform.rotation = camera.transform.rotation;
        transform.position = targert.position + offset;
    }
}
