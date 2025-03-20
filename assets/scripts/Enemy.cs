using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class Enemy : StaticBody2D
{
	[Export] string enemyName;
	[Export] PackedScene projectileScene;
	[Export] Node conductor;

	int calmMax = 50;
	int calmCurrent = 0;
	[Export] TextureProgressBar calmMeter;

	[Export] Color projectileColor;
	[Export] int projectileSpeed;

	string initialShootingGuide;
	string loopShootingGuide;
	string currentShootingGuide;
	int currentGuideIndex = 0;

    int guideLength;

	AnimatedSprite2D sprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LoadShootingGuide();
		sprite = GetChild<AnimatedSprite2D>(0);
		if (calmMeter != null)
		{
			calmMeter.MaxValue = calmMax;
			calmMeter.TintOver = projectileColor;
		}
		sprite.AnimationFinished += () => { sprite.Animation = "idle"; };
		currentShootingGuide = initialShootingGuide;
		guideLength = currentShootingGuide.Length;
	}

	/// <summary>
	/// Instead of _Process() let's use Beat() to run logic every beat.
	/// </summary>
	public void Beat(int beatIndex)
	{
		if (currentGuideIndex >= guideLength) ResetGuide(beatIndex);
		// Contingencies
		if (!(bool)conductor.Get("IsPlaying")) return;
		if (string.IsNullOrEmpty(currentShootingGuide))
		{
			SpawnProjectile();
			return;
		}
		if ((bool)conductor.Get("PrintToConsoleEnabled")) {
			GD.Print("Current Note: " + beatIndex);
		}

		//Convert the guide number to an int
		int guideNumber = currentShootingGuide[currentGuideIndex] - '0';
		if (guideNumber == 1)
			SpawnProjectile();
		else if (guideNumber > 0)
		{
			for (int i = 0; i < guideNumber; i++)
				SpawnProjectile();
		}
        currentGuideIndex++;
    }

    #region Shooting Guide Functions
    private void SpawnProjectile()
	{
		Projectile projectile = projectileScene.Instantiate() as Projectile;
		projectile.GlowColor = projectileColor;
		projectile.Speed = projectileSpeed;
		projectile.Orientation = GD.Randf() * 1.4f - 0.7f;
		AddChild(projectile);
		sprite.Animation = "swing";
		sprite.Play();
	}

	public void ResetGuide(int beatIndex)
	{
		if (currentShootingGuide != loopShootingGuide) currentShootingGuide = loopShootingGuide;
		currentGuideIndex = 0;
        guideLength = currentShootingGuide.Length;
	}
    private void LoadShootingGuide()
    {
        string filePath = "res://guides/" + (string)conductor.Get("TrackName") + "/" + enemyName + ".csv";

        using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        string fileContent = file.GetAsText();
        string[] fileContentArray = fileContent.Split(',');

		//File content is read in groups of 3:
		//1 - whether initial or loop, represented as i
		//2 - repetition count, represented as i + 1
		//3 - guide numbers, represented as i + 2
		for (int i = 0; i < fileContentArray.Length; i += 3)
		{
			int repetitionCount = int.Parse(fileContentArray[i + 1]);
			for (int j = 0; j < repetitionCount; j++)
			{
				if (fileContentArray[i] == "initial")
					initialShootingGuide += fileContentArray[i + 2];
				else
					loopShootingGuide += fileContentArray[i + 2];

            }
		}
    }
    #endregion

    public virtual void Pacify()
	{
		calmCurrent += 5;
		UpdateCalmness();
		if (calmCurrent >= calmMax) 
			GetParent<Node2D>().QueueFree();
	}

	private void UpdateCalmness()
	{
        calmMeter.Value = calmCurrent;
    }

	
}
