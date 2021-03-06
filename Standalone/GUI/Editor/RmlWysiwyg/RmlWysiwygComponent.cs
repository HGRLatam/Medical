﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;
using libRocketPlugin;
using Medical.GUI.AnomalousMvc;
using System.Xml;
using System.IO;
using Engine.Editing;
using Engine;
using Medical.GUI.RmlWysiwyg.Elements;
using FreeImageAPI;
using Anomalous.libRocketWidget;
using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Editor;

namespace Medical.GUI
{
    public class RmlWysiwygComponent : LayoutComponent
    {
        public const String DefaultImage = "~/Medical.Resources.ImagePlaceholder.png";

        public delegate void ElementOffDocumentDelegate(RmlWysiwygComponent sender, IntVector2 position, String innerRmlHint, String previewElementTagType);
        public delegate String GetMissingRmlDelegate(String file, AnomalousMvcContext context);

        public event Action<RmlWysiwygComponent> RmlEdited;
        public event ElementOffDocumentDelegate ElementDraggedOffDocument
        {
            add
            {
                draggingElementManager.ElementDraggedOffDocument += value;
            }
            remove
            {
                draggingElementManager.ElementDraggedOffDocument -= value;
            }
        }

        public event ElementOffDocumentDelegate ElementDroppedOffDocument
        {
            add
            {
                draggingElementManager.ElementDroppedOffDocument += value;
            }
            remove
            {
                draggingElementManager.ElementDroppedOffDocument -= value;
            }
        }

        public event ElementOffDocumentDelegate ElementReturnedToDocument
        {
            add
            {
                draggingElementManager.ElementReturnedToDocument += value;
            }
            remove
            {
                draggingElementManager.ElementReturnedToDocument -= value;
            }
        }

        private ElementStrategyManager elementStrategyManager = new ElementStrategyManager();
        private RocketWidget rocketWidget;
        private ImageBox rmlImage;
        private int imageHeight;
        private int imageWidth;
        private String documentStart = "<body>";
        private String documentEnd = "</body>";
        private bool disposed = false;
        private GuiFrameworkUICallback uiCallback;
        private RmlElementEditor currentEditor = null;
        private bool allowEdit = true;
        private SelectedElementManager selectedElementManager;
        private PreviewElement previewElement = new PreviewElement();
        private DraggingElementManager draggingElementManager;
        private bool lastInsertBefore = false;
        private UndoRedoBuffer undoBuffer;
        private String documentName;
        private Action<String> undoRedoCallback;
        private bool changesMade = false;
        private RmlWysiwygViewBase rmlWysiwygViewInterface;
        String contentId = null;
        private GetMissingRmlDelegate getMissingRmlCallback;

        private AnomalousMvcContext context;

        private RmlWysiwygComponent(AnomalousMvcContext context, MyGUIViewHost viewHost, RmlWysiwygViewBase rmlWysiwygViewInterface)
            : base("Medical.GUI.Editor.RmlWysiwyg.RmlWysiwygComponent.layout", viewHost)
        {
            undoRedoCallback = defaultUndoRedoCallback;
            this.context = context;
            this.rmlWysiwygViewInterface = rmlWysiwygViewInterface;

            rmlImage = (ImageBox)widget;
            rocketWidget = new RocketWidget(rmlImage, viewHost.View.Transparent);
            rmlImage.MouseButtonPressed += rmlImage_MouseButtonPressed;
            rmlImage.MouseButtonReleased += rmlImage_MouseButtonReleased;
            rmlImage.MouseDrag += new MyGUIEvent(rmlImage_MouseDrag);
            rmlImage.MouseWheel += new MyGUIEvent(rmlImage_MouseWheel);
            rmlImage.EventScrollGesture += new MyGUIEvent(rmlImage_EventScrollGesture);
            imageHeight = rmlImage.Height;

            selectedElementManager = new SelectedElementManager(rmlImage, rocketWidget.Context);
            draggingElementManager = new DraggingElementManager(this);

            foreach (var elementStrategy in rmlWysiwygViewInterface.CustomElementStrategies)
            {
                elementStrategyManager.add(elementStrategy);
            }

            if (rmlWysiwygViewInterface.GetMissingRmlCallback != null)
            {
                getMissingRmlCallback = rmlWysiwygViewInterface.GetMissingRmlCallback;
            }
            else
            {
                getMissingRmlCallback = getDefaultMissingRml;
            }
        }

