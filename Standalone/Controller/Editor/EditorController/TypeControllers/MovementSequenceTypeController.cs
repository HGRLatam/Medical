﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.Xml;
using Medical.Muscles;
using MyGUIPlugin;
using System.IO;

namespace Medical
{
    public class MovementSequenceTypeController : SaveableTypeController<MovementSequence>
    {
        private const String Icon = "MovementSequenceEditorIcon";

        public MovementSequenceTypeController(EditorController editorController)
            :base(".seq", editorController)
        {
            
        }

        public void saveFile(MovementSequence movementSequence, String file)
        {
            saveObject(file, movementSequence);
            EditorController.NotificationManager.showNotification(String.Format("{0} saved.", file), Icon, 2);
        }

        public override void addCreationMethod(ContextMenu contextMenu, string path, bool isDirectory, bool isTopLevel)
        {
            contextMenu.add(new ContextMenuItem("Create Movement Sequence", path, delegate(ContextMenuItem item)
            {
                InputBox.GetInput("Movement Sequence Name", "Enter a name for the movement sequence.", true, delegate(String result, ref String errorMessage)
                {
                    String filePath = Path.Combine(path, result);
                    filePath = Path.ChangeExtension(filePath, ".seq");
                    if (EditorController.ResourceProvider.exists(filePath))
                    {
                        MessageBox.show(String.Format("Are you sure you want to override {0}?", filePath), "Override", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle overrideResult)
                        {
                            if (overrideResult == MessageBoxStyle.Yes)
                            {
                                createNewMovementSequence(filePath);
                            }
                        });
                    }
                    else
                    {
                        createNewMovementSequence(filePath);
                    }
                    return true;
                });
            }));
        }

        void createNewMovementSequence(String filePath)
        {
            MovementSequence movementSequence = new MovementSequence();
            movementSequence.Duration = 5.0f;
            creatingNewFile(filePath);
            saveObject(filePath, movementSequence);
            openEditor(filePath);
        }
    }
}