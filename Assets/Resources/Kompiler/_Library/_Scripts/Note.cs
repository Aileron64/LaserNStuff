using UnityEngine;
using System.Collections;

public class Note : MonoBehaviour
{ // simple class to make visual notes in the scene view
	[Multiline(5)]
	public string text    = "";				// text of the note
	[Range(0.0f, 50.0f)]
	public float  offsetY = 5.0f;			// offset for the note in Y
	public Color  color   = Color.yellow;	// the note color

	public bool   showNote = true;			// display the note
	public bool   showONB  = true;			// display the transform ONB
	
#if UNITY_EDITOR
	void OnDrawGizmos()
	{ // draw the ONB/note into the scene view
		if (showNote)
		{ // draw the note into the scene view
			string info = gameObject.name + "\n\n" + text;
			_Debug.SceneNote(transform.position, info, new Vector2(0, offsetY), color);
		}
		if (showONB)
		{ // visualize the transform
			ONB o = new ONB(transform.forward, transform.right, transform.position);
			o.Draw(1.5f);
		}
	}
#endif // UNITY_EDITOR
}
