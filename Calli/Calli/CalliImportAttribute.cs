using System;
using System.Security;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Calli
{
    /// <summary>
    /// Calli library import attribute
    /// </summary>
    /// <remarks>
    /// By GaraQuor (.aka Trystan Delhaye) - 16/07/2018
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false), SuppressUnmanagedCodeSecurity]
    public class CalliImportAttribute : Attribute
    {
        /// <summary>
        /// Library name
        /// </summary>
        public string Library { get; set; }
        
        /// <summary>
        /// Entry Point
        /// </summary>
        public string EntryPoint { get; set; }

        /// <summary>
        /// Params pass mode (default: ByRef)
        /// </summary>
        public CalliPassMode PassMode { get; set; }

        /// <summary>
        /// Func is supported
        /// </summary>
        public bool Supported { get; private set; }

        /// <summary>
        /// Lock object
        /// </summary>
        private object LockObject;

        /// <summary>
        /// Constructor
        /// </summary>
        public CalliImportAttribute(string libraryName)
        {
            Library = libraryName;
            EntryPoint = null;
            PassMode = CalliPassMode.ByValue;
            
            Supported = false;
            LockObject = new object();
        }

        /// <summary>
        /// Load delegate
        /// </summary>
        public void LoadDelegate( FieldInfo _field, IntPtr _libraryHandle )
        {
            lock( LockObject )
            {
                if( string.IsNullOrEmpty( EntryPoint ) )
                    EntryPoint = _field.Name;

                IntPtr procAddress = GetProcAddress(_libraryHandle, EntryPoint);

                if( procAddress != IntPtr.Zero )
                {
                    Delegate _delegate = GenerateDelegate(_field, procAddress);
                    _field.SetValue(null, _delegate);
                    
                    Supported = true;

                    if(CalliImport.Debug)
                        Console.WriteLine( "Methode loaded: " + _field.DeclaringType.Name + "." + _field.Name + "()" );
                }
                else
                    Console.WriteLine(  "Methode: " + _field.DeclaringType.Name + "." + _field.Name + "() not found ! [EntryPoint: " + EntryPoint + ", Library: " + Library + "]" );
            }
        }

        /// <summary>
        /// Generate delegate
        /// </summary>
        private unsafe Delegate GenerateDelegate( FieldInfo _field, IntPtr _procAddress)
        {
            Type delegateType = _field.FieldType;

            MethodInfo methode = delegateType.GetMethods()[ 0 ];
            Type returnType = methode.ReturnType;

            ParameterInfo[] paramsInfo = methode.GetParameters();

            Type[] paramsList = new Type[paramsInfo.Length];
            for( int i = 0; i < paramsList.Length; i++ )
                paramsList[ i ] = paramsInfo[ i ].ParameterType;

            DynamicMethod dynamicMethode = new DynamicMethod(EntryPoint + "_" + Library, returnType, paramsList, methode.Module);

            // Gets a ILGenerator, used to send the required IL
            ILGenerator IL = dynamicMethode.GetILGenerator();

            for(int i = 0; i < paramsList.Length; i++ )
            {
                if(PassMode == CalliPassMode.ByValue )
                    IL.Emit( OpCodes.Ldarg, i );
                else
                    IL.Emit( OpCodes.Ldarga, i );
            }

            if( IntPtr.Size == 4 )
                IL.Emit( OpCodes.Ldc_I4, _procAddress.ToInt32() );
            else
            if( IntPtr.Size == 8 )
                IL.Emit( OpCodes.Ldc_I8, _procAddress.ToInt64() );
            else
                throw new PlatformNotSupportedException();

            //Calli
            IL.EmitCalli( OpCodes.Calli, CallingConvention.StdCall, returnType, paramsList );

            // The return value
            IL.Emit( OpCodes.Ret );

            return dynamicMethode.CreateDelegate( delegateType );
        }

        #region Kernel32 funcs

        [DllImport( "kernel32.dll" )]
        private static extern IntPtr GetProcAddress( IntPtr _libraryHandle, string _entryPoint );

        [DllImport( "opengl32.dll" )]
        private static extern IntPtr wglGetProcAddress( string _entryPoint );

        #endregion
    }

    /// <summary>
    /// Params pass mode
    /// </summary>
    public enum CalliPassMode
    {
        ByValue = 0x0001,
        ByRef = 0x0002
    }
}
