using Game;
using Engine;
using TemplatesDatabase;
using System.Xml.Linq;
using Engine.Graphics;
using Engine.Input;
using GameEntitySystem;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using Engine.Serialization;
using System.IO;



using Engine.Media;
using System.Linq;
using FindDominantColour;
using XmlUtilities;

namespace API_WE_Mod
{
    public class API_WE
    {

        internal static int blockCount;
        internal static List<BlockMem> BlockList = new List<BlockMem>();



        // public static readonly string sdcard = "/sdcard/Survivalcraft/WorldEdit";

        // public readonly string oneKeyFile = 
        // public readonly string CopyFile = string.Format("{0}/Cache/Cache.w", (object));



        public static void draw_img(bool mirror, int ofset_paletr, bool save_colors, bool use_castom_paletr, int deep_color, bool on_furn, int size, int furn_resol, bool pos, bool rotate, string pa, Point3 start, SubsystemTerrain subsystemTerrain, ComponentPlayer p)
        {


            Task.Run(() =>
            {
                try
                {
                    System.Drawing.Bitmap im = null;
                    try
                    {
                        im = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(pa);
                    }
                    catch (Exception ex)
                    {
                        p.ComponentGui.DisplaySmallMessage(string.Format("Error read image"), true, true);

                        return;
                    }



                    if (!on_furn)
                    {
                        furn_resol = 1;
                    }







                    if (!mirror)
                    {
                        im.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipX);
                    }





                    if (pos)
                    {
                        im.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
                    }



                    im = (new System.Drawing.Bitmap(im, new System.Drawing.Size(im.Width / size, im.Height / size)));



                    var num1 = 0;
                    for (var index = 0; index < 1024; ++index)
                        if (subsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(index) != null)
                            ++num1;
                    num1 = 1024 - num1;
                    var a = (im.Width / furn_resol) * (im.Height / furn_resol);

                    if (on_furn && a > num1)
                    {
                        p.ComponentGui.DisplaySmallMessage(string.Format("Not enough furniture blocks (" + num1.ToString() + "/" + a.ToString() + ")"), true, true);

                        return;
                    }



                    Image img = new Image(im.Width, im.Height);

                    for (int x = 0; x < im.Width; x++)
                    {
                        for (int y = 0; y < im.Height; y++)
                        {
                            img.SetPixel(x, y, new Color(im.GetPixel(x, y).R, im.GetPixel(x, y).G, im.GetPixel(x, y).B, im.GetPixel(x, y).A));
                        }
                    }


                    if (use_castom_paletr)
                    {
                        top_colors(ofset_paletr, save_colors, deep_color, img, subsystemTerrain.SubsystemPalette);
                    }


                    List<int> fur_bl_img = new List<int>();
                    for (int x = 0; x < img.Width / furn_resol; x++)
                    {
                        for (int y = 0; y < img.Height / furn_resol; y++)
                        {

                            if (on_furn)
                            {
                                List<int> bl_img = new List<int>();
                                bool is_a_bl = false;
                                for (int x1 = 0; x1 < furn_resol; x1++)
                                {
                                    for (int y1 = 0; y1 < furn_resol; y1++)
                                    {
                                        if (get_block_id(GetClosestColor(subsystemTerrain.SubsystemPalette.m_colors, img.GetPixel((x * furn_resol) + x1, (y * furn_resol) + y1)), subsystemTerrain.SubsystemPalette) != 0 && is_a_bl == false)
                                        {
                                            is_a_bl = true;
                                        }
                                        bl_img.Add(get_block_id(GetClosestColor(subsystemTerrain.SubsystemPalette.m_colors, img.GetPixel((x * furn_resol) + x1, (y * furn_resol) + y1)), subsystemTerrain.SubsystemPalette));
                                    }
                                }
                                if (is_a_bl)
                                {
                                    int img_id = create_dsg(rotate, pos, furn_resol, bl_img, subsystemTerrain, ("x " + x.ToString() + "  " + "y " + y.ToString()).ToString());
                                    if (pos)
                                    {
                                        if (rotate)
                                        {
                                            subsystemTerrain.ChangeCell(start.X, start.Y + x, start.Z + y, img_id);
                                        }
                                        else
                                        {
                                            subsystemTerrain.ChangeCell(start.X + y, start.Y + x, start.Z, img_id);
                                        }

                                    }
                                    else
                                    {
                                        subsystemTerrain.ChangeCell(start.X + y, start.Y, start.Z + x, img_id);

                                    }

                                }
                            }
                            else
                            {
                                int img_id = get_block_id(GetClosestColor(subsystemTerrain.SubsystemPalette.m_colors, img.GetPixel(x, y)), subsystemTerrain.SubsystemPalette);
                                if (pos)
                                {
                                    if (rotate)
                                    {
                                        subsystemTerrain.ChangeCell(start.X, start.Y + x, start.Z + y, img_id);
                                    }
                                    else
                                    {
                                        subsystemTerrain.ChangeCell(start.X + y, start.Y + x, start.Z, img_id);
                                    }

                                }
                                else
                                {
                                    subsystemTerrain.ChangeCell(start.X + y, start.Y, start.Z + x, img_id);

                                }
                            }




                        }
                    }

                    p.ComponentGui.DisplaySmallMessage(string.Format("Success " + a + " blocks"), true, true);

                    AnalyticsManager.LogEvent("WE image sucsees",new AnalyticsParameter("blocks",a.ToString()));
                }catch(Exception ex)
                {
                    p.ComponentGui.DisplaySmallMessage(string.Format("Failed cause: " + ex.Message ), true, true);
                    AnalyticsManager.LogError("WE image fail",ex);
                }

            });



        }




