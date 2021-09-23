using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace window_render
{
    class WindowRender
    {
        uint width = 1280;
        uint height = 800;
        Clock clock = new Clock();
        int refreshRate = 60;
        float totalElapsedTime = 0f;
        int fourierIndex = 1;
        int graphItensLimit = 500;

        public void Run()
        {
            var mode = new VideoMode(this.width, this.height);
            RenderWindow window = new RenderWindow(mode, "Fourier Series Visualization");
            window.KeyPressed += Window_KeyPressed;
            window.Closed += (_, __) => window.Close();

            var background = new RectangleShape(new Vector2f(width, height))
            {
                FillColor = Color.Black,
                Position = new Vector2f(0, 0)
            };

            IEnumerable<float> pointsList = new List<float>();

            Font robotoFont = new Font("Roboto-Regular.ttf");
            // Start the game loop
            while (window.IsOpen)
            {
                // Process events
                window.DispatchEvents();

                // Program logic respects the refresh rate
                if (clock.ElapsedTime.AsMilliseconds() > 1000 / this.refreshRate)
                {
                    clock.Restart();
                    window.Draw(background);


                    Text tutorialTxt = new Text($"Use the arrow keys to increase/decrease number of cicles", robotoFont)
                    {
                        FillColor = Color.White,
                        Position = new Vector2f(10, 30)
                    };
                    window.Draw(tutorialTxt);

                    Text fourierIdxTxt = new Text($"Cicles {fourierIndex.ToString()}", robotoFont)
                    {
                        FillColor = Color.White,
                        Position = new Vector2f(width - 200, 30)
                    };
                    window.Draw(fourierIdxTxt);

                    Vector2f currentPos = new Vector2f(width / 4, height / 2);
                    for (int i = 0; i < this.fourierIndex; i++)
                    {
                        currentPos = this.drawCircle(in window, currentPos, i);
                    }

                    VertexArray conectLine = new VertexArray(PrimitiveType.LineStrip);
                    conectLine.Append(new Vertex(currentPos));
                    conectLine.Append(new Vertex(new Vector2f(width / 2, currentPos.Y)));

                    window.Draw(conectLine);

                    pointsList = pointsList.Prepend(currentPos.Y);

                    if (pointsList.Count() > this.graphItensLimit)
                    {
                        pointsList = pointsList.Take(this.graphItensLimit);
                    }

                    VertexArray graphShape = new VertexArray(PrimitiveType.LineStrip);

                    uint idx = 0;
                    foreach (float pt in pointsList)
                    {
                        Vector2f currentDrawingPoint = new Vector2f(width / 2 + idx, pt);
                        graphShape.Append(new Vertex(currentDrawingPoint));

                        idx++;
                    }

                    window.Draw(graphShape);

                    totalElapsedTime += 0.05f;

                    // Finally, display the rendered frame on screen
                    window.Display();
                }

            }
        }

        /// <summary>
        /// Function called when a key is pressed
        /// </summary>
        private void Window_KeyPressed(object sender, KeyEventArgs e)
        {
            var window = (Window)sender;
            if (e.Code == Keyboard.Key.Escape)
            {
                window.Close();
            }
            else if (e.Code == Keyboard.Key.Up)
            {
                if (this.fourierIndex < 3000)
                {
                    this.fourierIndex++;
                }
            }
            else if (e.Code == Keyboard.Key.Down)
            {
                if (this.fourierIndex > 0)
                {
                    this.fourierIndex--;
                }
            }
        }

        private Vector2f drawCircle(in RenderWindow window, Vector2f prevPos, int idx, bool isLast = false)
        {
            int n = idx * 2 + 1;
            double currentRadius = 75 * (4 / (n * Math.PI));

            double currentXpos = prevPos.X + (currentRadius * Math.Cos(-n * this.totalElapsedTime));
            double currentYpos = prevPos.Y + (currentRadius * Math.Sin(-n * this.totalElapsedTime));
            Vector2f currentPos = new Vector2f(
                    (float)currentXpos,
                    (float)currentYpos
            );

            var innerCircle = new CircleShape((float)currentRadius)
            {
                Origin = new Vector2f((float)currentRadius, (float)currentRadius),
                FillColor = Color.Transparent,
                OutlineColor = Color.White,
                OutlineThickness = 2,
                Position = new Vector2f(
                    (float)(prevPos.X),
                    (float)(prevPos.Y)
                )
            };

            VertexArray valueLine = new VertexArray(PrimitiveType.LineStrip);
            valueLine.Append(new Vertex(currentPos));
            valueLine.Append(new Vertex(prevPos));

            window.Draw(valueLine);
            window.Draw(innerCircle);

            return currentPos;
        }
    }
}
