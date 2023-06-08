using Godot;
using System;

public partial class Projectile : RigidBody2D
{
	Character owner;
	int damage;
	double lifeTimeLimit = 20;
	double lifeTime = 0;

	//Godot's node instantiation does not allow for constructors with parameters to be run
	//this functions as one, and should be called whenever this scene is loaded
	public void instantiate(Character owner, Vector2 position, Vector2 velocity, int damage) {
		this.owner = owner;
		Position = position;
		LinearVelocity = velocity;
		this.damage = damage;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Connect("body_entered", new Callable(this, MethodName.onBodyEntered));
		BodyEntered += onBodyEntered;
		switch (owner.team) {
			case Character.Team.Player:
				Modulate = Color.Color8(100, 180, 255, 255);
				break;
			case Character.Team.Enemy:
				Modulate = Color.Color8(255, 50, 120, 255);
				break;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		lifeTime += delta;
		if (lifeTime > lifeTimeLimit) {
			QueueFree();
		}
	}



	private void onBodyEntered(Node body) {
		handleCollision(body);
	}

	public void handleCollision(Node body) {
		//if the collision belongs to a character
		if (body is Character) {
			Character hitChar = (Character)body; //cast character
			//if collision is with member of other team
			if (hitChar.team != owner.team) {
				owner.onScoreHit();
				hitChar.receiveDamage(damage, owner); //deal damage to character
				damage = 0;
				QueueFree(); //queue self for deletion
			}
		}
	}
}
