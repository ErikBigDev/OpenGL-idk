using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program
{
    private static void Main()
    {
        using (var window = new Window(800, 600, "OpenTK"))
        {
            window.Run(60.0);
        }
    }
}
