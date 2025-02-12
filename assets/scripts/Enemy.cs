using Godot;
using System;

public partial class Enemy : StaticBody2D
{
	[Export] PackedScene projectileScene;
	[Export] Conductor conductor;
	[Export] Color projectileColor;
	[Export] int projectileSpeed;
	[Export] string shootingGuide;
	[Export] int guidePhrase;

	int guideLength;
	int currentMeasure = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		conductor.OnBeat += Beat;
		ResetGuide();
	}

	/// <summary>
	/// Instead of _Process() let's use Beat() to run logic every beat.
	/// </summary>
	private void Beat(float beatIndex)
	{
		if (conductor.PrintToConsoleEnabled) {
			GD.Print("Current Measure: " + currentMeasure);
			GD.Print("Current Note: " + (beatIndex * conductor.BeatRate - conductor.BeatRate + (currentMeasure * conductor.BeatsPerMeasure)));
		}
		if (shootingGuide[Mathf.FloorToInt(beatIndex * conductor.BeatRate - conductor.BeatRate) + (currentMeasure * conductor.BeatsPerMeasure)] == '1')
			SpawnProjectile();
		if (beatIndex >= 4) currentMeasure++;		
        if (currentMeasure > guideLength) ResetGuide();
    }

	private void SpawnProjectile()
	{
		Projectile projectile = projectileScene.Instantiate() as Projectile;
		projectile.GlowColor = projectileColor;
		projectile.Speed = projectileSpeed;
		projectile.Orientation = GD.Randf() * 1.4f - 0.7f;
		AddChild(projectile);
	}

	public void ResetGuide()
	{
        guideLength = Mathf.FloorToInt(shootingGuide.Length / conductor.BeatsPerMeasure - 1);
		currentMeasure = 0;
	}
}
