using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public class Triangle
{ // functions for triangle calculations

	// without ref
	public Triangle(Vector3 a, Vector3 b, Vector3 c)
		{ Calculate(ref a,ref b,ref c); }	
	
	public Vector3 A;								// vertex A
	public Vector3 B;								// vertex B
	public Vector3 C;								// vertex C
	public Vector3 BA;								// B-A
	public Vector3 CA;								// C-A
	public Vector2 uvA;								// uv for A
	public Vector2 uvB;								// uv for B
	public Vector2 uvC;								// uv for C	
	public Vector3[] v;								// all vertices as array
	public Vector3 center;							// center of triangle
	public float   radius;							// radius that encapsulates triangle
	public float   sqrRadius;						// square of radius
	public float   area;							// total area of triangle
	public ONB     transform;						// uniform ONB for entire triangle
	public List<Vector3> maxEdgeVertices;			// vertices making up the max edge magnitude
	public Color   color = new Color(0,0,1,0.01f);	// color with which to Draw() it
	public int     layer = 0;						// which navigation layer it represents (optional)
	public Bounds  bounds;							// bounds of the triangle
	
	void Calculate
		(ref Vector3 a, ref Vector3 b, ref Vector3 c)
	{ // calculate a complete triangle from given vertices
		A = a; B = b; C = c;
		BA = b-a;
		CA = c-a;
		uvA = Vector2.zero;
		uvB = new Vector2(0,1);
		uvC = new Vector2(1,0);
		// calculate rest of particulars from given vectors
		v         = new Vector3[] { A,B,C,A };		// vertex array
		center    = Center (ref A,ref B,ref C);		// center of triangle
		radius    = Radius (ref A,ref B,ref C);		// encapsulating radius
		area      = Area   (ref A,ref B,ref C);		// total area
		sqrRadius = radius*radius;					// square of the radius
		transform = new ONB(B-A,C-A,A);				// uniform ONB across triangle
		bounds    = new Bounds(center, Vector3.zero);
		bounds.Encapsulate(A);
		bounds.Encapsulate(B);
		bounds.Encapsulate(C);
		// calculate the max edge vertices
		maxEdgeVertices = MaxEdgeVertices(ref A,ref B,ref C);
	}
	public bool    IsInRadius  (Triangle b)
	{ // do the two triangle radii intersect
		if (b == null)
			return false;
		float r = radius+b.radius;
		return (center - b.center).sqrMagnitude <= r*r;
	}
	// is point p in radius of the triangle area
	public bool    IsInRadius(ref Vector3 p)
		{ return (p - center).sqrMagnitude <= sqrRadius; }
	// calculate if point p is in triangle
	public bool    IsInside  (ref Vector3 p)
		{ return (IsInRadius(ref p) && IsInside(ref A,ref B,ref C,ref p)); }
	// get barycentric coordinate of p
	public Vector3 BarycentricCoordinate(ref Vector3 p)
		{ return BarycentricCoordinate(ref A,ref B,ref C,ref p); }
	public Vector3 BarycentricCoordinate(ref Vector2 p)
		{ return BarycentricCoordinate(ref uvA,ref uvB,ref uvC,ref p); }
	// get point X in triangle plane from barycentric coordinate bc
	public Vector3 FromBarycentricCoordinate(ref Vector3 bc)
		{ return FromBarycentricCoordinate(ref A,ref B,ref C,ref bc); }
	public void    RandomPoint(ref ONB to)
	{ // return a random point in the triangle
		Vector3 b      = _Random.BarycentricCoordinate(); // use a random barycentric
		Vector3 sample = (A * b.x)+(B * b.y)+(C * b.z);   // the result between A,B,C
		to.Calculate(ref transform, sample);
	}	
	// get the triangle vertices that make up the longest edge magnitude
	public List<Vector3> MaxEdgeVertices()
		{ return MaxEdgeVertices(ref A,ref B,ref C); }
	
	// get a hemisphere over a point in the triangle plane
	public List<Vector3> Hemisphere(
		ref Vector3 bc,                                 // barycentric coordinate for position
		ref Vector3 N,									// normal to use		
		float size    = 1.0f,							// size of the hemisphere
		int   samples = 256								// sample points to setup in hemisphere
	)
		{ return Hemisphere(ref A,ref B,ref C,ref bc,ref N,samples); }	
	// raycast this triangle primitive
	public float Raycast(ref Ray r, ref _Matrix3x4 m)
		{ return Raycast(ref A,ref BA,ref CA,ref r, ref m); }	
	// drawing functions
	public void    Draw(
		bool withONBAndTangent = true,	// draw the uniform ONB
		bool withCenterRadius  = true,	// draw center and radius circle
		bool withLabels        = true	// draw A,B,C labels
	)
		{ Draw(this, withONBAndTangent, withCenterRadius, withLabels); }
	public void    DrawSubTriangles(Vector3 p)
		{ DrawSubTriangles(ref A,ref B,ref C,ref p); }
	
	// STATIC --------------------------------------------------------------------------------------
	
	// calculate triangle center
	public static Vector3 Center
		(ref Vector3 a, ref Vector3 b, ref Vector3 c)
		{ return (a+b+c) * (1.0f / 3.0f); }
		
	public static float Radius
		(ref Vector3 a, ref Vector3 b, ref Vector3 c)
	{ // calculate a total radius from center to span of triangle
		Vector3 center = Center(ref a,ref b,ref c);
		float dA = (a - center).sqrMagnitude; // center->A
		float dB = (b - center).sqrMagnitude; // center->B
		float dC = (c - center).sqrMagnitude; // center->C
		// search largest magnitude delta and return as radius
		float largest = dA;
		if (dB > largest) largest = dB;
		if (dC > largest) largest = dC;
		return Mathf.Sqrt(largest);
	}	
	
	public static float Area
		(ref Vector3 a, ref Vector3 b, ref Vector3 c)
	{ // triangle area specified by unique vectors [a,b,c] -> (cross product parallelogram) / 2
		Vector3 u  = b-a;				 // get relative u from A->B
		Vector3 v  = c-a;				 // get relative v from A->C
		Vector3 cp = Vector3.Cross(u,v); // calculate the magnitude of the parellelogram
		return (0.5f * cp.magnitude);	 // return half of that magnitude
	}		

	public static Vector3 BarycentricCoordinate
		(ref Vector2 a, ref Vector2 b, ref Vector2 c, ref Vector2 p)
	{ // get barycentric coordinate from a point p in triangle [a,b,c] -> Vector2 f.e. UV's
		Vector3 A = a, B = b, C = c, P = p;
		return BarycentricCoordinate(ref A,ref B,ref C,ref P);
	}
	public static Vector3 BarycentricCoordinate
		(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 p)
	{ // get a barycentric coordinate for a point p in triangle [a,b,c]
		// calculated triangle areas:
		float A     = Area(ref a,ref b,ref c); // base (entire triangle)
		float As2   = Area(ref p,ref b,ref c); // -> sub alpha
		float As0   = Area(ref a,ref p,ref c); // -> sub beta
		float As1   = Area(ref a,ref b,ref p); // -> sub gamma
		// barycentric coordinates are based on triangle subArea factors
		float alpha = As2 / A;
		float beta  = As0 / A;
		float gamma = As1 / A;
		return new Vector3(alpha, beta, gamma);
	}

	// get point X in triangle plane from given vectors and barycentric coordinate bc
	public static Vector3 FromBarycentricCoordinate
		(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 bc)
		{ return bc.x * a + bc.y * b + bc.z * c; }		
		
	public static bool IsInside
		(ref Vector2 a, ref Vector2 b, ref Vector2 c, ref Vector2 p)
	{ // test if point p is in triangle [a,b,c]
		Vector3 A = a, B = b, C = c, P = p;
		return IsInside(ref A,ref B,ref C,ref P);
	}
	public static bool IsInside
		(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 p)
	{ // test if point p is in triangle [a,b,c]
		return Mathf.Approximately
			(BarycentricCoordinate(ref a,ref b,ref c,ref p).Sum(), 1.0f);
	}
	
	public static List<Vector3> MaxEdgeVertices
		(ref Vector3 a, ref Vector3 b, ref Vector3 c)
	{ // get the triangle vertices that make up the longest edge magnitude
		Vector3[] delta = new Vector3[] { b-a,c-a,c-b };
		float d = delta[0].sqrMagnitude;	// longest magnitude
		float r = 0;						// which vector set is listed in d
		for (int i = 1; i < delta.Length; i++)
			if (delta[i].sqrMagnitude > d)
				{ d = delta[i].sqrMagnitude; r = i; }
		if (r == 0) return new List<Vector3>(new Vector3[] { a,b });
		if (r == 1) return new List<Vector3>(new Vector3[] { a,c });
		return new List<Vector3>(new Vector3[] { b,c });
	}
	
	public static List<Vector3> Hemisphere(
		ref Vector3 a, ref Vector3 b, ref Vector3 c,	// triangle vertices
		ref Vector3 bc,                                 // barycentric coordinate for position
		ref Vector3 N,									// normal to use		
		int         samples = 256						// sample points to setup in hemisphere
	)
	{ // build a hemisphere over a point of the triangle described by barycentric coordinates
		Vector3 P = Triangle.FromBarycentricCoordinate(ref a,ref b,ref c,ref bc);
		return P.Hemisphere(N, null, samples);
	}
	
	public static float Raycast
		(ref Vector3 a, ref Vector3 ba, ref Vector3 ca, ref Ray r, ref _Matrix3x4 m)
	{ // raycast a triangle and return hit informations
		// equation is:
		// (beta)(b-a) + (gamma)(c-a) - (t)d = (o-a)
		Vector3    d      = -r.direction;
		Vector3    oa     = r.origin - a;		
		_Matrix3x4.Build(ref ba,ref ca,ref d,ref oa,ref m);
		Vector3    result = m.Gauss();
		// result is: [beta, gamma, t]. hit if [beta >= 0, gamma >= 0, (beta+gamma) <= 1]
		bool isHit = result.x >= 0 &&				// beta
		             result.y >= 0 &&				// gamma
					 (result.x+result.y) <= 1;		// (beta+gamma)
		return (isHit ? result.z : Mathf.Infinity);	// hit: return t, otherwise return Infinity
	}	
	
	public static int GetKeyTriangle(ref List<Triangle> list, Vector3 wp)
	{ // get triangle index that contains the point from a list of triangles
		if (list.Count > 0)
			for (int i = 0; i < list.Count; i++)
				if (list[i] != null &&			// triangle instance is valid
					list[i].IsInside(ref wp))	// and point is in triangle
						return i;
		return -1;
	}
	
	public static Statistics.DiscreetCDF
		GetDiscreetCDF(List<Triangle> list)
	{ // create a discreet area list usable with RandomCDF()
		if (list == null ||		// no list
		    list.Count <= 0)	// OR no entries
			return new Statistics.DiscreetCDF();
		List<float> areas = new List<float>(list.Count);
		for (int i = 0; i < list.Count; i++)
			if (list[i] != null)
				areas.Add(list[i].area);
		// create a DiscreetCDF from the source list
		Statistics.DiscreetCDF r = new Statistics.DiscreetCDF();
		r.Get(ref areas);
		return r;
	}

	public static Mesh GetMesh(List<Triangle> list)
	{ // create a mesh from a list of triangles
		if (list == null ||		// no list
		    list.Count <= 0)	// OR no entries
			return null;
		Mesh          r = new Mesh();			// the return mesh
		List<int    > f = new List<int    >();	// output triangles
		List<Vector3> v = new List<Vector3>();	// output vertices
		List<Vector3> n = new List<Vector3>();	// output normals
		// build up the lists from the given triangle primitices
		int atFace = 0;
		for (int i = 0; i < list.Count; i++)
			if (list[i] != null)
			{ // add up the lists of indices, vertices and normals
				Vector3 N = list[i].transform.up;
				f.AddRange(new int[]     { atFace*3+0,atFace*3+1,atFace*3+2 });
				v.AddRange(new Vector3[] { list[i].A, list[i].B, list[i].C  });
				n.AddRange(new Vector3[] { N,         N,         N          });
				atFace++;
			}
		// now dump the lists to the mesh
		r.vertices  = v.ToArray();
		r.normals   = n.ToArray();
		r.triangles = f.ToArray();
		return r;
	}
	
	public static void Draw(
		ref Vector3 a, ref Vector3 b, ref Vector3 c, Color color, int layer,
		bool withONB          = true,	// draw the uniform ONB
		bool withCenterRadius = true,	// draw center and radius circle
		bool withLabels       = true	// draw A,B,C labels
	)
	{ // draw a triangle in the viewport from three unique vectors [a,b,c]
#if UNITY_EDITOR
		Triangle triangle = new Triangle(a,b,c);
		triangle.color = color;
		triangle.layer = layer;
		Draw(triangle, withONB, withCenterRadius, withLabels);
#endif // UNITY_EDITOR		
	}	
	public static void Draw(
		Triangle t,						// the instance to draw
		bool withONB           = true,	// draw the uniform ONB
		bool withCenterRadius  = true,	// draw center and radius circle
		bool withLabels        = true	// draw A,B,C labels
	)
	{ // draw a triangle in the viewport from given instance
#if UNITY_EDITOR
		// draw body and enclosing radius
		Handles.color = Color.white;
		Handles.DrawSolidRectangleWithOutline(t.v, t.color, Color.black);
		if (withONB)
		{ // draw uniform ONB
			t.transform.Draw(0.25f);
			Gizmos.color = Color.yellow;
		}
		if (withCenterRadius)
		{ // draw center point and radius circle
			Handles.color = Color.white;
			Handles.DrawSolidDisc(t.center, t.transform.up, 0.015f);
			Handles.DrawWireDisc (t.center, t.transform.up, t.radius);
			Gizmos .color = Color.white;
			Gizmos .DrawWireCube (t.bounds.center, t.bounds.size);
		}
		// mark vertices with their names
		if (! withLabels)
			return;
		Handles.Label(t.A+t.transform.up*0.1f, "A");
		Handles.Label(t.B+t.transform.up*0.1f, "B");
		Handles.Label(t.C+t.transform.up*0.1f, "C");
#endif // UNITY_EDITOR
	}
	
	public static void DrawSubTriangles
		(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 p)
	{ // draw the three subtriangles in the viewport created by point p and triangle [a,b,c]
#if UNITY_EDITOR
		Handles.color = Color.white;
		Handles.Label(p + Vector3.up*0.5f, "P");
		float alpha = 0.1f;
		for (int i = 0; i < 3; i++)
		{ // for a point p in [a,b,c], there are three subtriangles
			if (i == 0) // -> sub alpha
				Draw(ref p,ref b,ref c,new Color(1,0,0,alpha),-1,false,false,false);
			if (i == 1) // -> sub beta
				Draw(ref a,ref p,ref c,new Color(0,1,0,alpha),-1,false,false,false);
			if (i == 2) // -> sub gamma
				Draw(ref a,ref b,ref p,new Color(0,0,1,alpha),-1,false,false,false);
		}
#endif // UNITY_EDITOR
	}	
}