        public RmlWysiwygComponent(RmlWysiwygView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
            : this(context, viewHost, view)
        {
            this.uiCallback = view.UICallback;
            this.undoBuffer = view.UndoBuffer;
            this.contentId = view.ContentId;
            rocketWidget.Context.ZoomLevel = view.ZoomLevel;

            if (view.UndoRedoCallback != null)
            {
                undoRedoCallback = view.UndoRedoCallback;
            }

            documentName = view.RmlFile;
            if (documentName != null)
            {
                this.FakeLoadLocation = RocketInterface.createValidFileUrl(context.ResourceProvider.getFullFilePath(documentName));
            }
            else
            {
                this.FakeLoadLocation = RocketInterface.createValidFileUrl(context.ResourceProvider.BackingLocation);
            }
            loadDocumentFile(documentName, false);

            view._fireComponentCreated(this);
        }

        public RmlWysiwygComponent(RawRmlWysiwygView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
            : this(context, viewHost, view)
        {
            if (view.FakePath != null)
            {
                this.FakeLoadLocation = RocketInterface.createValidFileUrl(context.ResourceProvider.getFullFilePath(view.FakePath));
            }
            else
            {
                this.FakeLoadLocation = RocketInterface.createValidFileUrl(context.ResourceProvider.BackingLocation);
            }

            this.uiCallback = view.UICallback;
            this.undoBuffer = view.UndoBuffer;
            this.contentId = view.ContentId;
            rocketWidget.Context.ZoomLevel = view.ZoomLevel;

            if (view.UndoRedoCallback != null)
            {
                undoRedoCallback = view.UndoRedoCallback;
            }

            documentName = null;
            setDocumentRml(view.Rml, false);

            view._fireComponentCreated(this);
        }

        public override void Dispose()
        {
            draggingElementManager.Dispose();
            previewElement.Dispose();
            disposed = true;
            rocketWidget.Dispose();
            rocketWidget = null;
            base.Dispose();
        }

        public override void topLevelResized()
        {
            if (widget.Height != imageHeight || widget.Width != imageWidth)
            {
                rocketWidget.resized();
                imageHeight = widget.Height;
                imageWidth = widget.Width;
                selectedElementManager.updateHighlightPosition();
                updateEditorPosition();
            }
            base.topLevelResized();
        }

        public override void changeScale(float newScale)
        {
            base.changeScale(newScale);
            if (rocketWidget != null)
            {
                rocketWidget.setScale(newScale);
            }
        }

        public float getScale()
        {
            return rocketWidget.Context.ZoomLevel;
        }

        public void aboutToSaveRml()
        {
            if (currentEditor != null)
            {
                currentEditor.hide();
            }
        }

        public void reloadDocument()
        {
            loadDocumentFile(documentName, true);
        }

        /// <summary>
        /// Set the current rml. If you pass true for considerAsChange the RmlEdited event will fire
        /// and the editor will be set as having changes.
        /// </summary>
        /// <param name="rml">The rml to set</param>
        /// <param name="keepScrollPosition">True to try to maintain the current scroll position.</param>
        /// <param name="considerAsChange">True to fire the RmlEdited event with the rml you have supplied.</param>
        public void setRml(String rml, bool keepScrollPosition, bool considerAsChange = false)
        {
            setDocumentRml(rml, keepScrollPosition);
            if (considerAsChange)
            {
                rmlModified();
            }
        }

        public void insertRml(String rml)
        {
            if (allowEdit)
            {
                cancelAndHideEditor();
                previewElement.hidePreviewElement();
                String undoRml = UnformattedRml;
                insertRmlIntoDocument(rml);
                updateUndoStatus(undoRml);
                selectedElementManager.clearSelectedAndHighlightedElement();
            }
        }

        public bool contains(IntVector2 position)
        {
            return widget.contains(position.x, position.y);
        }

        public void writeToGraphics(FreeImageBitmap g, Rectangle destRect)
        {
            rocketWidget.writeToGraphics(g, destRect);
        }

        internal void insertRml(String rml, String undoRml)
        {
            if (allowEdit)
            {
                insertRmlIntoDocument(rml);
                updateUndoStatus(undoRml);
            }
        }

        private void insertRmlIntoDocument(String rml)
        {
            if (allowEdit)
            {
                previewElement.hidePreviewElement();

                ElementDocument document = rocketWidget.Context.GetDocument(0);
                using (Element div = document.CreateElement("temp"))
                {
                    Element topContentElement = TopContentElement;
                    if (selectedElementManager.HasSelection && selectedElementManager.SelectedElement != topContentElement)
                    {
                        Element insertInto = selectedElementManager.SelectedElement.ParentNode;
                        if (insertInto != null)
                        {
                            insertInto.Insert(div, selectedElementManager.SelectedElement, lastInsertBefore);
                        }
                    }
                    else
                    {
                        topContentElement.AppendChild(div);
                    }

                    div.InnerRml = rml;

                    Element parent = div.ParentNode;
                    Element child;
                    Element next = div.FirstChild;

                    while (next != null)
                    {
                        child = next;
                        next = child.NextSibling;
                        parent.InsertBefore(child, div);
                    }
                    parent.RemoveChild(div);

                    rmlModified();
                }
            }
        }

        /// <summary>
        /// Set the preview element at position if position is inside this widget. Returns true if the position is inside this widget.
        /// </summary>
        public bool setPreviewElement(IntVector2 position, String innerRmlHint, String previewElementTagType)
        {
            if (widget.contains(position.x, position.y))
            {
                position.x -= widget.AbsoluteLeft;
                position.y -= widget.AbsoluteTop;

                Element toSelect = rocketWidget.Context.FindElementAtPoint(position);
                if (toSelect != null && !toSelect.isDescendentOf(TopContentElement))
                {
                    toSelect = null;
                }
                Element selectedElement = selectedElementManager.SelectedElement;

                bool insertBefore = lastInsertBefore;
                bool toSelectIsNotPreview = true;
                if (toSelect != null)
                {
                    toSelectIsNotPreview = !previewElement.isPreviewOrDescendent(toSelect);
                    if (toSelectIsNotPreview)
                    {
                        insertBefore = insertBeforeOrAfter(toSelect, position);
                    }
                }
                if (toSelectIsNotPreview && (toSelect != selectedElement || insertBefore != lastInsertBefore))
                {
                    if (toSelect != null)
                    {
                        Element topContentElement = TopContentElement;
                        if (toSelect != topContentElement)
                        {
                            selectedElementManager.SelectedElement = toSelect;
                            previewElement.hidePreviewElement();
                            ElementDocument document = rocketWidget.Context.GetDocument(0);
                            previewElement.showPreviewElement(document, innerRmlHint, toSelect.ParentNode, toSelect, previewElementTagType, insertBefore);
                            selectedElementManager.HighlightElement = previewElement.HighlightPreviewElement;
                        }
                        else
                        {
                            selectedElementManager.SelectedElement = null;
                            previewElement.hidePreviewElement();
                            ElementDocument document = rocketWidget.Context.GetDocument(0);
                            previewElement.showPreviewElement(document, innerRmlHint, toSelect, null, previewElementTagType, insertBefore);
                            selectedElementManager.HighlightElement = previewElement.HighlightPreviewElement;
                        }
                        selectedElementManager.ElementStrategy = null;
                    }
                    else
                    {
                        selectedElementManager.clearSelectedAndHighlightedElement();
                        previewElement.hidePreviewElement();
                    }

                    lastInsertBefore = insertBefore;

                    rocketWidget.Context.Update();
                    selectedElementManager.updateHighlightPosition();
                    rocketWidget.renderOnNextFrame();
                }
                return true;
            }
            else
            {
                selectedElementManager.clearSelectedAndHighlightedElement();
                previewElement.hidePreviewElement();
                rocketWidget.renderOnNextFrame();
                return false;
            }
        }

        public void clearPreviewElement(bool treatAsChanges = true)
        {
            selectedElementManager.clearSelectedAndHighlightedElement();
            previewElement.hidePreviewElement();
            rmlModified(treatAsChanges: treatAsChanges);
        }

        public void cancelAndHideEditor()
        {
            if (currentEditor != null)
            {
                currentEditor.cancelAndHide();
            }
        }

        public String CurrentRml
        {
            get
            {
                String rml = UnformattedRml;
                if (rml != null)
                {
                    rml = formatRml(rml);
                }
                return rml;
            }
        }

        public String UnformattedRml
        {
            get
            {
                Element topContentElemnt = TopContentElement;
                if (topContentElemnt != null)
                {
                    String contentRml = topContentElemnt.InnerRml;
                    StringBuilder sb = new StringBuilder(documentStart.Length + contentRml.Length + documentEnd.Length);
                    sb.Append(documentStart);
                    sb.Append(contentRml);
                    sb.Append(documentEnd);
                    return sb.ToString();
                }
                return null;
            }
        }

        public Element TopContentElement
        {
            get
            {
                if (rocketWidget == null)
                {
                    return null;
                }

                Element document = rocketWidget.Context.GetDocument(0);
                if (document == null)
                {
                    return null;
                }

                if (!String.IsNullOrEmpty(contentId))
                {
                    Element contentElement = document.GetElementById(contentId);
                    if (contentElement != null)
                    {
                        return contentElement;
                    }
                }

                Variant templateName = document.GetAttribute("template");
                if (templateName == null)
                {
                    return document;
                }
                else
                {
                    Template template = TemplateCache.GetTemplate(templateName.StringValue);
                    if (template != null)
                    {
                        Element contentDocument = document.GetElementById(template.Content);
                        if (contentDocument != null)
                        {
                            return contentDocument;
                        }
                        else
                        {
                            return document;
                        }
                    }
                    else
                    {
                        return document;
                    }
                }
            }
        }

        public bool ChangesMade
        {
            get
            {
                return changesMade;
            }
            set
            {
                changesMade = value;
            }
        }

        public String FakeLoadLocation { get; set; }

        internal IntVector2 localCoord(IntVector2 position)
        {
            position.x -= widget.AbsoluteLeft;
            position.y -= widget.AbsoluteTop;
            return position;
        }

        public void deleteElement(Element element)
        {
            Element parent = element.ParentNode;
            if (parent != null)
            {
                parent.RemoveChild(element);
                if (element == selectedElementManager.SelectedElement)
                {
                    selectedElementManager.clearSelectedAndHighlightedElement();
                }
            }
        }

        private String formatRml(String inputRml)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(inputRml);
                StringBuilder sb = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "\t";
                settings.NewLineChars = "\n";
                settings.NewLineHandling = NewLineHandling.Replace;
                settings.OmitXmlDeclaration = true; //.Net wants to write the string as UTF-16, but in libRocket rml files are always UTF-8, so we don't need a declaration and it would be wrong anyway.
                using (XmlWriter xmlWriter = XmlWriter.Create(sb, settings))
                {
                    xmlDoc.Save(xmlWriter);
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.show("There was an error parsing your RML back into a nice format.\nYou will want to correct it as it means your XML is malformed.\nThe error was:\n" + ex.Message, "RML Format Error", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
                return inputRml;
            }
        }

        private void saveDocumentStartAndEnd(String inputRml)
        {
            int bodyStart = inputRml.IndexOf("<body", StringComparison.InvariantCultureIgnoreCase);
            if (bodyStart > -1)
            {
                bodyStart = inputRml.IndexOf(">", bodyStart, StringComparison.InvariantCultureIgnoreCase) + 1;
                if (bodyStart > -1)
                {
                    documentStart = inputRml.Substring(0, bodyStart);

                    int closeBodyStart = inputRml.IndexOf("</body", bodyStart, StringComparison.InvariantCultureIgnoreCase);
                    if (closeBodyStart > -1)
                    {
                        documentEnd = inputRml.Substring(closeBodyStart);
                        allowEdit = true;
                    }
                    else
                    {
                        allowEdit = false;
                        MessageBox.show("Cannot find an closing body tag.\nPlease ensure that your source has a closing </body> element.\nYou will not be able to edit elements in the document until this is fixed.", "RML Format Error", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
                        bodyStart = 0;
                    }
                }
                else
                {
                    allowEdit = false;
                    MessageBox.show("Cannot find an opening body tag.\nPlease ensure that your source has an opening <body> element.\nYou will not be able to edit elements in the document until this is fixed.", "RML Format Error", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
                    bodyStart = 0;
                }
            }
            else
            {
                allowEdit = false;
                MessageBox.show("Cannot find an opening body tag.\nPlease ensure that your source has an opening <body> element.\nYou will not be able to edit elements in the document until this is fixed.", "RML Format Error", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
                bodyStart = 0;
            }
        }

        void rmlImage_EventScrollGesture(Widget source, EventArgs e)
        {
            selectedElementManager.updateHighlightPosition();
        }

        void rmlImage_MouseWheel(Widget source, EventArgs e)
        {
            selectedElementManager.updateHighlightPosition();
        }

        void rmlImage_MouseDrag(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == Engine.Platform.MouseButtonCode.MB_BUTTON0)
            {
                selectedElementManager.updateHighlightPosition();
                draggingElementManager.dragging(me.Position);
            }
        }

        void rmlImage_MouseButtonPressed(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == Engine.Platform.MouseButtonCode.MB_BUTTON0)
            {
                requestFocus();
                IntVector2 mousePosition = me.Position;
                IntVector2 localPosition = localCoord(mousePosition);
                Element element = selectedElementManager.SelectedElement;
                if (element == null || !element.IsPointWithinElement(localPosition))
                {
                    element = rocketWidget.Context.FindElementAtPoint(localPosition);
                }
                if (elementStrategyManager[element].AllowDragAndDrop)
                {
                    draggingElementManager.dragStart(mousePosition, element, elementStrategyManager[element].PreviewIconName);
                }
            }
        }

        void rmlImage_MouseButtonReleased(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == Engine.Platform.MouseButtonCode.MB_BUTTON0)
            {
                Element element = draggingElementManager.DragElement; //This will be the clicked element, or null if the element was moved
                ElementStrategy elementStrategy = elementStrategyManager[element];
                IntVector2 mousePosition = me.Position;
                draggingElementManager.dragEnded(mousePosition);

                if (!allowEdit)
                {
                    //Break if they cannot edit
                    return;
                }

                Element altElement = rocketWidget.Context.FindElementAtPoint(localCoord(mousePosition), element);
                if (altElement != null) //Another element was found see if we can use it
                {
                    ElementStrategy altStrategy = elementStrategyManager[altElement];
                    if (altStrategy != elementStrategyManager.DefaultStrategy)
                    {
                        element = altElement;
                        elementStrategy = altStrategy;
                    }
                }
                if (element != null)
                {
                    showRmlElementEditor(element, elementStrategy);
                }
                else
                {
                    cancelAndHideEditor();
                    selectedElementManager.clearSelectedAndHighlightedElement();
                }
            }
        }

        private void showRmlElementEditor(Element element, ElementStrategy strategy)
        {
            if (currentEditor == null || selectedElementManager.SelectedElement != element)
            {
                cancelAndHideEditor();
                RmlElementEditor editor = strategy.openEditor(element, uiCallback, 0, 0);
                if (editor == null)
                {
                    //The editor was null, which means editing is not supported so just clear the selection.
                    selectedElementManager.clearSelectedAndHighlightedElement();
                    return; //Return here to prevent more execution
                }

                editor.UndoRml = UnformattedRml;
                //Everything is good so setup.
                editor.Hiding += (src, arg) =>
                {
                    if (!disposed)
                    {
                        if (editor.deleteIfNeeded(this))
                        {
                            rmlModified();
                            updateUndoStatus(editor.UndoRml, true);
                            editor.UndoRml = UnformattedRml;
                        }
                        selectedElementManager.AllowShowResizeHandles = false;
                    }
                };
                editor.Hidden += (src, arg) =>
                {
                    if (currentEditor == editor)
                    {
                        currentEditor = null;
                    }
                };
                editor.ChangesMade += (applyElement) =>
                {
                    TwoWayCommand additionalUndoOperations;
                    if (!disposed && editor.applyChanges(this, out additionalUndoOperations))
                    {
                        rocketWidget.Context.GetDocument(0).MakeDirtyForScaleChange();
                        rmlModified();
                        updateUndoStatus(editor.UndoRml, true, additionalUndoOperations);
                        editor.UndoRml = UnformattedRml;
                    }
                };
                editor.MoveElementUp += upElement =>
                {
                    Element previousSibling = upElement.PreviousSibling;
                    if (previousSibling != null)
                    {
                        Element parent = upElement.ParentNode;
                        if (parent != null)
                        {
                            upElement.addReference();
                            parent.RemoveChild(upElement);
                            parent.InsertBefore(upElement, previousSibling);
                            upElement.removeReference();
                            rmlModified();
                            updateUndoStatus(editor.UndoRml);
                            editor.UndoRml = UnformattedRml;
                        }
                    }
                };
                editor.MoveElementDown += downElement =>
                {
                    Element parent = downElement.ParentNode;
                    if (parent != null)
                    {
                        Element nextSibling = downElement.NextSibling;
                        if (nextSibling != null)
                        {
                            downElement.addReference();
                            parent.RemoveChild(downElement);
                            nextSibling = nextSibling.NextSibling;
                            if (nextSibling != null)
                            {
                                parent.InsertBefore(downElement, nextSibling);
                            }
                            else
                            {
                                parent.AppendChild(downElement);
                            }
                            downElement.removeReference();

                            rmlModified();
                            updateUndoStatus(editor.UndoRml);
                            editor.UndoRml = UnformattedRml;
                        }
                    }
                };
                editor.DeleteElement += deleteElement =>
                {
                    Element parent = deleteElement.ParentNode;
                    if (parent != null)
                    {
                        Element nextSelectionElement = deleteElement.NextSibling;
                        if (nextSelectionElement == null)
                        {
                            nextSelectionElement = deleteElement.PreviousSibling;
                        }

                        parent.RemoveChild(deleteElement);
                        rmlModified(false);
                        updateUndoStatus(editor.UndoRml);
                        editor.UndoRml = UnformattedRml;

                        if (nextSelectionElement != null)
                        {
                            showRmlElementEditor(nextSelectionElement, elementStrategyManager[nextSelectionElement]);
                        }
                        else
                        {
                            selectedElementManager.clearSelectedAndHighlightedElement();
                        }
                    }
                };
                currentEditor = editor;
                selectedElementManager.SelectedElement = element;
                selectedElementManager.setHighlightElement(element, strategy.HighlightProvider);
                selectedElementManager.ElementStrategy = strategy;
                selectedElementManager.AllowShowResizeHandles = true;
                updateEditorPosition();
            }
        }

        private void updateEditorPosition()
        {
            Element element = selectedElementManager.SelectedElement;
            var editor = currentEditor;
            if (element != null && editor != null)
            {
                int left, top;
                switch (ViewHost.View.ElementName.LocationHint)
                {
                    case ViewLocations.Right:
                        left = widget.AbsoluteLeft - editor.Width;
                        top = (int)element.AbsoluteTop + rocketWidget.AbsoluteTop;
                        break;
                    case ViewLocations.Top:
                        left = widget.Width / 2 + widget.AbsoluteLeft - editor.Width / 2;
                        top = widget.AbsoluteTop + widget.Height;
                        break;
                    case ViewLocations.Bottom:
                        left = widget.Width / 2 + widget.AbsoluteLeft - editor.Width / 2;
                        top = rocketWidget.AbsoluteTop - editor.Height;
                        break;
                    case ViewLocations.Left:
                        left = widget.AbsoluteLeft + widget.Width;
                        top = (int)element.AbsoluteTop + rocketWidget.AbsoluteTop;
                        break;
                    default:
                        left = (int)(widget.AbsoluteLeft + element.AbsoluteLeft + element.ClientWidth);
                        top = (int)element.AbsoluteTop + rocketWidget.AbsoluteTop;
                        break;
                }
                editor.setPosition(left, top, true);
            }
        }

        private void rmlModified(bool updateHighlights = true, bool treatAsChanges = true)
        {
            if (treatAsChanges)
            {
                changesMade = true;
                if (RmlEdited != null)
                {
                    RmlEdited.Invoke(this);
                }
            }
            if (updateHighlights)
            {
                rocketWidget.Context.Update();
                selectedElementManager.updateHighlightPosition();
            }
            rocketWidget.renderOnNextFrame();
        }

        private bool insertBeforeOrAfter(Element element, IntVector2 position)
        {
            return position.y - element.AbsoluteTop < element.OffsetHeight / 2;
        }

        public void updateUndoStatus(String oldMarkup, bool check = false, TwoWayCommand additionalUndoOperations = null)
        {
            //This is a hacky way to check for changes (optionally) it should not be needed when the popup editor is overhauled.
            //You can remove check and keep only the line in the if statement when you no longer need the check.
            String currentMarkup = UnformattedRml;
            if (!check || currentMarkup != oldMarkup)
            {
                if (additionalUndoOperations == null)
                {
                    undoBuffer.pushAndSkip(new TwoWayDelegateCommand<String, String>(currentMarkup, oldMarkup, new TwoWayDelegateCommand<string, string>.Funcs()
                    {
                        ExecuteFunc = undoRedoCallback,
                        UndoFunc = undoRedoCallback
                    }));
                }
                else
                {
                    undoBuffer.pushAndSkip(new TwoWayDelegateCommand<String, String>(currentMarkup, oldMarkup, new TwoWayDelegateCommand<string, string>.Funcs()
                    {
                        ExecuteFunc = rml =>
                        {
                            undoRedoCallback(rml);
                            additionalUndoOperations.execute();
                        },
                        UndoFunc = rml =>
                        {
                            undoRedoCallback(rml);
                            additionalUndoOperations.undo();
                        }
                    }));
                }
            }
        }

        private void defaultUndoRedoCallback(String rml)
        {
            cancelAndHideEditor();
            if (setDocumentRml(rml, true))
            {
                rmlModified();
            }
        }

        private void loadDocumentFile(String file, bool maintainScrollPosition)
        {
            if (file != null)
            {
                if (context.ResourceProvider.fileExists(file))
                {
                    using (StreamReader sr = new StreamReader(context.ResourceProvider.openFile(file)))
                    {
                        setDocumentRml(sr.ReadToEnd(), maintainScrollPosition);
                    }
                }
                else
                {
                    setDocumentRml(getMissingRmlCallback(file, context), false);
                }
            }
        }

        private bool setDocumentRml(String rml, bool maintainScrollPosition)
        {
            float scrollLeft = 0.0f;
            float scrollTop = 0.0f;
            Element topContentElement;

            if (maintainScrollPosition)
            {
                topContentElement = TopContentElement;
                if (topContentElement != null)
                {
                    scrollLeft = topContentElement.ScrollLeft;
                    scrollTop = topContentElement.ScrollTop;
                }
            }

            RocketWidgetInterface.clearAllCaches();
            rocketWidget.Context.UnloadAllDocuments();
            cancelAndHideEditor();
            selectedElementManager.clearSelectedAndHighlightedElement();

            if (rml != null)
            {
                using (ElementDocument document = rocketWidget.Context.LoadDocumentFromMemory(rml, FakeLoadLocation))
                {
                    if (document != null)
                    {
                        saveDocumentStartAndEnd(rml);
                        document.Show();
                        rocketWidget.removeFocus();
                        rocketWidget.renderOnNextFrame();

                        if (maintainScrollPosition)
                        {
                            topContentElement = TopContentElement;
                            if (topContentElement != null)
                            {
                                topContentElement.ScrollLeft = scrollLeft;
                                topContentElement.ScrollTop = scrollTop;
                            }
                        }

                        return true;
                    }
                }
            }
            return false;
        }

        private void requestFocus()
        {
            rmlWysiwygViewInterface._fireRequestFocus();
        }

        private string getDefaultMissingRml(string file, AnomalousMvcContext context)
        {
            using (StreamReader stream = new StreamReader(this.GetType().Assembly.GetManifestResourceStream("Medical.GUI.Editor.RmlWysiwyg.MissingFile.rml")))
            {
                return String.Format(stream.ReadToEnd(), file, context.ResourceProvider.BackingLocation);
            }
        }
    }
}
