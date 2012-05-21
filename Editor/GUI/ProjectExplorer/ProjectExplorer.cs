﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using System.IO;
using Engine.Platform;

namespace Medical.GUI
{
    class ProjectExplorer : MDIDialog
    {
        private String windowTitle;
        private const String windowTitleFormat = "{0} - {1}";
        private readonly char[] SEPS = new char[] { '/', '\\' };

        //File Menu
        MenuBar menuBar;
        MenuItem newProject;
        MenuItem openProject;
        MenuItem saveAll;
        ExtensionActionCollection currentExtensionActions = null;

        Tree fileTree;

        //Dialogs
        private NewProjectDialog newProjectDialog;

        private EditorController editorController;

        public ProjectExplorer(EditorController editorController)
            :base("Medical.GUI.ProjectExplorer.ProjectExplorer.layout")
        {
            this.editorController = editorController;
            editorController.ProjectChanged += new EditorControllerEvent(editorController_ProjectChanged);
            editorController.ExtensionActionsChanged += new EditorControllerEvent(editorController_ExtensionActionsChanged);

            windowTitle = window.Caption;
            menuBar = window.findWidget("MenuBar") as MenuBar;

            fileTree = new Tree((ScrollView)window.findWidget("FileTableScroll"));
            fileTree.NodeMouseDoubleClick += new EventHandler<TreeEventArgs>(fileTree_NodeMouseDoubleClick);
            fileTree.NodeMouseReleased += new EventHandler<TreeMouseEventArgs>(fileTree_NodeMouseReleased);

            rebuildMenus();

            //Dialogs
            newProjectDialog = new NewProjectDialog();
            newProjectDialog.ProjectCreated += new EventHandler(newProjectDialog_ProjectCreated);

            this.Resized += new EventHandler(ProjectExplorer_Resized);
        }

        public override void Dispose()
        {
            fileTree.Dispose();
            newProjectDialog.Dispose();
            base.Dispose();
        }

        void createNewProjectClicked(Widget source, EventArgs e)
        {
            if (editorController.ResourceProvider == null || editorController.ResourceProvider.ResourceCache.Count == 0)
            {
                showNewProjectDialog(source.AbsoluteLeft, source.AbsoluteTop);
            }
            else
            {
                MessageBox.show("You have open files, would you like to save them before creating a new project?", "Save", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
                {
                    if (result == MessageBoxStyle.Ok)
                    {
                        editorController.saveAllCachedResources();
                    }
                    showNewProjectDialog(source.AbsoluteLeft, source.AbsoluteTop);
                });
            }
        }

        void showNewProjectDialog(int x, int y)
        {
            editorController.stopPlayingTimelines();
            newProjectDialog.open(true);
            newProjectDialog.Position = new Vector2(x, y);
            newProjectDialog.ensureVisible();
        }

        void newProjectDialog_ProjectCreated(object sender, EventArgs e)
        {
            editorController.createNewProject(newProjectDialog.FullProjectName, false);
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
            using (FileOpenDialog fileDialog = new FileOpenDialog(MainWindow.Instance, "Open a project.", "", "", "", false))
            {
                if (fileDialog.showModal() == NativeDialogResult.OK)
                {
                    editorController.openProject(fileDialog.Path);
                }
            }
        }

        void fileTree_NodeMouseDoubleClick(object sender, TreeEventArgs e)
        {
            ProjectExplorerFileNode fileNode = e.Node as ProjectExplorerFileNode;
            if(fileNode != null)
            {
                if (editorController.ResourceProvider.exists(fileNode.FilePath))
                {
                    editorController.openFile(fileNode.FilePath);
                }
            }
        }

        public void createNodesForPath(TreeNodeCollection parentCollection, String path)
        {
            ResourceProvider resourceProvider = editorController.ResourceProvider;
            if (resourceProvider != null)
            {
                fileTree.SuppressLayout = true;
                String[] directories = resourceProvider.listDirectories("*", path, false);
                foreach (String dir in directories)
                {
                    parentCollection.add(new ProjectExplorerDirectoryNode(dir, this));
                }
                String[] files = resourceProvider.listFiles("*", path, false);
                foreach (String file in files)
                {
                    parentCollection.add(new ProjectExplorerFileNode(file));
                }
                fileTree.SuppressLayout = false;
            }
        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            fileTree.Nodes.clear();
            if (editorController.ResourceProvider != null)
            {
                createNodesForPath(fileTree.Nodes, "");
                fileTree.layout();
                window.Caption = String.Format(windowTitleFormat, windowTitle, editorController.ResourceProvider.BackingLocation);
                editorController.ResourceProvider.FileCreated += new ResourceProviderFileEvent(ResourceProvider_FileCreated);
                editorController.ResourceProvider.FileDeleted += new ResourceProviderFileDeletedEvent(ResourceProvider_FileDeleted);
                editorController.ResourceProvider.FileRenamed += new ResourceProviderFileRenamedEvent(ResourceProvider_FileRenamed);
            }
            else
            {
                window.Caption = windowTitle;
            }
        }

        void ProjectExplorer_Resized(object sender, EventArgs e)
        {
            fileTree.layout();
        }

