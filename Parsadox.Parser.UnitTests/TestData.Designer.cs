//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Parsadox.Parser.UnitTests {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class TestData {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal TestData() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Parsadox.Parser.UnitTests.TestData", typeof(TestData).Assembly);
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
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] CompressedTextMultiple {
            get {
                object obj = ResourceManager.GetObject("CompressedTextMultiple", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] CompressedTextSingle {
            get {
                object obj = ResourceManager.GetObject("CompressedTextSingle", resourceCulture);
                return ((byte[])(obj));
            }
        }

        internal static string SaveGame {
            get {
                return ResourceManager.GetString("SaveGame", resourceCulture);
            }
        }

        internal static string SaveGame_NodesFilteredFull {
            get {
                return ResourceManager.GetString("SaveGame_NodesFilteredFull", resourceCulture);
            }
        }

        internal static string SaveGame_NodesFilteredMinimal {
            get {
                return ResourceManager.GetString("SaveGame_NodesFilteredMinimal", resourceCulture);
            }
        }

        internal static string SaveGame_NodesFull {
            get {
                return ResourceManager.GetString("SaveGame_NodesFull", resourceCulture);
            }
        }

        internal static string SaveGame_NodesMinimal {
            get {
                return ResourceManager.GetString("SaveGame_NodesMinimal", resourceCulture);
            }
        }

        internal static string SaveGame_Tokens {
            get {
                return ResourceManager.GetString("SaveGame_Tokens", resourceCulture);
            }
        }

        internal static string SaveGame_TokensFiltered {
            get {
                return ResourceManager.GetString("SaveGame_TokensFiltered", resourceCulture);
            }
        }

        internal static string SaveGame_TokensWithComments {
            get {
                return ResourceManager.GetString("SaveGame_TokensWithComments", resourceCulture);
            }
        }
    }
}
