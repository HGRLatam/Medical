﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine;

namespace Medical
{
    public class RangeOfMotionScale
    {
        public const String DefinitionName = "RangeOfMotionScale";

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition rangeOfMotion = new GenericSimObjectDefinition(DefinitionName);
            rangeOfMotion.Enabled = true;

            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = "RangeOfMotionScale.mesh";

            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            rangeOfMotion.addElement(nodeDefinition);

            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            rangeOfMotion.addElement(propFadeBehaviorDef);

            propFactory.addDefinition(DefinitionName, rangeOfMotion);
        }
    }
}
