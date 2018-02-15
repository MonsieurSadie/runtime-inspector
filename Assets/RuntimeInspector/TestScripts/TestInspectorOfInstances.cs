using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInspectorOfInstances : MonoBehaviour {
	[RuntimeInspector(1)]
	public float value1;
	[RuntimeInspector(1)]
	public Vector3 value2;
}
