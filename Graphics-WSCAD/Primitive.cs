using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Graphics_WSCAD
{
  /// <summary>
  /// Class Primitive that rappresent the graphic group of info in the json
  /// </summary>
  class Primitive
  {
    public readonly string type;
    public readonly PointF[] points;
    public readonly Color color;
    public readonly DashStyle lineType;
    public readonly float? radius;
    public readonly bool? filled;

    /// <summary>
    /// Constructor of tthe class where i deserialize the json in Primitive class
    /// </summary>
    public Primitive(dynamic json)
    {
      this.type = json.type.Value;

      int[] colorargb = GetColor(json.color.Value);
      this.color = Color.FromArgb(colorargb[0], colorargb[1], colorargb[2], colorargb[3]);

      switch (json.lineType.Value.ToLower())
      {
        case "solid":
          this.lineType = System.Drawing.Drawing2D.DashStyle.Solid;
          break;
        case "dot":
          this.lineType = System.Drawing.Drawing2D.DashStyle.Dot;
          break;
        case "dashdot":
          this.lineType = System.Drawing.Drawing2D.DashStyle.DashDot;
          break;
      }
      PointF[] pointlist;
      switch (json.type.Value)
      {
        case "line":
          PointF a = GetPointF(json.a.Value, false);
          PointF b = GetPointF(json.b.Value, false);
          this.points = new PointF[2] { a, b };
          this.filled = null;
          this.radius = null;
          break;
        case "circle":
          string[] centerpoints = json.center.Value.Split(';');
          PointF centercirccle = new PointF(int.Parse(centerpoints[0]), int.Parse(centerpoints[1]));
          this.points = new PointF[1] { centercirccle };
          this.filled = (bool)json.filled.Value;
          this.radius = (float)json.radius.Value;
          break;
        case "triangle":
          pointlist = new PointF[] { GetPointF(json.a.Value, false), GetPointF(json.b.Value, false), GetPointF(json.c.Value, false) };
          this.points = pointlist;
          this.filled = (bool)json.filled.Value;
          this.radius = null;
          break;
        case "rectangle":
          pointlist = new PointF[] { GetPointF(json.a.Value, false), GetPointF(json.b.Value, false), GetPointF(json.c.Value, false), GetPointF(json.d.Value, false) };
          this.points = pointlist;
          this.filled = (bool)json.filled.Value;
          this.radius = null;
          break;
      }
    }

    /// <summary>
    /// Constructor of the class where i deserialize the xml in Primitive class
    /// </summary>
    public Primitive(XmlNode node)
    {
      List<PointF> list = new List<PointF>();
      foreach (XmlNode child in node.ChildNodes)
      {
        switch (child.Name)
        {
          case "type":
            this.type = child.InnerText;
            break;
          case "color":
            int[] colorargb = GetColor(child.InnerText);
            this.color = Color.FromArgb(colorargb[0], colorargb[1], colorargb[2], colorargb[3]);
            break;
          case "lineType":
            switch (child.InnerText.ToLower())
            {
              case "solid":
                this.lineType = System.Drawing.Drawing2D.DashStyle.Solid;
                break;
              case "dot":
                this.lineType = System.Drawing.Drawing2D.DashStyle.Dot;
                break;
              case "dashdot":
                this.lineType = System.Drawing.Drawing2D.DashStyle.DashDot;
                break;
            }
            break;
          case "center":
            PointF center = GetPointF(child.InnerText, false);
            list.Add(center);
            break;
          case "filled":
            this.filled = bool.Parse(child.InnerText);
            break;
          case "radius":
            this.radius = float.Parse(child.InnerText);
            break;
          case "a":
          case "b":
          case "c":
          case "d":
            PointF a = GetPointF(child.InnerText, false);
            list.Add(a);
            break;
        }
      }
      this.points = list.ToArray();
    }

    /// <summary>
    /// Deserialize color property value in a array of int of 4 elements for argb
    /// </summary>
    private int[] GetColor(string colorstring)
    {
      int[] colorargb = new int[4];
      int i = 0;
      foreach (string s in colorstring.Split(';'))
      {
        int tmp = 0;
        if (int.TryParse(s.Trim(), out tmp) && i < 4)
        {
          colorargb[i] = tmp;
          i += 1;
        }
      }
      return colorargb;
    }

    /// <summary>
    /// get point location in the form
    /// </summary>
    private PointF GetPointF(string coordinates, bool withcoef = true)
    {
      string[] list = coordinates.Split(';');
      float x = float.Parse(list[0]), y = float.Parse(list[1]);
      return new PointF(x, y);
    }
  }
}
