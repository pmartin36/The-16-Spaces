﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelSelect : MenuCopyComponent
{
	public MenuCopyManager CopyManager;
	public LevelSelectButton Back;
	public NumberedLevelSelectButton[] NumberedLevelButtons;
	
	public bool LevelSelectOpen => WorldSelected > 0;
	public int WorldSelected { get; set; }

	private Vector2 WorldSelectedTilePosition;
	private Vector2[] LevelPositions;
	private Vector2[] WorldPositions;

	private int highestUnlockedWorld;
	private int highestUnlockedLevel;
	private MenuManager menuManager;

	void Awake() {
		float tileWidth = 150;
		WorldSelectedTilePosition = new Vector2(2.5f * tileWidth, 1.5f * tileWidth);
		WorldPositions = new[] {
			new Vector2(-3.5f * tileWidth,  0.5f * tileWidth),
			new Vector2(-1.5f * tileWidth,  0.5f * tileWidth),
			new Vector2( 0.5f * tileWidth,  0.5f * tileWidth),
			new Vector2( 2.5f * tileWidth,  0.5f * tileWidth),
			new Vector2(-2.5f * tileWidth, -0.5f * tileWidth),
			new Vector2(-0.5f * tileWidth, -0.5f * tileWidth),
			new Vector2( 1.5f * tileWidth, -0.5f * tileWidth),
			new Vector2( 3.5f * tileWidth, -0.5f * tileWidth),
			new Vector2(-3.5f * tileWidth, -1.5f * tileWidth),
			new Vector2(-1.5f * tileWidth, -1.5f * tileWidth),
			new Vector2( 0.5f * tileWidth, -1.5f * tileWidth),
			new Vector2( 2.5f * tileWidth, -1.5f * tileWidth)
		};
		LevelPositions = new [] {
			new Vector2(-0.5f * tileWidth,  0.5f * tileWidth),
			new Vector2( 1.5f * tileWidth,  0.5f * tileWidth),
			new Vector2( 3.5f * tileWidth,  0.5f * tileWidth),
			new Vector2(-1.5f * tileWidth, -0.5f * tileWidth),
			new Vector2( 0.5f * tileWidth, -0.5f * tileWidth),
			new Vector2( 2.5f * tileWidth, -0.5f * tileWidth),
			new Vector2(-2.5f * tileWidth, -1.5f * tileWidth),
			new Vector2(-0.5f * tileWidth, -1.5f * tileWidth),
			new Vector2( 1.5f * tileWidth, -1.5f * tileWidth),
			new Vector2( 3.5f * tileWidth, -1.5f * tileWidth),
			//garbage
			Vector2.zero,
			Vector2.zero
		};

		int highest = GameManager.Instance.HighestUnlockedLevel;
		SceneHelpers.GetWorldAndLevelFromBuildIndex(highest, out highestUnlockedWorld, out highestUnlockedLevel);

		NumberedLevelButtons = GetComponentsInChildren<NumberedLevelSelectButton>().OrderBy(g => g.name).ToArray(); // 1 - 12
		for(int i = 0; i < NumberedLevelButtons.Length; i++) {
			NumberedLevelSelectButton b = NumberedLevelButtons[i];
			b.Init(i + 1);
			SetLevelSelectButton(b);
			b.TryEnableInteractable();
		}
		Back.Init();

		if(LevelSelectOpen) {
			NumberedLevelButtons[WorldSelected - 1].SetButtonInfo(
				WorldSelectedTilePosition, 
				WorldSelected, 
				true, 
				false);
			Back.transform.localScale = Vector3.one;
		}

    }

	public void Init(int worldSelected, MenuManager manager) {
		WorldSelected = worldSelected;
		menuManager = manager;
	}

	public void SetLevelSelectButton(NumberedLevelSelectButton b) {
		if(LevelSelectOpen) {
			b.SetPaywalled(false);
			if (b.Number != WorldSelected) {
				bool worldFullyUnlocked = WorldSelected < highestUnlockedWorld;

				// what is usually #12 is used as a flex tile to fill the spot of the world selected
				if(b.Number == 12) {
					if (WorldSelected < 11) {
						b.SetStayHidden(false);
						b.SetButtonInfo(LevelSelectPosition(WorldSelected), WorldSelected, worldFullyUnlocked || WorldSelected <= highestUnlockedLevel, false);
					}
					else {
						// only 10 levels, don't need to show if level 11 is selected
						b.SetStayHidden(true);
					}
				}
				else if(b.Number == 11) {
					// since 12 is flexing, 11 is never needed in level select
					b.SetStayHidden(true);
				}
				else {
					b.SetStayHidden(false);
					b.SetButtonInfo(LevelSelectPosition(b.Number), b.Number, worldFullyUnlocked || b.Number <= highestUnlockedLevel, false);
  				}
			}	
		}
		else {
			b.SetStayHidden(false); // worlds are never hidden
			b.SetButtonInfo(
				position:	WorldSelectPosition(b.Number), 
				num:		b.Number, 
				unlocked:	b.Number <= highestUnlockedWorld,
				paywalled:	b.Number > GameManager.Instance.HighestOwnedWorld
			);
		}
	}

	public void ButtonSelected(NumberedLevelSelectButton button) {
		if(LevelSelectOpen) {
			int buildIndex = SceneHelpers.GetBuildIndexFromLevel(WorldSelected, button.TempNumber.HasValue ? button.TempNumber.Value : button.Number);
			GameManager.Instance.LoadScene(buildIndex, null);
		}
		else {
			if(button.Paywalled) {
				Debug.Log($"TAKE ME TO THE STORE TO BUY WORLD {button.Number}!");
			}
			else {
				CopyManager.OnLevelChange(button.Number);

				(this.MirroredComponent as LevelSelect).OnLevelSelectNoAction(button.MirroredComponent as NumberedLevelSelectButton);
				OnLevelSelectNoAction(button);

				// tell camera to wipe
				// world one and no world share the same style
				if (WorldSelected > 1) {
					StartCoroutine(CameraWipe(0));
				}
			}
		}
	}

	public void BackSelected() {
		// wipe
		// world one and no world share the same style
		if (WorldSelected > 1) {
			StartCoroutine(CameraWipe(1));
		}

		(this.MirroredComponent as LevelSelect).BackAction();
		BackAction();	
	}

	public void BackAction() {
		int prevSelected = WorldSelected;
		WorldSelected = 0;

		Back.Interactable = false;

		// hide back button
		Back.SetHidden(true, () => Back.Interactable = true);

		foreach (NumberedLevelSelectButton b in NumberedLevelButtons) {
			b.TempNumber = null;

			if (prevSelected == b.Number) {
				// move button back to original position
				b.SetSlidePosition(WorldSelectPosition(b.Number), true);
			}
			else {
				b.SetHidden(true, () => {
					SetLevelSelectButton(b);
					b.SetHidden(false, null);
				});
			}
		}

		
	}

	public void OnLevelSelectNoAction(NumberedLevelSelectButton button) {
		WorldSelected = button.Number;

		// move button to S position
		button.SetSlidePosition(WorldSelectedTilePosition, false);

		// move all other buttons to their position
		foreach (NumberedLevelSelectButton b in NumberedLevelButtons) {
			if (b != button) {
				b.SetHidden(true, () => {
					SetLevelSelectButton(b);
					b.SetHidden(false, null);
					Back.SetHidden(false, null);
				});
			}
		}
	}

	/// <summary>
	/// Get the position on the template for the ith level on the level select screen
	/// </summary>
	public Vector2 LevelSelectPosition(int num) {
		return LevelPositions[num - 1];
	}

	/// <summary>
	/// Get the position on the template for the ith level on the world select screen
	/// </summary>
	public Vector2 WorldSelectPosition(int num) {
		return WorldPositions[num - 1];
	}

	private IEnumerator CameraWipe(float target) {
		float time = 0;
		float start = 1 - target;
		float animationTime = 0.5f;
		//yield return new WaitForSeconds(0.25f);
		while (time < animationTime) {
			float v = Mathf.Lerp(start, target, time / animationTime);
			menuManager.LevelBlend = v;
			time += Time.deltaTime;
			yield return null;
		}
		menuManager.LevelBlend = target;
	}
}
