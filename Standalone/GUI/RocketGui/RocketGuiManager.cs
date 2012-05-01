﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libRocketPlugin;
using OgreWrapper;
using OgrePlugin;
using Engine;
using Engine.Platform;
using MyGUIPlugin;
using System.Reflection;

namespace Medical.GUI
{
    class RocketGuiManager : IDisposable
    {
        private EventListenerInstancer eventListenerInstancer;
        private RocketWidget rocketWidget;

        public RocketGuiManager()
        {

        }

        public void Dispose()
        {
            if (rocketWidget != null)
            {
                rocketWidget.Dispose();
            }
            if (eventListenerInstancer != null)
            {
                eventListenerInstancer.Dispose();
            }
        }

        public void initialize(PluginManager pluginManager, EventManager eventManager, UpdateTimer mainTimer)
        {
            //Create a rocket group in ogre
            OgreResourceGroupManager.getInstance().createResourceGroup("Rocket");

            eventListenerInstancer = new TestEventListenerInstancer();
            Factory.RegisterEventListenerInstancer(eventListenerInstancer);

            RocketInterface.Instance.FileInterface.addExtension(new RocketAssemblyResourceLoader(typeof(RocketInterface).Assembly));
            RocketInterface.Instance.FileInterface.addExtension(new RocketAssemblyResourceLoader(typeof(MyGUIInterface).Assembly));

            //String sample_path = "S:/Engine/libRocketPlugin/Resources/";//"S:/dependencies/libRocket/src/Samples/";
            //VirtualFileSystem.Instance.addArchive(sample_path);
            OgreResourceGroupManager.getInstance().addResourceLocation("/", "EngineArchive", "Rocket", false);

            FontDatabase.LoadFontFace("MyGUIPlugin_DejaVuSans.ttf", "DejaVuSans", Font.Style.STYLE_NORMAL, Font.Weight.WEIGHT_NORMAL);
            FontDatabase.LoadFontFace("MyGUIPlugin.Resources.MyGUIPlugin_DejaVuSans-Bold.ttf", "DejaVuSans", Font.Style.STYLE_NORMAL, Font.Weight.WEIGHT_BOLD);
            FontDatabase.LoadFontFace("MyGUIPlugin.Resources.MyGUIPlugin_DejaVuSans-BoldOblique.ttf", "DejaVuSans", Font.Style.STYLE_ITALIC, Font.Weight.WEIGHT_BOLD);
            FontDatabase.LoadFontFace("MyGUIPlugin.Resources.MyGUIPlugin_DejaVuSans-Oblique.ttf", "DejaVuSans", Font.Style.STYLE_ITALIC, Font.Weight.WEIGHT_NORMAL);

            //Debugger.Initialise(context);
        }
    }
}
