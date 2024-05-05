using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTextSwitcher : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private string _defaultLabel;
    [SerializeField] private string _altLabel;

    public bool IsAlt { get; private set; }
    public event Action<bool> OnClicked;

    private void Start()
    {
        _text.text = _defaultLabel;
        _button.onClick.AddListener(Clicked);
    }

    private void Clicked()
    {
        OnClicked?.Invoke(IsAlt);
        IsAlt = !IsAlt;

        if (IsAlt)
        {
            _text.text = _altLabel;
        }
        else
        {
            _text.text = _defaultLabel;
        }
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(Clicked);
    }
}