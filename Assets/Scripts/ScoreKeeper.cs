using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Jolly;

public class ScoreKeeper : MonoBehaviour
{
	public float TimeToWin;
	public Hero WinningHero;
	public Hero HeroAboutToWin;
	public GameObject HeroAboutToWinSound;
	public GameObject GameWonSound;
	public float[] TimeTillChangeSprite;
	private static int score1 = 0;
	private static int score2 = 0;
	private static int score3 = 0;
	private static int score4 = 0;
	private float ChangeTime;
	private float x_displacement = Screen.width/(7.2f); 
	private float y_displacement = (Screen.height/6);
	private GUIStyle guiStyleRoundCount = new GUIStyle();
	
	void Start()
	{
		guiStyleRoundCount.fontSize = Screen.width/18;
		guiStyleRoundCount.alignment = TextAnchor.UpperCenter;
	}

	Hero FindWinningPlayer()
	{
		List<Hero> livingPlayers = new List<Hero>();
		
		Hero[] heroes = FindObjectsOfType(typeof(Hero)) as Hero[];
		foreach (Hero hero in heroes) {
			if (hero.IsAlive) {
				livingPlayers.Add(hero);
			}
		}
		if (livingPlayers.Count == 1)
		{
			return livingPlayers[0];
		}
		
		return null;
	}
	
	void StartHeroAboutToWinSound(Hero hero)
	{
		SoundFX.Instance.StopMusic();
		this.HeroAboutToWinSound = SoundFX.Instance.OnHeroAboutToWin(hero);
	}
	
	void StopHeroAboutToWinSound()
	{
		SoundFX.Instance.StartMusic();
		AudioSourceExt.StopClipOnObject(this.HeroAboutToWinSound);
		Destroy(this.HeroAboutToWinSound);
	}
	
	void PlayVictorySound(Hero hero, int index)
	{
		this.GameWonSound = SoundFX.Instance.OnMatchWon(hero, index);
		SoundFX.Instance.StopMusic();
		ChangeTime = Time.time + TimeTillChangeSprite[index - 1];
		Hero.CurrentWinnerIndex = index;
		if(index == 1) score1++;
		if(index == 2) score2++;
		if(index == 3) score3++;
		if(index == 4) score4++;
	}
	
	void StopVictorySound()
	{
		AudioSourceExt.StopClipOnObject(this.GameWonSound);
		Destroy(this.GameWonSound);
	}
	
	void Update()
	{
		if(ChangeTime != 0 && ChangeTime < Time.time) {
			WinningHero.GetComponentInChildren<SpriteRenderer>().sprite = WinningHero.BodySprites[WinningHero.BodySprites.Length - 1];
		}
		if (this.WinningHero != null)
		{
			return;
		}
		Hero hero = this.FindWinningPlayer();
		if (hero != null) {
			this.WinningHero = hero;
			this.PlayVictorySound(this.WinningHero, this.WinningHero.PlayerIndex);
		}
	}

	void OnGUI()
	{
		if(this.WinningHero != null) {
			int i = 0;
			GUI.Label(new Rect(115 + (i * x_displacement), 20, (i * x_displacement) + 115 + 50, 60), "Player 1", guiStyleRoundCount);
			GUI.Label(new Rect(115 + (i * x_displacement), 100, (i * x_displacement) + 115 + 50, 140), score1.ToString(), guiStyleRoundCount);
			
			i = 1;
			GUI.Label(new Rect(115 + (i * x_displacement), 20, (i * x_displacement) + 115 + 50, 60), "Player 2", guiStyleRoundCount);
			GUI.Label(new Rect(115 + (i * x_displacement), 100, (i * x_displacement) + 115 + 50, 140), score2.ToString(), guiStyleRoundCount);
			
			i = 2;
			GUI.Label(new Rect(115 + (i * x_displacement), 20, (i * x_displacement) + 115 + 50, 60), "Player 3", guiStyleRoundCount);
			GUI.Label(new Rect(115 + (i * x_displacement), 100, (i * x_displacement) + 115 + 50, 140), score3.ToString(), guiStyleRoundCount);
			
			i = 3;
			GUI.Label(new Rect(115 + (i * x_displacement), 20, (i * x_displacement) + 115 + 50, 60), "Player 4", guiStyleRoundCount);
			GUI.Label(new Rect(115 + (i * x_displacement), 100, (i * x_displacement) + 115 + 50, 140), score4.ToString(), guiStyleRoundCount);
		
		}
	}
	
	public void ResetGame()
	{
		if (this.WinningHero != null) {
			Application.LoadLevel (Application.loadedLevelName);
		}
	}
}