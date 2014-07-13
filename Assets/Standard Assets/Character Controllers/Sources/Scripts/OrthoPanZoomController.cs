using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrthoPanZoomController : MonoBehaviour {
	public float Speed = 0.1F;
	public float GlideJourneyTime = 1F;

	private bool collideStop = false;
	private LinkedList<KeyValuePair<Vector3, float> > vecs;
	private float glideStartTime;
	private float glideMagnitudeX, glideMagnitudeY;	// glide magnitude
	private Vector3 glideVector;	// glide direction vector
	private Vector3 glideOrigin;

	const float LEFT_BOUNDS = -285.0F;
	const float RIGHT_BOUNDS = 285.0F;
	const float TOP_BOUNDS = 250.0F;
	const float BOTTOM_BOUNDS = 0.0F;
	

	// Use this for initialization
	void Start () {
		vecs = new LinkedList<KeyValuePair<Vector3, float> > ();
		Camera.main.orthographicSize = 83;
	}
	
	// Update is called once per frame
	void Update () {
		// TOUCHIES
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Moved) {
			// User is panning the camera with fingers
			Vector3 delta = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0) - getLastPos();
			if(delta != Vector3.zero) {
				addLastPos(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0));
				pan (delta);
			}
		} else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
			StopCoroutine("handleGlide");
			addLastPos(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0));
		} else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
			StartCoroutine("handleGlide");
		// MOUSIES
		} else if (Input.GetMouseButtonDown (0)) {
			// User initiates panning the camera with a mouse
			StopCoroutine("handleGlide");
			addLastPos(Input.mousePosition);
			print("x:" + transform.position.x + ", y:" + transform.position.y);
		} else if (Input.GetMouseButton (0)) {
			// User pans with the mouse AFTER initiating
			Vector3 delta = Input.mousePosition - getLastPos();
			if(delta != Vector3.zero) {
				addLastPos(Input.mousePosition);
				pan (delta);
			}
			print("x:" + transform.position.x + ", y:" + transform.position.y);
			
		} else if (Input.GetMouseButtonUp (0)) {
			// User ends a panning movement with the mouse
			StartCoroutine("handleGlide");
		}
	}

	IEnumerator handleGlide ()
	{
		if(vecs.Count > 0 && !touchExpired()) {
			glideStartTime = Time.time;
			glideOrigin = transform.position;
			glideMagnitudeX = -getMagnitudeX();
			glideMagnitudeY = -getMagnitudeY();
			float xy = Mathf.Clamp(System.Math.Abs((glideMagnitudeX+glideMagnitudeY)*0.001F),2,10);
			print ("Both:" + xy);
			glideVector = glideOrigin + (getVector() * xy);
			vecs.Clear();
			var fracComplete = 0.0F;
			while (fracComplete <= 1.0F) {
				Vector3 newPos = Vector3.Lerp(glideOrigin, glideVector, Mathfx.Sinerp(0.0F, 1.0F, fracComplete));
				if(IsClamped(newPos)) {
					yield break;
				}
				transform.position = newPos;
				fracComplete = (Time.time - glideStartTime) / GlideJourneyTime;
				yield return null;
			}
		}

	}

	private Vector3 ClampBounds(Vector3 newPos) {
		if(	transform.position.x - newPos.x <= LEFT_BOUNDS || 
		   transform.position.x - newPos.x >= RIGHT_BOUNDS) {
			newPos.x = 0.0F;
		}
		
		if(	transform.position.y - newPos.y <= BOTTOM_BOUNDS || 
		   transform.position.y - newPos.y >= TOP_BOUNDS) {
			newPos.y = 0.0F;
		}
		
		return newPos;
	}

	private bool IsClamped(Vector3 newPos) {
		if(	newPos.x <= LEFT_BOUNDS || 
		   	newPos.x >= RIGHT_BOUNDS) {
			return true;
		}
		
		if(	newPos.y <= BOTTOM_BOUNDS || 
		   	newPos.y >= TOP_BOUNDS) {
			return true;
		}
		
		return false;
	}
		
	void pan (Vector3 touchDeltaPosition) {
		Vector3 newPos = touchDeltaPosition * Speed;
		newPos = ClampBounds(newPos);
		transform.Translate (-newPos);			
	}

	private void addLastPos(Vector3 lastPos) {
		float deltaTime;
		if(vecs.Count == 0) {
			deltaTime = Time.time;
		} else {
			deltaTime = Time.time - vecs.Last.Value.Value;
		}

		KeyValuePair<Vector3, float> elem = new KeyValuePair<Vector3, float>(lastPos, Time.time);
		vecs.AddLast (elem);
		if(vecs.Count > 3) {
			vecs.RemoveFirst();
		}
	}

	Vector3 getLastPos ()
	{
		if(vecs.Count > 0) {		
			return vecs.Last.Value.Key;
		}
		return Vector3.zero;
	}

	bool touchExpired ()
	{
		return (Time.time - vecs.Last.Value.Value) > 0.03F;
	}

	float getMagnitudeX ()
	{
		KeyValuePair<Vector3, float> e1, e2;
		e1 = vecs.Last.Value;
		e2 = vecs.First.Value;

		var distance = (e2.Key.x - e1.Key.x);
		var time = System.Math.Abs (e1.Value - e2.Value);
		return distance/time;
	}

	float getMagnitudeY ()
	{
		KeyValuePair<Vector3, float> e1, e2;
		e1 = vecs.Last.Value;
		e2 = vecs.First.Value;
		
		var distance = (e2.Key.y - e1.Key.y);
		var time = System.Math.Abs (e1.Value - e2.Value);
		return distance/time;
	}

	Vector3 getVector ()
	{
		Vector3 e1, e2;
		e1 = vecs.Last.Value.Key;
		e2 = vecs.First.Value.Key;
		return e2 - e1;
	}
}
