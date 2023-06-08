using Godot;
using System;

public partial class Enemy : Character
{
	enum State {
		IntroMoving, Normal
	}
	State state = State.IntroMoving;
	Node2D target;
	float minTargetDistance = 120; //distance before enemy backs off from target

	GameManager gameManager;
	public double difficultyLevel;
	Random random = new Random();

	Vector2 basePosition;
	Sprite2D sprite;

	public Enemy() {
		team = Team.Enemy;
	}

	public void setDifficultyLevel(double difficultyLevel) {
		maxHealth = Convert.ToInt32(Math.Round(3 * difficultyLevel));
		//(maxHealth);
		health = maxHealth;
		projectileSpeed = 120 * (float)difficultyLevel;
		fireTime = 0.6 / difficultyLevel;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		gameManager = GetNode<GameManager>("/root/World");
		sprite = GetNode<Sprite2D>("Triangle");
		createStartPosition();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		target = GetNode<Node2D>("/root/World/PlayerBody");
		base._Process(delta);
		setColour();

		if (state == State.IntroMoving) {
			if (target != null) {
				faceTarget();
				fire();
			}
		}
		else if (state == State.Normal) {
			if (target != null) {
				faceTarget();
				fire();
			}
		}
	}

	public override void _PhysicsProcess(double delta) {
		if (state == State.Normal) {
			if (target != null) {
				move();
			}
		}
		else if (state == State.IntroMoving) {
			moveIntoPosition(delta);
		}
		MoveAndSlide(); //apply velocity
	}

	private void setColour()  {
		Vector3 baseColor = new Vector3(1, 0.6f, 0.8f);
		Vector3 color = baseColor * ((float)health / 5);
		//GD.Print(((float)health / 6)); //max health of any enemy is 6
		//GD.Print(color);
		//GD.Print(color.X, color.Y, color.Z);
		Modulate = new Color(color.X, color.Y, color.Z, 255);
	}

	private void createStartPosition() {
		basePosition = new Vector2(random.Next(40, (int)gameManager.worldBoundary.X - 40), 
									random.Next(40, (int)gameManager.worldBoundary.Y - 40));
		if (random.Next(0,2) == 0) {
			float xDiff = basePosition.X - (gameManager.worldBoundary.X / 2);
			if (xDiff < 0) {
				Position = new Vector2(-30, basePosition.Y);
			}
			else {
				Position = new Vector2(gameManager.worldBoundary.X + 30, basePosition.Y);
			}
		} else {
			float yDiff = basePosition.Y - (gameManager.worldBoundary.Y / 2);
			if (yDiff < 0) {
				Position = new Vector2(basePosition.X, -30);
			} 
			else {
				Position = new Vector2(basePosition.X, gameManager.worldBoundary.Y + 30);
			}
		}
	}

	private void moveIntoPosition(double delta) {
		float moveSpeed = 150;
		if (Position.DistanceTo(basePosition) <= moveSpeed * delta) {
			state = State.Normal;
		}
		else {
			Velocity = Position.DirectionTo(basePosition) * moveSpeed;
		}
	}

	private void faceTarget() {
		LookAt(target.GlobalPosition);
	}

	private void move() {
		float moveSpeed = 100;
		if (Position.DistanceTo(target.GlobalPosition) < minTargetDistance) {
			Velocity = Position.DirectionTo(target.GlobalPosition) * -1 * moveSpeed;
		}
		else {
			Velocity = new Vector2(0,0);
		}
	}

	private void fire() {
		if (canFire) {
			fireProjectile();
			timeSinceLastFire = 0; 
		}
	}
}
