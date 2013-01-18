﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using System.IO;
using Engine.Platform;
using Medical.Controller;

namespace Medical.GUI
{
    public class SlideshowExplorer : MDIDialog
    {
        private String windowTitle;
        private const String windowTitleFormat = "{0} - {1}";

        //File Menu
        MenuBar menuBar;
        MenuItem newProject;
        MenuItem openProject;
        MenuItem closeProject;
        MenuItem saveAll;

        private Slideshow slideshow;
        private EditorController editorController;
        private SlideshowEditController slideEditController;

        //Buttons
        Button addButton;
        Button removeButton;

        private ButtonGrid slideGrid;
        private ScrollView scroll;

        public SlideshowExplorer(EditorController editorController, SlideshowEditController slideEditController)
            : base("Medical.GUI.SlideshowExplorer.SlideshowExplorer.layout")
        {
            this.editorController = editorController;
            this.slideEditController = slideEditController;
            editorController.ProjectChanged += editorController_ProjectChanged;

            addButton = (Button)window.findWidget("Add");
            addButton.MouseButtonClick += addButton_MouseButtonClick;
            removeButton = (Button)window.findWidget("Remove");
            removeButton.MouseButtonClick += removeButton_MouseButtonClick;

            slideEditController.SlideshowLoaded += slideEditController_SlideshowLoaded;
            slideEditController.SlideshowClosed += slideEditController_SlideshowClosed;
            slideEditController.SlideAdded += slideEditController_SlideAdded;
            slideEditController.SlideRemoved += slideEditController_SlideRemoved;
            slideEditController.SlideSelected += slideEditController_SlideSelected;

            windowTitle = window.Caption;
            menuBar = window.findWidget("MenuBar") as MenuBar;

            scroll = (ScrollView)window.findWidget("Scroll");
            slideGrid = new ButtonGrid(scroll, new ButtonGridListLayout());
            slideGrid.SelectedValueChanged += slideGrid_SelectedValueChanged;

            //File Menu
            MenuItem fileMenuItem = menuBar.addItem("File", MenuItemType.Popup);
            MenuControl fileMenu = menuBar.createItemPopupMenuChild(fileMenuItem);
            fileMenu.ItemAccept += new MyGUIEvent(fileMenu_ItemAccept);
            newProject = fileMenu.addItem("New Project");
            openProject = fileMenu.addItem("Open Project");
            closeProject = fileMenu.addItem("Close Project");
            saveAll = fileMenu.addItem("Save");

            this.Resized += new EventHandler(ProjectExplorer_Resized);
        }

        public override void Dispose()
        {
            slideGrid.Dispose();
            base.Dispose();
        }

        void createNewProjectClicked(Widget source, EventArgs e)
        {
            if (editorController.ResourceProvider == null || editorController.ResourceProvider.ResourceCache.Count == 0)
            {
                showNewProjectDialog();
            }
            else
            {
                MessageBox.show("You have open files, would you like to save them before creating a new project?", "Save", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
                {
                    if (result == MessageBoxStyle.Ok)
                    {
                        editorController.saveAllCachedResources();
                    }
                    showNewProjectDialog();
                });
            }
        }

        void showNewProjectDialog()
        {
            editorController.stopPlayingTimelines();
            NewProjectDialog.ShowDialog((template, fullProjectName) =>
            {
                if (Directory.Exists(fullProjectName))
                {
                    MessageBox.show(String.Format("The project {0} already exists. Would you like to delete it and create a new one?", fullProjectName), "Overwrite?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, result =>
                    {
                        if (result == MessageBoxStyle.Yes)
                        {
                            editorController.createNewProject(fullProjectName, true, template);
                        }
                    });
                }
                else
                {
                    editorController.createNewProject(fullProjectName, false, template);
                }
            });
        }

        void openProjectClicked(Widget source, EventArgs e)
        {
            if (editorController.ResourceProvider == null || editorController.ResourceProvider.ResourceCache.Count == 0)
            {
                showOpenProjectDialog();
            }
            else
            {
                MessageBox.show("You have open files, would you like to save them before opening a new project?", "Save", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
                {
                    if (result == MessageBoxStyle.Ok)
                    {
                        editorController.saveAllCachedResources();
                    }
                    showOpenProjectDialog();
                });
            }
        }

