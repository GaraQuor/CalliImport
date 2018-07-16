using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Calli
{
    /// <summary>
    /// Load native library with Calli methode
    /// </summary>
    /// <remarks>
    /// By GaraQuor (.aka Trystan Delhaye) 14/07/2018
    /// </remarks>
    public class CalliImport
    {
        /// <summary>
        /// Debug library calli
        /// </summary>
        public static bool Debug { get; set; }

        /// <summary>
        /// Lock object
        /// </summary>
        private static object LockObject;

        /// <summary>
        /// Loaded library list
        /// </summary>
        private static Dictionary<string, IntPtr> LibraryList;

        /// <summary>
        /// Constructor
        /// </summary>
        static CalliImport()
        {
            Debug = true;

            LockObject = new object();
            LibraryList = new Dictionary<string, IntPtr>();
        }

        /// <summary>
        /// Load all entry point in library
        /// </summary>
        public static void LoadMethods( string _library )
        {
            if( string.IsNullOrEmpty( _library ) )
                throw new ArgumentNullException( "Library is null or empty" );

            lock( LockObject )
            {
                IntPtr libraryHandle = TryLoadLibrary( _library );

                FieldInfo[] fieldList = GetCalliFields();
                for( int i = 0; i < fieldList.Length; i++ )
                {
                    CalliImportAttribute calliAttribute = (CalliImportAttribute)fieldList[i].GetCustomAttribute( typeof( CalliImportAttribute ), false );

                    if( calliAttribute.Library == _library && !calliAttribute.Supported )
                        calliAttribute.LoadDelegate( fieldList[ i ], libraryHandle );
                }
            }
        }

        /// <summary>
        /// Load all entry points
        /// </summary>
        public static void LoadMethods()
        {
            lock( LockObject )
            {
                FieldInfo[] fieldList = GetCalliFields();
                for( int i = 0; i < fieldList.Length; i++ )
                {
                    CalliImportAttribute calliAttribute = (CalliImportAttribute)fieldList[i].GetCustomAttribute( typeof( CalliImportAttribute ), false );

                    //Load library
                    if( string.IsNullOrEmpty( calliAttribute.Library ) )
                        throw new CalliImportException( fieldList[ i ], "Library is null or empty" );

                    IntPtr libraryHandle = TryLoadLibrary( calliAttribute.Library );

                    if( !calliAttribute.Supported )
                        calliAttribute.LoadDelegate( fieldList[ i ], libraryHandle );
                }
            }
        }

        /// <summary>
        /// Load library
        /// </summary>
        public static void LoadLibrary( string _library )
        {
            if( string.IsNullOrEmpty( _library ) )
                throw new ArgumentNullException( "Library is null or empty" );

            lock( LockObject )
            {
                TryLoadLibrary( _library );
            }
        }

        /// <summary>
        /// Get fields contains Calli import attribute
        /// </summary>
        private static FieldInfo[] GetCalliFields()
        {
            List<FieldInfo> results = new List<FieldInfo>();

            Assembly[] assemblyList = AppDomain.CurrentDomain.GetAssemblies();
            for( int i = 0; i < assemblyList.Length; i++ )
            {
                Type[] typeList = assemblyList[ i ].GetTypes();
                for( int j = 0; j < typeList.Length; j++ )
                {
                    FieldInfo[] fieldList = typeList[j].GetFields();
                    for( int h = 0; h < fieldList.Length; h++ )
                    {
                        Attribute attribute = fieldList[ h ].GetCustomAttribute( typeof( CalliImportAttribute ), false );

                        if( attribute != null )
                            results.Add( fieldList[ h ] );
                    }
                }
            }

            return results.ToArray();
        }

        /// <summary>
        /// Load library
        /// </summary>
        private static IntPtr TryLoadLibrary( string _library )
        {
            IntPtr libraryHandle = IntPtr.Zero;

            //Check library already loaded
            LibraryList.TryGetValue( _library, out libraryHandle );

            if( libraryHandle != IntPtr.Zero )
                return libraryHandle;

            //try load library from kernel32
            libraryHandle = KernelLoadLibrary( _library );

            if( libraryHandle == IntPtr.Zero )
                throw new DllNotFoundException( "Library: " + _library + " not found !" );

            //Add to dictionary
            if( !LibraryList.ContainsKey( _library ) )
                LibraryList.Add( _library, libraryHandle );

            if( Debug )
                Console.WriteLine( "Library loaded: " + _library );

            return libraryHandle;
        }

        #region Kernel32 funcs

        [DllImport( "kernel32.dll", EntryPoint = "LoadLibrary" )]
        private static extern IntPtr KernelLoadLibrary( string _libraryName );
        
        [DllImport( "kernel32.dll", EntryPoint = "FreeLibrary" )]
        private static extern bool KernelFreeLibrary( IntPtr _libraryHandle );

        #endregion
    }

    #region Calli delegates

    public delegate void CalliMethode();
    public delegate void CalliMethode<in T1>( T1 arg1 );
    public delegate void CalliMethode<in T1, in T2>( T1 arg1, T2 arg2 );
    public delegate void CalliMethode<in T1, in T2, in T3>( T1 arg1, T2 arg2, T3 arg3 );
    public delegate void CalliMethode<in T1, in T2, in T3, in T4>( T1 arg1, T2 arg2, T3 arg3, T4 arg4 );
    public delegate void CalliMethode<in T1, in T2, in T3, in T4, in T5>( T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5 );
    public delegate void CalliMethode<in T1, in T2, in T3, in T4, in T5, in T6>( T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6 );
    public delegate void CalliMethode<in T1, in T2, in T3, in T4, in T5, in T6, in T7>( T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7 );
    public delegate void CalliMethode<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>( T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8 );

    public delegate TResult CalliMethodeType<out TResult>();
    public delegate TResult CalliMethodeType<out TResult, in T1>( T1 arg1 );
    public delegate TResult CalliMethodeType<out TResult, in T1, in T2>( T1 arg1, T2 arg2 );
    public delegate TResult CalliMethodeType<out TResult, in T1, in T2, in T3>( T1 arg1, T2 arg2, T3 arg3 );
    public delegate TResult CalliMethodeType<out TResult, in T1, in T2, in T3, in T4>( T1 arg1, T2 arg2, T3 arg3, T4 arg4 );
    public delegate TResult CalliMethodeType<out TResult, in T1, in T2, in T3, in T4, in T5>( T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5 );
    public delegate TResult CalliMethodeType<out TResult, in T1, in T2, in T3, in T4, in T5, in T6>( T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6 );
    public delegate TResult CalliMethodeType<out TResult, in T1, in T2, in T3, in T4, in T5, in T6, in T7>( T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7 );
    public delegate TResult CalliMethodeType<out TResult, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>( T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8 );

    #endregion
}
