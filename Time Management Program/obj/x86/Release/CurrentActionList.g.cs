﻿

#pragma checksum "C:\Users\~Archi~\Desktop\Курсовая работа\Time Management Program\Time Management Program\CurrentActionList.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A26005E06F32F202E851F4A639FF4851"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time_Management_Program
{
    partial class CurrentActionList : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 45 "..\..\..\CurrentActionList.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.tempClearListButton_Click;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 46 "..\..\..\CurrentActionList.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.addActionButton_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 47 "..\..\..\CurrentActionList.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.deleteActionButton_Click;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 40 "..\..\..\CurrentActionList.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.addActionReadyToAddButton_Click;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 24 "..\..\..\CurrentActionList.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.GoBack;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


