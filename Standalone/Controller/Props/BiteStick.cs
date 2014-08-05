﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine;

namespace Medical
{
    public class BiteStick
    {
        public const String DefinitionName = "BiteStick";

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition biteStick = new GenericSimObjectDefinition(DefinitionName);
            biteStick.Enabled = true;

            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = "BiteStick.mesh";

            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            biteStick.addElement(nodeDefinition);

            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            biteStick.addElement(propFadeBehaviorDef);

            PropDefinition propDefinition = new PropDefinition(biteStick);

            ShowPropTrackInfo biteStickData = propDefinition.TrackInfo;
            biteStickData.addTrack(new ShowPropSubActionPrototype(typeof(MovePropAction), "Move"));
            biteStickData.addTrack(new ShowPropSubActionPrototype(typeof(SetPropTransparencyAction), "Set Transparency"));
            propFactory.addDefinition(propDefinition);
        }
    }
}
