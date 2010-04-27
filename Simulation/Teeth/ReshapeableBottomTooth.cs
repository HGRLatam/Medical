﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;
using BulletPlugin;
using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.Saving;
using OgrePlugin;
using Engine.Renderer;
using Logging;

namespace Medical
{
    class ReshapeableBottomTooth : BottomTooth
    {
        [DoNotSave]
        private List<ToothSection> toothSections = new List<ToothSection>();

        [Editable]
        private ToothSection mainToothSection = new ToothSection("MainTooth");

        [DoNotCopy]
        [DoNotSave]
        Vector3[] verticesArray;

        protected override void constructed()
        {
            //temporary get entity
            SceneNodeElement sceneNodeElement = Owner.getElement(sceneNodeName) as SceneNodeElement;
            Entity entity = null;
            if (sceneNodeElement == null)
            {
                blacklist("Could not find SceneNodeElement {0}.", sceneNodeName);
            }
            else
            {
                entity = sceneNodeElement.getNodeObject(entityName) as Entity;
                if (entity == null)
                {
                    blacklist("Could not find Entity {0}.", entityName);
                }
            }
            RigidBody actorElement = Owner.getElement(actorName) as RigidBody;
            if (actorElement == null)
            {
                blacklist("Could not find Actor {0}.", actorName);
            }

            using (MeshPtr meshPtr = entity.getMesh())
            {
                SubMesh subMesh = meshPtr.Value.getSubMesh(0);

                VertexData vertexData = subMesh.vertexData;
                IndexData indexData = subMesh.indexData;
                if (subMesh.UseSharedVertices)
                {
                    vertexData = meshPtr.Value.SharedVertexData;
                }

                VertexDeclaration vertexDeclaration = vertexData.vertexDeclaration;
                VertexElement positionElement = vertexDeclaration.findElementBySemantic(VertexElementSemantic.VES_POSITION);

                VertexBufferBinding vertexBinding = vertexData.vertexBufferBinding;
                using (HardwareVertexBufferSharedPtr vertexBuffer = vertexBinding.getBuffer(positionElement.getSource()))
                {
                    using (HardwareIndexBufferSharedPtr indexBuffer = indexData.IndexBuffer)
                    {
                        uint vertexSize = vertexBuffer.Value.getVertexSize();

                        uint numVertices = vertexBuffer.Value.getNumVertices();
                        uint positionOffset = positionElement.getOffset();

                        uint numIndices = indexBuffer.Value.getNumIndexes();
                        uint numTriangles = numIndices / 3;
                        unsafe
                        {
                            verticesArray = new Vector3[vertexBuffer.Value.getNumVertices()];
                            uint[] indicesArray = new uint[indexBuffer.Value.getNumIndexes()];

                            // Get vertex data
                            byte* vertexBufferData = (byte*)vertexBuffer.Value.@lock(HardwareBuffer.LockOptions.HBL_DISCARD);
                            float* elemStart;
                            for (int i = 0; i < numVertices; ++i)
                            {
                                positionElement.baseVertexPointerToElement(vertexBufferData, &elemStart);
                                verticesArray[i].x = *elemStart++;
                                verticesArray[i].y = *elemStart++;
                                verticesArray[i].z = *elemStart++;
                                vertexBufferData += vertexSize;
                            }
                            vertexBuffer.Value.unlock();

                            // Get index data
                            if (indexBuffer.Value.getType() == HardwareIndexBuffer.IndexType.IT_16BIT)
                            {
                                ushort* indexBufferData = (ushort*)indexBuffer.Value.@lock(HardwareBuffer.LockOptions.HBL_DISCARD);
                                for (int i = 0; i < numIndices; ++i)
                                {
                                    indicesArray[i] = indexBufferData[i];
                                }
                                indexBuffer.Value.unlock();
                            }
                            else if (indexBuffer.Value.getType() == HardwareIndexBuffer.IndexType.IT_32BIT)
                            {
                                uint* indexBufferData = (uint*)indexBuffer.Value.@lock(HardwareBuffer.LockOptions.HBL_DISCARD);
                                for (int i = 0; i < numIndices; ++i)
                                {
                                    indicesArray[i] = indexBufferData[i];
                                }
                                indexBuffer.Value.unlock();
                            }

                            ReshapeableRigidBody body = (ReshapeableRigidBody)actorElement;
                            mainToothSection.checkTriangles(verticesArray, indicesArray);
                            foreach (ToothSection section in toothSections)
                            {
                                section.checkTriangles(verticesArray, indicesArray);
                            }

                            mainToothSection.createSection(verticesArray, body);
                            foreach (ToothSection section in toothSections)
                            {
                                section.createSection(verticesArray, body);
                            }
                            body.recomputeMassProps();                            
                        }
                    }
                }
            }

            base.constructed();
        }

