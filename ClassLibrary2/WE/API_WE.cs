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

namespace API_WE_Mod
{
    public class API_WE
    {

        internal static int blockCount;
        internal static List<BlockMem> BlockList = new List<BlockMem>();



       // public static readonly string sdcard = "/sdcard/Survivalcraft/WorldEdit";

        // public readonly string oneKeyFile = 
        // public readonly string CopyFile = string.Format("{0}/Cache/Cache.w", (object));






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
                            if((int)Math.Sqrt((double)(index2 * index2 + index3 * index3)) <= radius && (!s || (int)Math.Sqrt((double)((index2 - 1) * (index2 - 1) + index3 * index3)) > radius || (int)Math.Sqrt((double)(index2 * index2 + (index3 - 1) * (index3 - 1))) > radius))
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

            FileStream fileStream = new FileStream(path,FileMode.Create);
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
            catch(Exception ex)
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
