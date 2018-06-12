using UnityEngine;
using System.Collections.Generic;

public static class _Vector3
{ // extension methods to Vector3

	// use for invalid coordinates, etc.
	public static Vector3 Infinity()
		{ return Vector3.one * Mathf.Infinity; }
	
	// EXTENSION METHODS --------------------------------------------------------------------
	public static Vector3 Clamp
		(this Vector3 self, Vector3 min, Vector3 max)
	{ // clamp a vector3 in Range [min, max], sort min/max to prevent errors
		Vector3 r = Vector3.zero;
		r.x = Mathf.Clamp(self.x, (min.x < max.x ? min.x : max.x), (max.x > min.x ? max.x : min.x));
		r.y = Mathf.Clamp(self.y, (min.y < max.y ? min.y : max.y), (max.y > min.y ? max.y : min.y));
		r.z = Mathf.Clamp(self.z, (min.z < max.z ? min.z : max.z), (max.z > min.z ? max.z : min.z));
		return r;
	}
	
	public static float CrossYAngleDegree
		(this Vector3 self, Vector3 b)
	{ // return the cross Y angle rotation in +/- degrees for the normalized cross product of A,B
		// get angle in degrees between A,B
		Vector3 A     = self.normalized;
		Vector3 B     = b.normalized;
		Vector3 cp    = Vector3.Cross(A,B);		
		float   angle = Mathf.Asin(cp.y) * Mathf.Rad2Deg;
		/*	if A->B angle is larger than 90 degrees,
			the length of cp.y tends towards 0 again.
			
			Essentially the length behaves like this:
			degrees A->B    : 0->90->180
			resulting length: 0-> 1->  0
		    
			Also: depending on the order of A/B, we invert
			the turning direction. so we need to watch out
			for this to get [-180,180] degrees coverage.
			
			cp.y <  0.0f -> we're in range [-180,  0]
			cp.y >= 0.0f -> we're in range [   0,180]		   
		*/
		// A->B angle larger than 90 degrees? change range to either +/- 180
		if (Vector3.Dot(A,B) < 0)
			angle = (cp.y < 0.0f ? -180.0f : 180.0f) - angle;
		return angle;
	}
	
	public static Vector3 MaxComponent
		(this Vector3 self)
	{ // get a vector with the largest Abs() component, rest 0
		int largest = 0;					// start with x as "largest"
		for (int i = 1; i < 3; i++)
			if (Mathf.Abs(self[i]) >		// test y,z
				Mathf.Abs(self[largest]))	// against "largest"
				largest = i;
		// now set the largest component in the return value
		Vector3 r  = Vector3.zero;
		r[largest] = self[largest];
		return r;	
	}
	// return total sum of all the components
	public static float Sum(this Vector3 self)
		{ return (self.x+self.y+self.z); }
	
	// return the vector with coordinates in plane XZ,XY or YZ
	public static Vector3 XZ(this Vector3 self)
		{ return new Vector3(self.x,0.0f,self.z); }
	public static Vector3 XY(this Vector3 self)
		{ return new Vector3(self.x,self.y,0.0f); }
	public static Vector3 YZ(this Vector3 self)
		{ return new Vector3(0.0f,self.y,self.z); }
	
	public static List<Vector3> Hemisphere(
		this Vector3 self,			// self reference
		Vector3   N,				// normal to use		
		Transform toWorld = null,	// transform for moving coordinates into WORLD (optional)
		int       samples = 256		// sample points to setup in hemisphere
	)
	{ // build a hemisphere over a point of the triangle described by barycentric coordinates
		List<Vector3> hemisphere = new List<Vector3>();
		// point 0 is always the origin, rest are points in hemisphere
		N            = (toWorld == null ? N.normalized : toWorld.TransformDirection(N).normalized);
		Vector3    P = (toWorld == null ? self         : toWorld.TransformPoint(self));
		Quaternion R = Quaternion.LookRotation(N);
		hemisphere.Add(P);
		for (int i = 0; i < samples; i++)
		{ // calculate a point in the vertex hemisphere grid
			Vector3 hp = Random.onUnitSphere;
			hp.z = Mathf.Abs(hp.z);
			hemisphere.Add(P + R * hp);
		}
		return hemisphere;
	}
	
	public static List<Ray> RayCircle(
		this Vector3 self,			// self reference
		Vector3 N,					// normal for orientation of plane
		float   direction = 0.0f,	// starting direction in XY plane, around Z
		int     samples   = 6		// amount of samples on circle
	)
	{ // get X rays facing outwards of a circle center, positioned in a plane oriented toward N
		float      increment = 360.0f / samples;						// step amount
		List<Ray>  rays      = new List<Ray>(samples);					// final rays
		Quaternion R         = Quaternion.LookRotation(N.normalized);	// look rotation for plane
		for (int i = 0; i < samples; i++)
		{ // create rays in circle center origin, facing outwards
			Vector3 d =
				R *												// rotated into look direction of N
				Quaternion.Euler(0,0,direction + increment*i) *	// rotated around Z, starting at direction
				Vector3.up;										// base is in XY
			rays.Add(new Ray(self, d.normalized));
		}
		return rays;	
	}

	public static Vector3 Bezier(
		Vector3 P0,					// start of section
		Vector3 weightP0,			// results in P1 -> P0+weightP0
		Vector3 P3,					// end of section
		Vector3 weightP3,			// results in P2 -> P3-weightP3
		float t,					// position in section, range [0,1]
		float weightFactor = 1.0f	// scaling for the weights
	)
	{ // sample a bezier section with p0,p1,p2,p3
		// create bezier section
		Vector3 P1 = P0 + weightP0 * weightFactor;
		Vector3 P2 = P3 - weightP3 * weightFactor;
		// precalculate some term components
		t = Mathf.Clamp01(t);
		float omt  = 1.0f - t;		// (1-t)
		float omt2 = omt * omt;		// (1-t)^2
		float omt3 = omt * omt2;	// (1-t)^3
		float t2   = t * t;			// t^2
		float t3   = t * t2;		// t^3
		// now compute the position on the curve
		Vector3 r =
			omt3             * P0 + // B(t) = (1-t)^3      * P0 +
			3.0f * omt2 * t  * P1 + //        3(1-t)^2 * t * P1 +
			3.0f * omt  * t2 * P2 + //        3(1-t) * t^2 * P2 +
			t3               * P3;  //        t^3          * P3
		return r;		
	}
}
