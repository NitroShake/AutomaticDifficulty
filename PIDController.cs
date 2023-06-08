using System;
public class PIDController
{
	public double targetValue, proportionalGain, integralGain, derivativeGain, max, min, offset, lowPassFilterGain;
	double previousError = 0, totalError = 0;
	double timeCounter = 0;
	double previousDerivative = 0;

	public PIDController(double targetValue, double Kp, double Ki, double Kd, double offset, double lowPassFilterGain, double maxOutput, double minOutput) {
		this.proportionalGain = Kp;
		this.integralGain = Ki;
		this.derivativeGain = Kd;
		this.lowPassFilterGain = lowPassFilterGain;
		this.offset = offset;
		this.max = maxOutput;
		this.min = minOutput;
		this.targetValue = targetValue;
	}

	public double calculateValue(double currentInput, double deltaTime) {
		double error = currentInput - targetValue;
		double proportional = proportionalGain * error;

		totalError += error * deltaTime;
		totalError = Math.Clamp(totalError, (min / integralGain) / 1.3, (max / integralGain) / 1.3);
		double integral = integralGain * totalError;

		double derivative = (lowPassFilterGain * previousDerivative) + (1 - lowPassFilterGain) * ((error - previousError) / deltaTime);
		previousDerivative = derivative;
		previousError = error;
		derivative = derivative * derivativeGain;

		double output = offset + Math.Clamp(proportional + integral + derivative, min, max);

		timeCounter += deltaTime;
		Godot.GD.Print(timeCounter, ",", currentInput, ",", proportional, ",", integral, ",", derivative, ",", error, ",", output);

		return output;
	}
}

