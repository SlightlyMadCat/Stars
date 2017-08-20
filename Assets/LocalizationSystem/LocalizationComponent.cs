using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocalizationComponent : MonoBehaviour
{

    // обязательная сериализация даных полей
    [SerializeField]
    private Text _target;
    [SerializeField]
    private int _hash;
    [SerializeField]
    private bool _isCustom;
    [Header("Custom Field")]
    [SerializeField]
    private string _content;

    private string[] custom;
    private int last_id;

    public int hash
    {
        get { return _hash; }
    }

    public Text target
    {
        get { return _target; }
    }

    public bool isCustom
    {
        get { return _isCustom; }
    }

    public string content
    {
        get { return _content; }
    }

    public void SetComponent()
    {
        Text t = GetComponent<Text>();

        if (t == null)
        {
            _target = null;
            _hash = 0;
            _isCustom = false;
        }
        else
        {
            _target = t;

            if (_content != null && _content.Trim().Length > 0)
            {
                _hash = content.GetHashCode();
                _isCustom = true;
            }
            else
            {
                _hash = t.text.GetHashCode();
                _isCustom = false;
            }
        }
    }

    public void SetCustomLoad(string text)
    {
        _content = text;
        custom = text.Split(new char[] { '|' });
        _target.text = custom[last_id];
    }

    public void SetCustom(int index)
    {
        if (index < 0 || index > custom.Length - 1) return;
        _target.text = custom[index];
        last_id = index;
    }
}