        private static Color GetClosestColor(Color[] colorArray, Color baseColor)
        {
            if (baseColor.A == 0)
            {
                return new Color(0, 0, 0, 0);
            }

            var colors = colorArray.Select(x => new { Value = x, Diff = GetDiff(x, baseColor) }).ToList();
            var min = colors.Min(x => x.Diff);
            return colors.Find(x => x.Diff == min).Value;
        }

        private static int GetDiff(Color color, Color baseColor)
        {
            int a = color.A - baseColor.A,
                r = color.R - baseColor.R,
                g = color.G - baseColor.G,
                b = color.B - baseColor.B;
            return a * a + r * r + g * g + b * b;
        }


        private static int[] arr = new int[]
{
    16456,
    49224,
    81992,
    114760,
    147528,
    180296,
    213064,
    245832,
    278600,
    311368,
    344136,
    376904,
    409672,
    442440,
    475208,
    507976
};

        public static Color[] p_c__;
        //public  static string  p_c;
        public static string n_w;

        public static void top_colors(int ofs_p, bool sv_c, int dc, Image img, SubsystemPalette p)
        {
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(img.Width, img.Height);

            List<System.Drawing.Color> colors = new List<System.Drawing.Color>(image.Width * image.Height);

            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {

                    colors.Add(System.Drawing.Color.FromArgb(img.GetPixel(x, y).R, img.GetPixel(x, y).G, img.GetPixel(x, y).B));

                }
            }


            KMeansClusteringCalculator clustering = new KMeansClusteringCalculator();
            IList<System.Drawing.Color> dominantColours = clustering.Calculate(dc, colors, 6.0d);

            Color[] c_c = WorldPalette.DefaultColors;


            for (int i = 0; i < dominantColours.Count; i++)
            {
                c_c[i + ofs_p] = new Engine.Color(dominantColours[i].R, dominantColours[i].G, dominantColours[i].B);
            }

            p.m_colors = c_c.ToArray();

