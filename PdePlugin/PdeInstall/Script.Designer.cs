﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PdeInstall {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Script {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Script() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PdeInstall.Script", typeof(Script).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Windows Registry Editor Version 5.00
        ///
        ///[HKEY_CURRENT_USER\Software\Microsoft\Office\Excel\Addins\PdePlugin]
        ///&quot;Description&quot;=&quot;PdePlugin&quot;
        ///&quot;FriendlyName&quot;=&quot;PdePlugin&quot;
        ///&quot;LoadBehavior&quot;=dword:00000003
        ///&quot;Manifest&quot;=&quot;{0}&quot;.
        /// </summary>
        internal static string InstallScript {
            get {
                return ResourceManager.GetString("InstallScript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Windows Registry Editor Version 5.00.
        /// </summary>
        internal static string String {
            get {
                return ResourceManager.GetString("String", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [HKEY_CURRENT_USER\Software\Microsoft\Office\Excel\Addins\PdePlugin].
        /// </summary>
        internal static string String1 {
            get {
                return ResourceManager.GetString("String1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Windows Registry Editor Version 5.00
        ///
        ///[-HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office\12.0\User Settings\PDW]
        ///
        ///[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office\12.0\User Settings\PDW]
        ///&quot;Count&quot;= dword:00000002
        ///
        ///[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office\12.0\User Settings\PDE\Delete]
        ///
        ///[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office\12.0\User Settings\PDE\Delete\Software]
        ///
        ///[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office\12.0\User Settings\PDE\Delete\Software\Microsoft]
        ///
        ///[HKEY_LOCAL_MACHINE\SOFTWARE\Mic [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string UninstallScript {
            get {
                return ResourceManager.GetString("UninstallScript", resourceCulture);
            }
        }
    }
}