using UnityEngine;

public class RuntimeInspectorAttribute : PropertyAttribute
{
	public int group; // gives the possibility to assign a variable to a specific ui window

    public RuntimeInspectorAttribute(int group_id = 0)
    {
        group = group_id;
    }
}
