using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class RaycastController2D : MonoBehaviour {

    // Creates raycast for base objects that have a BoxCollider2D.
    
    public LayerMask colMask;

    public const float skinWidth = .015f;
    public const float distSpread = .2f;

    [HideInInspector]
    public int horiRayCount;
    [HideInInspector]
    public int vertRayCount;
    [HideInInspector]
    public float horiRaySpace;
    [HideInInspector]
    public float vertRaySpace;

    [HideInInspector]
    public BoxCollider2D boxCol2D;
    public RaycastOrigins rcOrigins;
    
    public virtual void Awake()
    {
        boxCol2D = GetComponent<BoxCollider2D>();
    }

	public virtual void Start () {
        CalcRaySpace();
	}

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCol2D.bounds;
        bounds.Expand(skinWidth * -2);

        rcOrigins.botLeft = new Vector2(bounds.min.x, bounds.min.y);
        rcOrigins.botRight = new Vector2(bounds.max.x, bounds.min.y);
        rcOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        rcOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void CalcRaySpace()
    {
        Bounds bounds = boxCol2D.bounds;
        bounds.Expand(skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        horiRayCount = Mathf.RoundToInt(boundsHeight / distSpread);
        vertRayCount = Mathf.RoundToInt(boundsWidth / distSpread);

        horiRaySpace = bounds.size.y / (horiRayCount - 1);
        vertRaySpace = bounds.size.x / (vertRayCount - 1);
    }

	
	public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 botLeft, botRight;
    }
}
