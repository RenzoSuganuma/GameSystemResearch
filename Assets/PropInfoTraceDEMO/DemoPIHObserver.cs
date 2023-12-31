using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DemoPIHObserver : MonoBehaviour, IPropInfoObserver
{
    PropertyInfoObserver _observer;
    [SerializeField] Text _text;
    private void Awake()
    {
        _observer = GameObject.FindAnyObjectByType<PropertyInfoObserver>();
    }
    private void OnEnable()
    {
        _observer.OnSenderDataHasChanged += OnSenderPropertyValueChanged;
        _observer.OnReceiverDataHasChanged += OnReciverPropertyValueChanged;
    }
    private void OnDisable()
    {
        _observer.OnSenderDataHasChanged -= OnSenderPropertyValueChanged;
        _observer.OnReceiverDataHasChanged -= OnReciverPropertyValueChanged;
    }
    public void OnReciverPropertyValueChanged(PropInfoCallBackContext context)
    {
        Debug.Log(context.ToString());
        _text.text = context.ToString();
    }
    public void OnSenderPropertyValueChanged(PropInfoCallBackContext context)
    {
        Debug.Log(context.ToString());
        _text.text = context.ToString();
    }
}