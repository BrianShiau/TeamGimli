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
	private float ChangeTime;
	
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
	
	public void ResetGame()
	{
		if (this.WinningHero != null) {
			Application.LoadLevel (Application.loadedLevelName);
		}
	}
}