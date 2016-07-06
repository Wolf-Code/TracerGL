using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using TracerRenderer.Data;

namespace TracerRenderer
{
    /// <summary>
    /// A shader that is run on the GPU.
    /// </summary>
    public class Shader
    {
        private readonly Dictionary<string, int> uniformLocations = new Dictionary<string, int>( );
        private readonly List<int> addedShaders = new List<int>( );

        private static int inUseShader;
            
        /// <summary>
        /// The default shader to use when no shader has been set.
        /// </summary>
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

        /// <summary>
        /// Constructor, creates a new shader program.
        /// </summary>
        public Shader( )
        {
            programID = GL.CreateProgram( );
        }

        /// <summary>
        /// Adds a shader source to the shader.
        /// </summary>
        /// <param name="source">The source code of the shader.</param>
        /// <param name="type">The type of the shader.</param>
        /// <returns>Whether or not the shader was compiled succesfully.</returns>
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

        /// <summary>
        /// Links the created shader program.
        /// </summary>
        public void Link( )
        {
            GL.LinkProgram( programID );
        }

        /// <summary>
        /// Starts using this shader.
        /// </summary>
        public void Use( )
        {
            inUseShader = programID;
            GL.UseProgram( programID );
        }

        /// <summary>
        /// Reverts the shader back to the default.
        /// </summary>
        public static void UseDefault( )
        {
            GL.UseProgram( 0 );
            inUseShader = 0;
        }

        /// <summary>
        /// Gets the location of an attribute.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>The location of the attribute.</returns>
        public int GetAttributeLocation( string name )
        {
            return GL.GetAttribLocation( programID, name );
        }

        /// <summary>
        /// Binds an attribute to a given location.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="location">The location of the attribute.</param>
        public void BindAttributeLocation( string name, int location )
        {
            CheckIfCurrentShader( );
            GL.BindAttribLocation( programID, location, name );
        }

        /// <summary>
        /// Gets the location inside the shader of a uniform.
        /// </summary>
        /// <param name="name">The name of the uniform, as defined in the shader.</param>
        /// <returns>The location of the uniform in the shader.</returns>
        public int GetUniformLocation( string name )
        {
            if ( uniformLocations.ContainsKey( name ) )
                return uniformLocations[ name ];

            int loc = GL.GetUniformLocation( programID, name );
            uniformLocations[ name ] = loc;

            return loc;
        }

        /// <summary>
        /// Sets a matrix inside the shader.
        /// </summary>
        /// <param name="name">The name of the matrix.</param>
        /// <param name="matrix">The matrix.</param>
        public void SetMatrix( string name, Matrix4 matrix )
        {
            CheckIfCurrentShader( );

            int loc = GetUniformLocation( name );
            GL.UniformMatrix4( loc, false, ref matrix );
        }

        /// <summary>
        /// Sets a texture inside the shader.
        /// </summary>
        /// <param name="samplerName">The name of the texture sampler.</param>
        /// <param name="texture">The ID of the texture.</param>
        public void SetTexture( string samplerName, int texture )
        {
            GL.ActiveTexture( TextureUnit.Texture0 + texture );
            GL.BindTexture( TextureTarget.Texture2D, texture );
            GL.Uniform1( GetUniformLocation( samplerName ), texture );
        }

        /// <summary>
        /// Sets a vec4 inside the shader to a given color.
        /// </summary>
        /// <param name="name">The name of the color in the shader.</param>
        /// <param name="color">The color.</param>
        public void SetColor4( string name, Color4 color )
        {
            int loc = GetUniformLocation( name );
            GL.Uniform4( loc, color );
        }

        /// <summary>
        /// Sets a vec4 inside the shader to a given color.
        /// </summary>
        /// <param name="name">The name of the color in the shader.</param>
        /// <param name="color">The color.</param>
        public void SetColor( string name, Color color )
        {
            this.SetColor4( name, color.ToColor4( ) );
        }

        private void CheckIfCurrentShader( )
        {
            if ( programID != inUseShader )
                throw new Exception(
                    "Attempting to use shader, without setting it as the current shader first. Try running Shader.Use( ) before attempting to use it." );
        }
    }
}
