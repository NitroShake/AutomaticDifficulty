using Godot;
using System;
using System.IO;

public partial class GameManager : Node
{
	public readonly Vector2 worldBoundary = new Vector2(1280, 720);
	Player player;
	PIDController difficultyPID = new PIDController(1000, 0.0003, 0.000015, 0.02, 1, 0.99995, 0.6, -0.5);
	double difficultyLevel;

	bool isGameOver = false;
	
	double enemySpawnTime = 2;
	double timeSinceLastSpawn = 0;

	//StreamWriter analyticWriter;

	Label debugLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//analyticWriter = File.CreateText("analytics.txt");

		player = GetNode<Player>("PlayerBody");
		debugLabel = GetNode<Label>("Label");
		ProcessPriority = -1; //run update after other nodes. important for detecting player health before it gets dequeued
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		adjustDifficulty(delta);
		if (!isGameOver) {
			runEnemySpawn(delta);
		}
		updateDebugLabel();

		if (player.getHealth() <= 0 && !isGameOver) {
			deleteAllEnemies();
			showGameOver();
			isGameOver = true;
		}

		if (Input.IsActionJustPressed("debugDeleteEnemies")) {
			deleteAllEnemies();
		}

		handleReloadScene();
	}

	void runEnemySpawn(double delta) {
		timeSinceLastSpawn += delta;
		if (timeSinceLastSpawn > enemySpawnTime) {
			timeSinceLastSpawn = 0;
			PackedScene enemyScene = GD.Load<PackedScene>("res://Enemy.tscn");
			Enemy instance = (Enemy)enemyScene.Instantiate();
			instance.setDifficultyLevel(difficultyLevel);
			AddChild(instance);
		}
	}

	void showGameOver() {
		Label label = new Label();
		label.Text = "Game Over - Press L to reset";
		label.Position = new Vector2(520, 340);
		AddChild(label);
	}

	void handleReloadScene() {
		if (Input.IsActionJustPressed("reset")) {
			GetTree().ReloadCurrentScene();
		}
	}

	void adjustDifficulty(double deltaTime) {
		difficultyLevel = difficultyPID.calculateValue(player.sumScores(), deltaTime);
		//debug difficultyLevel = 1;
		enemySpawnTime = 1.8 / (1 + ((difficultyLevel - 1) / 1.7));
	}

	void updateDebugLabel() {
		if (debugLabel != null) {
			debugLabel.Text = $"Player Health: {player.getHealth()}\n"
							+ $"Player Skill: {player.sumScores()}\n"
							+ $"PID output: {difficultyLevel}";
		}
	}

	void deleteAllEnemies() {
		foreach (Node node in GetChildren()) {
			if (node is Enemy) {
				node.QueueFree();
			}
		}
	}
}
