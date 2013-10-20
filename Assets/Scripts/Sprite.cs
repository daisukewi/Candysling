using UnityEngine;
using System.Collections;

public enum AnchorMethod
{
	TopLeft,
	TopCenter,
	TopRight,
	CenterLeft,
	Center,
	CenterRight,
	BottomLeft,
	BottomCenter,
	BottomRight,
}

[AddComponentMenu("Scripts/Sprite")]
[ExecuteInEditMode]
public class Sprite : MonoBehaviour
{
	protected Camera cam;
	
	protected MeshFilter meshFilter;
	protected MeshRenderer meshRenderer;
	protected Mesh mesh;
	
	public Texture mainTexture;
	public FilterMode filterMode = FilterMode.Point;
	
	public AnchorMethod dock = AnchorMethod.Center;
	public Vector2 canvasSize;
	public AnchorMethod anchor = AnchorMethod.Center;
	
	protected Vector3 dockOffset;
	
	protected BoxCollider boxCollider = null;
	
	protected Vector3[] vertices = new Vector3[4];
	protected Vector2[] uvs = new Vector2[4];

	#region Get & Set
	
	public bool Visible {
		get {
			return meshRenderer == null ? false : meshRenderer.enabled;
		}
		set {
			if (meshRenderer == null)
				return;
      
			meshRenderer.enabled = value;
		}
	}
 
	public void AddBoxCollider ()
	{
		if (collider == null) {
			boxCollider = gameObject.AddComponent<BoxCollider> ();
      
			UpdateCollider ();
		}
	}
  
	public void UpdateCollider ()
	{
		if (mesh == null || boxCollider == null)
			return;
  
		boxCollider.center = mesh.bounds.center;
		boxCollider.size = mesh.bounds.size;
	}

	public Texture MainTexture {
		get { return mainTexture; }
		set {
			mainTexture = value;
      
			if (mainTexture != null) {
				meshRenderer.sharedMaterial.SetTexture ("_MainTex", mainTexture);
        
				mainTexture.filterMode = filterMode;
				mainTexture.mipMapBias = -0.5f;
				mainTexture.anisoLevel = 0;
				mainTexture.wrapMode = TextureWrapMode.Clamp;
        
				if (canvasSize == Vector2.zero) {
					canvasSize.x = mainTexture.width;
					canvasSize.y = mainTexture.height;
				}
        
				CanvasSize = canvasSize;
			} else
				canvasSize.x = canvasSize.y = 0.0f;
		}
	}

	public FilterMode Filter {
		get { return filterMode; }
		set {
			if (filterMode != value) {
				filterMode = value;
        
				if (mainTexture != null)
					mainTexture.filterMode = filterMode;
			}
		}
	}
  
	public TextureWrapMode WrapMode {
		get { return mainTexture != null ? mainTexture.wrapMode : TextureWrapMode.Clamp; }
		set {
			if (mainTexture != null)
				mainTexture.wrapMode = value;
		}
	}

	public Vector3[] Vertices {
		get { return vertices; }
	}

	public Vector2[] UVs {
		get { return uvs; }
	}

	public Color Color {
		get { return meshRenderer.material.GetColor ("_Color"); }
		set { meshRenderer.material.SetColor ("_Color", value); }
	}

	public Vector2 CanvasSize {
		get { return canvasSize; }
		set {
			canvasSize = value;
      
			// CW
			// 0 ___ 3
			//  |  /|
			// 1|/__|2
			Vector3 offset = new Vector3 ();
  
			switch (anchor) {
			case AnchorMethod.TopLeft:
				offset = new Vector3 (canvasSize.x * 0.5f, -canvasSize.y * 0.5f, 0.0f);
				break;
  
			case AnchorMethod.TopCenter:
				offset = new Vector3 (0.0f, -canvasSize.y * 0.5f, 0.0f);
				break;
  
			case AnchorMethod.TopRight:
				offset = new Vector3 (-canvasSize.x * 0.5f, -canvasSize.y * 0.5f, 0.0f);
				break;
  
			case AnchorMethod.CenterLeft:
				offset = new Vector3 (canvasSize.x * 0.5f, 0.0f, 0.0f);
				break;
        
			case AnchorMethod.Center:
				offset = Vector3.zero;
				break;
        
			case AnchorMethod.CenterRight:
				offset = new Vector3 (-canvasSize.x * 0.5f, 0.0f, 0.0f);
				break;
        
			case AnchorMethod.BottomLeft:
				offset = new Vector3 (canvasSize.x * 0.5f, canvasSize.y * 0.5f, 0.0f);
				break;
  
			case AnchorMethod.BottomCenter:
				offset = new Vector3 (0.0f, canvasSize.y * 0.5f, 0.0f);
				break;
  
			case AnchorMethod.BottomRight:
				offset = new Vector3 (-canvasSize.x * 0.5f, canvasSize.y * 0.5f, 0.0f);
				break;
			}
    
			vertices [0] = new Vector3 (-canvasSize.x * 0.5f, canvasSize.y * 0.5f, 0.0f) + offset;
			vertices [1] = new Vector3 (-canvasSize.x * 0.5f, -canvasSize.y * 0.5f, 0.0f) + offset;
			vertices [2] = new Vector3 (canvasSize.x * 0.5f, -canvasSize.y * 0.5f, 0.0f) + offset;
			vertices [3] = new Vector3 (canvasSize.x * 0.5f, canvasSize.y * 0.5f, 0.0f) + offset;
      
			UpdateVertices ();
		}
	}
	
