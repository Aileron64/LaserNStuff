using UnityEngine;

public class _Random
{ // extension methods to Random

	public static Vector3 Vector3()
	{ // get a random Vector3 with every component in Range [0,1]
		return new Vector3(
			UnityEngine.Random.value, // X
			UnityEngine.Random.value, // Y
			UnityEngine.Random.value  // Z
		);
	}
	
	public static Vector3 BarycentricCoordinate()
	{ // get a random barycentric coordinate, so that x+y+z=1
		UnityEngine.Vector3 rnd = _Random.Vector3();
		float               f   = 1.0f / (rnd.x + rnd.y + rnd.z);
		return UnityEngine.Vector3.Scale(rnd, new UnityEngine.Vector3(f,f,f));
	}	
}
