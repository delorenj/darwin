using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrthoPanZoomController : MonoBehaviour {
	public float Speed = 0.1F;
	public RaycastHit hit;
	public float GlideJourneyTime = 1F;
	public float MaxOrthSize = 140.0F;
	public float MinOrthSize = 40.0F;
	public float MidOrthSize = 83.0F;
	public float LeftBounds = -385.0F;
	public float RightBounds = 385.0F;
	public float TopBounds = 367.0F;
	public float BottomBounds = -76.0F;
		
	LinkedList<KeyValuePair<Vector3, float> > vecs;
	float glideStartTime;
	float glideMagnitudeX, glideMagnitudeY;	// glide magnitude
	Vector3 glideVector;	// glide direction vector
	Vector3 glideOrigin;

	// Use this for initialization
	void Start () {
		vecs = new LinkedList<KeyValuePair<Vector3, float> > ();
		Camera.main.orthographicSize = MidOrthSize;
		print ("Sleeping Rigidbody: " + rigidbody.IsSleeping());
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
			glideOrigin = transform.position;
			glideVector = vecs.Last.Value.Key - vecs.First.Value.Key;
			glideVector.Normalize();
			glideVector.z = glideVector.y;
			glideVector.y = 0;		
			vecs.Clear();
			var fracComplete = 0.0F;
			while (fracComplete <= 1.0F) {
				var easeParam = Mathfx.Sinerp(1, 0, fracComplete);
				if(isClamped(-glideVector * easeParam)) {
					print ("Hit detected");
					yield break;
				}				
				transform.Translate(-glideVector * easeParam);
				fracComplete = (Time.time - glideStartTime) / GlideJourneyTime;
				yield return null;
			}
		}

	}

	bool isClamped (Vector3 v)
	{
		if(transform.position.x + v.x < LeftBounds || transform.position.x + v.x > RightBounds) {
			return true;
		}

		if(transform.position.z + v.z < BottomBounds || transform.position.y + v.z > TopBounds) {
			return true;
		}		
		
		return false;
	}

	void pan (Vector3 touchDeltaPosition) {
		Vector3 newPos = touchDeltaPosition * Speed;
		transform.Translate (new Vector3(-newPos.x, 0, -newPos.y*2));			
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

	float getMagnitude ()
	{
		KeyValuePair<Vector3, float> e1, e2;
		e1 = vecs.Last.Value;
		e2 = vecs.First.Value;
		var distance = Vector3.Distance(e2.Key, e1.Key);
		var time = System.Math.Abs (e1.Value - e2.Value);
		return distance/time;	
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
		Vector3 newVec = (e2 - e1);
		newVec.z = newVec.y;
		newVec.y = 0;
		newVec = Camera.main.ScreenToWorldPoint(newVec);
		newVec.y = 0;
		return newVec;
	}
}
