﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtons : MonoBehaviour
{
	private Animator _animator;
	private Animator Animator {
		get {
			_animator = _animator ?? GetComponent<Animator>();
			return _animator;
		}
	}

	public Button ResetButton;
	public GameObject SpawnButton;
	private Image SpawnHighlightBorder;
	private Image SpawnButtonImage;
	public float SpawnHighlightBorderRadius;

	public Sprite PlaySprite;
	public Sprite PauseSprite;


	public void Awake() {
		Image[] images = SpawnButton.GetComponentsInChildren<Image>();
		SpawnHighlightBorder = images.First(g => g.transform.parent == SpawnButton.transform);
		SpawnButtonImage = images.First(g => g.transform.parent != SpawnButton.transform);
	}

	public void PlayPauseButtonClicked() {
		GameManager.Instance.LevelManager.PlayPauseButtonClicked();
	}

	public void LateUpdate() {
		SpawnHighlightBorder.material.SetFloat("_Radius", SpawnHighlightBorderRadius);
	}

	public void ForceSetBasedOnPlayerAlive(bool alive) {
		Pause(!alive);
		ResetButton.interactable = alive;
		Animator.SetBool("highlight", !alive);
	}

	public void Pause(bool paused) {
		SpawnButtonImage.sprite = paused ? PlaySprite : PauseSprite;
	}

	public void Reset() {
		GameManager.Instance.LevelManager.Reset(true);
	}

	public void Menu() {
		GameManager.Instance.LoadScene(SceneHelpers.MenuBuildIndex);
	}
}
