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

	Hero FindWinningPlayer()
	{
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

	void PlayVictorySound(Hero hero)
	{
		this.GameWonSound = SoundFX.Instance.OnMatchWon(hero);
		SoundFX.Instance.StopMusic();
	}

	void StopVictorySound()
	{
		AudioSourceExt.StopClipOnObject(this.GameWonSound);
		Destroy(this.GameWonSound);
	}

	void Update()
	{
		
	}

	public void ResetGame()
	{
		this.StopHeroAboutToWinSound();
		this.StopVictorySound();
		SoundFX.Instance.StartMusic();
		this.WinningHero = null;
		this.HeroAboutToWin = null;

		Hero[] heroes = FindObjectsOfType(typeof(Hero)) as Hero[];
		foreach (Hero hero in heroes)
		{
			hero.Reset();
		}
	}
}
