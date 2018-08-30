using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ControlScript : MonoBehaviour
{
    private Camera targetCamera;
    private ScreenScript screenScript;
    private Action onDisable;
    private Action onStart;
    private Action onUpdate;
    private bool started = false;

    private T Find<T>(string name) where T : class
    {
        var obj = transform.Find(name);
        if (obj == null)
        {
            return null;
        }
        return obj.GetComponent<T>();
    }
    private void OnEnable()
    {
        targetCamera = FindObjectOfType<Camera>();
        Debug.Assert(targetCamera != null);
        screenScript = FindObjectOfType<ScreenScript>();
        Debug.Assert(screenScript != null);
        foreach (var screenMode in new ScreenScript.ScreenModeType[] { ScreenScript.ScreenModeType.Normal8K, ScreenScript.ScreenModeType.Distotion4K, ScreenScript.ScreenModeType.Normal4K, ScreenScript.ScreenModeType.Distotion2K, ScreenScript.ScreenModeType.Normal2K })
        {
            var toggle = Find<Toggle>(screenMode.ToString());
            if (toggle != null)
            {
                var action = new UnityAction<bool>((val) =>
                {
                    if (val)
                    {
                        screenScript.ScreenMode = screenMode;
                    }
                });
                onStart += () =>
                {
                    toggle.isOn = screenScript.ScreenMode == screenMode;
                    toggle.onValueChanged.AddListener(action);
                };
                onDisable += () =>
                {
                    if (toggle != null)
                    {
                        toggle.onValueChanged.RemoveListener(action);
                    }
                };
            }
        }

        {
            var slider = Find<Slider>("FieldOfView");
            if (slider != null)
            {
                var action = new UnityAction<float>((val) => targetCamera.fieldOfView = val);
                onStart += () =>
                {
                    slider.value = targetCamera.fieldOfView;
                    slider.onValueChanged.AddListener(action);
                };
                onDisable += () =>
                {
                    if (slider != null)
                    {
                        slider.onValueChanged.RemoveListener(action);
                    }
                };
            }
        }
        {
            var slider = Find<Slider>("V-Dist");
            if (slider != null)
            {
                var action = new UnityAction<float>((val) => screenScript.VerticalDistotion = val);
                onStart += () =>
                {
                    slider.value = screenScript.VerticalDistotion;
                    slider.onValueChanged.AddListener(action);
                };
                onDisable += () =>
                {
                    if (slider != null)
                    {
                        slider.onValueChanged.RemoveListener(action);
                    }
                };
            }
        }
        {
            var slider = Find<Slider>("H-Dist");
            if (slider != null)
            {
                var action = new UnityAction<float>((val) => screenScript.HorizontalDistotion = val);
                onStart += () =>
                {
                    slider.value = screenScript.HorizontalDistotion;
                    slider.onValueChanged.AddListener(action);
                };
                onDisable += () =>
                {
                    if (slider != null)
                    {
                        slider.onValueChanged.RemoveListener(action);
                    }
                };
            }
        }
        {
            var toggle = Find<Toggle>("Soft");
            if (toggle != null)
            {
                var action = new UnityAction<bool>((val) => screenScript.SoftFilter = val);
                onStart += () =>
                {
                    toggle.isOn = screenScript.SoftFilter;
                    toggle.onValueChanged.AddListener(action);
                };
                onDisable += () =>
                {
                    if (toggle != null)
                    {
                        toggle.onValueChanged.RemoveListener(action);
                    }
                };
            }
        }
        {
            var toggle = Find<Toggle>("Sharp");
            if (toggle != null)
            {
                var action = new UnityAction<bool>((val) => screenScript.SharpFilter = val);
                onStart += () =>
                {
                    toggle.isOn = screenScript.SharpFilter;
                    toggle.onValueChanged.AddListener(action);
                };
                onDisable += () =>
                {
                    if (toggle != null)
                    {
                        toggle.onValueChanged.RemoveListener(action);
                    }
                };
            }
        }
        {
            var rawImage = Find<RawImage>("RawImage");
            if (rawImage != null)
            {
                onUpdate += () =>
                {
                    if (screenScript.Updated)
                    {
                        rawImage.texture = screenScript.Thumbnail;
                    }
                };
                onDisable += () =>
                {
                    if (rawImage != null)
                    {
                        rawImage.texture = null;
                    }
                };
            }
        }
        started = false;
    }
    private void OnDisable()
    {
        if (onDisable != null)
        {
            onDisable.Invoke();
        }
        onDisable = null;
        onUpdate = null;
        targetCamera = null;
    }
    private void Update()
    {
        if (!started)
        {
            if (onStart != null)
            {
                onStart.Invoke();
                onStart = null;
            }
            started = true;
        }
        if (onUpdate != null)
        {
            onUpdate.Invoke();
        }
    }
}
