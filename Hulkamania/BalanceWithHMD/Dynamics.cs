using log4net;
using System;
using System.Configuration;
using System.Reflection;

namespace Brandeis.AGSOL.Hulkamania.Tasks.BalanceWithHmd
{
    #region Delegates

    /// <summary>
    /// Function that defines the acceleration of the system.
    /// </summary>
    delegate double AccelerationFunction(Trial trial, AxisMotionState state, ControlInput controlInput);

    #endregion

    /// <summary>
    /// Dynamics calculations for inverted pendulum balancing task.
    /// </summary>
    internal static class Dynamics
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Enums
        
        /// <summary>
        /// What the joystick input changes. Joystick input is added before the dynamics calculations are run.
        /// </summary>
        private enum ControlMode
        {
            Acceleration,       // Joystick input changes acceleration
            Velocity,           // Joystick input changes velocity
            Position            // Joystick input changes position
        }

        #endregion

        #region Fields

        private static readonly ControlMode controlMode;

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor.
        /// </summary>
        static Dynamics()
        {
            logger.Debug("Static create: Dynamics()");

            switch (ConfigurationManager.AppSettings["ControlMode"]) {
                case "Acceleration":
                    controlMode = ControlMode.Acceleration;
                    break;
                case "Velocity":
                    controlMode = ControlMode.Velocity;
                    break;
                default:
                    controlMode = ControlMode.Position;
                    break;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Calculates the next motion, based on the current system state.
        /// </summary>
        /// <param name="trial">Trial parameters</param>
        /// <param name="currentState">Position, velocity, etc of the system</param>
        /// <param name="originalControlInput">Current joystick input</param>
        /// <param name="noise">Noise values to add to position before calculations</param>
        /// <param name="dt">Time elapsed since last calculation</param>
        /// <returns>The new position, velocity, etc of the system</returns>
        public static MotionCommand CalculateDynamics(Trial trial, MotionCommand currentState, ControlInput originalControlInput, 
            RotationAngles noise, double dt)
        {
            AxisMotionState axis1State;
            AxisMotionState axis2State;
            ControlInput controlInput;
            MotionCommand newMotionCommand;

            // Convert to structs used in dynamics code
            axis1State = new AxisMotionState(currentState.outerPosition, currentState.outerVelocity, 0);
            axis2State = new AxisMotionState(currentState.innerPosition, currentState.innerVelocity, 0);

            if (originalControlInput.blanked) {
                controlInput = new ControlInput(0, 0, false, false);
            } else {
                controlInput = new ControlInput(originalControlInput.x, originalControlInput.y, false, false);
            }

            // Handle joystick input + noise
            
            // figure out what the noise for each axis is
            double axis1Noise = noise.pitch;
            double axis2Noise = (Hulk.InnerAxis == Hulk.Axis.Roll) ? noise.roll : noise.yaw;

            // figure out how joystick input should affect each axis
            double axis1Delta = controlInput.y * trial.JoystickGain;
            double axis2Delta = controlInput.x * trial.JoystickGain;

            // uncomment for debugging purpose
            // System.Diagnostics.Debug.Write("Axis 1 noise: " + axis1Noise + ", joystick delta: " + axis1Delta + ", total delta: " + (axis1Delta + axis1Noise));
            // System.Diagnostics.Debug.WriteLine(", Axis 2 noise: " + axis2Noise + ", joystick delta: " + axis2Delta + ", total delta: " + (axis2Delta + axis2Noise));

            // Apply joystick input + noise
            switch (controlMode)
            {
                case ControlMode.Velocity:
                    axis1State.velocity -= axis1Delta + axis1Noise;
                    axis2State.velocity -= axis2Delta + axis2Noise;
                    break;

                case ControlMode.Position:
                    axis1State.position -= axis1Delta + axis1Noise;
                    axis2State.position -= axis2Delta + axis2Noise;
                    break;
            }

            // Integrate to obtain new motion
            Integrate(trial, axis1State, dt, ComputeAcceleration, controlInput);
            Integrate(trial, axis2State, dt, ComputeAcceleration, controlInput);

            // Upper limit on velocity for safety
            axis1State.velocity = Math.Min(axis1State.velocity, trial.MaxVelocity);
            axis1State.velocity = Math.Max(axis1State.velocity, -trial.MaxVelocity);
            axis2State.velocity = Math.Min(axis2State.velocity, trial.MaxVelocity);
            axis2State.velocity = Math.Max(axis2State.velocity, -trial.MaxVelocity);

            // Lower limit on velocity to match precision of motion controller
            int signInner = (Hulk.InnerAxis == Hulk.Axis.Roll) ? Math.Sign(axis1State.position - trial.MovingDirectionOfBalance.roll) : Math.Sign(axis1State.position - trial.MovingDirectionOfBalance.yaw);
            int signOuter = Math.Sign(axis2State.position - trial.MovingDirectionOfBalance.pitch);
            if (Math.Abs(axis1State.velocity) < 0.5)
            {
                axis1State.velocity = 0.5 * signInner;
            }
            if (Math.Abs(axis2State.velocity) < 0.5)
            {
                axis2State.velocity = 0.5 * signOuter;
            }

            // old code, in case we need to revert back
            //if (axis1State.velocity > 0.0) {
            //    axis1State.velocity = Math.Max(axis1State.velocity, 0.5);
            //} else {
            //    axis1State.velocity = Math.Min(axis1State.velocity, -0.5);
            //}
            //if (axis2State.velocity > 0.0) {
            //    axis2State.velocity = Math.Max(axis2State.velocity, 0.5);
            //} else {
            //    axis2State.velocity = Math.Min(axis2State.velocity, -0.5);
            //}

            // Convert back to structs used in rest of code
            newMotionCommand = new MotionCommand {
                innerPosition = axis2State.position,
                innerVelocity = axis2State.velocity,
                innerAcceleration = Math.Abs(axis2State.acceleration),
                outerPosition = axis1State.position,
                outerVelocity = axis1State.velocity,
                outerAcceleration = Math.Abs(axis1State.acceleration)
            };

            return newMotionCommand;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Computes the new acceleration for a given axis.
        /// This function can be modified to create a new dynamics model.
        /// </summary>
        /// <param name="trial">The current trial</param>
        /// <param name="state">The current motion of an axis</param>
        /// <param name="controlInput">The current joystick or forcing function input</param>
        /// <returns>The acceleration</returns>
        private static double ComputeAcceleration(Trial trial, AxisMotionState state, ControlInput controlInput)
        {
            if (controlMode == ControlMode.Acceleration) {
                return trial.AccelerationConstant * Math.Sin(state.position * Math.PI / 180.0) -
                    (controlInput.x * trial.JoystickGain);
            } else {
                return trial.AccelerationConstant * Math.Sin(state.position * Math.PI / 180.0);
            }
        }

        /// <summary>
        /// RK4 solver.
        /// </summary>
        /// <param name="trial">The current trial</param>
        /// <param name="state">State of the system (input / output)</param>
        /// <param name="dt">Time since last evaluation</param>
        /// <param name="accelFunction">Function to calculate acceleration</param>
        /// <param name="controlInput">The current joystick or forcing function input</param>
        private static void Integrate(Trial trial, AxisMotionState state, double dt, AccelerationFunction accelFunction,
            ControlInput controlInput)
        {
            AxisMotionDerivative a;
            AxisMotionDerivative b;
            AxisMotionDerivative c;
            AxisMotionDerivative d;
            double dadt;
            double dvdt;

            a = new AxisMotionDerivative(state.velocity, accelFunction(trial, state, controlInput));
            b = RK4Evaluate(trial, state, dt * 0.5, a, controlInput, accelFunction);
            c = RK4Evaluate(trial, state, dt * 0.5, b, controlInput, accelFunction);
            d = RK4Evaluate(trial, state, dt, c, controlInput, accelFunction);

            dadt = 1.0 / 6.0 * (a.dAngle + 2.0 * (b.dAngle + c.dAngle) + d.dAngle);
            dvdt = 1.0 / 6.0 * (a.dVelocity + 2.0 * (b.dVelocity + c.dVelocity) + d.dVelocity);

            state.position += dadt * dt;
            state.velocity += dvdt * dt;

            state.acceleration = dvdt;
        }

        /// <summary>
        /// Helper function for the RK4 solver.
        /// </summary>
        private static AxisMotionDerivative RK4Evaluate(Trial trial, AxisMotionState initial, double dt,
            AxisMotionDerivative d, ControlInput controlInput, AccelerationFunction accelFunction)
        {
            AxisMotionState state;

            state = new AxisMotionState(initial.position + d.dAngle * dt, initial.velocity + d.dVelocity * dt, 
                initial.acceleration);

            return new AxisMotionDerivative(state.velocity, accelFunction(trial, state, controlInput));
        }

        #endregion
    }

    #region Helper Classes

    /// <summary>
    /// The rate of change of motion of a single axis.
    /// </summary>
    internal sealed class AxisMotionDerivative
    {
        internal readonly double dAngle;
        internal readonly double dVelocity;

        internal AxisMotionDerivative(double dAng, double dVel)
        {
            dAngle = dAng;
            dVelocity = dVel;
        }
    }

    /// <summary>
    /// The motion of a single axis.
    /// </summary>
    internal sealed class AxisMotionState
    {
        internal double position;
        internal double velocity;
        internal double acceleration;

        internal AxisMotionState()
        {
        }

        internal AxisMotionState(double pos, double vel, double accel)
        {
            position = pos;
            velocity = vel;
            acceleration = accel;
        }
    }

    #endregion
}
