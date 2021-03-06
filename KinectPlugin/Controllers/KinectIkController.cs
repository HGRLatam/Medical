﻿using BEPUikPlugin;
using Engine;
using Engine.ObjectManagement;
using Engine.Renderer;
using Engine.Saving;
using Medical;
using Medical.Controller;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectPlugin
{
    class KinectIkController : KinectPoseController
    {
        private DebugDrawingSurface ikDebug;

        private MedicalController medicalController;
        private GenericSimObjectDefinition dragSimObjectDefinition;
        private BEPUikDragControlDefinition dragControl;
        private KinectIKBone hips;
        private KinectIKFace face;
        private bool debugVisible = true;
        private bool allowMovement = false;
        private bool jawTracking = true;
        private bool skeletonTracking = true;
        private List<SimObject> ikDragSimObjects = new List<SimObject>();
        //private FilterDoubleExponential filter = new FilterDoubleExponential();

        public event Action<KinectPoseController> AllowMovementChanged;

        public KinectIkController(StandaloneController controller)
        {
            this.medicalController = controller.MedicalController;

            dragSimObjectDefinition = new GenericSimObjectDefinition("TestArrow");
            dragControl = new BEPUikDragControlDefinition("DragControl");
            dragSimObjectDefinition.addElement(dragControl);

            //filter.Init(0.5f, 0.5f, 0.0f, 0.1f, 0.04f);
            //filter.Init(1.0f, 0.0f, 0.0f, 0.1f, 0.04f);
        }

        public void createIkControls(SimScene scene)
        {
            if(!canConnectToScene())
            {
                return;
            }

            var subScene = scene.getDefaultSubScene();

            ikDebug = medicalController.PluginManager.RendererPlugin.createDebugDrawingSurface("KinectIKDebug", subScene);
            ikDebug.setVisible(debugVisible);

            hips = createKinectBone(JointType.SpineBase, "Pelvis", "Pelvis", null, scene, subScene);
            KinectIKBone hipsLeft = createKinectBone(JointType.HipRight, "Pelvis", "Pelvis", hips, new Vector3(15, 0, 0), scene, subScene);
            KinectIKBone hipsRight = createKinectBone(JointType.HipLeft, "Pelvis", "Pelvis", hips, new Vector3(-15, 0, 0), scene, subScene);

            KinectIKBone neck = createKinectBone(JointType.SpineShoulder, "UpperTSpineMover", "UpperTSpineMover", hips, scene, subScene);
            KinectIKBone leftShoulderSpine = createKinectBone(JointType.ShoulderRight, "UpperTSpineMover", "LeftScapula", neck, Vector3.Zero, scene, subScene, "SpineShoulder");
            KinectIKBone rightShoulderSpine = createKinectBone(JointType.ShoulderLeft, "UpperTSpineMover", "RightScapula", neck, Vector3.Zero, scene, subScene, "SpineShoulder");
            KinectIKBone skull = createKinectBone(JointType.Head, "CSpineMover", "CSpineMover", neck, scene, subScene);            

            KinectIKBone leftKnee = createKinectBone( JointType.KneeRight,  "LeftFemur",    "LeftFemurTibiaJoint",    hipsLeft,                             scene, subScene);
            KinectIKBone leftAnkle = createKinectBone(JointType.AnkleRight, "LeftTibia",    "LeftTibiaFootBaseJoint", leftKnee,                         scene, subScene);
            KinectIKBone leftFoot = createKinectBone( JointType.FootRight,  "LeftFootBase", "LeftFootBase",           leftAnkle, new Vector3(0, -2, 5), scene, subScene);

            KinectIKBone rightKnee = createKinectBone( JointType.KneeLeft,   "RightFemur",    "RightFemurTibiaJoint",    hipsRight,                              scene, subScene);
            KinectIKBone rightAnkle = createKinectBone(JointType.AnkleLeft,  "RightTibia",    "RightTibiaFootBaseJoint", rightKnee,                         scene, subScene);
            KinectIKBone rightFoot = createKinectBone( JointType.FootLeft,   "RightFootBase", "RightFootBase",           rightAnkle, new Vector3(0, -2, 5), scene, subScene);

            KinectIKBone leftShoulder = createKinectBone(JointType.ShoulderRight, "LeftScapula", "LeftScapula", neck, scene, subScene);
            KinectIKBone leftElbow = createKinectBone(JointType.ElbowRight, "LeftHumerus", "LeftHumerusUlnaJoint", leftShoulder, scene, subScene);
            KinectIKBone leftWrist = createKinectBone(JointType.WristRight, "LeftUlna", "LeftRadiusHandBaseJoint", leftElbow, scene, subScene);
            KinectIKBone leftThumb = createKinectBone(JointType.ThumbRight, "LeftHandBase", "LeftHandBase", leftWrist, new Vector3(10, 0, 0), scene, subScene);
            KinectIKBone leftHand = createKinectBone(JointType.HandRight, "LeftHandBase", "LeftHandBase", leftWrist, new Vector3(0, -15, 2), scene, subScene);

            KinectIKBone rightShoulder = createKinectBone(JointType.ShoulderLeft, "RightScapula", "RightScapula", neck, scene, subScene);
            KinectIKBone rightElbow = createKinectBone(JointType.ElbowLeft, "RightHumerus", "RightHumerusUlnaJoint", rightShoulder, scene, subScene);
            KinectIKBone rightWrist = createKinectBone(JointType.WristLeft, "RightUlna", "RightRadiusHandBaseJoint", rightElbow, scene, subScene);
            KinectIKBone rightThumb = createKinectBone(JointType.ThumbLeft, "RightHandBase", "RightHandBase", rightWrist, new Vector3(-10, 0, 0), scene, subScene);
            KinectIKBone rightHand = createKinectBone(JointType.HandLeft, "RightHandBase", "RightHandBase", rightWrist, new Vector3(0, -15, 2), scene, subScene);

            face = createKinectFace("CSpineMover", "CSpineMover", skull, Vector3.Forward * 20, scene, subScene);
            face.JawTracking = jawTracking;
        }

        public void destroyIkControls(SimScene scene)
        {
            if(ikDebug != null)
            {
                medicalController.PluginManager.RendererPlugin.destroyDebugDrawingSurface(ikDebug);
                ikDebug = null;
            }
            hips = null;
            ikDragSimObjects.Clear();
        }

        public void updateControls(Body skel)
        {
            if (allowMovement && hips != null && skel.IsTracked)
            {
                //filter.Update(skel);

                //hips.update(filter.FilteredJoints);
                if (skeletonTracking)
                {
                    hips.update(skel);
                    face.skeletonUpdated();
                }
                if (debugVisible)
                {
                    ikDebug.begin("Main", DrawingType.LineList);
                    ikDebug.Color = Color.Red;
                    hips.render(ikDebug);
                    face.render(ikDebug);
                    ikDebug.end();
                }
            }
        }

        public void updateFace(FaceAlignment faceAlignment)
        {
            if (allowMovement && face != null)
            {
                face.update(faceAlignment);
            }
        }

        public bool DebugVisible
        {
            get
            {
                return debugVisible;
            }
            set
            {
                debugVisible = value;
                if(ikDebug != null)
                {
                    ikDebug.setVisible(debugVisible);
                }
            }
        }

        public bool AllowMovement
        {
            get
            {
                return allowMovement;
            }
            set
            {
                if (allowMovement != value)
                {
                    allowMovement = value;
                    foreach(var simObj in ikDragSimObjects)
                    {
                        simObj.Enabled = allowMovement;
                    }
                    if(AllowMovementChanged != null)
                    {
                        AllowMovementChanged.Invoke(this);
                    }
                }
            }
        }

        public bool SkeletonTracking
        {
            get
            {
                return skeletonTracking;
            }
            set
            {
                skeletonTracking = value;
            }
        }

        public bool JawTracking
        {
            get
            {
                return jawTracking;
            }
            set
            {
                jawTracking = value;
                if (face != null)
                {
                    face.JawTracking = value;
                }
            }
        }

        private KinectIKBone createKinectBone(JointType jointType, String boneSimObjectName, String translationSimObjectName, KinectIKBone parent, SimScene scene, SimSubScene subScene)
        {
            return createKinectBone(jointType, boneSimObjectName, translationSimObjectName, parent, Vector3.Zero, scene, subScene);
        }

        private KinectIKBone createKinectBone(JointType jointType, String boneSimObjectName, String translationSimObjectName, KinectIKBone parent, Vector3 additionalOffset, SimScene scene, SimSubScene subScene, String additionalName = "")
        {
            dragControl.BoneSimObjectName = boneSimObjectName;

            var targetSimObject = medicalController.getSimObject(dragControl.BoneSimObjectName);
            var ikBone = targetSimObject.getElement("IKBone") as BEPUikBone;
            ikBone.Pinned = false;

            dragSimObjectDefinition.Name = jointType + "DragControl" + additionalName;
            dragSimObjectDefinition.Enabled = allowMovement;
            dragSimObjectDefinition.Translation = medicalController.getSimObject(translationSimObjectName).Translation + additionalOffset;
            SimObjectBase instance = dragSimObjectDefinition.register(subScene);
            medicalController.addSimObject(instance);
            scene.buildScene();

            ikDragSimObjects.Add(instance);

            float distanceToParent = 0;
            if (parent != null)
            {
                distanceToParent = (instance.Translation - parent.Translation).length();
            }

            var bone = new KinectIKBone(jointType, distanceToParent, instance);

            if (parent != null)
            {
                parent.addChild(bone);
            }
            return bone;
        }

        private KinectIKFace createKinectFace(String boneSimObjectName, String translationSimObjectName, KinectIKBone parent, Vector3 additionalOffset, SimScene scene, SimSubScene subScene)
        {
            dragControl.BoneSimObjectName = boneSimObjectName;

            var targetSimObject = medicalController.getSimObject(dragControl.BoneSimObjectName);
            var ikBone = targetSimObject.getElement("IKBone") as BEPUikBone;
            ikBone.Pinned = false;

            dragSimObjectDefinition.Name = "KinectFaceControl";
            dragSimObjectDefinition.Enabled = allowMovement;
            dragSimObjectDefinition.Translation = medicalController.getSimObject(translationSimObjectName).Translation + additionalOffset;
            SimObjectBase instance = dragSimObjectDefinition.register(subScene);
            medicalController.addSimObject(instance);
            scene.buildScene();

            ikDragSimObjects.Add(instance);

            float distanceToParent = 0;
            if (parent != null)
            {
                distanceToParent = (instance.Translation - parent.Translation).length();
            }

            return new KinectIKFace(parent, distanceToParent, instance);
        }

        /// <summary>
        /// A quick check to see if the desired sim objects exist, does not check them all, but
        /// this should be enough with the current scenes.
        /// </summary>
        /// <returns></returns>
        private bool canConnectToScene()
        {
            return medicalController.getSimObject("LeftUlna") != null;
        }
    }
}
