﻿#pragma checksum "..\..\MainWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "5FA908A13FE67074FEA357B58C98C8D0906B1495"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using EXEIntegrator;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace EXEIntegrator {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 1 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal EXEIntegrator.MainWindow Window;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image Background;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image CloseButton;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image MinimizeButton;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image IntegrateButton;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox IntegrationPathTextbox;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/EXEIntegrator;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.Window = ((EXEIntegrator.MainWindow)(target));
            return;
            case 2:
            this.Background = ((System.Windows.Controls.Image)(target));
            
            #line 13 "..\..\MainWindow.xaml"
            this.Background.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Background_MouseDown);
            
            #line default
            #line hidden
            return;
            case 3:
            this.CloseButton = ((System.Windows.Controls.Image)(target));
            
            #line 14 "..\..\MainWindow.xaml"
            this.CloseButton.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.CloseButton_MouseLeftButtonUp);
            
            #line default
            #line hidden
            return;
            case 4:
            this.MinimizeButton = ((System.Windows.Controls.Image)(target));
            
            #line 15 "..\..\MainWindow.xaml"
            this.MinimizeButton.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.MinimizeButton_MouseLeftButtonUp);
            
            #line default
            #line hidden
            return;
            case 5:
            this.IntegrateButton = ((System.Windows.Controls.Image)(target));
            
            #line 16 "..\..\MainWindow.xaml"
            this.IntegrateButton.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.IntegrateButton_MouseLeftButtonUp);
            
            #line default
            #line hidden
            
            #line 16 "..\..\MainWindow.xaml"
            this.IntegrateButton.MouseEnter += new System.Windows.Input.MouseEventHandler(this.IntegrateButton_MouseEnter);
            
            #line default
            #line hidden
            
            #line 16 "..\..\MainWindow.xaml"
            this.IntegrateButton.MouseLeave += new System.Windows.Input.MouseEventHandler(this.IntegrateButton_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 6:
            this.IntegrationPathTextbox = ((System.Windows.Controls.TextBox)(target));
            
            #line 17 "..\..\MainWindow.xaml"
            this.IntegrationPathTextbox.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.IntegrationPathTextbox_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

