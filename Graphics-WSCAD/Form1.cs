using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using System.Xml;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Drawing.Drawing2D;

namespace Graphics_WSCAD
{
  public partial class Form1 : Form
  {
    List<Primitive> jsondata;
    int scrwidth;
    int scrheight;
    PointF center;
    float proportionalcoef;
    List<Graphics> vectorlist;
    Bitmap bitmap;

    /// <summary>
    /// Init Form1
    /// </summary>
    public Form1()
    {
      InitializeComponent();
      vectorlist = new List<Graphics>();
    }
    /// <summary>
    /// Load Json structure in dynamic object and map to Primitive class
    /// </summary>
    private void LoadJson()
    {
      jsondata = new List<Primitive>();
      using (StreamReader r = new StreamReader("json1.json"))
      {
        string json = r.ReadToEnd();
        dynamic dynJson = JsonConvert.DeserializeObject<dynamic>(json);
        foreach (var x in dynJson)
          jsondata.Add(new Primitive(x));
      }
    }
    /// <summary>
    /// Load Xml structure in dynamic object and map to Primitive class
    /// </summary>
    private void LoadXml()
    {
      jsondata = new List<Primitive>();
      XmlDocument doc = new XmlDocument();
      doc.Load("primitives.xml");
      foreach (XmlNode node in doc.DocumentElement.ChildNodes)
      {
        jsondata.Add(new Primitive(node));
      }
    }
    /// <summary>
    /// draws the axes of the Cartesian plane
    /// </summary>
    private void DrawCartesian(Bitmap bitmap)
    {
      Pen pen;
      Graphics vector = Graphics.FromImage(bitmap);
      pen = new Pen(Color.Black);

      vector.DrawLine(pen, 0, center.Y, scrwidth, center.Y);
      vector.DrawLine(pen, center.X, 0, center.X, scrheight);
      vectorlist.Add(vector);
    }
    /// <summary>
    /// for every item in the json the procedure set the style and color of the pen and draw the graphics
    /// </summary>
    private Bitmap Printprimitives(int canvaswidth, int canvasheight)
    {
      //calculation of the resolution coef to draw the objet as big as they can completly fit in the screen
      proportionalcoef = CalculateCoef(canvaswidth, canvasheight);
      bitmap = new Bitmap(Convert.ToInt32(scrwidth), Convert.ToInt32(scrheight), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      DrawCartesian(bitmap);

      Graphics vector = Graphics.FromImage(bitmap);

      //Graphics vector = CreateGraphics();
      foreach (Primitive x in jsondata)
      {
        Pen pen;

        //set pen color from json property
        pen = new Pen(x.color);

        //set pen style from json property
        pen.DashStyle = x.lineType;

        //in base of the type of object to draw i get the points and 
        PointF[] pointlist;
        switch (x.type)
        {
          case "line":
            //draw the line
            vector.DrawLine(pen, x.points[0], x.points[1]);
            break;
          case "circle":
            //get center location from json
            PointF circlecenter = new PointF(center.X + x.points[0].X, center.Y + x.points[0].X);
            //get radiusffrom json
            float radius = (float)x.radius.Value;
            //draw the circle
            vector.DrawEllipse(pen, (circlecenter.X - (radius * proportionalcoef)), (circlecenter.Y - (radius * proportionalcoef)), (radius * 2) * proportionalcoef, (radius * 2) * proportionalcoef);
            //if the json property "filled" is set to true 
            if ((bool)x.filled)
            {
              Brush brush = Brushes.Yellow;
              vector.FillEllipse(brush, (circlecenter.X - radius) * proportionalcoef, (circlecenter.Y - radius) * proportionalcoef, (radius * 2) * proportionalcoef, (radius * 2) * proportionalcoef);
            }
            break;
          case "triangle":
            //get the points from the json
            pointlist = new PointF[] { SetPointCoordinates(x.points[0]), SetPointCoordinates(x.points[1]), SetPointCoordinates(x.points[2]) };
            //draw the triangle
            vector.DrawPolygon(pen, pointlist);
            //if the json property "filled" is set to true 
            if ((bool)x.filled)
            {
              //fill the triangle
              Brush brush = new SolidBrush(x.color);
              vector.FillPolygon(brush, pointlist);
            }
            break;
          case "rectangle":
            //get the points from the json
            pointlist = new PointF[] { SetPointCoordinates(x.points[0]), SetPointCoordinates(x.points[1]), SetPointCoordinates(x.points[2]), SetPointCoordinates(x.points[3]) };
            //draw the triangle
            vector.DrawPolygon(pen, pointlist);
            //if the json property "filled" is set to true 
            if ((bool)x.filled)
            {
              //fill the triangle
              Brush brush = new SolidBrush(x.color);
              vector.FillPolygon(brush, pointlist);
            }
            break;
        }
      }
      //vector.Clear(Color.Green);
      vectorlist.Add(vector);
      return bitmap;
    }
    /// <summary>
    /// get point location in the form
    /// </summary>
    private PointF SetPointCoordinates(PointF p)
    {
      float x = p.X, y = p.Y;
      return new PointF((center.X + (x * proportionalcoef)), (center.Y + (y * proportionalcoef)));
    }
    /// <summary>
    /// Calculate the coefficent for keeping the proportions or the objects in the form
    /// </summary>
    private float CalculateCoef(int width, int height)
    {
      scrwidth = width;
      scrheight = height;
      center = new PointF(scrwidth / 2, scrheight / 2);

      float minHeight = 0;
      float maxHeight = 0;
      float minWidth = 0;
      float maxWidth = 0;

      //get from the json the min and max dimentions o the objects to draw
      foreach (Primitive x in jsondata)
      {
        switch (x.type)
        {
          case "line":
            PointF a = x.points[0];
            PointF b = x.points[1];
            if (a.X < minWidth)
              minWidth = a.X;
            if (a.Y < minHeight)
              minHeight = a.Y;
            if (b.X > maxWidth)
              maxWidth = b.X;
            if (b.Y > maxHeight)
              maxHeight = b.Y;
            break;
          case "circle":
            float radius = (float)x.radius;
            PointF point = x.points[0];
            if ((point.X - radius) < minWidth)
              minWidth = point.X - radius;
            if ((point.X + radius) > maxWidth)
              maxWidth = point.X + radius;
            if ((point.Y - radius) < minHeight)
              minHeight = point.Y - radius;
            if ((point.Y + radius) > maxHeight)
              maxHeight = point.Y + radius;
            break;
          case "rectangle":
          case "triangle":
            foreach (PointF p in x.points)
            {
              if (p.X < minWidth)
                minWidth = p.X;
              if (p.Y < minHeight)
                minHeight = p.Y;
              if (p.X > maxWidth)
                maxWidth = p.X;
              if (p.Y > maxHeight)
                maxHeight = p.Y;
            }
            break;
        }
      }

      float totalwidth = 0;
      float totalheight = 0;
      if (Math.Abs(minWidth) > Math.Abs(maxWidth))
        totalwidth = Math.Abs(minWidth) * 3;
      else
        totalwidth = Math.Abs(maxWidth) * 3;
      if (Math.Abs(minHeight) > Math.Abs(maxHeight))
        totalheight = Math.Abs(minHeight) * 3;
      else
        totalheight = Math.Abs(maxHeight) * 3;

      //it use the biggest lenght to keep always visible the vectors
      if (totalwidth > totalheight)
        return (scrwidth / totalwidth);
      else
        return (scrheight / totalheight);
    }
    /// <summary>
    /// clear all the shape in the screen
    /// </summary>
    private void clearVectors()
    {
      foreach (Graphics vector in vectorlist)
        vector.Clear(Color.White);
      vectorlist = new List<Graphics>();
    }
    /// <summary>
    /// Resize and export the bitmap image as pdf
    /// </summary>
    private void ExportToPdf(int width, int height)
    {
      //get the bitmap of the requested sizes
      Bitmap btm = Printprimitives(width, height);
      try
      {
        //new pdf doc
        PdfDocument doc = new PdfDocument();
        doc.Pages.Add(new PdfPage());
        XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[0]);
        XImage img;
        //get the bitmap and save it as pdf doc
        string tempfile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName().Replace(".", "") + ".bmp");
        btm.Save(tempfile);
        img = XBitmapImage.FromFile(tempfile);
        string name = Path.GetRandomFileName() + ".pdf";
        xgr.DrawImage(img, 0, 0);
        doc.Save(name);
        doc.Close();
        btm.Dispose();
        img.Dispose();
        MessageBox.Show($"File saved in the app directory with name {name}");
        //delete temp file
        File.Delete(tempfile);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      LoadJson();
      //draw the json graphics
      bitmap = Printprimitives(pictureBox1.Width, pictureBox1.Height);
      pictureBox1.Image = bitmap;
    }

