using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Brandeis.AGSOL.Hulkamania.Tasks.BalanceWithHmd
{
    /// <summary>
    /// Parameters for a single trial.
    /// </summary>
    public sealed class Trial
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly RotationAngles directionOfBalance;
        private readonly RotationAngles visualOrientationOffset;

        private readonly String endSound;
        private readonly String moveSound;
        private readonly String reminderSound;
        private readonly String resetEndSound;
        private readonly String resetStartSound;
        private readonly String startSound;

        private readonly int trialNumber;

        private readonly double timeLimit;

        #endregion

        #region Enums
        
        /// <summary>
        /// How the joystick is mapped.
        /// </summary>
        public enum JoystickControl
        {
            Normal,             // Normal joystick control
            Clockwise,          // Left is up, up is right, right is down, down is left
            Counterclockwise,   // Left is down, down is right, right is up, up is left
            Mirrored            // Left is right, up is down, right is left, down is up
        }

        /// <summary>
        /// How the list of trials should be run.
        /// </summary>
        public enum Status
        {
            Initializing,           // Trial has not yet been started 
            Moving,                 // Chair is being moved to a specified location and is not under participant control
            BalancingDOBChanging,   // In between trials. DOB is moved while participant balances
            BalancingDOBStable,     // Trial is being run. DOB is stable while participant balances
            Resetting,              // Chair needs to be repositioned
            Complete                // Trial has been completed
        }

        #endregion

        #region Fields

        private List<RotationAngles> noisePoints;

        private JoystickControl joystickControl;

        private RotationAngles movingDirectionOfBalance;
        private RotationAngles beginAtPoint;

        private SoundPlayer endSoundPlayer;
        private SoundPlayer moveSoundPlayer;
        private SoundPlayer reminderSoundPlayer;
        private SoundPlayer resetEndSoundPlayer;
        private SoundPlayer resetStartSoundPlayer;
        private SoundPlayer startSoundPlayer;

        private Status status;

        private String condition;
        private String noiseProfile;

        private double accelerationConstant;
        private double joystickGain;
        private double maxAcceleration;
        private double maxVelocity;
        private double maxAngle;
        private double noiseAmplitude;
        private double restartDOBOffsetMinYaw;
        private double restartDOBOffsetMaxYaw;
        private double restartDOBOffsetMinPitch;
        private double restartDOBOffsetMaxPitch;
        private double restartDOBOffsetMinRoll;
        private double restartDOBOffsetMaxRoll;
        private int numIndications;
        private int noisePointsIndex;

        private bool usePitch;
        private bool useRoll;
        private bool useYaw;

        private bool restartWhenMaxAngle;
        private bool reminderGiven;

        private bool joystickIndicationsMandatory;

        #endregion

        #region Properties

        internal double RestartDOBOffsetMinYaw { get { return restartDOBOffsetMinYaw; } }
        internal double RestartDOBOffsetMaxYaw { get { return restartDOBOffsetMaxYaw; } }
        internal double RestartDOBOffsetMinPitch { get { return restartDOBOffsetMinPitch; } }
        internal double RestartDOBOffsetMaxPitch { get { return restartDOBOffsetMaxPitch; } }
        internal double RestartDOBOffsetMinRoll { get { return restartDOBOffsetMinRoll; } }
        internal double RestartDOBOffsetMaxRoll { get { return restartDOBOffsetMaxRoll; } }

        /// <summary>
        /// Acceleration of the inverted pendulum. Combines mass and gravity into single number.
        /// Increase this to increase how quickly the pendulum falls over.
        /// </summary>
        internal double AccelerationConstant
        {
            get
            {
                return accelerationConstant;
            }
        }

        /// <summary>
        /// If joystick indications are mandatory, the protocol execution will not advance to the next trial until at least one joystick indication has been given in the last 'TimeLimit' seconds
        /// </summary>
        internal bool JoystickIndicationsMandatory
        {
            get
            {
                return joystickIndicationsMandatory;
            }
        }

        /// <summary>
        /// The direction of balance, specified as roll/pitch/yaw angles.
        /// </summary>
        internal RotationAngles BeginAt
        {
            get {
                return beginAtPoint;
            }
        }

        /// <summary>
        /// Trial condition. Not used by Hulkamania, but needed for analysis.
        /// </summary>
        internal String Condition
        {
            get {
                return condition;
            }
        }

        /// <summary>
        /// The direction of balance, specified as roll/pitch/yaw angles.
        /// </summary>
        internal RotationAngles DirectionOfBalance
        {
            get {
                return directionOfBalance;
            }
        }

        /// <summary>
        /// The filename of the MP3 played at the end of the trial.
        /// </summary>
        internal String EndSound
        {
            get {
                return endSound;
            }
        }

        /// <summary>
        /// The mapping of the joystick directions.
        /// </summary>
        internal JoystickControl JoystickControlType
        {
            get {
                return joystickControl;
            }
        }

        /// <summary>
        /// How much the joystick affects the balance task. Higher numbers give larger effects.
        /// </summary>
        internal double JoystickGain
        {
            get {
                return joystickGain;
            }
        }

        /// <summary>
        /// The maximum acceleration (in degrees per second per second) that the chair is allowed to reach
        /// during the balancing task. 
        /// </summary>
        internal double MaxAcceleration
        {
            get {
                return maxAcceleration;
            }
        }

        /// <summary>
        /// The maximum angle (in degrees) that the chair can deviate from the direction of balance.
        /// If this angle is exceeded, the trial is stopped.
        /// </summary>
        internal double MaxAngle
        {
            get {
                return maxAngle;
            }
        }

        /// <summary>
        /// The maximum velocity (in degrees per second per second) that the chair is allowed to reach
        /// during the balancing task. 
        /// </summary>
        internal double MaxVelocity
        {
            get {
                return maxVelocity;
            }
        }

        /// <summary>
        /// The filename of the MP3 played as the HULK begins moving to the trial's start location.
        /// </summary>
        internal String MoveSound
        {
            get {
                return moveSound;
            }
        }

        /// <summary>
        /// The direction of balance, specified as roll/pitch/yaw angles, used before the trial starts.
        /// </summary>
        internal RotationAngles MovingDirectionOfBalance
        {
            get {
                return movingDirectionOfBalance;
            }
            set {
                movingDirectionOfBalance = value;
            }
        }

        /// <summary>
        /// The amount that the noise profile should be scaled before being added to the balance calculations.
        /// </summary>
        internal double NoiseAmplitude
        {
            get {
                return noiseAmplitude;
            }
        }

        /// <summary>
        /// The filename of the noise profile.
        /// </summary>
        internal String NoiseProfile
        {
            get {
                return noiseProfile;
            }
        }

        /// <summary>
        /// The number of times that the joystick trigger has been pressed this trial.
        /// </summary>
        internal int NumberIndications
        {
            get {
                return numIndications;
            }
            set {
                numIndications = value;
            }
        }

        /// <summary>
        /// True if the reminder sound has been played already this trial.
        /// </summary>
        internal bool ReminderGiven
        {
            get {
                return reminderGiven;
            }
            set {
                reminderGiven = value;
            }
        }

        /// <summary>
        /// The filename of the MP3 played to remind the participant to use the trigger button.
        /// </summary>
        internal String ReminderSound
        {
            get {
                return reminderSound;
            }
        }

        /// <summary>
        /// The filename of the MP3 played after the chair has been reset following a deviation to the max angle.
        /// </summary>
        internal String ResetEndSound
        {
            get {
                return resetEndSound;
            }
        }

        /// <summary>
        /// The filename of the MP3 played before the chair is reset following a deviation to the max angle.
        /// </summary>
        internal String ResetStartSound
        {
            get {
                return resetStartSound;
            }
        }

        /// <summary>
        /// True to restart trial when max angle is reached, false to continue to next trial.
        /// </summary>
        internal bool RestartWhenMaxAngle
        {
            get {
                return restartWhenMaxAngle;
            }
        }

        /// <summary>
        /// The filename of the MP3 played at the start of the trial.
        /// </summary>
        internal String StartSound
        {
            get {
                return startSound;
            }
        }

        /// <summary>
        /// The maximum number of seconds that the trial should last.
        /// </summary>
        internal double TimeLimit
        {
            get {
                return timeLimit;
            }
        }

        /// <summary>
        /// The sequential order number of the trial.
        /// </summary>
        internal int TrialNumber
        {
            get {
                return trialNumber;
            }
        }

        /// <summary>
        /// Current status (initializing, balancing, resetting, etc) of the trial.
        /// </summary>
        internal Status TrialStatus
        {
            get {
                return status;
            }
            set {
                status = value;
            }
        }

        /// <summary>
        /// Whether to activate the pitch axis during this trial.
        /// </summary>
        internal bool UsePitch
        {
            get {
                return usePitch;
            }
        }

        /// <summary>
        /// Whether to activate the roll axis during this trial.
        /// </summary>
        internal bool UseRoll
        {
            get {
                return useRoll;
            }
        }

        /// <summary>
        /// Whether to activate the yaw axis during this trial.
        /// </summary>
        internal bool UseYaw
        {
            get {
                return useYaw;
            }
        }

        /// <summary>
        /// The orientation offset in the visual scene, specified as roll/pitch/yaw angles.
        /// </summary>
        internal RotationAngles VisualOrientationOffset
        {
            get {
                return visualOrientationOffset;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Creates the trial from the row/column data contained in the csvreader
        /// </summary>
        /// <param name="csvReader">The csvreader containing the row and column data</param>
        /// <param name="row">The row number of the data to use for this trial</param>
        internal Trial(CsvReader csvReader, int row)
        {
            logger.Debug("Create: Trial(CsvReader, int)");

            status = Status.Initializing;

            directionOfBalance = new RotationAngles();
            movingDirectionOfBalance = new RotationAngles();
            visualOrientationOffset = new RotationAngles();

            beginAtPoint = new RotationAngles();

            reminderGiven = false;

            trialNumber = csvReader.getValueAsInt("TrialNumber", row);
            condition = csvReader.getValueAsString("Condition", row);
            directionOfBalance.roll = csvReader.getValueAsDouble("DOBRoll", row);
            directionOfBalance.pitch = csvReader.getValueAsDouble("DOBPitch", row);
            directionOfBalance.yaw = csvReader.getValueAsDouble("DOBYaw", row);
            accelerationConstant = csvReader.getValueAsDouble("AccelConstant", row);
            maxAcceleration = csvReader.getValueAsDouble("MaxAcceleration", row);
            maxVelocity = csvReader.getValueAsDouble("MaxVelocity", row);
            maxAngle = csvReader.getValueAsDouble("MaxAngle", row);
            maxAcceleration = csvReader.getValueAsDouble("MaxAcceleration", row);
            restartWhenMaxAngle = csvReader.getValueAsBool("RestartWhenMaxAngle", row);
            timeLimit = csvReader.getValueAsDouble("TimeLimit", row);
            joystickGain = csvReader.getValueAsDouble("JoystickGain", row);

            restartDOBOffsetMaxYaw = csvReader.getValueAsDouble("RestartDOBOffsetMaxYaw", row);
            restartDOBOffsetMinYaw = csvReader.getValueAsDouble("RestartDOBOffsetMinYaw", row);
            restartDOBOffsetMaxPitch = csvReader.getValueAsDouble("RestartDOBOffsetMaxPitch", row);
            restartDOBOffsetMinPitch = csvReader.getValueAsDouble("RestartDOBOffsetMinPitch", row);
            restartDOBOffsetMaxRoll = csvReader.getValueAsDouble("RestartDOBOffsetMaxRoll", row);
            restartDOBOffsetMinRoll = csvReader.getValueAsDouble("RestartDOBOffsetMinRoll", row);

            string val = csvReader.getValueAsString("JoystickControl", row);
            if (val.ToUpper() == "MIRROR")
            {
                joystickControl = JoystickControl.Mirrored;
            }
            else if (val.ToUpper() == "CW")
            {
                joystickControl = JoystickControl.Clockwise;
            }
            else if (val.ToUpper() == "CCW")
            {
                joystickControl = JoystickControl.Counterclockwise;
            }
            else
            {
                joystickControl = JoystickControl.Normal;
            }

            noiseProfile = csvReader.getValueAsString("NoiseProfile", row);
            noiseAmplitude = csvReader.getValueAsDouble("NoiseAmplitude", row);
            useRoll = csvReader.getValueAsBool("UseRoll", row);
            usePitch = csvReader.getValueAsBool("UsePitch", row);
            useYaw = csvReader.getValueAsBool("UseYaw", row);

            joystickIndicationsMandatory = csvReader.getValueAsBool("JoystickIndicationsMandatory", row);

            bool useBeginPoint = false;
            if (csvReader.hasValueAsDouble("BeginAtPitch", row) &&
                csvReader.hasValueAsDouble("BeginAtYaw", row) &&
                csvReader.hasValueAsDouble("BeginAtRoll", row))
            {
                beginAtPoint.yaw = csvReader.getValueAsDouble("BeginAtYaw", row);
                beginAtPoint.pitch = csvReader.getValueAsDouble("BeginAtPitch", row);
                beginAtPoint.roll = csvReader.getValueAsDouble("BeginAtRoll", row);
                useBeginPoint = true;
            }

            if (!useBeginPoint)
            {
                beginAtPoint = null;
            }

            visualOrientationOffset.yaw = csvReader.getValueAsDouble("VisualYawOffset", row);
            visualOrientationOffset.pitch = csvReader.getValueAsDouble("VisualPitchOffset", row);
            visualOrientationOffset.roll = csvReader.getValueAsDouble("VisualRollOffset", row);

            moveSound = csvReader.getValueAsString("MoveSound", row);
            startSound = csvReader.getValueAsString("TrialStartSound", row);
            endSound = csvReader.getValueAsString("TrialEndSound", row);
            resetStartSound = csvReader.getValueAsString("ResetStartSound", row);
            resetEndSound = csvReader.getValueAsString("ResetEndSound", row);
            reminderSound = csvReader.getValueAsString("ReminderSound", row);

            // sanity check
            if (restartDOBOffsetMaxYaw > maxAngle)
            {
                throw new Exception("Error when reading row " + (row+1) + " from file " + csvReader.FileName + "\r\n" + "Reason: the maximum DOB yaw offset when restarting (column: RestartDOBOffsetMaxYaw) is larger than the maximum angle (column: MaxAngle)"); 
            }
            if (restartDOBOffsetMaxPitch> maxAngle)
            {
                throw new Exception("Error when reading row " + (row + 1) + " from file " + csvReader.FileName + "\r\n" + "Reason: the maximum DOB pitch offset when restarting (column: RestartDOBOffsetMaxPitch) is larger than the maximum angle (column: MaxAngle)");
            }
            if (restartDOBOffsetMaxRoll > maxAngle)
            {
                throw new Exception("Error when reading row " + (row + 1) + " from file " + csvReader.FileName + "\r\n" + "Reason: the maximum DOB roll offset when restarting (column: RestartDOBOffsetMaxRoll) is larger than the maximum angle (column: MaxAngle)");
            }

//            SetNoiseProfile();
            SetSoundPlayers();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// True if the max angle for the trial has been exceeded.
        /// </summary>
        /// <param name="currentMotion">The current position</param>
        /// <returns>True if the angle is exceeded</returns>
        internal bool ExceedsMaxAngle(MotionCommand currentMotion)
        {
            return ((Math.Abs(currentMotion.innerPosition) >= MaxAngle) || (Math.Abs(currentMotion.outerPosition) >= MaxAngle));
        }

        /// <summary>
        /// Returns the next noise value. Noise values are specified in a .csv file.
        /// </summary>
        /// <returns>The noise value, multiplied by the desired noise amplitude</returns>
        internal RotationAngles GetNextNoise()
        {
            if (noisePoints == null) {
                return new RotationAngles();
            }

            if (noisePointsIndex >= noisePoints.Count) {
                noisePointsIndex = 0;
            }

            return noisePoints[noisePointsIndex++];
        }

        /// <summary>
        /// Plays the sound that signals the end of a trial.
        /// </summary>
        internal void PlayEndSound()
        {
            if (endSoundPlayer != null) {
                SoundFactory.dimAllVolumes(true, endSoundPlayer);
                endSoundPlayer.Play(PlaybackCompleted);
            }
        }

        /// <summary>
        /// Plays the sound that signals the beginning of a move to the starting location.
        /// </summary>
        internal void PlayMoveSound()
        {
            if (moveSoundPlayer != null) {
                SoundFactory.dimAllVolumes(true, moveSoundPlayer);
                moveSoundPlayer.Play(PlaybackCompleted);
            }
        }

        /// <summary>
        /// Plays the sound that reminds the participant to use the trigger button.
        /// </summary>
        internal void PlayReminderSound()
        {
            if (reminderSoundPlayer != null) {
                SoundFactory.dimAllVolumes(true, reminderSoundPlayer);
                reminderSoundPlayer.Play(PlaybackCompleted);
            }
        }

        /// <summary>
        /// Plays the sound that signals the end of a move to the direction of balance location.
        /// </summary>
        internal void PlayResetEndSound()
        {
            if (resetEndSoundPlayer != null) {
                SoundFactory.dimAllVolumes(true, resetEndSoundPlayer);
                resetEndSoundPlayer.Play(PlaybackCompleted);
            }
        }

        /// <summary>
        /// Plays the sound that signals the beginning of a move to the direction of balance location.
        /// </summary>
        internal void PlayResetStartSound()
        {
            if (resetStartSoundPlayer != null) {
                SoundFactory.dimAllVolumes(true, resetStartSoundPlayer);
                resetStartSoundPlayer.Play(PlaybackCompleted);
            }
        }

        /// <summary>
        /// Plays the sound that signals the start of a trial.
        /// </summary>
        internal void PlayStartSound()
        {
            if (startSoundPlayer != null) {
                SoundFactory.dimAllVolumes(true, startSoundPlayer);
                startSoundPlayer.Play(PlaybackCompleted);
            }
        }

        internal bool SetNoiseProfile()
        {
            RotationAngles angles;
            StreamReader reader;
            String[] fragments;
            String line;

            logger.Debug("Enter: SetNoiseProfile()");

            if ((noiseProfile == "") || (noiseProfile == null) || (noiseProfile.ToUpper() == "NULL") || (noiseProfile.ToUpper() == "NONE"))
            {
                return true;
            }

            if (!Directory.Exists(ConfigurationManager.AppSettings["NoiseDirectory"]))
            {
                logger.Error("[Trial] Could not load noise profile '" + noiseProfile + "' because the noise directory does not exist: " + ConfigurationManager.AppSettings["NoiseDirectory"]);
                return false;
            }

            try
            {
                reader = new StreamReader(ConfigurationManager.AppSettings["NoiseDirectory"] + "\\" + noiseProfile);

                noisePoints = new List<RotationAngles>();

                while ((line = reader.ReadLine()) != null)
                {

                    // Ignore comment lines
                    if (line.StartsWith(";"))
                    {
                        continue;
                    }

                    // Parse the line for a set of noise values

                    fragments = line.Split(',');

                    angles = new RotationAngles();
                    angles.roll = Double.Parse(fragments[0]) * noiseAmplitude;
                    angles.pitch = Double.Parse(fragments[1]) * noiseAmplitude;
                    angles.yaw = Double.Parse(fragments[2]) * noiseAmplitude;

                    noisePoints.Add(angles);
                }
            }
            catch (Exception e)
            {
                logger.Error("[Trial] Could not load noise profile '" + ConfigurationManager.AppSettings["NoiseDirectory"] + "\\" + noiseProfile + "'. Reason: " + e.Message);
                return false;
            }

            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Delegate method that is called when playback has completed
        /// </summary>
        private void PlaybackCompleted()
        {
            SoundFactory.dimAllVolumes(false, null);
        }
    
        /// <summary>
        /// Create a sound player, log an error if it fails
        /// </summary>
        /// <param name="filename">The name of the audio file to use</param>
        /// <returns></returns>
        private SoundPlayer CreateSoundPlayer(string filename)
        {
            SoundPlayer retval = null;
            if ((filename.Length != 0) && (filename.ToUpper() != "NONE") && (filename.ToUpper() != "NULL")) {
                if(System.IO.File.Exists(AppMain.BaseDirectory + "//TrialAudio//" + filename)){
                    try {
                        retval = SoundFactory.createSoundPlayer(AppMain.BaseDirectory + "//TrialAudio//" + filename);
                    } catch (Exception ex) {
                        logger.Error("[Trial] Error creating end trial sound player: " + ex.Message);
                        retval = null;
                    } 
                } else {
                    logger.Error("[Trial] Error creating end trial sound player: the file does not exist - " + AppMain.BaseDirectory + "//TrialAudio//" + filename);
                }
            }

            return retval;
        }


        /// <summary>
        /// Create the sound players for playing audio files.
        /// </summary>
        private void SetSoundPlayers()
        {
            logger.Debug("Enter: SetSoundPlayers()");

            endSoundPlayer = CreateSoundPlayer(endSound);
            moveSoundPlayer = CreateSoundPlayer(moveSound);
            reminderSoundPlayer = CreateSoundPlayer(reminderSound);
            resetEndSoundPlayer = CreateSoundPlayer(resetEndSound);
            resetStartSoundPlayer = CreateSoundPlayer(resetStartSound);
            startSoundPlayer = CreateSoundPlayer(startSound);
        }

        #endregion
    }
}