        private void rebuildMenus()
        {
            menuBar.removeAllItems();
            //File Menu
            MenuItem fileMenuItem = menuBar.addItem("File", MenuItemType.Popup);
            MenuControl fileMenu = menuBar.createItemPopupMenuChild(fileMenuItem);
            fileMenu.ItemAccept += new MyGUIEvent(fileMenu_ItemAccept);
            newProject = fileMenu.addItem("New Project");
            openProject = fileMenu.addItem("Open Project");

            if (currentExtensionActions != null)
            {
                Dictionary<String, MenuControl> menus = new Dictionary<string, MenuControl>();
                menus.Add("File", fileMenu);

                foreach (ExtensionAction action in currentExtensionActions)
                {
                    MenuControl menu;
                    if (!menus.TryGetValue(action.Category, out menu))
                    {
                        MenuItem menuItem = menuBar.addItem(action.Category, MenuItemType.Popup);
                        menu = menuBar.createItemPopupMenuChild(menuItem);
                        menu.ItemAccept += new MyGUIEvent(menu_ItemAccept);
                        menus.Add(action.Category, menu);
                    }
                    MenuItem item = menu.addItem(action.Name);
                    item.UserObject = action;
                }
            }

            saveAll = fileMenu.addItem("Save All");
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
            else
            {
                menu_ItemAccept(source, e);
            }
        }

        void menu_ItemAccept(Widget source, EventArgs e)
        {
            MenuCtrlAcceptEventArgs mcae = (MenuCtrlAcceptEventArgs)e;
            ExtensionAction action = mcae.Item.UserObject as ExtensionAction;
            if (action != null)
            {
                action.execute();
            }
        }

        void ResourceProvider_FileCreated(string path, bool isDirectory)
        {
            String parentPath = Path.GetDirectoryName(path);
            if (String.IsNullOrEmpty(parentPath))
            {
                fileTree.SuppressLayout = true;
                if (isDirectory)
                {
                    fileTree.Nodes.add(new ProjectExplorerDirectoryNode(path, this));
                }
                else
                {
                    fileTree.Nodes.add(new ProjectExplorerFileNode(path));
                }
                fileTree.SuppressLayout = false;
                fileTree.layout();
            }
            else
            {
                ProjectExplorerDirectoryNode node = findNodeForPath(parentPath) as ProjectExplorerDirectoryNode;
                if (node != null && node.ListedChildren)
                {
                    fileTree.SuppressLayout = true;
                    if (isDirectory)
                    {
                        node.addDirectoryNode(new ProjectExplorerDirectoryNode(path, this));
                    }
                    else
                    {
                        node.addFileNode(new ProjectExplorerFileNode(path));
                    }
                    fileTree.SuppressLayout = false;
                    fileTree.layout();
                }
            }
        }

        void ResourceProvider_FileRenamed(string path, string oldPath, bool isDirectory)
        {
            TreeNode node = findNodeForPath(oldPath);
            if (node != null)
            {
                if (isDirectory)
                {
                    ((ProjectExplorerDirectoryNode)node).changePath(path);
                }
                else
                {
                    ((ProjectExplorerFileNode)node).changePath(path);
                }
            }
        }

        void ResourceProvider_FileDeleted(string path)
        {
            TreeNode node = findNodeForPath(path);
            if (node != null)
            {
                fileTree.SuppressLayout = true;
                if (node.Parent == null)
                {
                    fileTree.Nodes.remove(node);
                }
                else
                {
                    node.Parent.Children.remove(node);
                }
                fileTree.SuppressLayout = false;
                fileTree.layout();
            }
        }

        TreeNode findNodeForPath(String path)
        {
            String[] names = path.Split(SEPS);
            TreeNode result = null;
            TreeNodeCollection nodes = fileTree.Nodes;
            for (int i = 0; i < names.Length; ++i)
            {
                result = nodes.findByText(names[i]);
                if (result != null)
                {
                    nodes = result.Children;
                }
                else
                {
                    i = names.Length;
                }
            }
            return result;
        }

        void editorController_ExtensionActionsChanged(EditorController editorController)
        {
            currentExtensionActions = editorController.ExtensionActions;
            rebuildMenus();
        }

        void fileTree_NodeMouseReleased(object sender, TreeMouseEventArgs e)
        {
            if (e.Button == MouseButtonCode.MB_BUTTON1)
            {
                ProjectExplorerDirectoryNode dirNode = e.Node as ProjectExplorerDirectoryNode;
                if (dirNode != null)
                {
                    fileTree.SelectedNode = dirNode;
                    PopupMenu directoryPopupMenu = (PopupMenu)Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1, 1, Align.Default, "Overlapped", "");
                    directoryPopupMenu.Visible = false;
                    directoryPopupMenu.ItemAccept += new MyGUIEvent(directoryPopupMenu_ItemAccept);
                    directoryPopupMenu.Closed += new MyGUIEvent(directoryPopupMenu_Closed);
                    MenuItem item = directoryPopupMenu.addItem("Create Directory", MenuItemType.Normal, "Create Directory");
                    item.UserObject = dirNode;
                    LayerManager.Instance.upLayerItem(directoryPopupMenu);
                    directoryPopupMenu.setPosition(e.MousePosition.x, e.MousePosition.y);
                    directoryPopupMenu.ensureVisible();
                    directoryPopupMenu.setVisibleSmooth(true);
                }
                else
                {
                    ProjectExplorerFileNode fileNode = e.Node as ProjectExplorerFileNode;
                    if (fileNode != null)
                    {

                    }
                }
            }
        }

        void directoryPopupMenu_ItemAccept(Widget source, EventArgs e)
        {
            MenuCtrlAcceptEventArgs mcae = (MenuCtrlAcceptEventArgs)e;
            switch (mcae.Item.ItemId)
            {
                case "Create Directory":
                    ProjectExplorerDirectoryNode dirNode = mcae.Item.UserObject as ProjectExplorerDirectoryNode;
                    InputBox.GetInput("Directory Name", "Please enter a name for the directory.", true, delegate(String result, ref String errorPrompt)
                    {
                        editorController.ResourceProvider.createDirectory(dirNode.DirectoryPath, result);
                        return true;
                    });
                    break;
            }
        }

        void directoryPopupMenu_Closed(Widget source, EventArgs e)
        {
            Gui.Instance.destroyWidget(source);
        }
    }
}