    private void Form1_ResizeEnd(object sender, EventArgs e)
    {
      //if something is drawn in the form i will clear them and redraw with the new proportions.
      if (vectorlist != null && vectorlist.Count > 0)
      {
        clearVectors();
        bitmap = Printprimitives(pictureBox1.Width, pictureBox1.Height);
        pictureBox1.Image = bitmap;
      }
    }

    private void Form1_Scroll(object sender, ScrollEventArgs e)
    {
      //if something is drawn in the form i will clear them and redraw with the new proportions.
      if (vectorlist != null && vectorlist.Count > 0)
      {
        clearVectors();
        bitmap = Printprimitives(pictureBox1.Width, pictureBox1.Height);
        pictureBox1.Image = bitmap;
      }
    }

    private void Form1_Resize(object sender, EventArgs e)
    {
      //if something is drawn in the form i will clear them and redraw with the new proportions.
      if (vectorlist != null && vectorlist.Count > 0)
      {
        clearVectors();
        bitmap = Printprimitives(pictureBox1.Width, pictureBox1.Height);
        pictureBox1.Image = bitmap;
      }
    }

    private void button2_Click(object sender, EventArgs e)
    {
      LoadXml();
      //draw the xml graphics
      bitmap = Printprimitives(pictureBox1.Width, pictureBox1.Height);
      pictureBox1.Image = bitmap;
    }

    private void button3_Click(object sender, EventArgs e)
    {
      int width = 0, height = 0;
      if (!int.TryParse(textBox1.Text, out width) || !int.TryParse(textBox2.Text, out height))
        MessageBox.Show("Insert integer values.");
      ExportToPdf(width, height);
    }
  }
}
