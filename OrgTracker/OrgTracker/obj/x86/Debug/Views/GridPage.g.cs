﻿#pragma checksum "C:\Users\swick\Documents\IgniteDemos\OrgTracker\OrgTracker\Views\GridPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "092B0F2F348BFDA134C1FCA542F891FA"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrgTracker
{
    partial class GridPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.16.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1: // Views\GridPage.xaml line 13
                {
                    this.grid = (global::Telerik.UI.Xaml.Controls.Grid.RadDataGrid)(target);
                    ((global::Telerik.UI.Xaml.Controls.Grid.RadDataGrid)this.grid).SelectionChanged += this.grid_SelectionChanged;
                }
                break;
            case 2: // Views\GridPage.xaml line 24
                {
                    this.employeeView = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 3: // Views\GridPage.xaml line 27
                {
                    global::Windows.UI.Xaml.Controls.SymbolIcon element3 = (global::Windows.UI.Xaml.Controls.SymbolIcon)(target);
                    ((global::Windows.UI.Xaml.Controls.SymbolIcon)element3).Tapped += this.SymbolIcon_Tapped_1;
                }
                break;
            case 4: // Views\GridPage.xaml line 28
                {
                    global::Windows.UI.Xaml.Controls.SymbolIcon element4 = (global::Windows.UI.Xaml.Controls.SymbolIcon)(target);
                    ((global::Windows.UI.Xaml.Controls.SymbolIcon)element4).Tapped += this.SymbolIcon_Tapped;
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.16.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

