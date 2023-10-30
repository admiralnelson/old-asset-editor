﻿<UserControl x:Class="AssetManagement.Strategies.Fbx.ExportDIalog.UserControls.ExportPanelUserControl"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
             xmlns:local="clr-namespace:AssetManagement.Strategies.Fbx.ExportDIalog.UserControls"
             mc:Ignorable="d" 
             d:DesignHeight="450" d:DesignWidth="800">
    <Grid>
        <Expander Name="{Binding ExpanderName}" Header="File Info" ExpandDirection="Down" IsExpanded="True">

            <DockPanel Name="DockPanel" LastChildFill="true">

                <GroupBox x:Name="DefaultGroupBox" Visibility="{Binding AnimationPanelVisibility}">
                </GroupBox>

            </DockPanel>
        </Expander>
    </Grid>
</UserControl>