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
using Microsoft.Speech.Recognition;
using Microsoft.Speech.AudioFormat;

namespace SLAMBotClasses
{
    public class VoiceControl
    {
        #region members

        public event EventHandler<VoiceArgs> voiceCommandHeard;
        private KinectSensor kinectSensor;
        private DateTime lastSend;
        private double setRightPower;
        private double setLeftPower;
        private SpeechRecognitionEngine sre;
        private bool follow;


        #endregion

        #region Properties

        #endregion

        #region HelperClasses

        public class VoiceArgs : EventArgs
        {
            public double rightPower;
            public double leftPower;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Starts up the SkeletonSlam class.
        /// </summary>
        public VoiceControl()
        {
            kinectSensor = KinectSensor.KinectSensors[0];

            TransformSmoothParameters smoothingParam = new TransformSmoothParameters();
            {
                smoothingParam.Smoothing = 0.5f;
                smoothingParam.Correction = 0.5f;
                smoothingParam.Prediction = 0.5f;
                smoothingParam.JitterRadius = 0.05f;
                smoothingParam.MaxDeviationRadius = 0.04f;
            };



            kinectSensor.SkeletonStream.Enable(smoothingParam);

            kinectSensor.SkeletonFrameReady += getSkeleton;

            sre = CreateSpeechRecognizer();

            kinectSensor.Start();

            sre.SetInputToAudioStream(kinectSensor.AudioSource.Start(),
                 new SpeechAudioFormatInfo(
                 EncodingFormat.Pcm, 16000, 16, 1,
                 32000, 2, null));

            sre.RecognizeAsync(RecognizeMode.Multiple);



            reset();

        }

        #endregion

        #region Public Methods

        public void reset()
        {
            lastSend = DateTime.Now;
            follow = false;
        }

        #endregion

        #region Private Methods

        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

        //here is the fun part: create the speech recognizer
        private SpeechRecognitionEngine CreateSpeechRecognizer()
        {
            // Select a speech recognizer that supports English.
            //RecognizerInfo info = GetKinectRecognizer();
            RecognizerInfo info = null;
            foreach (RecognizerInfo ri in SpeechRecognitionEngine.InstalledRecognizers())
            {
                if (ri.Culture.TwoLetterISOLanguageName.Equals("en"))
                {
                    info = ri;
                    break;
                }
            }
            if (info == null)
                return null;
            SpeechRecognitionEngine sre;
            sre = new SpeechRecognitionEngine(info.Id);

            //Now we need to add the words we want our program to recognise
            var grammar = new Choices();
            grammar.Add("go");
            grammar.Add("stop");
            grammar.Add("turn right");
            grammar.Add("turn left");
            grammar.Add("skeleton");
            //Add words here


            //set culture - language, country/region
            var gb = new GrammarBuilder { Culture = info.Culture };
            gb.Append(grammar);

            //set up the grammar builder
            var g = new Grammar(gb);
            sre.LoadGrammar(g);

            //Set events for recognizing, hypothesising and rejecting speech
            sre.SpeechRecognized += SreSpeechRecognized;
            //sre.SpeechHypothesized += SreSpeechHypothesized;
            sre.SpeechRecognitionRejected += SreSpeechRecognitionRejected;
            return sre;
        }

        //Speech is recognised
        private void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //Very important! - change this value to adjust accuracy - the higher the value
            //the more accurate it will have to be, lower it if it is not recognizing you
            if (e.Result.Confidence < 1.0)
            {
                RejectSpeech(e.Result);
            }
            //and finally, here we set what we want to happen when 
            //the SRE recognizes a word
            switch (e.Result.Text.ToUpperInvariant())
            {
                case ("GO"):
                    if (!follow)
                    {
                        setRightPower = .5;
                        setLeftPower = .5;
                    }
                    break;
                case ("STOP"):
                    if (!follow)
                    {
                        setRightPower = 0;
                        setLeftPower = 0;
                    }
                    break;
                case ("TURN RIGHT"):
                    if (!follow)
                    {
                        setRightPower = .8;
                        setLeftPower = .2;
                    }
                    break;
                case ("TURN LEFT"):
                    if (!follow)
                    {
                        setRightPower = .2;
                        setLeftPower = .8;
                    }
                    break;
                case("SKELETON"):
                    follow = !follow;
                    break;
                default:
                    break;
            }

            TestControls();
        }

        //if speech is rejected
        private void RejectSpeech(RecognitionResult result)
        {
            /*setRightPower = -.5;
            setLeftPower = -.5;

            TestControls();*/
        }

        private void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            RejectSpeech(e.Result);
        }

        public void getSkeleton(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (follow)
            {
                setRightPower = 1.0f;
                setLeftPower = -1.0f;
                SkeletonFrame skelFrame = e.OpenSkeletonFrame();
                if (skelFrame != null)
                {
                    Skeleton[] skeletons = new Skeleton[skelFrame.SkeletonArrayLength];

                    skelFrame.CopySkeletonDataTo(skeletons);

                    skelFrame.Dispose();

                    foreach (Skeleton skel in skeletons)
                    {
                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {

                            //setRightPower = 1.0f;
                            //setLeftPower = -1.0f;

                            TestControls();

                            break;
                        }
                        else
                        {



                            TestControls();
                        }
                    }
                }
            }
        }

        private void TestControls()
        {
            if (voiceCommandHeard != null)
            {
                VoiceArgs args = new VoiceArgs();
                args.rightPower = setRightPower;
                args.leftPower = setLeftPower;
                voiceCommandHeard(this, args);
            }
        }

        #endregion

    }
}