using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.ComponentModel;

public class RuntimeVariable
{
	public GameObject gameObject;
	public UnityEngine.Component component;
	public FieldInfo field;
	public int ui_display_group;

	public RuntimeVariable(GameObject gameObject, UnityEngine.Component component, FieldInfo field, int ui_group)
	{
		this.gameObject = gameObject;
		this.component = component;
		this.field = field;
		this.ui_display_group = ui_group;
	}
}

public class RuntimeInspector : MonoBehaviour {

	public List<RuntimeVariable> variables_list = new List<RuntimeVariable>();

	void Start () 
	{
		// TODO: move outside of this, maybe in an init call ?
		LoadFromFile(Application.dataPath + "/runtime_params.txt");

		// retrieve all properties that has the CustomInspectorAttribute
		GameObject[] all_game_objects = FindObjectsOfType<GameObject>();
		foreach(GameObject go in all_game_objects)
		{
			UnityEngine.Component[] components = go.GetComponents<UnityEngine.Component>();
			for (uint i = 0; i< components.Length; i++)
			{
				UnityEngine.Component component = components[i];
				System.Type type = component.GetType();
				FieldInfo[] fields = type.GetFields();
				foreach (FieldInfo field in fields)
				{
					if(System.Attribute.IsDefined(field, typeof(RuntimeInspectorAttribute)))
					{
						//Debug.Log("RuntimeInspector defined for field " + field.Name + " of component " + component.name + " of game object " + go.name);
						Debug.Log(go.name + " > " + component.GetType().Name + " > " + field.FieldType.Name + " " + field.Name + " <b>" + field.GetValue(component) +"</b>" );
						RuntimeInspectorAttribute attribute = (RuntimeInspectorAttribute)System.Attribute.GetCustomAttribute(field, typeof(RuntimeInspectorAttribute));
						variables_list.Add( new RuntimeVariable(go, component, field, attribute.group) );
					}
				}
			}
		}
	}

	void LoadFromFile(string path)
	{
		if(System.IO.File.Exists(path))
		{
			string[] variables_infos = System.IO.File.ReadAllLines(path);
			GameObject[] all_game_objects = FindObjectsOfType<GameObject>();

			// GUID/field type name/field name/value
			foreach(string line in variables_infos)
			{
				string[] infos = line.Split('/');
				foreach(GameObject go in all_game_objects)
				{
					UnityEngine.Component[] components = go.GetComponents<UnityEngine.Component>();
					for (uint i = 0; i < components.Length; i++)
					{
						UnityEngine.Component component = components[i];
						string go_name = infos[0];
						string component_type = infos[1];
						// TODO: doesn't work if several component of same type on this game object.
						// doesn't work if game object changes name
						if(go_name == go.name && component_type == component.GetType().Name)
						{
							string field_type = infos[2];
							string field_name = infos[3];
							string field_value = infos[4];
							Debug.Log("field type: " + field_type);
							Debug.Log("field name: " + field_name);
							Debug.Log("value: " + field_value);
							
							component.GetType().GetField(field_name).SetValue(component, TypeUtilities.ConvertFromString(field_value, field_type));
						}
					}
				}
			}
		}
	}


	Vector2 scroll_pos = Vector2.zero;
	void OnGUI()
	{
		GUI.backgroundColor = Color.black;
		GUI.contentColor = Color.white;


		GUILayout.BeginArea(new Rect(Vector2.zero, new Vector2(Screen.width/2, Screen.height)));

		scroll_pos = GUILayout.BeginScrollView(scroll_pos, false, true);
		string[] variables_infos = new string[variables_list.Count];
		int i = 0;
		foreach(RuntimeVariable variable in variables_list)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(variable.field.Name);
			GUILayout.FlexibleSpace();
			string new_val = GUILayout.TextField(variable.field.GetValue(variable.component).ToString());
			object converted_val = TypeUtilities.ConvertFromString(new_val, variable.field.FieldType.Name);
			variable.field.SetValue(variable.component, converted_val);
			GUILayout.FlexibleSpace();
			GUILayout.Label(converted_val.ToString());
			GUILayout.Label(variable.ui_display_group.ToString());
			GUILayout.Label(variable.component.GetInstanceID().ToString());
			GUILayout.EndHorizontal();

			variables_infos[i] = variable.gameObject.name + "/" + variable.component.GetType().Name + "/" + variable.field.FieldType.Name + "/" + variable.field.Name + "/" + converted_val.ToString();
			i++;
		}
		GUILayout.EndScrollView();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("Save"))
		{
			Debug.Log("Saving");
			
			// TODO : récuppérer l'outil de gestion des assets externes
			System.IO.File.WriteAllLines(Application.dataPath + "/runtime_params.txt", variables_infos);
		}
		GUILayout.EndArea();
	} 
}
