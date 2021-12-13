﻿using CommonControls.Common;
using CommonControls.FileTypes.PackFiles.Models;
using CommonControls.PackFileBrowser;
using CommonControls.Services;
using KitbasherEditor.Services;
using KitbasherEditor.ViewModels.MenuBarViews;
using MonoGame.Framework.WpfInterop;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using View3D.Components;
using View3D.Components.Component;
using View3D.Components.Component.Selection;
using View3D.Components.Gizmo;
using View3D.Components.Input;
using View3D.Components.Rendering;
using View3D.Rendering.Geometry;
using View3D.Scene;
using View3D.SceneNodes;
using View3D.Services;
using View3D.Utility;

namespace KitbasherEditor.ViewModels
{
    public class KitbasherViewModel : NotifyPropertyChangedImpl, IEditorViewModel, IDropTarget
    {
        ILogger _logger = Logging.Create<KitbasherViewModel>();
        PackFileService _packFileService;
        SkeletonAnimationLookUpHelper _skeletonAnimationLookUpHelper;

        SceneContainer _scene;
        public SceneContainer Scene { get => _scene; set => SetAndNotify(ref _scene, value); }
        public SceneExplorerViewModel SceneExplorer { get; set; }
        public MenuBarViewModel MenuBar { get; set; }
        public AnimationControllerViewModel Animation { get; set; }


        string _displayName = "3d viewer";
        public string DisplayName { get => _displayName; set => SetAndNotify(ref _displayName, value); }

        public PackFile MainFile { get; set; }

        KitbashSceneCreator _modelLoader;

        public KitbasherViewModel(PackFileService pf, SkeletonAnimationLookUpHelper skeletonHelper)
        {
            _packFileService = pf;
            _skeletonAnimationLookUpHelper = skeletonHelper;

            Scene = new SceneContainer();
            Scene.AddComponent(new DeviceResolverComponent(Scene));
            Scene.AddComponent(new ResourceLibary(Scene, pf));
            Scene.AddComponent(new FpsComponent(Scene));
            Scene.AddComponent(new KeyboardComponent(Scene));
            Scene.AddComponent(new MouseComponent(Scene));
            Scene.AddComponent(new ArcBallCamera(Scene));
            Scene.AddComponent(new SceneManager(Scene));
            Scene.AddComponent(new SelectionManager(Scene));
            Scene.AddComponent(new CommandExecutor(Scene));
            Scene.AddComponent(new GizmoComponent(Scene));
            Scene.AddComponent(new SelectionComponent(Scene));
            Scene.AddComponent(new ObjectEditor(Scene));
            Scene.AddComponent(new FaceEditor(Scene));
            Scene.AddComponent(new FocusSelectableObjectComponent(Scene));
            Scene.AddComponent(new ClearScreenComponent(Scene));
            Scene.AddComponent(new RenderEngineComponent(Scene));
            Scene.AddComponent(new GridComponent(Scene));
            Scene.AddComponent(new AnimationsContainerComponent(Scene));
            Scene.AddComponent(new ViewOnlySelectedComponent(Scene));
            Scene.AddComponent(new LightControllerComponent(Scene));
            Scene.AddComponent(_skeletonAnimationLookUpHelper);

            Animation = new AnimationControllerViewModel(Scene, _packFileService);
            SceneExplorer = Scene.AddComponent(new SceneExplorerViewModel(Scene, _packFileService, Animation));
            
            MenuBar = new MenuBarViewModel(Scene, _packFileService);
            
            Scene.SceneInitialized += OnSceneInitialized;
        }

        private void OnSceneInitialized(WpfGame scene)
        {
            _modelLoader = new KitbashSceneCreator(scene, _packFileService, Animation, MainFile, GeometryGraphicsContextFactory.CreateInstance(Scene.GraphicsDevice));
            MenuBar.ModelLoader = _modelLoader;
            MenuBar.General.ModelSaver = new SceneSaverService(_packFileService, this, _modelLoader.EditableMeshNode);
            
            SceneExplorer.EditableMeshNode = _modelLoader.EditableMeshNode;
            
            if (MainFile != null)
            {
                try
                {
                    _modelLoader.LoadMainEditableModel(MainFile );
                    var nodes = _modelLoader.EditableMeshNode.GetMeshNodes(0)
                        .Select(x => x as ISelectable)
                        .Where(x => x != null)
                        .ToList();
                    Scene.GetComponent<FocusSelectableObjectComponent>().FocusObjects(nodes);
                   DisplayName = MainFile.Name;
                }
                catch (Exception e)
                {
                    _logger.Here().Error($"Error loading file {MainFile?.Name} - {e}");
                    MessageBox.Show($"Unable to load file\n+{e.Message}");
                }
            }
        }

        public bool Save()
        {
            return true;
        }

        public void Close()
        {
            Scene.Dispose();
            Scene.SceneInitialized -= OnSceneInitialized;
            _modelLoader = null;
            MenuBar = null;
            Scene = null;
            _packFileService = null;
            _skeletonAnimationLookUpHelper = null;
            Scene = null;
            SceneExplorer = null;
            MenuBar = null;
            Animation = null;
            MainFile = null;
            _modelLoader = null;
        }

        public bool HasUnsavedChanges()
        {
            return false;
        }

        public bool AllowDrop(TreeNode node)
        {
            if (node != null && node.NodeType == NodeType.File)
            {
                var extention = Path.GetExtension(node.Name).ToLower();
                if (extention == ".rigid_model_v2" || extention == ".wsmodel" || extention == ".variantmeshdefinition")
                    return true;
            }
            return false;
        }

        public bool Drop(TreeNode node)
        {
            _modelLoader.LoadReference(node.Item );
            return true;
        }
    }
}
