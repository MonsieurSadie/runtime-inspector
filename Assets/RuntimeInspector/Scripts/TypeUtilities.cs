using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeUtilities : MonoBehaviour {

	public static object ConvertFromString(string value, string type_name)
	{
		switch(type_name)
		{
			case "Vector3": // (single, single, single)
			string[] vals = value.Substring(1, value.Length-2).Split(',');
			return new Vector3(float.Parse(vals[0]), float.Parse(vals[1]), float.Parse(vals[2]));

			case "Single":
			return float.Parse(value);

			case "Int32":
			return int.Parse(value);
		}
		return (object)0;
	}
}
