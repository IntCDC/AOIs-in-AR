using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnnotationFilter : MonoBehaviour
{
    [SerializeField]
    private InputField inputField;

    [SerializeField]
    private Dropdown dropdown;

    private List<Dropdown.OptionData> dropdownOptions;

    // Start is called before the first frame update
    void Start()
    {
        dropdownOptions = dropdown.options;

    }

    public void FilterDropdown(string input)
    {
        List<Dropdown.OptionData> filteredOptions = dropdownOptions.FindAll(option => option.text.IndexOf(input) >= 0);
        if (filteredOptions.Count > 0)
            dropdown.options = filteredOptions;

        dropdownOptions.Add(new Dropdown.OptionData() { text = input });
    }

}
