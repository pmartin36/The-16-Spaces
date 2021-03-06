﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    void Start() {
		foreach(Canvas c in GetComponentsInChildren<Canvas>()) {
			c.worldCamera = CameraManager.Instance.Camera;
			if (!c.CompareTag("WinType")) {
				c.renderMode = RenderMode.WorldSpace;
			}
		}
    }
}
