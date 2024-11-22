using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FractalFlair.Models;

/// <summary>
///   Insert your own color maps here. Simply an array of colors.
///   The pixel's iteration count indicates the color by modding
///   the length of this list and getting the remainder as the
///   color index.
/// </summary>
public class ColorMap //: ICloneable
{
  public ColorMap(string name, int gradations, IEnumerable<Color> map)
  {
    Name = name;
    Map = map.ToArray();
    Gradations = gradations;
  }

  public int Gradations { get; }
  public Color[] Map { get; }
  public string Name { get; }

  public ColorMap Clone()
  {
    return new ColorMap(Name, Gradations, Map);
  }
}