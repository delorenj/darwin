using UnityEngine;
using System.Collections;

public class DontDestroy : MonoBehaviourBase {

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);
	}
}
