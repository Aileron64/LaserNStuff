using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public class _Debug : MonoBehaviour
{ // extension methods to Debug

	public static bool IsInViewport(Vector3 wp)
	{ // check if point is behind scene camera. if yes, clip
#if UNITY_EDITOR
		return (Camera.current.WorldToViewportPoint(wp).z > 0);
#else
		return false;
#endif // UNITY_EDITOR
	}

	public static void SceneNote
	( // display a dot and a line in Y to a text in the scene
		Vector3 wp,		 // WORLD position of the note
		string  text,	 // the text of the note
		Vector2 offset,	 // offset factor of label (WP+camera right/up)*f.x/.y
		Color   color	 // color of the dot/line
	)
	{ // draw a debug note in the scene
#if UNITY_EDITOR
		// check if point is behind scene camera. if yes, clip
		if (! IsInViewport(wp))
			return;
		// create an end position for the line to the note
		Vector3 labelWp = wp +
						  Camera.current.transform.right * offset.x +
						  Camera.current.transform.up    * offset.y;
		Handles.color   = color;								 // color of the note
		Handles.DrawAAPolyLine(3, new Vector3[]{ wp, labelWp }); // line from point to note
		Handles.DrawSolidDisc									 // at position of point
			(wp, Camera.current.transform.forward, 0.05f);
		Handles.RectangleCap									 // near position of note
			(0, labelWp, Camera.current.transform.rotation, 0.05f);
		
		// check if we're too far away. if yes, don't draw the label specifics
		if (HandleUtility.GetHandleSize(wp) > 30)
			return;
		
		// move the note a bit to the right, get a style, calculate size, draw it.
		float      hs = HandleUtility.GetHandleSize(labelWp);		// used for constant size when zooming
		GUIContent g  = new GUIContent(text);						// convert text
		GUIStyle   s  = new GUIStyle(EditorStyles.whiteMiniLabel);	// style of f.e. the FPS display		
		s.normal.textColor = color;									// set font color
		s.wordWrap         = true;									// use word wrap in the label
		s.fixedWidth       = 200;									// wrap at this pixel width
		s.fixedHeight      = s.CalcHeight(g, s.fixedWidth);			// calculate the height based on this
		labelWp       += Camera.current.transform.right * hs * 0.15f; // move a bit to the right		
		Handles.Label(labelWp, g, s);								// draw the label
#endif // UNITY_EDITOR
	}

	public static void LogBreak
	(
		string text,	 // info text displayed in the log
		int level = 1	 // level of StackTrace() that is used for additional info
	)
	{ // log error text and break editor, printing "text\nCallerClass->MethodName"
#if UNITY_EDITOR	
		level = Mathf.Clamp(level, 0, level);
		System.Reflection.MethodBase m =
			new System.Diagnostics.StackTrace().GetFrame(level).GetMethod();
		string callerID = m.DeclaringType.FullName + "->" + m.Name + "()";
		Debug.LogError(text + "\n" + callerID);
		Debug.Break();
#endif // UNITY_EDITOR		
	}
	
	// FPS DEBUGGING -------------------------------------------------------------------------
	public Text drawTo;							// UI element for drawing the current framerate
	public bool autoDimScreen = false;			// should the screen be allowed to dim?

	List<string> fpsText = new List<string>();	// fps amount text cache
	void Awake()
	{ // set the screen dimming setting
		if (! autoDimScreen)
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
		// cache text string from "000" to "120" for fps display
		for (int i = 0; i <= 120; i++)
			fpsText.Add(i.ToString("d3"));
	}
	
	float totalTime = 0.0f;	// the elapsed time between measurements
	float frames	= 0.0f;	// frames up to now
	float fps 		= 0.0f;	// the calculated FPS rate
	void LateUpdate()
	{ // count up time and rendered frames, then calculate FPS rate
		totalTime += Time.deltaTime;
		frames++;
		if (totalTime < 1.0f)
			return;
		// calculate framerate
		fps    = frames / totalTime;
		frames = totalTime = 0.0f;
		if (drawTo == null)
			return;
		// if we have a UI text, use it for showing the current FPS rate
		int textKey = (int)Mathf.Clamp(Mathf.Round(fps), 0, fpsText.Count-1);	// key for string cache
		drawTo.text = fpsText[textKey];											// set text from cache
	}
}
