using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TracerRenderer
{
    public class Shader
    {
        private readonly Dictionary<string, int> uniformLocations = new Dictionary<string, int>( );
        private readonly List<int> addedShaders = new List<int>( );

        private static int inUseShader = 0;
            
        public static Shader Default
        {
            get
            {
                string vertexShader = File.ReadAllText( "Shaders/default.vert" );
                string fragmentShader = File.ReadAllText( "Shaders/default.frag" );

                Shader defaultShader = new Shader( );
                defaultShader.AddShader( vertexShader, ShaderType.VertexShader );
                defaultShader.AddShader( fragmentShader, ShaderType.FragmentShader );
                defaultShader.Link( );

                return defaultShader;
            }
        }

        private readonly int programID;

        public Shader( )
        {
            programID = GL.CreateProgram( );
        }

        public bool AddShader( string source, ShaderType type )
        {
            int shaderID = GL.CreateShader( type );
            int isCompiled;

            GL.AttachShader( programID, shaderID );
            GL.ShaderSource( shaderID, source );
            GL.CompileShader( shaderID );
            addedShaders.Add( shaderID );

            GL.GetShader( shaderID, ShaderParameter.CompileStatus, out isCompiled );
            if ( isCompiled == 1 ) return true;

            string logInfo;
            GL.GetShaderInfoLog( shaderID, out logInfo );
            Console.WriteLine( "Error compiling " + type + " shader: " + logInfo );

            return false;
        }

        private void RemoveShaders( )
        {
            foreach ( int shader in addedShaders )
                GL.DeleteShader( shader );
        }

        public void Link( )
        {
            GL.LinkProgram( programID );
        }

        public void Use( )
        {
            inUseShader = programID;
            GL.UseProgram( programID );
        }

        public static void UseDefault( )
        {
            GL.UseProgram( 0 );
            inUseShader = 0;
        }

        public int GetAttributeLocation( string name )
        {
            return GL.GetAttribLocation( programID, name );
        }

        public void BindAttributeLocation( string name, int location )
        {
            CheckIfCurrentShader( );
            GL.BindAttribLocation( programID, location, name );
        }

        public int GetUniformLocation( string name )
        {
            if ( uniformLocations.ContainsKey( name ) )
                return uniformLocations[ name ];

            int loc = GL.GetUniformLocation( programID, name );
            uniformLocations[ name ] = loc;

            return loc;
        }

        public void SetMatrix( string name, Matrix4 matrix )
        {
            CheckIfCurrentShader( );

            int loc = GetUniformLocation( name );
            GL.UniformMatrix4( loc, false, ref matrix );
        }

        public void SetTexture( string samplerName, int texture )
        {
            GL.ActiveTexture( TextureUnit.Texture0 + texture );
            GL.BindTexture( TextureTarget.Texture2D, texture );
            GL.Uniform1( GetUniformLocation( samplerName ), texture );
        }

        private void CheckIfCurrentShader( )
        {
            if ( programID != inUseShader )
                throw new Exception(
                    "Attempting to use shader, without setting it as the current shader first. Try running Shader.Use( ) before attempting to use it." );
        }
    }
}
