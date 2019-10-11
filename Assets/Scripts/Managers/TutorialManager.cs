﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : LevelManager
{
	public Image Finger;
	private TrailRenderer FingerTrail;
	private RectTransform FingerRectTransform;

	private bool shouldClearFingerTrail;
	public float FingerRadius;

	public TutorialTile TutorialTile;

	private Animator anim;

	private bool playerEnteredTile, pressed;
	private bool PlayerEnteredTile {
		get => playerEnteredTile;
		set {
			anim.SetBool("Spawned", value);
			playerEnteredTile = value;
		}
	}
	private bool Pressed {
		get => pressed;
		set
		{
			anim.SetBool("Pressed", value);
			pressed = value;
		}
	}

	public override void Start() {
		base.Start();
		anim = GetComponent<Animator>();
		FingerTrail = Finger.GetComponentInChildren<TrailRenderer>(true);
		FingerRectTransform = Finger.GetComponent<RectTransform>();

		AcceptingInputs = false;

		string sn = gameObject.scene.name;
		int tutorialNumber = int.Parse(sn.Substring(sn.Length - 1));
		anim.SetInteger("Level", tutorialNumber);

		RespawnManager.Player.aliveChanged += PlayerAliveChanged;
		TutorialTile.TutorialInit(
			() => Pressed = true,
			() => {
				GameManager.Instance.SetTimescale(1f);
				Finger.gameObject.SetActive(false);
			},
			(entered) => {
				if(entered) {
					GameManager.Instance.SetTimescale(0.05f);
				}
				else {
					GameManager.Instance.SetTimescale(1f);
				}
			}
		);
	}

	public override void Update() {
		base.Update();
		Finger.material.SetFloat("_Radius",FingerRadius);
	}

	public void LateUpdate() {
		if(shouldClearFingerTrail) {
			FingerTrail.Clear();
			shouldClearFingerTrail = false;
		}
	}

	public override void CreateRespawnManager() {
		RespawnManager = new RespawnManager(gameObject.scene);
	}

	public override void Reset(bool fromButton) {
		base.Reset(fromButton);

		ResetAnimation();
	}

	public override void Respawn() {
		Finger.gameObject.SetActive(true);
		PlayerEnteredTile = true;

		//StartCoroutine(TimeScaleIndepedentMoveTo(new Vector2(-92,-99), 1f, ReachedTile));

		base.Respawn();
	}

	public void ClearTrail() {
		shouldClearFingerTrail = true;
	}

	public void ReachedTile() {
		AcceptingInputs = true;
		anim.SetBool("AtStartingTile", true);
		FingerTrail.enabled = true;
	}

	public void PlayerAliveChanged(object sender, bool alive) {
		if(!alive) {
			ResetAnimation();
			Grid.Reset();
		}
	}

	private void ResetAnimation() {
		PlayerEnteredTile = false;
		Pressed = false;
		anim.SetBool("AtStartingTile", false);
		AcceptingInputs = false;
		FingerTrail.enabled = false;
		FingerTrail.gameObject.SetActive(false);
		Finger.gameObject.SetActive(true);
	}

	private IEnumerator TimeScaleIndepedentMoveTo(Vector3 moveTo, float time, Action onReachLocation = null) {
		float t = 0f;
		float yt = 1/120f;
		var yieldTime = new WaitForSecondsRealtime(yt);
		Vector3 startPosition = FingerRectTransform.anchoredPosition;

		while(t < time) {
			var l = Vector3.Lerp(startPosition, moveTo, t / time);
			FingerRectTransform.anchoredPosition = Vector3.Lerp(startPosition, moveTo, t/time);
			t += yt;
			yield return yieldTime;
		}

		FingerRectTransform.anchoredPosition = moveTo;
		onReachLocation?.Invoke();
	}
}
