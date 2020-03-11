using System;
using System.Text;
using System.Configuration;

namespace Brandeis.AGSOL.Hulkamania
{
    abstract class MotionController
    {

#region Fields
    private static MotionController instance = null;
#endregion

#region Singleton
        internal static MotionController Instance { 
            get {
                if(instance==null){
                    instance = AppMain.UseDummyMotionController ? (new MotionControllerDummy() as MotionController) : (new MotionControllerHulk() as MotionController);
                }
                return instance;
            }
        }
#endregion

        #region Properties
        /// <summary>
        /// Current motion of the HULK. Read from the board each control loop cycle.
        /// </summary>
        internal virtual MotionCommand CurrentMotion { get { return new MotionCommand(); } }
        #endregion

        #region Internal Methods

        #region Motion Commands


        /// <summary>
        /// Starts the homing process. The HULK will seek out the reference locations for each axis.
        /// </summary>
        internal abstract void FindReference();

        /// <summary>
        /// Checks whether the homing reference locations have been found.
        /// </summary>
        /// <returns>True if references have been found</returns>
        internal abstract bool HasFoundReference();

        /// <summary>
        /// Checks whether the ongoing move command has finished.
        /// </summary>
        /// <returns>True if move is finished, false otherwise</returns>
        internal abstract bool IsMoveComplete();

        /// <summary>
        /// Sets the desired acceleration for the inner and outer axes.
        /// </summary>
        /// <param name="command">The motion command, containing the accelerations</param>
        internal abstract void SetAcceleration(MotionCommand command);


        /// <summary>
        /// Sets the desired position for the inner and outer axes.
        /// </summary>
        /// <param name="command">The motion command, containing the positions</param>
        internal abstract void SetPosition(MotionCommand command);


        /// <summary>
        /// Sets the desired velocity for the inner and outer axes.
        /// </summary>
        /// <param name="command">The motion command, containing the velocities</param>
        internal abstract void SetVelocity(MotionCommand command);


        /// <summary>
        /// Starts the HULK moving. Motion must be pre-programmed using SetCommandType and SetCommand.
        /// </summary>
        internal abstract void StartMotion();

        /// <summary>
        /// Stops all motors.
        /// </summary>
        internal abstract void StopMotion();

        
        /// <summary>
        /// Sets up the HULK to use position commands in a vector space.
        /// </summary>
        internal abstract void UseModulusPositionCommands();


        /// <summary>
        /// Sets up the HULK to use velocity commands.
        /// </summary>
        internal abstract void UseVelocityCommands();


        /// <summary>
        /// Use the current position as the zero reference for all future position coordinates.
        /// </summary>
        internal abstract void ZeroReference();


        #endregion

        #region Status Updates
                
        /// <summary>
        /// Reads the current motion of the Hulk from its encoders.
        /// </summary>
        /// <param name="updateRate">The number of motion reads in the past second</param>
        internal abstract void ReadCurrentMotion(int updateRate);

        #endregion

        #endregion

        #region Error Checking

        /// <summary>
        /// Checks whether the motion board has reported any modal (runtime) errors. If there is an error, 
        /// the motion board is immediately stopped and the application is signaled to cleanup and exit.
        /// </summary>

        internal abstract void CheckModalErrors();
        #endregion
 
    }
}
