using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderUpdaterScript : MonoBehaviour
{
    private Slider slider;
    private Text text;
    private void OnEnable()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnValueChanged);
        text = GetComponentsInChildren<Text>().First((a) => a.name == "Value");
        if (text != null)
        {
            text.text = slider.value.ToString();
        }
    }
    private void OnDisable()
    {
        if(slider != null)
        {
            slider.onValueChanged.RemoveListener(OnValueChanged);
        }
    }
    private void OnValueChanged(float value)
    {
        if(text != null)
        {
            text.text = value.ToString();
        }
    }
}
