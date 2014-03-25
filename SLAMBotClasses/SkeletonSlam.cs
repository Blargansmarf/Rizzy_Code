using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLAMBotClasses;
using System.Net;
using System.Threading;
using Microsoft.Kinect;
using System.Windows.Documents;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SLAMBotClasses
{
    public class SkeletonSlam
    {
        #region members

        public event EventHandler<SkeletonArgs> onHandHeightChanged;
        public event EventHandler<SkeletonArgs> onRightAngleTurn;
        public event EventHandler<SkeletonArgs> onLeftQuarterTurn;
        public event EventHandler<SkeletonArgs> onRightQuarterTurn;
        public event EventHandler<SkeletonArgs> onLeftHalfTurn;
        public event EventHandler<SkeletonArgs> onLeftFullTurn;
        public event EventHandler<SkeletonArgs> onRightHalfTurn;
        public event EventHandler<SkeletonArgs> onRightFullTurn;
        private KinectSensor kinectSensor;
        private DateTime lastSend;
        private double rHandHeight;
        private double lastHeightRHand;
        private double lHandHeight;
        private double lastHeightLHand;
        private double rHandDistHip;
        private double lHandDistHip;
        private bool rHandFwd;
        private bool lHandFwd;
        private bool _turnRight;

        private bool halfSpinFlag;
        private bool fullSpinFlag;

        private bool halfSpinRightFlag;
        private bool halfSpinLeftFlag;

        private bool fullRightFlag;
        private bool fullLeftFlag;

        private bool quarterRightFlag;
        private bool quarterLeftFlag;

        //private DateTime startTurn = DateTime.Now;
        //private DateTime endTurn;

        #endregion

        #region Properties

        public double heightRHandAboveShoulder
        {
            get
            {
                return rHandHeight;
            }
        }

        #endregion

        #region HelperClasses

        public class SkeletonArgs : EventArgs
        {
            //if the right hand is moved
            public bool rightHandFwd;
            //how high right hand is above shoulder
            public double RHandHeight;
            public bool leftHandFwd;
            public double LHandHeight;
            public bool turnRight;

        }

        #endregion

        #region Constructor

        /// <summary>
        /// Starts up the SkeletonSlam class.
        /// </summary>
        public SkeletonSlam()
        {
            halfSpinFlag = false;
            fullSpinFlag = false;

            fullRightFlag = false;
            fullLeftFlag = false;
            halfSpinRightFlag = false;
            halfSpinLeftFlag = false;
            quarterRightFlag = false;
            quarterLeftFlag = false;


            
            //kinectSensor = KinectSensor.KinectSensors[0];

            TransformSmoothParameters smoothingParam = new TransformSmoothParameters();
            {
                smoothingParam.Smoothing = 0.5f;
                smoothingParam.Correction = 0.5f;
                smoothingParam.Prediction = 0.5f;
                smoothingParam.JitterRadius = 0.05f;
                smoothingParam.MaxDeviationRadius = 0.04f;
            };

            
            
            //kinectSensor.SkeletonStream.Enable(smoothingParam);
            //kinectSensor.Start();
            
            reset();

            //kinectSensor.SkeletonFrameReady += getSkeleton;

            //processHandMove = new Thread(ProcessHandMove);
            //processHandMove.Start();
            
        }

        #endregion

        #region Public Methods

        public void reset()
        {
            lastSend = DateTime.Now;
            rHandHeight = lastHeightRHand = 0;
            lHandHeight = lastHeightLHand = 0;
            rHandFwd = false;
            lHandFwd = false;
            _turnRight = false;
        }

        #endregion

        #region Private Methods

        private void getSkeleton(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame skelFrame = e.OpenSkeletonFrame();
            if (skelFrame != null)
            {
                Skeleton[] skeletons = new Skeleton[skelFrame.SkeletonArrayLength];

                skelFrame.CopySkeletonDataTo(skeletons);

                foreach (Skeleton skel in skeletons)
                {
                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        double rShoulder = 0;
                        double rHandy = 0;
                        double rHandx = 0;
                        double rHipy = 0;
                        double rHipx = 0;

                        double lShoulder = 0;
                        double lHandy = 0;
                        double lHandx = 0;
                        double lHipy = 0;
                        double lHipx = 0;

                        foreach (Joint joint in skel.Joints)
                        {
                            if (joint.JointType == JointType.HandRight)
                            {
                                rHandy = joint.Position.Y;
                                rHandx = joint.Position.X;
                            }
                            if (joint.JointType == JointType.ShoulderRight)
                            {
                                rShoulder = joint.Position.Y;
                            }
                            if (joint.JointType == JointType.HipRight)
                            {
                                rHipy = joint.Position.Y;
                                rHipx = joint.Position.X;
                            }
                            if (joint.JointType == JointType.HandLeft)
                            {
                                lHandy = joint.Position.Y;
                                lHandx = joint.Position.X;
                            }
                            if (joint.JointType == JointType.ShoulderLeft)
                            {
                                lShoulder = joint.Position.Y;
                            }
                            if (joint.JointType == JointType.HipLeft)
                            {
                                lHipy = joint.Position.Y;
                                lHipx = joint.Position.X;
                            }
                        }

                        double rShoulderAboveHip = Math.Abs(rShoulder - rHipy);
                        double rHandAboveHip = Math.Abs(rHandy - rHipy);

                        if (rHandy < rHipy - .12 && rHandy > rHipy - .2 && rHandx > rHipx && rHandx < rHipx + .3)
                            rHandHeight = 0;
                        else if (rHandy >= rHipy - .12 && rHandx > rHipx && rHandx < rHipx + .3)
                        {
                            rHandFwd = true;
                            rHandHeight = rShoulderAboveHip - (rShoulderAboveHip - rHandAboveHip);
                        }
                        else if (rHandy < rHipy && rHandx > rHipx + .2)
                        {
                            //rHandHeight = -.5;  // commented to turn off right side reverse
                            lHandHeight = -.5;
                        }
                        else
                            rHandHeight = 0;
                        
                        double lShoulderAboveHip = Math.Abs(lShoulder - lHipy);
                        double lHandAboveHip = Math.Abs(lHandy - lHipy);

                        if (lHandy < lHipy - .12 && lHandy > lHipy - .2 && lHandx < lHipx && lHandx > lHipx - .3)
                            lHandHeight = 0;
                        else if (lHandy >= lHipy - .12 && lHandx < lHipx && lHandx > lHipx - .3)
                        {
                            lHandFwd = true;
                            lHandHeight = lShoulderAboveHip - (lShoulderAboveHip - lHandAboveHip);
                        }
                        else if (lHandy < lHipy && lHandx < lHipx - .2)
                        {
                            //lHandHeight = -.5;  //commented to turn off left side reverse
                            rHandHeight = -.5;
                        }
                        else
                            lHandHeight = 0;


                        
                        if(rHandy > rHipy + 0.6 && rHandx > rHipx + 0.3)  // this works; upper right
                        {
                            //fullRightFlag = true;
                            fullLeftFlag = true;
                        }
                        //else if (rHandy >= rHipy + 0.15 && rHandy < rHipy + 0.6 && rHandx > rHipx + 0.4) // Testing.. might work
                        else if (rHandy >= rHipy + 0.2 && rHandy < rHipy + 0.6 && rHandx > rHipx + 0.4)
                        {
                            // attempt at right mid-lvl hand pos, for 180 right turn  (use of shoulder)                            
                            //halfSpinRightFlag = true;
                            //halfSpinLeftFlag = true;

                            quarterLeftFlag = true;
                        }                        
                        else if(lHandy > 0.2 && rHandx > rHipx + 0.2)
                        {
                            //quarterRightFlag = true;
                            //quarterLeftFlag = true;
                            halfSpinLeftFlag = true;
                        }
                        else if(lHandy > lHipy + 0.6 && lHandx < lHipx - 0.4)
                        {
                            //fullLeftFlag = true;
                            fullRightFlag = true;
                        }
                        //else if(lHandy >= lHipy + 0.15 && lHandy < lHipy + 0.6 && lHandx < lHipx - 0.4)
                        else if (lHandy >= lHipy + 0.2 && lHandy < lHipy + 0.6 && lHandx < lHipx - 0.25)
                        {
                            //halfSpinLeftFlag = true;
                            //halfSpinRightFlag = true;

                            quarterRightFlag = true;
                        }
                        else if (rHandy > 0.2 && lHandx < lHipx - 0.2)
                        {
                            //quarterLeftFlag = true;
                            //quarterRightFlag = true;

                            halfSpinRightFlag = true;
                        }


                        TestControls();

                        break;
                    }
                    else
                    {
                        lHandHeight = 0;
                        rHandHeight = 0;

                        TestControls();
                    }
                }
            }
        }

        private void TestControls()
        {

            if ((DateTime.Now - lastSend).TotalMilliseconds >= 50)
            {
                if (rHandHeight != lastHeightRHand)
                {
                    lastHeightRHand = rHandHeight;
                    lastSend = DateTime.Now;
                }
                if (lHandHeight != lastHeightLHand)
                {
                    lastHeightLHand = lHandHeight;
                    lastSend = DateTime.Now;
                }
            }

            if (onHandHeightChanged != null)
            {
                SkeletonArgs args = new SkeletonArgs();
                args.LHandHeight = lHandHeight;
                args.RHandHeight = rHandHeight;
                args.rightHandFwd = rHandFwd;
                args.leftHandFwd = lHandFwd;
                //onHandHeightChanged(this, args);


                if (fullLeftFlag == true)
                {
                    onLeftFullTurn(this, args);
                    fullLeftFlag = false;
                }
                else if(fullRightFlag == true)
                {
                    onRightFullTurn(this, args);
                    fullRightFlag = false;                    
                }
                else if (halfSpinLeftFlag == true)
                {
                    onLeftHalfTurn(this, args);
                    halfSpinLeftFlag = false;
                }
                else if(halfSpinRightFlag == true)
                {
                    onRightHalfTurn(this, args);
                    halfSpinRightFlag = false;
                }
                else if(quarterLeftFlag == true)
                {
                    onLeftQuarterTurn(this, args);
                    quarterLeftFlag = false;
                }
                else if (quarterRightFlag == true)
                {
                    onRightQuarterTurn(this, args);
                    quarterRightFlag = false;
                }
                else
                {
                    onHandHeightChanged(this, args);
                }

            }
        }

        #endregion

    }
}
