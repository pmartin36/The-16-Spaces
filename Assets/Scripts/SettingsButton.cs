﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
	private Image image;
	[SerializeField]
	private Sprite SettingsOpenSprite;
	[SerializeField]
	private Sprite SettingsClosedSprite;

    void Start() {
		image = GetComponentsInChildren<Image>().First(i => i.gameObject != this.gameObject);
    }
	
	public void SetIcon(bool open) {
		image.sprite = open ? SettingsOpenSprite : SettingsClosedSprite;
	}
}
