using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using TracerRenderer.Properties;

namespace TracerRenderer
{
    public class Shader
    {
        private static Shader defaultShader;
        private readonly Dictionary<string, int> uniformLocations = new Dictionary<string, int>( );
         
        public static Shader Default
        {
            get
            {
                if ( defaultShader != null ) return defaultShader;


                string vertexShader, fragmentShader;

                using ( TextReader read = new StreamReader( new MemoryStream( Resources.default_vert ) ) )
                    vertexShader = read.ReadToEnd( );

                using ( TextReader read = new StreamReader( new MemoryStream( Resources.default_frag ) ) )
                    fragmentShader = read.ReadToEnd( );

                defaultShader = new Shader( );
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

            GL.GetShader( shaderID, ShaderParameter.CompileStatus, out isCompiled );
            if ( isCompiled == 1 ) return true;

            string logInfo;
            GL.GetShaderInfoLog( shaderID, out logInfo );
            Console.WriteLine( "Error compiling " + type + " shader: " + logInfo );
            return false;
        }

        public void Link( )
        {
            GL.LinkProgram( programID );
        }

        public void Use( )
        {
            GL.UseProgram( programID );
        }

        public int GetAttributeLocation( string name )
        {
            return GL.GetAttribLocation( programID, name );
        }

        public void BindAttributeLocation( string name, int location )
        {
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
            int loc = GetUniformLocation( name );
            GL.UniformMatrix4( loc, false, ref matrix );
        }
    }
}