	#endregion
  
	protected void OnTriggerEnter (Collider c)
	{
		Debug.Log ("Collided!");
	}
	
	void Setup ()
	{
		meshFilter = gameObject.GetComponent<MeshFilter> ();
		meshRenderer = gameObject.GetComponent<MeshRenderer> ();
	}

	void Start ()
	{
		cam = gameObject.transform.parent != null ? gameObject.transform.parent.gameObject.GetComponent<Camera> () : null;
		if (cam != null)
			gameObject.transform.position.Set (gameObject.transform.position.x, gameObject.transform.position.y, cam.nearClipPlane);
    
		if (meshFilter == null)
			meshFilter = gameObject.AddComponent<MeshFilter> ();

		if (meshRenderer == null)
			meshRenderer = gameObject.AddComponent<MeshRenderer> ();
    
		mesh = new Mesh ();
		mesh.name = gameObject.name + "_mesh";
		
		if (meshRenderer.sharedMaterial == null) {
			Shader shader = (Shader)Resources.Load ("Shaders/Sprite/SpriteTintAlpha");
			meshRenderer.sharedMaterial = new Material (shader);
      
			MainTexture = mainTexture != null ? mainTexture : (Texture)Resources.Load ("Textures/test_pattern_256");
		} else
			MainTexture = meshRenderer.sharedMaterial.mainTexture;
  
		UpdateAnchor ();
    
		uvs [0] = new Vector2 (0.0f, 1.0f);
		uvs [1] = new Vector2 (0.0f, 0.0f);
		uvs [2] = new Vector2 (1.0f, 0.0f);
		uvs [3] = new Vector2 (1.0f, 1.0f);
  
		UpdateVertices ();

		int[] triIndices = new int[6];
		triIndices [0] = 0;  //  0_ 1
		triIndices [1] = 3;  //  | /
		triIndices [2] = 1;  // 2|/

		triIndices [3] = 3;  //    3
		triIndices [4] = 2;  //   /|
		triIndices [5] = 1;  // 5/_|4
    
		mesh.triangles = triIndices;
    
		UpdateUVs ();
    
		UpdateDock ();
    
		meshFilter.sharedMesh = mesh;
    
		// HACK: Esto está hecho para saber cuando se ha cargado el prefab. Hay que buscar una forma mejor de hacerlo.
		//Notifications.Invoke ("OnLoadPausePrefab");
	}

	public void UpdateVertices ()
	{
		if (mesh != null) {
			mesh.vertices = vertices;

			mesh.RecalculateBounds ();
      
			UpdateCollider ();
		}
	}
  
	protected void UpdateUVs ()
	{
		if (mesh != null)
			mesh.uv = uvs;
	}
  
	public void UpdateAnchor ()
	{
		CanvasSize = canvasSize;
	}
  
	public void UpdateDock ()
	{
		cam = gameObject.transform.parent != null ? gameObject.transform.parent.gameObject.GetComponent<Camera> () : null;
		if (cam != null) {
			switch (dock) {
			case AnchorMethod.TopLeft:
				dockOffset = new Vector3 (cam.pixelWidth * -0.5f, cam.pixelHeight * 0.5f, cam.nearClipPlane + gameObject.transform.position.z);
				break;
        
			case AnchorMethod.Center:
				dockOffset = new Vector3 (0.0f, 0.0f, cam.nearClipPlane + gameObject.transform.position.z);
				break;
			}
		}
	}
  
	public void SetUVScale (Vector2 uvScale)
	{
		uvs [0] = new Vector2 (0.0f, 1.0f * uvScale.y);
		uvs [1] = new Vector2 (0.0f, 0.0f);
		uvs [2] = new Vector2 (1.0f * uvScale.x, 0.0f);
		uvs [3] = new Vector2 (1.0f * uvScale.x, 1.0f * uvScale.y);
    
		UpdateUVs ();
	}
}
