using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public class ONB
{ // orthonormal base for creating coordinates -> think of them as a regular Transform, but code-creatable
	public Vector3 position = Vector3.zero; // a world offset		
	public Vector3 right    = Vector3.zero;	// or local X-axis
	public Vector3 up       = Vector3.zero;	// or local Y-axis
	public Vector3 forward  = Vector3.zero;	// or local Z-axis
	
	// this (optional) data is produced by NavCache.Raycast()
	public Triangle source = null;

	public ONB()
		{ Invalidate(); }
	public ONB(ref ONB a)
		{ Set(ref a); }
	public ONB(Vector3 wp)
		{ Invalidate(); position = wp; }
	public ONB(Vector3 u, Vector3 v, Vector3 wp) 
		{ Calculate(u,v,wp); }	

	// reset the ONB to a state in which it is meaningless
	public void Invalidate()
		{ right = up = forward = Vector3.zero; position = _Vector3.Infinity(); }
	public void Set(ref ONB a)
		{ right = a.right; up = a.up; forward = a.forward; position = a.position; }
	public void Set(Transform t)
		{ right = t.right; up = t.up; forward = t.forward; position = t.position; }
	public void Calculate(ref ONB a, Vector3 wp)
		{ Set(ref a); position = wp; }
	// two valid vectors, world offset
	public void Calculate(Vector3 u, Vector3 v, Vector3 wp) 
	{ // calculate orthonormal base from [u,v].
		u.Normalize();
		v.Normalize();
		forward  = u; 								 // forward is parallel to u
		up       = Vector3.Cross(u,  v ).normalized; // perpendicular to [u,  v]
		right    = Vector3.Cross(up, u ).normalized; // perpendicular to [up, u]		
		position = wp;
	}
	
	public Vector3 Dot()
	{ // calculate the dot-product between each axis of the ONB and the world axes
		return new Vector3(Vector3.Dot(right,   Vector3.right  ),
						   Vector3.Dot(up,      Vector3.up     ),
						   Vector3.Dot(forward, Vector3.forward));
	}	
	public Vector3 Dot(ref ONB b)
	{ // calculate the dot-product between each axis of the two ONBs
		return new Vector3(Vector3.Dot(right,   b.right  ),
						   Vector3.Dot(up,      b.up     ),
						   Vector3.Dot(forward, b.forward));
	}
	// calcuate a random forward vector in the XZ plane
	public Vector3 RandomForward()
		{ return (Quaternion.AngleAxis(Random.value * 360.0f, up) * forward).normalized; }
	
	// get a degenerate ONB which cannot compute anything meaningful
	public bool IsValid()
	{ // a valid ONB should have three linearly independent vectors as base
		return right  .sqrMagnitude  >= 0.0f &&
			   up     .sqrMagnitude  >= 0.0f &&
			   forward.sqrMagnitude  >= 0.0f &&
			   position.sqrMagnitude <  Mathf.Infinity;
	}	
	// return details as string
	public override string ToString()
		{ return string.Format("P:{0} U:{1} V:{2} W:{3}", position, right, up, forward); }
	
	public bool IsInSqrRadius(ref List<ONB> list, float sqrRadius)
	{ // search for any ONB in list, that is within sqrRadius of b
		if (sqrRadius > 0.0f &&	// we need a radius
		    list != null &&		// AND a list
			list.Count > 0)		// AND entries
			for (int i = 0; i < list.Count; i++)
				if (IsInSqrRadius(list[i], sqrRadius))
					return true;
		return false;
	}
	public bool IsInSqrRadius(ONB b, float sqrRadius)
	{ // test if a,b are within a square distance from each other
		return b != null &&			// valid instance
		       b != this &&			// AND not testing against ourselves
		       sqrRadius > 0.0f &&	// AND square radius is larger 0
		       (position - b.position).sqrMagnitude <= sqrRadius;
	}
	public bool IsInSqrRadius(Transform b, float sqrRadius)
	{ // test if a,b are within a square distance from each other
		return b != null &&			// instance is valid
		       sqrRadius > 0.0f &&	// AND square radius is larger 0
		       (position - b.position).sqrMagnitude <= sqrRadius;
	}
	
	public void Draw(
		float scale              = 1.0f,// gizmo scale
		bool  drawSourceTriangle = true	// draw the (optional) triangle graphics
	)
	{ // draw ONB into scene view
#if UNITY_EDITOR
		const float w = 3.0f;
		Handles.color = Color.red;
		Handles.DrawAAPolyLine(w, new Vector3[]{position,position+right*scale  }); // X-axis
		Handles.color = Color.green;
		Handles.DrawAAPolyLine(w, new Vector3[]{position,position+up*scale     }); // Y-axis
		Handles.color = Color.blue;
		Handles.DrawAAPolyLine(w, new Vector3[]{position,position+forward*scale}); // Z-axis
		// if an optional source triangle is given, draw it
		if (source != null && drawSourceTriangle)
			source.Draw();
#endif // UNITY_EDITOR
	}
}
