using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
[System.Serializable]
public class FloorPrefabCS : MonoBehaviour
{

    [SerializeField]
    private Vector2 size = new Vector2(10.0f, 10.0f);

    [SerializeField]
    private Vector2 borderSize = new Vector2(0.25f, 0.1f);

    [SerializeField]
    private Material floorMaterial;    

    [SerializeField]
    private Material floorMatInstance = null;

    private Transform floorPlane;
    private Transform top;
    private Transform bottom;
    private Transform right;
    private Transform left;

    void Awake()
    {
        // find the children (must be in this specific arrangement)
        floorPlane = transform.FindChild("FloorPlane");

        Transform borderParent = transform.FindChild("Border");
        top = borderParent.FindChild("T");
        bottom = borderParent.FindChild("B");
        right = borderParent.FindChild("R");
        left = borderParent.FindChild("L");

        if (floorMatInstance == null)
        {
            floorMatInstance = Instantiate(floorMaterial);
            floorPlane.GetComponent<MeshRenderer>().material = floorMatInstance;
        }

    }

    public void UpdateSettings()
    {
        // update texture scale (we assume we want a tiling resolution of 1x m^2)
		floorMatInstance.SetTextureScale("_MainTex", size);

        // update floor plane scaling (default unity plane is 10mx10m, so we have to downscale)
		floorPlane.localScale = new Vector3(0.1f * size.x, 1.0f, 0.1f * size.y);

        // update border scales (x = length, y = height, z = width)
        left.localScale = right.localScale = new Vector3(size.y, borderSize.y, borderSize.x);
        top.localScale = bottom.localScale = new Vector3(size.x + borderSize.x * 2.0f, borderSize.y, borderSize.x);       // top and bottom need to be a little larger

        // update border
		left.localPosition = new Vector3(-0.5f * size.x, 0.0f, 0.0f);
		right.localPosition = new Vector3(0.5f * size.x, 0.0f, 0.0f);
		top.localPosition = new Vector3(0.0f, 0.0f, 0.5f * size.y);
		bottom.localPosition = new Vector3(0.0f, 0.0f, -0.5f * size.y);
    }
}





[CustomEditor(typeof(FloorPrefabCS))]
[CanEditMultipleObjects]
class FloorPrefabEditor : Editor
{
	SerializedProperty sizeProp;
	SerializedProperty borderSizeProp;
	SerializedProperty floorMaterial;
	
	void OnEnable()
	{
		// Setup the serializedProperties
		sizeProp = serializedObject.FindProperty("size");
		borderSizeProp = serializedObject.FindProperty("borderSize");
		floorMaterial = serializedObject.FindProperty("floorMaterial");
	}
	
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

        FloorPrefabCS fbp = target as FloorPrefabCS;

        EditorGUILayout.PropertyField(sizeProp, new GUIContent("Size", "Change width and length of the floor prefab"));
        EditorGUILayout.PropertyField(borderSizeProp, new GUIContent("Border Size", "Change width and height of the border"));
        EditorGUILayout.PropertyField(floorMaterial, new GUIContent("Floor Material", "Change the material to spread on the floor"));

		EditorGUILayout.Space();

		serializedObject.ApplyModifiedProperties();

        // force an update
        fbp.UpdateSettings();
	}
}


