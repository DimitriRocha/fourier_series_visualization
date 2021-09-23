using SFML.Window;
using window_render;

namespace fourier_series_sfml
{
    class Program
    {
        static void Main(string[] args)
        {
            var window = new WindowRender();
            window.Run();
        }
    }
}
