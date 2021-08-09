﻿using AnimationEditor.Common.ReferenceModel;
using AnimationEditor.PropCreator;
using Common;
using CommonControls.Common;
using CommonControls.Services;
using Filetypes.RigidModel;
using Microsoft.Xna.Framework;
using MonoGame.Framework.WpfInterop;
using System.Linq;
using System.Windows;
using View3D.Animation;

namespace AnimationEditor.CampaignAnimationCreator
{
    public class Editor : NotifyPropertyChangedImpl
    {
        public FilterCollection<SkeletonBoneNode> ModelBoneList { get; set; } = new FilterCollection<SkeletonBoneNode>(null);

        AssetViewModel _selectedUnit;
        PackFileService _pfs;
        AnimationClip _selectedAnimationClip;

        public Editor(PackFileService pfs, SkeletonAnimationLookUpHelper skeletonAnimationLookUpHelper, AssetViewModel rider, IComponentManager componentManager)
        {
            _pfs = pfs;
            _selectedUnit = rider;
            _selectedUnit.SkeletonChanged += SkeletonChanged;
            _selectedUnit.AnimationChanged += AnimationChanged;

            SkeletonChanged(_selectedUnit.Skeleton);
            AnimationChanged(_selectedUnit.AnimationClip);
        }

        public void SaveAnimation()
        {
            var animFile = _selectedUnit.AnimationClip.ConvertToFileFormat(_selectedUnit.Skeleton);
            var bytes = AnimationFile.GetBytes(animFile);
            SaveHelper.SaveAs(_pfs, bytes, ".anim");
        }

        public void Convert()
        {
            if (_selectedAnimationClip == null)
            {
                MessageBox.Show("No animation selected");
                return;
            }

            if (ModelBoneList.SelectedItem == null)
            {
                MessageBox.Show("No root bone selected");
                return;
            }

            var newAnimation = _selectedAnimationClip.Clone();
            newAnimation.MergeStaticAndDynamicFrames();

            for (int frameIndex = 0; frameIndex < newAnimation.DynamicFrames.Count; frameIndex++)
            {
                var frame = newAnimation.DynamicFrames[frameIndex];
                frame.Position[ModelBoneList.SelectedItem.BoneIndex] = Vector3.Zero;
                frame.Rotation[ModelBoneList.SelectedItem.BoneIndex] = Quaternion.Identity;
            }

            _selectedUnit.AnimationChanged -= AnimationChanged;
            _selectedUnit.SetAnimationClip(newAnimation, new SkeletonAnimationLookUpHelper.AnimationReference("Generated animation", null));
            _selectedUnit.AnimationChanged += AnimationChanged;
        }

        private void AnimationChanged(AnimationClip newValue)
        {
            _selectedAnimationClip = newValue;
        }

        private void SkeletonChanged(GameSkeleton newValue)
        {
            if (newValue == null)
            {
                ModelBoneList.UpdatePossibleValues(null);
            }
            else
            {
                ModelBoneList.UpdatePossibleValues(SkeletonHelper.CreateFlatSkeletonList(newValue));
                ModelBoneList.SelectedItem = ModelBoneList.PossibleValues.FirstOrDefault(x => x.BoneName.ToLower() == "animroot");
            }
        }
    }
}
