using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        int boardSize = GameManager.Instance.BoardSize;
        transform.position = new Vector3(boardSize / 2.0f, 0, boardSize / 2.0f);
        transform.rotation = Quaternion.identity;
        transform.Rotate(new Vector3(60, 0, 0));
        transform.position -= transform.TransformDirection(new Vector3(0, 0, 1.3f * boardSize));
    }
}
