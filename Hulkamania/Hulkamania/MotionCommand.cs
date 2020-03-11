namespace Brandeis.AGSOL.Hulkamania
{
    /// <summary>
    /// A description of the motion of the Hulk at a point in time.
    /// </summary>
    public sealed class MotionCommand
    {
        #region Fields

        public double innerAcceleration;
        public double innerPosition;
        public double innerVelocity;
        public double outerAcceleration;
        public double outerPosition;
        public double outerVelocity;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new motion command.
        /// By default, all of the components are set to be ignored.
        /// </summary>
        public MotionCommand() {
            innerAcceleration = Hulk.ANY_MOTION;
            outerAcceleration = Hulk.ANY_MOTION;
            innerVelocity = Hulk.ANY_MOTION;
            outerVelocity = Hulk.ANY_MOTION;
            innerPosition = Hulk.ANY_MOTION;
            outerPosition = Hulk.ANY_MOTION;
        }

        /// <summary>
        /// Creates a new motion command based on a previous one.
        /// </summary>
        public MotionCommand(MotionCommand c)
        {
            innerAcceleration = c.innerAcceleration;
            outerAcceleration = c.outerAcceleration;
            innerVelocity = c.innerVelocity;
            outerVelocity = c.outerVelocity;
            innerPosition = c.innerPosition;
            outerPosition = c.outerPosition;
        }
        #endregion
    }
}
