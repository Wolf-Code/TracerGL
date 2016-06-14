using OpenTK;

namespace TracerGL
{
    class Program
    {
        static void Main( string[ ] args )
        {
            using ( GameWindow w = new Window( ) )
                w.Run( 60 );
        }
    }
}
