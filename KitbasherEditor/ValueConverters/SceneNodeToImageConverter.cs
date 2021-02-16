﻿using CommonControls.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using View3D.Components.Component;

namespace KitbasherEditor.ValueConverters
{
    [ValueConversion(typeof(SceneNode), typeof(BitmapImage))]
    public class SceneNodeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value is VariantMeshNode)
                return ResourceController.VmdIcon;
            else if (value is Rmv2ModelNode)
                return ResourceController.Rmv2ModelIcon;
            else if (value is MeshNode)
                return ResourceController.MeshIcon;
            if (value is GroupNode)
                return ResourceController.GroupIcon;

            throw new Exception("Unknown type " + value.GetType().FullName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}