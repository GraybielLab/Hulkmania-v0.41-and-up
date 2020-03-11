using log4net;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Brandeis.AGSOL.Hulkamania.Tasks.BalanceWithHmd
{
    /// <summary>
    /// Grid displaying the trials that were read in from a CSV file.
    /// </summary>
    internal sealed class TrialGrid : DataGridView
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the panel.
        /// </summary>
        public TrialGrid()
        {
             logger.Debug("Create: TrialGrid");

            InitializeGUI();
        }

        #endregion

        #region GUI Creation

        /// <summary>
        /// Constructs the panel GUI.
        /// </summary>
        private void InitializeGUI()
        {
            DataGridViewCheckBoxColumn restartWhenMaxAngleColumn;
            DataGridViewCheckBoxColumn usePitchColumn;
            DataGridViewCheckBoxColumn useRollColumn;
            DataGridViewCheckBoxColumn useYawColumn;
            DataGridViewCheckBoxColumn joystickIndicationsMandatoryColumn;

             logger.Debug("Enter: InitializeGUI()");

            AllowUserToAddRows = false;
            AllowUserToResizeRows = false;
            BackgroundColor = ColorScheme.Instance.GridBackground;
            BorderStyle = BorderStyle.None;
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ColumnHeadersDefaultCellStyle.Font = new Font(ColumnHeadersDefaultCellStyle.Font.FontFamily, 8.0f);
            ColumnHeadersDefaultCellStyle.BackColor = ColorScheme.Instance.GridColumnHeaderBackground;
            ColumnHeadersDefaultCellStyle.ForeColor = ColorScheme.Instance.GridColumnHeaderForeground;
            ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DefaultCellStyle.BackColor = ColorScheme.Instance.GridBackground;
            DefaultCellStyle.Font = new Font(DefaultCellStyle.Font.FontFamily, 9.0f);
            DefaultCellStyle.ForeColor = ColorScheme.Instance.GridForeground;
            DoubleBuffered = true;
            EnableHeadersVisualStyles = false;
            ForeColor = ColorScheme.Instance.GridForeground;
            ReadOnly = true;
            RowHeadersVisible = false;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            
            // 0
            Columns.Add("trialNumber", "#");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 30;

            // 1
            Columns.Add("condition", "Condition");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;
                
            // 2
            Columns.Add("balanceRoll", "Balance (R)");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;
            
            // 3 
            Columns.Add("balancePitch", "Balance (P)");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;
            
            // 4
            Columns.Add("balanceYaw", "Balance (Y)");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;

            // 5
            Columns.Add("acceleration", "Acceleration");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;
            
            // 6
            Columns.Add("maxAccel", "Max Accel");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;
            
            // 7
            Columns.Add("maxVel", "Max Velocity");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;
            
            // 8
            Columns.Add("maxAngle", "Max Angle");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;

            // 9
            restartWhenMaxAngleColumn = new DataGridViewCheckBoxColumn {
                HeaderText = "Restart When Max Angle",
                Resizable = DataGridViewTriState.False,
                Width = 75
            };
            Columns.Add(restartWhenMaxAngleColumn);

            // 10
            Columns.Add("restartDOBOFfsetMin", "Restart DOB Offset min (Y)");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;

            // 11
            Columns.Add("restartDOBOFfsetMax", "Restart DOB Offset max (Y)");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;

            // 12
            Columns.Add("restartDOBOFfsetMin", "Restart DOB Offset min (P)");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;

            // 13
            Columns.Add("restartDOBOFfsetMax", "Restart DOB Offset max (P)");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;

            // 14
            Columns.Add("restartDOBOFfsetMin", "Restart DOB Offset min (R)");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;

            // 15
            Columns.Add("restartDOBOFfsetMax", "Restart DOB Offset max (R)");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;

            // 16
            Columns.Add("timeLimit", "Time Limit");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;

            // 17
            joystickIndicationsMandatoryColumn = new DataGridViewCheckBoxColumn {
                HeaderText = "Joystick indications mandatory",
                Resizable = DataGridViewTriState.False,
                Width = 75
            };
            Columns.Add(joystickIndicationsMandatoryColumn);
            
            // 18
            Columns.Add("joystickGain", "Joystick Gain");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;
            
            // 19
            Columns.Add("joystickControlType", "Controls");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;

            // 20
            Columns.Add("noiseProfile", "Noise Profile");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 100;
            
            // 21
            Columns.Add("noiseAmplitude", "Noise Amplitude");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;

            // 22
            useRollColumn = new DataGridViewCheckBoxColumn {
                HeaderText = "Use Axis (R)",
                Resizable = DataGridViewTriState.False,
                Width = 75
            };
            Columns.Add(useRollColumn);

            // 23
            usePitchColumn = new DataGridViewCheckBoxColumn {
                HeaderText = "Use Axis (P)",
                Resizable = DataGridViewTriState.False,
                Width = 75
            };
            Columns.Add(usePitchColumn);
            
            // 24
            useYawColumn = new DataGridViewCheckBoxColumn {
                HeaderText = "Use Axis (Y)",
                Resizable = DataGridViewTriState.False,
                Width = 75
            };
            Columns.Add(useYawColumn);

            // 25
            Columns.Add("beginAtRoll", "Begin At (R)");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;
            
            // 26
            Columns.Add("beginAtPitch", "Begin At (P)");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;
            
            // 27
            Columns.Add("beginAtYaw", "Begin At (Y)");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 75;

            // 28
            Columns.Add("visualRoll", "Visual Offs (R)");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 90;
            
            // 29
            Columns.Add("visualPitch", "Visual Offs (P)");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 90;
            
            // 30
            Columns.Add("visualYaw", "Visual Offs (Y)");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 90;

            // 31
            Columns.Add("trialMoveSound", "Move Sound");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 100;
            
            // 32
            Columns.Add("trialStartSound", "Start Sound");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 100;
            
            // 33
            Columns.Add("trialEndSound", "End Sound");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 100;
            
            // 34
            Columns.Add("resetStartSound", "Reset Start Sound");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 100;
            
            // 35
            Columns.Add("resetEndSound", "Reset End Sound");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 100;
            
            // 36
            Columns.Add("reminderSound", "Reminder Sound");
            Columns[Columns.Count - 1].Resizable = DataGridViewTriState.False;
            Columns[Columns.Count - 1].Width = 100;

            PopulateList();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Fills the grid with a list of trials.
        /// </summary>
        internal void PopulateList()
        {
            Trial trial;
            
             logger.Debug("Enter: PopulateList()");

            Rows.Clear();
            for (int i = 0; i < Trials.List.Count; i++) {

                Rows.Add();
                Rows[i].Resizable = DataGridViewTriState.False;

                trial = Trials.List[i];
                this[0, i].Value = trial.TrialNumber;
                this[1, i].Value = trial.Condition;
                this[2, i].Value = trial.DirectionOfBalance.roll;
                this[3, i].Value = trial.DirectionOfBalance.pitch;
                this[4, i].Value = trial.DirectionOfBalance.yaw;
                this[5, i].Value = trial.AccelerationConstant;
                this[6, i].Value = trial.MaxAcceleration;
                this[7, i].Value = trial.MaxVelocity;
                this[8, i].Value = trial.MaxAngle;
                this[9, i].Value = trial.RestartWhenMaxAngle;
                this[10, i].Value = trial.RestartDOBOffsetMinYaw;
                this[11, i].Value = trial.RestartDOBOffsetMaxYaw;
                this[12, i].Value = trial.RestartDOBOffsetMinPitch;
                this[13, i].Value = trial.RestartDOBOffsetMaxPitch;
                this[14, i].Value = trial.RestartDOBOffsetMinRoll;
                this[15, i].Value = trial.RestartDOBOffsetMaxRoll;
                this[16, i].Value = trial.TimeLimit;
                this[17, i].Value = trial.JoystickIndicationsMandatory;
                this[18, i].Value = trial.JoystickGain;
                this[19, i].Value = trial.JoystickControlType;
                this[20, i].Value = trial.NoiseProfile;
                this[21, i].Value = trial.NoiseAmplitude;
                this[22, i].Value = trial.UseRoll;
                this[23, i].Value = trial.UsePitch;
                this[24, i].Value = trial.UseYaw;
                if (trial.BeginAt == null) {
                    this[25, i].Value = "---";
                    this[26, i].Value = "---";
                    this[27, i].Value = "---";
                } else {
                    this[25, i].Value = trial.BeginAt.roll;
                    this[26, i].Value = trial.BeginAt.pitch;
                    this[27, i].Value = trial.BeginAt.yaw;
                }
                this[28, i].Value = trial.VisualOrientationOffset.roll;
                this[29, i].Value = trial.VisualOrientationOffset.pitch;
                this[30, i].Value = trial.VisualOrientationOffset.yaw;
                this[31, i].Value = trial.MoveSound;
                this[32, i].Value = trial.StartSound;
                this[33, i].Value = trial.EndSound;
                this[34, i].Value = trial.ResetStartSound;
                this[35, i].Value = trial.ResetEndSound;
                this[36, i].Value = trial.ReminderSound;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Updates the GUI to show the current trial.
        /// </summary>
        public void updateTimer_Tick()
        {
            foreach (DataGridViewRow row in SelectedRows) {
                row.Selected = false;
            }

            if (Trials.CurrentTrialIndex >= Rows.Count) {
                return;
            }

            // Highlight the row of the current trial.
            // CurrentTrial is 1-based, while grid is 0-based.

            for (int i = 0; i < Trials.List.Count; i++) {
                        
                if ((int)this[0, i].Value != (Trials.CurrentTrial.TrialNumber)) {
                    continue;
                }

                Rows[i].Selected = true;
                break;
            }
        }

        #endregion
    }
}
