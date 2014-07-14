using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrthoPanZoomController : MonoBehaviour {
	public float Speed = 0.1F;
	public float GlideJourneyTime = 1F;
	public float MaxOrthSize = 140.0F;
	public float MinOrthSize = 40.0F;
	public float MidOrthSize = 83.0F;
	public float LeftBounds = -385.0F;
	public float RightBounds = 385.0F;
	public float TopBounds = 367.0F;
	public float BottomBounds = -76.0F;
		
	bool collideStop = false;
	LinkedList<KeyValuePair<Vector3, float> > vecs;
	float glideStartTime;
	float glideMagnitudeX, glideMagnitudeY;	// glide magnitude
	Vector3 glideVector;	// glide direction vector
	Vector3 glideOrigin;

	// Use this for initialization
	void Start () {
		vecs = new LinkedList<KeyValuePair<Vector3, float> > ();
		Camera.main.orthographicSize = MidOrthSize;
	}
	
	// Update is called once per frame
	void Update () {
		// TOUCHIES
		
		if(Input.touchCount == 2) {
			var d1 = Input.GetTouch(0).deltaPosition;
			var p1 = Input.GetTouch(0).position;
			var d2 = Input.GetTouch(1).deltaPosition;
			var p2 = Input.GetTouch(1).position;
			var oldDist = Vector3.Distance(p1, p2);
			var newDist = Vector3.Distance(p1+d1, p2+d2);
			var newZoom = Camera.main.orthographicSize;
			if(newDist > oldDist) {
				newZoom-=2;
			} else if(newDist < oldDist) {
				newZoom+=2;
			}
			newZoom = Mathf.Clamp(newZoom, MinOrthSize, MaxOrthSize);
			Camera.main.orthographicSize = newZoom;
			
			
		} else if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Moved) {
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
			print("x:" + transform.localPosition.x + ", z:" + transform.localPosition.z);
		} else if (Input.GetMouseButton (0)) {
			// User pans with the mouse AFTER initiating
			Vector3 delta = Input.mousePosition - getLastPos();
			if(delta != Vector3.zero) {
				addLastPos(Input.mousePosition);
				pan (delta);
			}			
		} else if (Input.GetMouseButtonUp (0)) {
			// User ends a panning movement with the mouse
			StartCoroutine("handleGlide");
		}
	}

	IEnumerator handleGlide ()
	{
		if(vecs.Count > 0 && !touchExpired()) {
			glideStartTime = Time.time;
			glideOrigin = transform.localPosition;
			glideMagnitudeX = -getMagnitudeX();
			glideMagnitudeY = -getMagnitudeY();
			float xy = Mathf.Clamp(System.Math.Abs((glideMagnitudeX+glideMagnitudeY)*0.001F),2,8);
			glideVector = glideOrigin + (getVector() * xy);
			vecs.Clear();
			var fracComplete = 0.0F;
			while (fracComplete <= 1.0F) {
				Vector3 newPos = Vector3.Lerp(glideOrigin, glideVector, Mathfx.Sinerp(0.0F, 1.0F, fracComplete));
				if(IsClamped(newPos)) {
					yield break;
				}
				transform.localPosition = newPos;
				fracComplete = (Time.time - glideStartTime) / GlideJourneyTime;
				yield return null;
			}
		}

	}

	Vector3 ClampBounds(Vector3 newPos) {
		if(	transform.localPosition.x - newPos.x <= LeftBounds || 
		   transform.localPosition.x - newPos.x >= RightBounds) {
			newPos.x = 0.0F;
		}
		
		if(	transform.localPosition.z - newPos.y <= BottomBounds || 
		   transform.localPosition.z - newPos.y >= TopBounds) {
			newPos.y = 0.0F;
		}
		
		return newPos;
	}

	bool IsClamped(Vector3 newPos) {
		if(	newPos.x <= LeftBounds || 
		   	newPos.x >= RightBounds) {
			return true;
		}
		
		if(	newPos.z <= BottomBounds || 
		   	newPos.z >= TopBounds) {
			return true;
		}
		
		return false;
	}
		
	void pan (Vector3 touchDeltaPosition) {
		Vector3 newPos = touchDeltaPosition * Speed;
		newPos = ClampBounds(newPos);
		transform.Translate (new Vector3(-newPos.x, 0, -newPos.y));			
	}

	void addLastPos(Vector3 lastPos) {
		float deltaTime;
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
		var newVec = e2 - e1;
		return new Vector3(newVec.x, 0, newVec.y);
	}
}
