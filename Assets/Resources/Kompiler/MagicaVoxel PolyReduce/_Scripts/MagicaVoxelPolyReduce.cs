using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public class MagicaVoxelPolyReduce : MonoBehaviour
{ // reduce MagicaVoxel OBJ mesh files to polycount optimized quad meshes.

#if UNITY_EDITOR
	// acceleration structure with raw mesh triangles converted to triangle primitives
	class triangleByList   : List      <Triangle                  > {}
	class triangleByRow    : Dictionary<Vector2,  triangleByList  > {};
	class triangleByNormal : Dictionary<Vector3,  triangleByRow   > {};
	class triangleByUV     : Dictionary<Vector2,  triangleByNormal> {};
	class triangleByQuad   : Dictionary<Triangle, Triangle        > {};
	
	// acceleration structure with converted quads as pure vertex coordinates
	class quadByVertex : List      <HashSet<Vector3>     > {};
	class quadByRow    : Dictionary<float,   quadByVertex> {};
	class quadByNormal : Dictionary<Vector3, quadByRow   > {};
	class quadByUV     : Dictionary<Vector2, quadByNormal> {};

	const string title = "MagicaVoxel PolyReduce";
	[MenuItem ("Tools/" + title + " &c")]
	static void Rebuild()
	{		
		if (Selection.activeGameObject == null)
			{ Debug.Log(title + ": Nothing selected."); return; }
		List<MeshFilter> all = new List<MeshFilter>();
		foreach (Transform t in Selection.transforms)
			all.AddRange(t.gameObject.GetComponentsInChildren<MeshFilter>());
		if (all == null ||	// no MeshFilter(s)
		    all.Count <= 0)	// OR no entries
			{ Debug.Log(title + ": No mesh(s) on selection."); return; }

		int processing = 0;
		foreach (MeshFilter mf in all)
			if (mf != null &&			// instance needs to be valid
			    mf.sharedMesh != null)	// AND have a shared mesh
			{ // convert every mesh we find below
				string path = AssetDatabase.GetAssetPath(mf.sharedMesh);
				if (path.ToLower().IndexOf("assets/") < 0)
					{ Debug.LogWarning(title + ": Conversion of mesh ['" + path + "'] not supported."); continue; }
			
				processing++;			// current item we're processing
				bool   cancel = false;	// did we cancel an operation?
				string at     = "(" + processing + " of " + all.Count + ")";
			
				// save start timestamp
				double startAt         = EditorApplication.timeSinceStartup;
				// sorted by "UV->normal direction->(row, horizontal axis)"->[Triangle list]
				triangleByUV triangles = new triangleByUV  ();
				// sorted by "UV->normal direction->row"->[List of "Quad vertices list"]
				quadByUV     quads     = new quadByUV();
				// convert the established mesh to triangle primitives
				float trianglesBefore  = (mf.sharedMesh.triangles.Length / 3.0f);
				int   totalTriangles   = 0;
				cancel                 = MeshToTriangles(mf, triangles, ref totalTriangles, at);
				if (triangles.Count <= 0)
					{ Debug.LogError(title + ": Conversion of mesh to Triangle primitives failed."); continue; }
				if (cancel)
					continue;				
				// search for quad connections and fill up the quad list
				int totalQuads = 0;
				cancel         = TrianglesToQuads(triangles, quads, totalTriangles, ref totalQuads, at);
				if (quads.Count <= 0)
					{ Debug.LogError(title + ": Conversion of Triangles->Quads failed."); continue; }
				if (cancel)
					continue;				
				// optimize the mesh structure by grouping quads
				cancel = GroupQuads(quads, totalQuads, at);
				if (cancel)
					continue;				
				// spit out the newly generated mesh
				string outputAs       = QuadsToMesh(mf, quads, title);
				// print out total time in seconds
				double totalTime      = EditorApplication.timeSinceStartup - startAt;
				float  trianglesAfter = (mf.sharedMesh.triangles.Length / 3.0f);
				float  fRemaining     = (trianglesAfter / trianglesBefore) * 100.0f;
				float  fReduction     = 100.0f - fRemaining;
				Debug.Log(string.Format(
					"{0}: Done. Processing Time: {1:0.00}s. [Triangles:{2}->{3}, remaining:{4:0.00}%, reduction:{5:0.00}%]",
					outputAs, totalTime, trianglesBefore, trianglesAfter, fRemaining, fReduction
				));
			}
	}
	
	const int progressUpdates = 4;		// how often to update the progress bar
	static bool MeshToTriangles(
		MeshFilter   mf,				// input mesh source
		triangleByUV triangles,			// output structure -> triangles
		ref int      totalTriangles,	// amount of triangles we're processing
		string       at					// element we're processing...
	)
	{ // convert an established mesh into a complex list of Triangle primitives
		if (mf == null ||				// no MeshFilter
		    mf.sharedMesh == null ||	// OR no shared mesh
			triangles == null)			// OR no output list
			return true;
		// get the triangles from the established mesh
		int[]     f  = mf.sharedMesh.triangles;
		Vector3[] v  = mf.sharedMesh.vertices;
		Vector3[] n  = mf.sharedMesh.normals;
		Vector2[] uv = mf.sharedMesh.uv;
		triangles.Clear();
		string title       = "Mesh->Triangle Primitives " + at;
		float  increment   = 1.0f / (f.Length-1);
		bool   wasCanceled = false;
		totalTriangles     = f.Length / 3;
		for (int i = 0; i < f.Length; i+=3)
		{ // create triangle primitives from all faces
			if (i % (f.Length / progressUpdates) == 0 &&
			    EditorUtility.DisplayCancelableProgressBar
					(title, "Converting...", i * increment))
				{ wasCanceled = true; break; }
			int     A  = f[i+0];
			int     B  = f[i+1];
			int     C  = f[i+2];
			Vector3 UV = uv[A];
			Vector3 N  = n [A].normalized;
			// winding cannot be trusted. we need to calculate a lot on the go
			Triangle t = new Triangle(v[A], v[B], v[C]);
			Vector2  r = Vector2.zero;
			if      (Mathf.Abs(N.x) > 0.0f) r = new Vector2(t.A.x,t.A.z); // X+,-: YZ plane, search Z dir
			else if (Mathf.Abs(N.y) > 0.0f) r = new Vector2(t.A.y,t.A.x); // Y+,-: XZ plane, search X dir
			else if (Mathf.Abs(N.z) > 0.0f) r = new Vector2(t.A.z,t.A.x); // Z+,-: XY plane, search X dir
			// create missing acceleration structure levels
			if (! triangles       .ContainsKey(UV)) triangles[UV]       = new triangleByNormal();
			if (! triangles[UV]   .ContainsKey(N )) triangles[UV][N]    = new triangleByRow   ();
			if (! triangles[UV][N].ContainsKey(r )) triangles[UV][N][r] = new triangleByList  ();
			// now finally add triangle by "UV->Normal->Row" key
			triangles[UV][N][r].Add(t);
		}		
		// clear progress bar, signal "wasn't canceled"
		EditorUtility.ClearProgressBar();
		return wasCanceled;
	}
	
	static bool TrianglesToQuads(
		triangleByUV triangles,			// input structure  -> triangle primitives
		quadByUV     quads,				// output structure -> quad primitives
		int          totalTriangles,	// how many input triangles we have
		ref int      totalQuads,		// how many output quads were generated
		string       at					// element we're processing...
	)
	{ // pair the triangles into quads by max edge connections
		if (triangles == null ||	// no triangle list
		    triangles.Count <= 0 ||	// OR no primitives
			quads == null)			// OR no output list
			return true;
		// calculate progess amount
		string title       = "Triangle Primitives->Quads " + at;
		float  increment   = 1.0f / (totalTriangles-1);
		bool   wasCanceled = false;
		int    atTriangle  = 0;
		// sorted by "triangle A connectes to triangle B"
		triangleByQuad connections = new triangleByQuad();
		quads.Clear();
		foreach (Vector2 UV in triangles.Keys)							// by UV color
			foreach (Vector3 N in triangles[UV].Keys)					// by normal direction
				foreach (Vector2 r in triangles[UV][N].Keys)			// by row in normal direction, axis plane
					foreach (Triangle A in triangles[UV][N][r])			// triangle A
					{ // go through all listed triangles as A/B
						atTriangle++;									// up the current progress
						if (atTriangle % (totalTriangles / progressUpdates) == 0 &&
						    EditorUtility.DisplayCancelableProgressBar
								(title, "Searching...", atTriangle*increment))
							{ wasCanceled = true; goto Cancel; }						
						if (! connections.ContainsValue(A))				// not yet listed in quads
							foreach (Triangle B in triangles[UV][N][r])	// check all triangles against A
								if (A != B &&							// not self-comparing
									! connections.ContainsValue(B) &&	// AND not yet listed in quads
									A.maxEdgeVertices.TrueForAll		// AND the same two vertices are in both edge lists
										(o => B.maxEdgeVertices.Contains(o)))
									{ // this is the connection A->B, forming a single quad
										connections[A] = B;
										// figure out the row of the quad
										float qr = 0;
										if      (Mathf.Abs(N.x) > 0.0f) qr = A.A.x; // X+,-: YZ plane
										else if (Mathf.Abs(N.y) > 0.0f) qr = A.A.y; // Y+,-: XZ plane
										else if (Mathf.Abs(N.z) > 0.0f) qr = A.A.z; // Z+,-: XY plane
										// create missing acceleration structure levels
										if (! quads       .ContainsKey(UV)) quads[UV]        = new quadByNormal();
										if (! quads[UV]   .ContainsKey(N )) quads[UV][N]     = new quadByRow   ();
										if (! quads[UV][N].ContainsKey(qr)) quads[UV][N][qr] = new quadByVertex();
										// now add the quad by "UV->Normal->Row" key
										quads[UV][N][qr].Add(
											new HashSet<Vector3>
												(new Vector3[] { A.A,A.B,A.C,B.A,B.B,B.C })
										);
										break;
									}
					}
		// export the amount of generated quad connections
		totalQuads = connections.Count;
Cancel:					
		// clear progress bar, signal "wasn't canceled"
		EditorUtility.ClearProgressBar();
		return wasCanceled;
	}
		
	static bool GroupQuads(
		quadByUV quads,			// input structure of quads
		int      totalQuads,	// amount of input quads
		string   at				// element we're processing...
	)
	{ // recursively group quads together as long as possible 
		if (quads == null ||	// no input list
		    quads.Count <= 0)	// OR no quad primitives
			return true;		
		// calculate progess amount
		string title       = "Grouping Quads " + at;
		float  increment   = 1.0f / ((totalQuads*2)-1);
		bool   wasCanceled = false;
		int    atQuad      = 0;
		// pick a grouping direction, start grouping
		foreach (Vector2 UV in quads.Keys)										// by UV color
			foreach (Vector3 N in quads[UV].Keys)								// by normal direction
				foreach (float r in quads[UV][N].Keys)							// by row
					for (int direction = 0; direction < 2; direction++)
					{ // grouping is done horizontally, then vertically
						atQuad += quads[UV][N][r].Count;						// up the current progress
						if (EditorUtility.DisplayCancelableProgressBar
								(title, "Searching...", atQuad*increment))
							{ wasCanceled = true; goto Cancel; }						
						// build stripe direction grouping list
						Dictionary<float, quadByVertex> inDirection = 
							new Dictionary<float, quadByVertex>();
RestartDirection:							
						BuildStripes(ref inDirection, quads[UV][N][r], N, direction);
						int groupedQuads = 0;
RestartSearch:
						// now process all the stripes
						foreach (float k in inDirection.Keys)
							foreach (HashSet<Vector3> A in inDirection[k])		// for every     "quad vertex set A"
								foreach (HashSet<Vector3> B in inDirection[k])	// against every "quad vertex set B"
									if (A != B &&								// no self-comparing
										A.Overlaps(B))							// AND shared vertices are present
									{ // are the two faces connected by two shared vertices?
										// get only the shared vertices between the quads
										HashSet<Vector3> shared = new HashSet<Vector3>(A);
										shared.IntersectWith(B);							
										if (shared.Count != 2)
											continue;		
										// combine vertices of both quads
										HashSet<Vector3> union = new HashSet<Vector3>();
										union.UnionWith(A);
										union.UnionWith(B);
										// now remove the shared vertices, leaving only the unique ones
										union.ExceptWith(shared);
										if (union.Count != 4)
											continue;
										// remove the two quads A,B. Add the new combined quad, restart search
										inDirection[k].Remove(A);
										inDirection[k].Remove(B);
										inDirection[k].Add(union);
										groupedQuads++;
										goto RestartSearch;
									}
						// merge the set back into the row list
						quads[UV][N][r].Clear();
						foreach (float k in inDirection.Keys)
							quads[UV][N][r].AddRange(inDirection[k]);					
						// did a new grouping occur? if yes, restart direction building
						if (groupedQuads > 0)
							goto RestartDirection;
					}
Cancel:					
		// clear progress bar, signal "wasn't canceled"
		EditorUtility.ClearProgressBar();
		return wasCanceled;
	}

	static void BuildStripes(
		ref Dictionary						// output dictionary
			<float, quadByVertex> r,		
		quadByVertex              list,		// input list of hashsets
		Vector3                   N,		// axis plane normal of quad list
		int                       direction	// build for which direction in axis plane
	)
	{ // build a list of quads connected by a direction in a given axis plane
		if (list.Count <= 0)
			return;
		// which vector component do we need to check for grouping direction
		int i = 0;
		if      (Mathf.Abs(N.x) > 0.0f)			// X+,-: YZ plane
			i = (direction == 0 ? 1 : 2); 		// X -> Z
		else if (Mathf.Abs(N.y) > 0.0f)			// Y+,-: XZ plane
			i = (direction == 0 ? 2 : 0); 		// Z -> X
		else if (Mathf.Abs(N.z) > 0.0f)			// Z+,-: XY plane
			i = (direction == 0 ? 1 : 0);		// Y -> X
		// now start grouping the quads into directions
		r.Clear();
		foreach (HashSet<Vector3> A in list)						// for every "quad vertex set A"
		{ // calculate center, add to dictionary
			Vector3 center = Vector3.zero;							// calculate a center for the quad
			foreach (Vector3 p in A)								// sum up all the vertices
				center += p;
			center /= A.Count;										// divide by fraction count
			// get the correct center direction for grouping, add quad to list
			float k = Mathf.Round(center[i] * 100.0f) / 100.0f;		// round key to second fraction
			if (! r.ContainsKey(k))									// key not yet present
				r[k] = new quadByVertex();							// add new list of hashsets
			r[k].Add(A);											// add current quad to list
		}
	}
	
	class Vertex
	{ // holds a single unique vertex with normal+UV
		Vector3 P  = Vector3.zero; // position
		Vector3 N  = Vector3.zero; // normal
		Vector2 UV = Vector2.zero; // UV
	
		public Vertex(Vector3 p, Vector3 n, Vector2 uv)
			{ P = p; N = n; UV = uv; }
		public void Dump
			(List<Vector3> asP, List<Vector3> asN, List<Vector2> asUV, List<Color32> asC)
		{ // every entry dumps into three lists
			if (asP == null || asN == null || asUV == null || asC == null)
				return;
			asP .Add(P          );	// position
			asN .Add(N          ); 	// normal
			asUV.Add(UV         ); 	// UV
			asC .Add(Color.white);	// color
		}
	}
	
	static string QuadsToMesh(
		MeshFilter mf,			// output mesh filter
		quadByUV   quads,		// input structure -> quads
		string     undoTitle	// title for undo operation
	)
	{ // output a quad list to a mesh structure
		if (mf == null ||		// no output MeshFilter
		    quads == null ||	// OR no quad list
			quads.Count <= 0)	// OR no quad entries
			return "";
		// get original asset path and convert to a path we can load from/save to
		string path   = AssetDatabase.GetAssetPath(mf.sharedMesh);
		string suffix = path.Substring(path.LastIndexOf("."));
		string name   = path.Replace(suffix, ".asset");
		// get a previous instance of the mesh. if none, create new asset at path.
		Mesh   m      = AssetDatabase.LoadAssetAtPath<Mesh>(name);
		if (m == null)
		{ // no old asset could be loaded. create new asset with new GUID.
			AssetDatabase.CreateAsset(new Mesh(), name);	// create file in DB
			AssetDatabase.SaveAssets ();					// save changes
			AssetDatabase.Refresh    ();					// update DB
			m = AssetDatabase.LoadAssetAtPath<Mesh>(name);	// reload from new path
		}		
		// now dump every quad into the mesh. decimate vertices by only saving unique sets
		HashSet<Vertex>       allVertices = new HashSet<Vertex      >();	// unique vertex entries of (P,N,UV)
		List   <List<Vertex>> allQuads    = new List   <List<Vertex>>();	// list of quad faces for indexing
		foreach (Vector2 UV in quads.Keys)									// by UV color
			foreach (Vector3 N in quads[UV].Keys)							// by normal direction
				foreach (float r in quads[UV][N].Keys)						// by row
					foreach (HashSet<Vector3> A in quads[UV][N][r])			// for every "quad vertex set A"
					{ // dump every quads contents into the lists					
						// get clockwise winding, create vertex entries
						List<Vector3> v = SortVerticesClockwise(A,N);		// raw A,B,C,D vectors
						List<Vertex > e = new List<Vertex>();				// face vertex entries
						foreach (Vector3 p in v)							// create vertex entry
							e.Add(new Vertex(p, N, UV));
						allVertices.UnionWith(e);							// add them to the global list
						// now create the references and add them to the face list
						List<Vertex> singleQuad = new List<Vertex>();
						singleQuad.AddRange									// add vertex-links for A+B triangle
							(new Vertex[] { e[0],e[1],e[2],	e[0],e[2],e[3] });
						allQuads.Add(singleQuad);							// add to global quad face list
					}			
		// convert the vertex lists to arrays. (V,N,UV)-lists have to be of same size (WTF, Unity?)
		List<Vertex > entries  = new List<Vertex >(allVertices);
		List<Vector3> vertices = new List<Vector3>();
		List<Vector3> normals  = new List<Vector3>();
		List<Vector2> uvs      = new List<Vector2>();
		List<Color32> colors   = new List<Color32>();
		foreach (Vertex e in entries)
			e.Dump(vertices, normals, uvs, colors);
		// relink the faces to a proper index into the dumped lists
		List<int> faces = new List<int>();
		foreach (List<Vertex> vertexIndices in allQuads)
			foreach (Vertex e in vertexIndices)
				faces.Add(entries.IndexOf(e));
		// set final contents of output mesh
		m.Clear();									// clear old contents first
		m.vertices = vertices.ToArray();			// vertices
		m.normals  = normals .ToArray();			// normals
		m.uv       = uvs     .ToArray();			// UV1
		m.colors32 = colors  .ToArray();			// vertex colors
		m.SetTriangles(faces.ToArray(), 0);			// triangles indices
		// optimize structures
		m.Optimize();
		m.RecalculateBounds();
		// generate lightmap set as UV2
		UnwrapParam up;								// unwrapping parameters
		UnwrapParam.SetDefaults(out up);			// set to default
		up.hardAngle  *= 4;							// increase hard angles for less seams
		up.packMargin *= 4;							// increase margin (otherwise: black streaks)
		Unwrapping.GenerateSecondaryUVSet(m, up);	// generate UV2

		// save changes, update AssetDatabase again
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh   ();
		
		// update the MeshFilter to use the generated mesh
		Undo.RecordObject(mf, undoTitle);
		mf.sharedMesh = m;
		// check if we have to update a MeshCollider as well
		MeshCollider mc = mf.GetComponent<MeshCollider>();
		if (mc != null)
		{ // if a MeshCollider is present, also update it's shared mesh
			Undo.RecordObject(mc, undoTitle);
			mc.sharedMesh = m;
		}
		// return the title of the converted new mesh asset
		return name;
	}
	
	static List<Vector3> SortVerticesClockwise(
		HashSet<Vector3> v,	// list of four vectors making up a quad
		Vector3          n	// axis plane normal to sort against
	)
	{ // sort a set of four vertices in a clockwise winding
		// normal needs to have search axis != 0, other components zeroed out.
		int  A = 0;				// vector component A
		int  B = 0;				// vector component B
		bool reverse = false;	// if normal is in negative range, reverse list
		if      (Mathf.Abs(n.x) > 0) { A = 2; B = 1; reverse = (n.x < 0); } // ZY plane
		else if (Mathf.Abs(n.y) > 0) { A = 0; B = 2; reverse = (n.y < 0); } // XZ plane
		else if (Mathf.Abs(n.z) > 0) { A = 1; B = 0; reverse = (n.z < 0); } // YX plane
		// calculate a center. this will be subtracted from every vertex
		Vector3 c = Vector3.zero;
		foreach (Vector3 p in v)
			c += p;
		c *= 1.0f / v.Count;
		/* now figure out, which vertices are positioned where in relation to center.
		   ABCD are identifiable by their center-delta and the two axis plane
		   vector components:
		   
		  (-,+) B     C (+,+)
			    -------
			    |  c  |
			    -------
		  (-,-) A     D (+,-) */
		List<Vector3> r = new List<Vector3>();
		for (int a = 0; a < 4; a++)		
			foreach (Vector3 p in v)
			{ // sort A,B,C,D according to their delta
				Vector3 d = p - c;
				if ((a == 0 && d[A] < 0 && d[B] < 0) ||	// A (-,-)
					(a == 1 && d[A] < 0 && d[B] > 0) ||	// B (-,+)
					(a == 2 && d[A] > 0 && d[B] > 0) ||	// C (+,+)
					(a == 3 && d[A] > 0 && d[B] < 0))	// D (+,-)
					{ r.Add(p); break; }
			}
		// depending on the normal, we might need to reverse the list
		if (reverse)
			r.Reverse();
		return r;
	}
	
#endif // UNITY_EDITOR
}
