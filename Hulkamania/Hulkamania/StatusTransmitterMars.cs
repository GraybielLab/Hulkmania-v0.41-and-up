using System;
using System.Net.Sockets;
using System.Threading;
using System.Reflection;
using Brandeis.AGSOL.Network;

namespace Brandeis.AGSOL.Hulkamania
{

    public class StatusTransmitterMars
    {
        private Vector3 mVisualOrientationOffset = new Vector3(0,0,0);
        private Vector3 mOrientation = new Vector3(0, 0, 0);
        private Vector3 mAcceleration = new Vector3(0, 0, 0);
        private Vector3 mVelocity = new Vector3(0, 0, 0);
        private bool mPitchRollMount = true;

        private ServerHandler mServerHandler = null;

        // -------------------------------------------------------------------------------------------------------------------------
        public Vector3 Velocity { get { return mVelocity; } set { mVelocity = value; } }
        public Vector3 Acceleration { get { return mAcceleration; } set { mAcceleration = value; } }
        public Vector3 Orientation { get { return mOrientation; } set { mOrientation = value; } }
        public Vector3 VisualOrientationOffset { get { return mVisualOrientationOffset; } set { mVisualOrientationOffset = value; } }
        public bool PitchRollMount { get { return mPitchRollMount; } set { mPitchRollMount = value; } }

        // -------------------------------------------------------------------------------------------------------------------------
        public void transmitStatus()
        {      
            ICommand c = new ICommand();
            c.CommandType = (int)eCommands.MarsStatus;

            c.addParameter((int)eMarsStatusCommandParameters.Orientation, mOrientation.ToString());
            c.addParameter((int)eMarsStatusCommandParameters.VisualOrientationOffset, mVisualOrientationOffset.ToString());
            c.addParameter((int)eMarsStatusCommandParameters.Velocity, mVelocity.ToString());
            c.addParameter((int)eMarsStatusCommandParameters.Acceleration, mAcceleration.ToString());
            c.addParameter((int)eMarsStatusCommandParameters.PitchRollMount, mPitchRollMount.ToString());

            if (mServerHandler != null)
            {
                mServerHandler.sendCommandToRegisteredClients(c, null);
             }
        }

        // -------------------------------------------------------------------------------------------------------------------------
        public void initialize(ServerHandler serverHandler)
        {
            if (mServerHandler != null)
            {
                shutdown();
            } 

            mServerHandler = serverHandler;
        }

        // -------------------------------------------------------------------------------------------------------------------------
        public void shutdown()
        {
           mServerHandler = null;
        }

        // -------------------------------------------------------------------------------------------------------------------------
        public StatusTransmitterMars()
        {
        }
    }

}