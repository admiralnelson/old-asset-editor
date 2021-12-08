﻿using CommonControls.Common;
using CommonControls.FileTypes.RigidModel;
using CommonControls.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using View3D.SceneNodes;
using static CommonControls.FilterDialog.FilterUserControl;

namespace KitbasherEditor.ViewModels.SceneExplorerNodeViews
{
    public class MainEditableNodeViewModel : NotifyPropertyChangedImpl, ISceneNodeViewModel
    {
        MainEditableNode _mainNode;
        SkeletonAnimationLookUpHelper _skeletonAnimationLookUpHelper;
        AnimationControllerViewModel _animationControllerViewModel;
        PackFileService _pf;

        public ObservableCollection<string> SkeletonNameList { get; set; }

        string _skeletonName;
        public string SkeletonName { get { return _skeletonName; } set { SetAndNotify(ref _skeletonName, value); UpdateSkeletonName(); } }
        public OnSeachDelegate FilterByFullPath { get { return (item, expression) => { return expression.Match(item.ToString()).Success; }; } }


        public ObservableCollection<RmvVersionEnum> PossibleOutputFormats { get; set; } = new ObservableCollection<RmvVersionEnum>();

        RmvVersionEnum _selectedOutputFormat;
        public RmvVersionEnum SelectedOutputFormat { get => _selectedOutputFormat; set { SetAndNotify(ref _selectedOutputFormat, value); _mainNode.SelectedOutputFormat = value; } }


        public MainEditableNodeViewModel(MainEditableNode mainNode, SkeletonAnimationLookUpHelper skeletonAnimationLookUpHelper, AnimationControllerViewModel animationControllerViewModel, PackFileService pf)
        {
            _mainNode = mainNode;
            _skeletonAnimationLookUpHelper = skeletonAnimationLookUpHelper;
            _animationControllerViewModel = animationControllerViewModel;
            _pf = pf;

            SkeletonNameList = _skeletonAnimationLookUpHelper.SkeletonFileNames;
            if (_mainNode.Model != null)
            {
                SkeletonName = SkeletonNameList.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).ToLower() == _mainNode.Model.Header.SkeletonName.ToLower());
                UpdateSkeletonName();
            }

            SetCurrentOuputFormat(_mainNode.SelectedOutputFormat);
        }

        public void SetCurrentOuputFormat(RmvVersionEnum format)
        {
            SelectedOutputFormat = format;

            PossibleOutputFormats.Clear();
            if (SelectedOutputFormat == RmvVersionEnum.RMV2_V6)
            {
                PossibleOutputFormats.Add(RmvVersionEnum.RMV2_V6);
                PossibleOutputFormats.Add(RmvVersionEnum.RMV2_V7);
                PossibleOutputFormats.Add(RmvVersionEnum.RMV2_V8);
            }
            else if (SelectedOutputFormat == RmvVersionEnum.RMV2_V7 || SelectedOutputFormat == RmvVersionEnum.RMV2_V8)
            {
                PossibleOutputFormats.Add(RmvVersionEnum.RMV2_V6);
                PossibleOutputFormats.Add(RmvVersionEnum.RMV2_V7);
                PossibleOutputFormats.Add(RmvVersionEnum.RMV2_V8);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        void UpdateSkeletonName()
        {
            string cleanName = "";
            if (!string.IsNullOrWhiteSpace(SkeletonName))
                cleanName = Path.GetFileNameWithoutExtension(SkeletonName);

            if (_mainNode.Model != null)
                SetSkeletonName(_mainNode, cleanName);

            _animationControllerViewModel.SetActiveSkeleton(cleanName);   
        }

        void SetSkeletonName(MainEditableNode node, string skeletonName)
        {
            var header = node.Model.Header;
            header.SkeletonName = skeletonName;
            node.Model.Header = header;
        }

        public void CopyTexturesToOutputPack()
        {
            var meshes = _mainNode.GetMeshesInLod(0, false);


        }

        public void Dispose()
        {
            _skeletonAnimationLookUpHelper = null;
            _mainNode = null;
        }

     
    }
}
