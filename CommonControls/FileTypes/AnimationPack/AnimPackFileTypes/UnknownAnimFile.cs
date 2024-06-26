﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommonControls.Common;

namespace CommonControls.FileTypes.AnimationPack.AnimPackFileTypes
{
    public class UnknownAnimFile : IAnimationPackFile
    {
        public AnimationPackFile Parent { get; set; }
        public string FileName { get; set; }
        public bool IsUnknownFile { get; set; } = true;
        public NotifyAttr<bool> IsChanged { get; set; } = new NotifyAttr<bool>(false);

        byte[] _data;

        public UnknownAnimFile(string fileName, byte[] data)
        {
            FileName = fileName;
            _data = data;
        }

        public void CreateFromBytes(byte[] bytes)
        {
            _data = bytes;
        }

        public byte[] ToByteArray()
        {
            return _data;
        }
    }
}