        public override bool rayIntersects(Ray3 worldRay, out float distance, out uint vertexNumber)
        {
            Ray3 localRay = worldRay;
            Quaternion rotationDir = Owner.Rotation.inverse();
            localRay.Direction = Quaternion.quatRotate(rotationDir, worldRay.Direction);
            localRay.Origin = localRay.Origin - Owner.Translation;
            localRay.Origin = Quaternion.quatRotate(rotationDir, localRay.Origin);

            //Find the closest section that actually hits the tooth.
            Vector3 hitLocation;
            float closestSectionDistance = float.MaxValue;
            ToothSection closestSection = null;

            if (mainToothSection.intersects(localRay, out hitLocation))
            {
                closestSectionDistance = (hitLocation - localRay.Origin).length2();
                closestSection = mainToothSection;
            }
            foreach (ToothSection section in toothSections)
            {
                if (section.intersects(localRay, out hitLocation))
                {
                    float distance2 = (hitLocation - localRay.Origin).length2();
                    if (distance2 < closestSectionDistance)
                    {
                        closestSectionDistance = distance2;
                        closestSection = section;
                    }
                }
            }

            //Check the triangles in the closest section
            if (closestSection != null)
            {
                return closestSection.checkTriangleCollision(verticesArray, localRay, out distance, out vertexNumber);
            }
            else
            {
                vertexNumber = 0;
                distance = float.MaxValue;
                return false;
            }
        }

        public override unsafe void moveVertex(uint vertex)
        {
            using (MeshPtr meshPtr = entity.getMesh())
            {
                SubMesh subMesh = meshPtr.Value.getSubMesh(0);

                VertexData vertexData = subMesh.vertexData;
                IndexData indexData = subMesh.indexData;
                if (subMesh.UseSharedVertices)
                {
                    vertexData = meshPtr.Value.SharedVertexData;
                }

                VertexDeclaration vertexDeclaration = vertexData.vertexDeclaration;
                VertexElement positionElement = vertexDeclaration.findElementBySemantic(VertexElementSemantic.VES_POSITION);

                VertexBufferBinding vertexBinding = vertexData.vertexBufferBinding;
                using (HardwareVertexBufferSharedPtr vertexBuffer = vertexBinding.getBuffer(positionElement.getSource()))
                {
                    uint vertexSize = vertexBuffer.Value.getVertexSize();

                    // Modify vertex data
                    byte* vertexBufferData = (byte*)vertexBuffer.Value.@lock(HardwareBuffer.LockOptions.HBL_NORMAL);
                    vertexBufferData += vertex * vertexSize;
                    float* elemStart;
                    positionElement.baseVertexPointerToElement(vertexBufferData, &elemStart);
                    elemStart[0] = 0;
                    elemStart[1] = 0;
                    elemStart[2] = 0;
                    vertexBuffer.Value.unlock();
                }
            }
        }

        public override void drawDebugInfo(DebugDrawingSurface debugDrawing)
        {
            debugDrawing.begin("ToothRay" + Owner.Name, DrawingType.LineList);

            mainToothSection.drawBoundsWorld(debugDrawing, Owner.Translation, Owner.Rotation);

            foreach (ToothSection section in toothSections)
            {
                section.drawBoundsWorld(debugDrawing, Owner.Translation, Owner.Rotation);
            }

            debugDrawing.end();
        }

        protected override void customLoad(LoadInfo info)
        {
            info.RebuildList<ToothSection>("ToothSections", toothSections);
        }

        protected override void customSave(SaveInfo info)
        {
            info.ExtractList<ToothSection>("ToothSections", toothSections);
        }

        #region EditInterface

        [DoNotCopy]
        [DoNotSave]
        private EditInterfaceManager<ToothSection> sectionManager;

        [DoNotCopy]
        [DoNotSave]
        private EditInterface editInterface;

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            this.editInterface = editInterface;
            sectionManager = new EditInterfaceManager<ToothSection>(editInterface);
            sectionManager.addCommand(new EditInterfaceCommand("Remove", removeSectionCallback));
            ToothEditRenderer toothEditRenderer = new ToothEditRenderer();
            editInterface.Renderer = toothEditRenderer;
            toothEditRenderer.addSubRenderer(mainToothSection.getEditInterface(null, null).Renderer);//dont need args as this will already be created
            editInterface.addCommand(new EditInterfaceCommand("Add Tooth Section", addSectionCallback));
            foreach (ToothSection section in toothSections)
            {
                onToothSectionAdded(section);
            }
        }

        private void addSectionCallback(EditUICallback callback, EditInterfaceCommand command)
        {
            String name;
            bool accept = callback.getInputString("Enter a name for the section.", out name, validateSectionCreate);
            if (accept)
            {
                ToothSection section = new ToothSection(name);
                toothSections.Add(section);
                onToothSectionAdded(section);
            }
        }

        private bool validateSectionCreate(String input, out String errorPrompt)
        {
            if (input == null || input == "")
            {
                errorPrompt = "Please enter a non empty name.";
                return false;
            }
            foreach (ToothSection section in toothSections)
            {
                if (section.Name == input)
                {
                    errorPrompt = "That name is already in use. Please provide another.";
                    return false;
                }
            }
            errorPrompt = "";
            return true;
        }

        private void removeSectionCallback(EditUICallback callback, EditInterfaceCommand command)
        {
            EditInterface edit = callback.getSelectedEditInterface();
            ToothSection section = sectionManager.resolveSourceObject(edit);
            toothSections.Remove(section);
            onToothSectionRemoved(section, edit);
        }

        private void onToothSectionAdded(ToothSection section)
        {
            EditInterface edit = section.getEditInterface(section.Name, BehaviorEditMemberScanner.Scanner);
            ((ToothEditRenderer)editInterface.Renderer).addSubRenderer(edit.Renderer);
            sectionManager.addSubInterface(section, edit);
        }

        private void onToothSectionRemoved(ToothSection section, EditInterface edit)
        {
            ((ToothEditRenderer)editInterface.Renderer).removeSubRenderer(edit.Renderer);
            sectionManager.removeSubInterface(section);
        }

        #endregion
    }
}
