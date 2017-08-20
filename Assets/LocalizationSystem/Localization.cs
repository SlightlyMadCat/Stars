using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class Localization : MonoBehaviour
{

    // обязательная сериализация даных полей
    [SerializeField]
    private Canvas[] canvas; // здесь указываем Canvas, для дочерних текстовых элементов которых, предусмотрена локаль
    [SerializeField]
    private Dropdown dropdown; // меню для переключения языка
    [SerializeField]
    private LocalizationComponent[] source; // базовый массив, заполняется автоматически, после генерирования локали

    private TextAsset[] binary;
    private static Localization _internal;

    public static Localization Internal
    {
        get { return _internal; }
    }

    void Awake()
    {
        _internal = this;
        StartScene();
    }

    public void Custom(int id, int index) // работа с настраиваемым текстом
    {
        foreach (LocalizationComponent t in source)
        {
            if (t.hash == id) t.SetCustom(index);
        }
    }

    void StartScene()
    {
        Load(); // загружаем все локали в массив
        dropdown.value = -1; // выбор локали на старте сцены, для самого первого элемента списка: -1
    }

    void Load()
    {
        binary = Resources.LoadAll<TextAsset>("Localization"); // папка в Resources, где лежат локали
        dropdown.options = new List<Dropdown.OptionData>();

        if (binary.Length == 0)
        {
            ListData("List empty...");
            dropdown.value = -1;
            Debug.Log(this + " файлы не обнаружены.");
            return;
        }

        for (int i = 0; i < binary.Length; i++)
        {
            ListData(binary[i].name);
        }

        dropdown.onValueChanged.AddListener(delegate { Locale(); });
    }

    void ListData(string value) // добавление элемента в выпадающее меню (выбор языка)
    {
        Dropdown.OptionData option = new Dropdown.OptionData();
        option.text = value;
        dropdown.options.Add(option);
    }

    int GetInt(string text)
    {
        int value;
        if (int.TryParse(text, out value)) return value;
        return 0;
    }

    void InnerText(int id, string text)
    {
        foreach (LocalizationComponent t in source)
        {
            if (t.hash == id) t.target.text = text;
        }
    }

    void InnerCustomText(int id, string text)
    {
        foreach (LocalizationComponent t in source)
        {
            if (t.hash == id) t.SetCustomLoad(text);
        }
    }

    void Locale() // чтение XML
    {
        XmlTextReader reader = new XmlTextReader(new StringReader(binary[dropdown.value].text));
        while (reader.Read())
        {
            if (reader.IsStartElement("content"))
                InnerText(GetInt(reader.GetAttribute("id")), reader.ReadString());
            else if (reader.IsStartElement("custom"))
                InnerCustomText(GetInt(reader.GetAttribute("id")), reader.ReadString());
        }
        reader.Close();
    }

    public void SetComponents() // создание шаблона локали, используется только в редакторе
    {
        source = LocalizationGenerator.GenerateLocale(canvas);
    }
}