            if (sv_c)
            {


                p_c__ = c_c.ToArray();
                //p_c = "158,2,81;158,131,100;1,0,1;190,2,107;186,2,80;91,45,42;128,4,62;178,90,65;130,78,56;224,2,96;209,102,76;143,94,88;186,144,135;242,123,90;227,174,167;255,255,255";
                n_w = GameManager.WorldInfo.DirectoryName;
                //dat.ValuesDictionary.GetValue<ValuesDictionary>("GameInfo").GetValue<ValuesDictionary>("Palette").SetValue<string>(str1,"Colors");


            }






        }


        public static void save_colors()
        {
            if (p_c__ == null)
            {
                return;
            }



            WorldInfo i = WorldsManager.GetWorldInfo(API_WE_Mod.API_WE.n_w);


            var path = Storage.CombinePaths(n_w, "Project.xml");
            if (!Storage.FileExists(path))
                return;
            var xelement = (XElement)null;
            using (var stream = Storage.OpenFile(path, OpenFileMode.Read))
            {
                xelement = XmlUtils.LoadXmlFromStream(stream, null, true);
            }

            var gameInfoNode = GetGameInfoNode(xelement);
            var valuesDictionary = new ValuesDictionary();
            valuesDictionary.ApplyOverrides(gameInfoNode);


            i.WorldSettings.Save(valuesDictionary, true);
            Log.Warning("cur palette " + valuesDictionary.GetValue<ValuesDictionary>("Palette").GetValue<string>("Colors"));



            valuesDictionary.SetValue("Palette", Save());


            gameInfoNode.RemoveNodes();
            valuesDictionary.Save(gameInfoNode);

            Log.Warning("cur palette 2 " + valuesDictionary.GetValue<ValuesDictionary>("Palette").GetValue<string>("Colors"));
            using (var stream = Storage.OpenFile(path, OpenFileMode.Create))
            {
                XmlUtils.SaveXmlToStream(xelement, stream, null, true);
            }
            Log.Warning("Color saves");
            p_c__ = null;



        }

        public static ValuesDictionary Save()
        {


            var valuesDictionary = new ValuesDictionary();

            var str1 = string.Join(";", p_c__.Select((c, i) =>
            {
                return HumanReadableConverter.ConvertToString(c);
            }));
            var str2 = string.Join(";", WorldPalette.DefaultNames.Select((n, i) =>
            {
                if (!(n == WorldPalette.DefaultNames[i]))
                    return n;
                return string.Empty;
            }));
            valuesDictionary.SetValue("Colors", str1);
            valuesDictionary.SetValue("Names", str2);

            return valuesDictionary;
        }

        private static XElement GetGameInfoNode(XElement projectNode)
        {
            var xelement = projectNode.Element("Subsystems").Elements("Values")
                .Where(n => XmlUtils.GetAttributeValue(n, "Name", string.Empty) == "GameInfo").FirstOrDefault();
            if (xelement != null)
                return xelement;
            throw new InvalidOperationException("GameInfo node not found in project.");
        }

        public static int get_block_id(Color c, SubsystemPalette p)
        {
            if (c.A == 0)
            {
                return 0;
            }

            for (int i = 0; i < p.m_colors.Length; i++)
            {
                if (p.m_colors[i] == c)
                {
                    return arr[i];
                }
            }
            return 1;
        }


        public static int create_dsg(bool rot, bool pos, int resol, List<int> color, SubsystemTerrain ter, string name)
        {


            var design = (FurnitureDesign)null;
            var valuesDictionary = new Dictionary<Point3, int>();
            var point1 = new Point3(0, 0, 0);
            var point2 = new Point3(0, 0, 0);

            for (int x = 0; x < resol; x++)
            {
                for (int y = 0; y < resol; y++)
                {

                    if (pos)
                    {
                        if (rot)
                        {
                            valuesDictionary.Add(new Point3(0, y, x), color[(x + y * resol)]);
                        }
                        else
                        {
                            valuesDictionary.Add(new Point3(x, y, 0), color[(x + y * resol)]);
                        }
                    }
                    else
                    {
                        valuesDictionary.Add(new Point3(x, 0, y), color[(x + y * resol)]);
                    }



                }
            }

            design = new FurnitureDesign(ter);

            var resolution = resol;
            var values = new int[resolution * resolution * resolution];
            foreach (var keyValuePair in valuesDictionary)
            {
                var point3_2 = keyValuePair.Key - point1;
                values[point3_2.X + point3_2.Y * resolution + point3_2.Z * resolution * resolution] =
                    keyValuePair.Value;
            }

            design.SetValues(resolution, values);
            design.Name = name;


            FurnitureDesign dsg = ter.SubsystemFurnitureBlockBehavior.TryAddDesign(design);

            var num = Terrain.MakeBlockValue(227, 0, FurnitureBlock.SetDesignIndex(0, dsg.Index, dsg.ShadowStrengthFactor, false));


            return num;

        }






        public static void Round(bool s, int radius, int lenght, int pos, int id, TerrainRaycastResult? Point1, SubsystemTerrain subsystemTerrain, ComponentPlayer player)
        {

            Task.Run(() =>
            {
                string s2 = Convert.ToString(pos);
                //int num1 = 0;
                for (int index1 = 0; index1 < lenght; ++index1)
                {
                    for (int index2 = -radius; index2 <= 0; ++index2)
                    {
                        for (int index3 = -radius; index3 <= 0; ++index3)
                        {
                            if ((int)Math.Sqrt((double)(index2 * index2 + index3 * index3)) <= radius && (!s || (int)Math.Sqrt((double)((index2 - 1) * (index2 - 1) + index3 * index3)) > radius || (int)Math.Sqrt((double)(index2 * index2 + (index3 - 1) * (index3 - 1))) > radius))
                            {

                                int num2 = !true ? -index1 : index1;
                                if (pos == 2)
                                {

                                    subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X + num2, Point1.Value.CellFace.Point.Y - index2, Point1.Value.CellFace.Point.Z - index3, id, true);
                                    subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X + num2, Point1.Value.CellFace.Point.Y + index2, Point1.Value.CellFace.Point.Z - index3, id, true);
                                    subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X + num2, Point1.Value.CellFace.Point.Y - index2, Point1.Value.CellFace.Point.Z + index3, id, true);
                                    subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X + num2, Point1.Value.CellFace.Point.Y + index2, Point1.Value.CellFace.Point.Z + index3, id, true);
                                }
                                if (pos == 1)
                                {

                                    subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X + index2, Point1.Value.CellFace.Point.Y + num2, Point1.Value.CellFace.Point.Z + index3, id, true);
                                    subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X + index2, Point1.Value.CellFace.Point.Y + num2, Point1.Value.CellFace.Point.Z - index3, id, true);
                                    subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X - index2, Point1.Value.CellFace.Point.Y + num2, Point1.Value.CellFace.Point.Z + index3, id, true);
                                    subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X - index2, Point1.Value.CellFace.Point.Y + num2, Point1.Value.CellFace.Point.Z - index3, id, true);
                                }
                                if (pos == 3)
                                {


                                    subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X + index2, Point1.Value.CellFace.Point.Y - index3, Point1.Value.CellFace.Point.Z + num2, id, true);
                                    subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X + index2, Point1.Value.CellFace.Point.Y + index3, Point1.Value.CellFace.Point.Z + num2, id, true);
                                    subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X - index2, Point1.Value.CellFace.Point.Y + index3, Point1.Value.CellFace.Point.Z + num2, id, true);
                                    subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X - index2, Point1.Value.CellFace.Point.Y - index3, Point1.Value.CellFace.Point.Z + num2, id, true);
                                }

                                // num1 += 4;

                            }
                        }
                    }
                }

            });
        }
        public static void LinePoint(Point3 v, Point3 v2, int id, SubsystemTerrain subsystemTerrain)
        {

            int lengin = Math.Max(Math.Max(Math.Abs(v.X - v2.X), Math.Abs(v.Y - v2.Y)), Math.Abs(v.Z - v2.Z));
            Task.Run((Action)(() =>
            {
                int num = 0;
                for (int index = 0; index <= lengin; ++index)
                {

                    int cellValueFast = subsystemTerrain.Terrain.GetCellValueFast(v.X + (int)Math.Round((double)index / (double)lengin * (double)(v2.X - v.X)), v.Y + (int)Math.Round((double)index / (double)lengin * (double)(v2.Y - v.Y)), v.Z + (int)Math.Round((double)index / (double)lengin * (double)(v2.Z - v.Z)));
                    if (cellValueFast != id && Terrain.ExtractContents(cellValueFast) != id)
                    {
                        subsystemTerrain.ChangeCell(v.X + (int)Math.Round((double)index / (double)lengin * (double)(v2.X - v.X)), v.Y + (int)Math.Round((double)index / (double)lengin * (double)(v2.Y - v.Y)), v.Z + (int)Math.Round((double)index / (double)lengin * (double)(v2.Z - v.Z)), id, true);
                        ++num;

                    }
                }
                // this.player.ComponentGui.DisplaySmallMessage(string.Format("操作成功，共生成{0}个方块", (object)num), true, true);
            }));
        }

        public static void Sphere(bool s, int r, int id, TerrainRaycastResult? Point1, SubsystemTerrain subsystemTerrain)
        {

            Task.Run(() =>
            {
                int num = 0;
                for (int index1 = -r; index1 <= 0; ++index1)
                {
                    for (int index2 = -r; index2 <= 0; ++index2)
                    {
                        for (int index3 = -r; index3 <= 0; ++index3)
                        {
                            if ((int)Math.Sqrt(index1 * index1 + index2 * index2 + index3 * index3) <= r && (!s || (int)Math.Sqrt((double)((index1 - 1) * (index1 - 1) + index2 * index2 + index3 * index3)) > r || ((int)Math.Sqrt((double)(index1 * index1 + (index2 - 1) * (index2 - 1) + index3 * index3)) > r || (int)Math.Sqrt((double)(index1 * index1 + index2 * index2 + (index3 - 1) * (index3 - 1))) > r)))
                            {
                                subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X + index1, Point1.Value.CellFace.Point.Y + index2, Point1.Value.CellFace.Point.Z + index3, id, true);
                                subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X - index1, Point1.Value.CellFace.Point.Y + index2, Point1.Value.CellFace.Point.Z + index3, id, true);
                                subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X - index1, Point1.Value.CellFace.Point.Y - index2, Point1.Value.CellFace.Point.Z + index3, id, true);
                                subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X - index1, Point1.Value.CellFace.Point.Y + index2, Point1.Value.CellFace.Point.Z - index3, id, true);
                                subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X - index1, Point1.Value.CellFace.Point.Y - index2, Point1.Value.CellFace.Point.Z - index3, id, true);
                                subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X + index1, Point1.Value.CellFace.Point.Y - index2, Point1.Value.CellFace.Point.Z + index3, id, true);
                                subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X + index1, Point1.Value.CellFace.Point.Y + index2, Point1.Value.CellFace.Point.Z - index3, id, true);
                                subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X + index1, Point1.Value.CellFace.Point.Y - index2, Point1.Value.CellFace.Point.Z - index3, id, true);
                                num += 8;

                            }
                        }
                    }
                }
                //this.player.ComponentGui.DisplaySmallMessage(string.Format("操作成功，共生成{0}个方块", (object)num), true, true);
            });
        }



        public static void Prism(bool s, int r, int blockID, TerrainRaycastResult? Point1, SubsystemTerrain subsystemTerrain)
        {
            int num = 0;
            for (int index1 = -r; index1 <= r; ++index1)
            {
                for (int index2 = -r; index2 <= r; ++index2)
                {
                    for (int index3 = -r; index3 <= r; ++index3)
                    {
                        if (Math.Abs(index1) + Math.Abs(index2) + Math.Abs(index3) <= r && (!s || Math.Abs(index1) + Math.Abs(index2) + Math.Abs(index3) >= r))
                        {

                            subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X + index1, Point1.Value.CellFace.Point.Y + index2, Point1.Value.CellFace.Point.Z + index3, blockID);
                            ++num;


                        }
                    }
                }
            }
            // m_componentPlayer.ComponentGui.DisplaySmallMessage(string.Format("8989789797", (object)num), true, true);
        }



        public static void Square(bool s, int r, int lenght, Position pos, int blockID, TerrainRaycastResult? Point1, SubsystemTerrain subsystemTerrain)
        {

            Task.Run(() =>
            {
                int num1 = 0;
                for (int index1 = 0; index1 < lenght; ++index1)
                {
                    for (int index2 = -r; index2 <= r; ++index2)
                    {
                        for (int index3 = -r; index3 <= r; ++index3)
                        {
                            if (Math.Abs(index2) + Math.Abs(index3) <= r && (!s || Math.Abs(index2) + Math.Abs(index3) >= r))
                            {
                                int num2 = !true ? -index1 : index1;
                                if (pos == Position.flat)
                                    subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X + num2, Point1.Value.CellFace.Point.Y + index2, Point1.Value.CellFace.Point.Z + index3, blockID, true);
                                else if (pos == Position.X)
                                    subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X + index2, Point1.Value.CellFace.Point.Y + num2, Point1.Value.CellFace.Point.Z + index3, blockID, true);
                                else
                                    subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X + index2, Point1.Value.CellFace.Point.Y + index3, Point1.Value.CellFace.Point.Z + num2, blockID, true);
                                ++num1;

                            }
                        }
                    }
                }
                // this.player.ComponentGui.DisplaySmallMessage(string.Format("操作成功，共生成{0}个方块", (object)num1), true, true);
            });
        }

        /*

        public static void lingZhui(bool s, int r, int blockID, TerrainRaycastResult? Point1, SubsystemTerrain subsystemTerrain)
        {
            
            Task.Run(() =>
            {
                int num = 0;
                for (int index1 = -r; index1 <= r; ++index1)
                {
                    for (int index2 = -r; index2 <= r; ++index2)
                    {
                        for (int index3 = -r; index3 <= r; ++index3)
                        {
                            if (Math.Abs(index1) + Math.Abs(index2) + Math.Abs(index3) <= r && (!s || Math.Abs(index1) + Math.Abs(index2) + Math.Abs(index3) >= r))
                            {

                                subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X + index1, Point1.Value.CellFace.Point.Y + index2, Point1.Value.CellFace.Point.Z + index3, blockID, true);
                                ++num;

                            }
                        }
                    }
                }
                // this.player.ComponentGui.DisplaySmallMessage(string.Format("操作成功，共生成{0}个方块", (object)num), true, true);
            });
        }
        */



        public static void CreativeMaze(Point3 Start, Point3 End, int BlockID, SubsystemTerrain subsystemTerrain, ComponentPlayer player)
        {


            if (Start.X < End.X)
            {
                int x = Start.X;
                Start.X = End.X;
                End.X = x;
            }
            if (Start.Y < End.Y)
            {
                int y = Start.Y;
                Start.Y = End.Y;
                End.Y = y;
            }
            if (Start.Z < End.Z)
            {
                int z = Start.Z;
                Start.Z = End.Z;
                End.Z = z;
            }
            if (Start.Z - End.Z < 5 || Start.X - End.X < 5)
                player.ComponentGui.DisplaySmallMessage("Too small", true, true);
            else
                Task.Run(() =>
                {
                    int num1 = 0;
                    int num2 = Start.X - End.X;
                    int num3 = Start.Z - End.Z;
                    bool[,] boolArray = new Maze(num2 / 2, num3 / 2).GetBoolArray();
                    for (int index1 = 0; index1 <= (num2 % 2 != 0 ? num2 - 1 : num2); ++index1)
                    {
                        for (int index2 = 0; index2 <= (num3 % 2 != 0 ? num3 - 1 : num3); ++index2)
                        {
                            if ((index1 != 1 || index2 != 0) && (index1 != (num2 % 2 != 0 ? num2 - 1 : num2) || index2 != (num3 % 2 != 0 ? num3 - 1 : num3) - 1) && boolArray[index1, index2])
                            {
                                for (int index3 = 0; index3 <= Start.Y - End.Y; ++index3)
                                {

                                    subsystemTerrain.ChangeCell(End.X + index1, End.Y + index3, End.Z + index2, BlockID, true);
                                    ++num1;

                                }
                            }
                        }
                    }
                    player.ComponentGui.DisplaySmallMessage(string.Format("Sucsses {0} place blocks", num1), true, true);
                });


        }

        public static void Rectangle(int pos, int blockId, Point3 Start, Point3 End, ComponentPlayer player, SubsystemTerrain subsystemTerrain)
        {

            if (Start.X < End.X)
            {
                int x = Start.X;
                Start.X = End.X;
                End.X = x;
            }
            if (Start.Y < End.Y)
            {
                int y = Start.Y;
                Start.Y = End.Y;
                End.Y = y;
            }
            if (Start.Z < End.Z)
            {
                int z = Start.Z;
                Start.Z = End.Z;
                End.Z = z;
            }
            Task.Run((Action)(() =>
            {
                int num = 0;
                for (int index1 = 0; index1 <= Start.X - End.X; ++index1)
                {
                    for (int index2 = 0; index2 <= Start.Y - End.Y; ++index2)
                    {
                        for (int index3 = 0; index3 <= Start.Z - End.Z; ++index3)
                        {
                            if ((pos != 1 || index1 <= 0 || (index1 >= Start.X - End.X || index2 <= 0) || (index2 >= Start.Y - End.Y || index3 <= 0 || index3 >= Start.Z - End.Z)) && (pos != 2 || (index1 < 0 || index1 > Start.X - End.X || (index2 <= 0 || index2 >= Start.Y - End.Y) || (index3 <= 0 || index3 >= Start.Z - End.Z)) && (index2 < 0 || index2 > Start.Y - End.Y || (index1 <= 0 || index1 >= Start.X - End.X) || (index3 <= 0 || index3 >= Start.Z - End.Z)) && (index3 < 0 || index3 > Start.Z - End.Z || (index2 <= 0 || index2 >= Start.Y - End.Y) || (index1 <= 0 || index1 >= Start.X - End.X))))
                            {

                                subsystemTerrain.ChangeCell(End.X + index1, End.Y + index2, End.Z + index3, blockId, true);
                                ++num;

                            }
                        }
                    }
                }
                player.ComponentGui.DisplaySmallMessage(string.Format("Sucsses {0} place blocks", num), true, true);
            }));
        }

        public static void Mountain(Point3 Start, int Radius, int Size, SubsystemTerrain subsystemTerrain, int id, int id1, int id2, ComponentPlayer player)
        {

            Task.Run(() =>
            {
                Game.Random random = new Game.Random();
                double num1 = Math.PI / 2.0;
                float num2 = (float)Size + 0.8f;
                int num3 = 0;
                double num4 = 10.0;
                double num5 = 15.0;
                float num6 = random.UniformFloat((float)num4, (float)num5) + 10f;
                for (int index1 = -Radius; index1 < Radius; ++index1)
                {
                    for (int index2 = -Radius; index2 < Radius; ++index2)
                    {
                        double num7 = Math.Cos(num1 * (double)index1 / (double)Radius) * Math.Cos(num1 * (double)index2 / (double)Radius) * (double)num2;
                        double num8 = Math.Sin(num1 * (double)index1 * 1.39999997615814 / (double)Radius + 4.0) * Math.Cos(num1 * (double)index2 * 1.39999997615814 / (double)Radius + 7.0) * (double)num2 * 0.25;
                        double num9 = Math.Cos(num1 * (double)index1 * 1.39999997615814 * 2.0 / (double)Radius + 4.0 * (double)num6) * Math.Sin(num1 * (double)index2 * 1.39999997615814 * 2.0 / (double)Radius + 8.0 * (double)num6) * (double)num2 * 0.200000002980232;
                        double num10 = Math.Sin(num1 * (double)index1 * 1.39999997615814 * 4.0 / (double)Radius + 4.0 * (double)num6 * 1.5) * Math.Sin(num1 * (double)index2 * 1.39999997615814 * 4.0 / (double)Radius + 8.0 * (double)num6 * 1.5) * (double)num2 * 0.150000005960464;
                        double num11 = num8;
                        double num12 = num7 - num11 + num9 - num10;
                        if (num12 > 0.0)
                        {
                            for (int index3 = 0; (double)index3 <= num12; ++index3)
                            {

                                if (index3 > 3)
                                {
                                    subsystemTerrain.ChangeCell(Start.X + index1, Start.Y + (int)num12 - index3, Start.Z + index2, id, true);
                                    ++num3;

                                }
                                else if (index3 > 0)
                                {
                                    subsystemTerrain.ChangeCell(Start.X + index1, Start.Y + (int)num12 - index3, Start.Z + index2, id1, true);
                                    ++num3;

                                }
                                else if (index3 == 0)
                                {
                                    subsystemTerrain.ChangeCell(Start.X + index1, Start.Y + (int)num12, Start.Z + index2, id2, true);
                                    ++num3;

                                }
                            }
                        }
                    }
                }
                player.ComponentGui.DisplaySmallMessage(string.Format("Sucsses {0} place blocks", num3), true, true);
            });
        }

        public static void Coppy_zone(string path, Point3 Point1, Point3 Point2, SubsystemTerrain subsystemTerrain, ComponentPlayer player)
        {
            int startX = Math.Min(Point1.X, Point2.X);
            int endX = Math.Max(Point1.X, Point2.X);
            int startY = Math.Min(Point1.Y, Point2.Y);
            int endY = Math.Max(Point1.Y, Point2.Y);
            int startZ = Math.Min(Point1.Z, Point2.Z);
            int endZ = Math.Max(Point1.Z, Point2.Z);

            FileStream fileStream = new FileStream(path, FileMode.Create);
            EngineBinaryWriter engineBinaryWriter = new EngineBinaryWriter((Stream)fileStream, true);
            engineBinaryWriter.Write((byte)79);
            engineBinaryWriter.Write((byte)110);
            engineBinaryWriter.Write((byte)101);
            engineBinaryWriter.Write((byte)75);
            engineBinaryWriter.Write((byte)101);
            engineBinaryWriter.Write((byte)121);
            engineBinaryWriter.Write((byte)0);
            engineBinaryWriter.Write(0);


            blockCount = 0;
            BlockList.Clear();

            for (int x = 0; x <= endX - startX; x++)
            {
                for (int y = 0; y <= endY - startY; y++)
                {
                    for (int z = 0; z <= endZ - startZ; z++)
                    {
                        BlockMem blmem = new BlockMem();
                        int X, Y, Z;
                        if (Point1.X > Point2.X)
                        {
                            blmem.x = -x;
                            X = Point1.X - x;
                        }
                        else
                        {
                            blmem.x = x;
                            X = Point1.X + x;
                        }

                        if (Point1.Y > Point2.Y)
                        {
                            blmem.y = -y;
                            Y = Point1.Y - y;
                        }
                        else
                        {
                            blmem.y = y;
                            Y = Point1.Y + y;
                        }

                        if (Point1.Z > Point2.Z)
                        {
                            blmem.z = -z;
                            Z = Point1.Z - z;
                        }
                        else
                        {
                            blmem.z = z;
                            Z = Point1.Z + z;
                        }
                        blmem.id = subsystemTerrain.Terrain.GetCellValueFast(X, Y, Z);


                        engineBinaryWriter.Write(blmem.x);
                        engineBinaryWriter.Write(blmem.y);
                        engineBinaryWriter.Write(blmem.z);
                        engineBinaryWriter.Write(blmem.id);

                        ++blockCount;


                        BlockList.Add(blmem);
                        blockCount++;
                    }
                }
            }
            player.ComponentGui.DisplaySmallMessage(string.Format("Sucsses {0} place blocks", blockCount), true, true);
            engineBinaryWriter.BaseStream.Position = 7L;
            engineBinaryWriter.Write(blockCount);
            engineBinaryWriter.Dispose();
            fileStream.Dispose();
        }










        public static void Paste_zone(Point3 Point3, SubsystemTerrain subsystemTerrain, ComponentPlayer player, string path)
        {

            FileStream fileStream = new FileStream(path, FileMode.Open);

            BinaryReader engineBinaryReader = new BinaryReader(fileStream);




            engineBinaryReader.BaseStream.Position = 7L;

            int num2 = engineBinaryReader.ReadInt32();
            try
            {

                for (var i = 0; i < num2; i++)
                {

                    var xPos = Point3.X + engineBinaryReader.ReadInt32();
                    var yPos = Point3.Y + engineBinaryReader.ReadInt32();
                    var zPos = Point3.Z + engineBinaryReader.ReadInt32();
                    var id = engineBinaryReader.ReadInt32();

                    subsystemTerrain.ChangeCell(xPos, yPos, zPos, id);





                }
            }
            catch (Exception ex)
            {
                player.ComponentGui.DisplaySmallMessage(string.Format("Sucsses {0} place blocks", num2), true, true);
            }

            engineBinaryReader.Dispose();
            fileStream.Dispose();




        }







    }

    public class BlockMem
    {
        internal int x;
        internal int y;
        internal int z;
        internal int id;
    }

    public class Category
    {
        //public Color Color = Color.White;
        public string Name;

    }

    public enum Position
    {
        flat,
        X,
        Y,

    }
}
