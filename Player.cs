using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Character
{
	List<PerformanceScore> scores = new();
	class PerformanceScore {
		public double score;
		public double durationRemaining; //time (seconds) before score unqueued/discounted
		public PerformanceScore(double score) {
			this.score = score;
			this.durationRemaining = 20;
		}

		public PerformanceScore(double score, double duration) {
			this.score = score;
			this.durationRemaining = duration;
		}
	}

	public Player() {
		team = Team.Player;
		maxHealth = 5;
		projectileSpeed = 800;
		fireTime = 0.25;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		handleTurn();
		handleFire();
		updateScores(delta);
	}

    public override void _PhysicsProcess(double delta)
    {
		handleMovement();
        base._PhysicsProcess(delta);
    }

	public override void receiveDamage(int damage, Character dmgOwner) {
		base.receiveDamage(damage, dmgOwner);
		scores.Add(new PerformanceScore(-300, 30));
	}

	public override void onScoreHit() {
		scores.Add(new PerformanceScore(10, 20));
	}

	public override void onScoreKill() {
		scores.Add(new PerformanceScore(100, 20));
	}

	public int getHealth() {
		return health;
	}

	public void updateScores(double deltaTime) {	
		bool loop = true;
		int i = 0;
		if (scores.Count > 0) {
			while (loop) {
				scores[i].durationRemaining -= deltaTime;
				//GD.Print(scores[i].score, scores[i].durationRemaining);
				if (scores[i].durationRemaining <= 0) {
					scores.RemoveAt(i);
				}
				else {
					i++; //i should only be increased if no item is removed, otherwise it'd skip an item
				}
				if (i >= scores.Count) {
					loop = false;
				}
			}
		}
	}

	public double sumScores() {
		double totalScore = 0;
		foreach (PerformanceScore score in scores) {
			totalScore += score.score;
		}
		return totalScore;
	}

	private void handleMovement() {
		float moveSpeed = 250;
		Vector2 direction = new Vector2();
		if (Input.IsActionPressed("up")) {
			direction.Y += -1;
		}
		if (Input.IsActionPressed("down")) {
			direction.Y += 1;
		}
		if (Input.IsActionPressed("left")) {
			direction.X += -1;
		}
		if (Input.IsActionPressed("right")) {
			direction.X += 1;
		}
		direction = direction.Normalized() * moveSpeed;
		Velocity = direction;
		MoveAndSlide();
	}

	private void handleFire() {
		if (Input.IsActionPressed("shoot") && canFire) {
			fireProjectile();
			timeSinceLastFire = 0; 
		}
	}
	
	private void handleTurn() {
		LookAt(GetGlobalMousePosition());
	}
}