        void showOpenProjectDialog()
        {
            editorController.stopPlayingTimelines();
            FileOpenDialog fileDialog = new FileOpenDialog(MainWindow.Instance, "Open a project.", "", "", "", false);
            fileDialog.showModal((result, paths) =>
            {
                if (result == NativeDialogResult.OK)
                {
                    String path = paths.First();
                    editorController.openProject(Path.GetDirectoryName(path), path);
                }
            });
        }

        void editorController_ProjectChanged(EditorController editorController, String defaultFile)
        {
            if (editorController.ResourceProvider != null)
            {
                window.Caption = String.Format(windowTitleFormat, windowTitle, editorController.ResourceProvider.BackingLocation);
            }
            else
            {
                window.Caption = windowTitle;
            }
        }

        void ProjectExplorer_Resized(object sender, EventArgs e)
        {
            slideGrid.resizeAndLayout(scroll.ClientCoord.width);
        }

        void fileMenu_ItemAccept(Widget source, EventArgs e)
        {
            MenuCtrlAcceptEventArgs menuEventArgs = (MenuCtrlAcceptEventArgs)e;
            if (menuEventArgs.Item == newProject)
            {
                createNewProjectClicked(source, e);
            }
            else if (menuEventArgs.Item == openProject)
            {
                openProjectClicked(source, e);
            }
            else if (menuEventArgs.Item == saveAll)
            {
                editorController.saveAllCachedResources();
            }
            else if (menuEventArgs.Item == closeProject)
            {
                editorController.closeProject();
            }
        }

        void addButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (slideshow != null)
            {
                AddItemDialog.AddItem(editorController.ItemTemplates, (itemTemplate) =>
                {
                    try
                    {
                        ((ProjectItemTemplate)itemTemplate).createItem("", editorController);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.show(String.Format("Error creating item.\n{0}", ex.Message), "Error Creating Item", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                    }
                });
            }
        }

        void removeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (slideshow != null)
            {
                ButtonGridItem selectedItem = slideGrid.SelectedItem;
                if (selectedItem != null)
                {
                    slideEditController.removeSlide((Slide)selectedItem.UserObject);
                }
            }
        }

        void fileBrowser_FileSelected(FileBrowserTree tree, string path)
        {
            editorController.openEditor(path);
        }

        void slideEditController_SlideshowLoaded(Slideshow show)
        {
            slideGrid.SuppressLayout = true;
            slideGrid.clear();
            foreach (Slide slide in show.Slides)
            {
                addSlideToGrid(slide);
            }
            slideGrid.SuppressLayout = false;
            slideGrid.resizeAndLayout(scroll.ClientCoord.width);
            this.slideshow = show;
        }

        void slideEditController_SlideAdded(Slide slide)
        {
            addSlideToGrid(slide);
        }

        void slideEditController_SlideRemoved(Slide slide)
        {
            removeSlideFromGrid(slide);
        }

        void slideGrid_SelectedValueChanged(object sender, EventArgs e)
        {
            ButtonGridItem item = slideGrid.SelectedItem;
            if (item != null)
            {
                slideEditController.editSlide((Slide)item.UserObject);
            }
        }

        void slideEditController_SlideshowClosed()
        {
            slideGrid.clear();
            slideshow = null;
        }

        void addSlideToGrid(Slide slide)
        {
            ButtonGridItem item = slideGrid.addItem("", "Slide " + (slideGrid.Count + 1));
            item.UserObject = slide;
        }

        void removeSlideFromGrid(Slide slide)
        {
            ButtonGridItem item = slideGrid.findItemByUserObject(slide);
            if (item != null)
            {
                slideGrid.removeItem(item);
            }
        }

        void slideEditController_SlideSelected(Slide slide)
        {
            slideGrid.SelectedItem = slideGrid.findItemByUserObject(slide);
        }
    }
}