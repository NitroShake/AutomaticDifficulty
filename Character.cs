using Godot;
using System;

public partial class Character : CharacterBody2D
{
	public enum Team { Player, Enemy }
	public Team team;
	protected int health;
	protected int maxHealth;
	protected float projectileSpeed = 1000;
	protected double fireTime; //time between shots (seconds)
	protected double timeSinceLastFire = 0;
	protected bool canFire;
	Node2D projectileSpawn;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		health = maxHealth;
		projectileSpawn = GetNode<Node2D>("ProjectileSpawnPoint");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		trackFireTime(delta);
	}

	public virtual void receiveDamage(int damage, Character dmgOwner) {
		health -= damage;
		if (health <= 0) {
			dmgOwner.onScoreKill();
			QueueFree();
		}
	}

	protected void trackFireTime(double deltaTime) {
		timeSinceLastFire += deltaTime;
		if (timeSinceLastFire >= fireTime) {
			canFire = true;
			timeSinceLastFire = fireTime;
		} else {
			canFire = false;
		}
	}

	protected void fireProjectile() {
		Vector2 velocity = Vector2.FromAngle(Transform.Rotation) * projectileSpeed;
		//Vector2 spawnPos = Position + (Vector2.FromAngle(Transform.Rotation) * 100);
		//GD.Print(Mathf.RadToDeg(Transform.Rotation));
		PackedScene projectileScene = GD.Load<PackedScene>("res://Projectile.tscn");
		Projectile instance = (Projectile)projectileScene.Instantiate();
		//GD.Print(GetTree());
		instance.instantiate(this, projectileSpawn.GlobalPosition, velocity, 1);
		//GetOwner<Node>().AddChild(instance);
		GetNode<Node>("/root/World").AddChild(instance);
	}

	public virtual void onScoreHit() {

	}

	public virtual void onScoreKill() {

	}
}