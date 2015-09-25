using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour {

    public float time = 1f;
	// Use this for initialization
	IEnumerator Start () {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
	}
	
}
