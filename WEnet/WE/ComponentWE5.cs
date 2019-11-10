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
using System.IO;
using Engine.Serialization;
using Engine.Audio;
using Engine.Media;
using Engine.Content;
using System.Linq;
using API_WE_Mod;

namespace API_WE_Mod
{
    class ComponentWE5 : Component, IDrawable, IUpdateable

    {
        private static int[] m_drawOrders = new int[1] { 2000 };
        public static ComponentPlayer m_componentPlayer;
        public static SubsystemTerrain m_subsystemTerrain;


        private List<string> names = new List<string>();
        private List<Category> m_categories = new List<Category>();


        public static SubsystemGameInfo m_subsystemGameInfo;
        public static BitmapButtonWidget F1;
        public static BitmapButtonWidget F2;
        public static BitmapButtonWidget F3;
        public static BitmapButtonWidget F5;
        public static BitmapButtonWidget F6;
        public static BitmapButtonWidget F7;
        public static BitmapButtonWidget F8;
        public static BitmapButtonWidget F9;
        public static BitmapButtonWidget F10;
        public static BitmapButtonWidget F11;
        public static BitmapButtonWidget F12;
        public static BitmapButtonWidget WorldEditMenu;
        public static StackPanelWidget WorldEditMenuContainerTop;
        public static StackPanelWidget WorldEditMenuContainerBottom;
        List<Camera> m_cameras = new List<Camera>();

        public static int speed = 100;

        //private static List<BlockMem> BlockList;
        public readonly PrimitivesRenderer2D PrimitivesRenderer2D;
        public readonly PrimitivesRenderer3D PrimitivesRenderer3D;
        private static int OldLookControlMode;
        private static TerrainRaycastResult? Point1;
        private static TerrainRaycastResult? Point2;
        private static TerrainRaycastResult? Point3;
        private static int SelectedBlock;
        private static int ReplaceableBlock;
        internal static int blockCount;
        internal static List<BlockMem> BlockList = new List<BlockMem>();
        //private static int blockCount;
        private Camera cam;
        //private TerrainRaycastResult terrainRaycastResult;

        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            base.Load(valuesDictionary, idToEntityMap);
            m_componentPlayer = Entity.FindComponent<ComponentPlayer>(true);
            //pilot = Entity.FindComponent<ComponentPilot>(true);
            m_subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(true);
            m_subsystemGameInfo = Project.FindSubsystem<SubsystemGameInfo>(true);

            names.Add("Image");
            names.Add("Round");
            names.Add("Sphere");
            names.Add("Prism");
            names.Add("Square");
            names.Add("Frame or box");
            names.Add("Maze");
            names.Add("Mountain");
            names.Add("Line");
            names.Add("Coppy/Paste zone in file");
            names.Add("Fast Run");


            m_cameras.Add(new DebugCamera(m_componentPlayer.View));



            Directory.CreateDirectory(ZoneDialog.Path_mod);


            foreach (string category in names)
                m_categories.Add(new Category()
                {
                    Name = category,

                });



            this.LoadBTN();

        }

        public void SetCell(int x, int y, int z, int id)
        {
            m_subsystemTerrain.ChangeCell(x, y, z, id, true);
        }


        public void Draw(Camera camera, int drawOrder)
        {

            this.cam = camera;
            if (camera.View != m_componentPlayer.View)
                return;

            if (!camera.UsesMovementControls)
            {
                if (Point1.HasValue && Point2.HasValue && WorldEditMenu.IsChecked) // Выделение зоны между точками 1 и 2
                {
                    int startX = Math.Min(Point1.Value.CellFace.Point.X, Point2.Value.CellFace.Point.X);
                    int endX = Math.Max(Point1.Value.CellFace.Point.X, Point2.Value.CellFace.Point.X);
                    int startY = Math.Min(Point1.Value.CellFace.Point.Y, Point2.Value.CellFace.Point.Y);
                    int endY = Math.Max(Point1.Value.CellFace.Point.Y, Point2.Value.CellFace.Point.Y);
                    int startZ = Math.Min(Point1.Value.CellFace.Point.Z, Point2.Value.CellFace.Point.Z);
                    int endZ = Math.Max(Point1.Value.CellFace.Point.Z, Point2.Value.CellFace.Point.Z);

                    PrimitivesRenderer3D PrimitivesRenderer3D = new PrimitivesRenderer3D();
                    Vector3 pointStart = new Vector3(startX, startY, startZ);
                    Vector3 pointEnd = new Vector3(endX + 1, endY + 1, endZ + 1);
                    BoundingBox boundingBox = new BoundingBox(pointStart, pointEnd);
                    PrimitivesRenderer3D.FlatBatch(-1, DepthStencilState.None, (RasterizerState)null, (BlendState)null).QueueBoundingBox(boundingBox, Color.Green);
                    PrimitivesRenderer3D.Flush(cam.ViewProjectionMatrix, true);
                }

                if (Point3.HasValue && WorldEditMenu.IsChecked) // Выделение зоны вставки
                {

                    int startX = Math.Min(Point1.Value.CellFace.Point.X, Point2.Value.CellFace.Point.X);
                    int endX = Math.Max(Point1.Value.CellFace.Point.X, Point2.Value.CellFace.Point.X);
                    int startY = Math.Min(Point1.Value.CellFace.Point.Y, Point2.Value.CellFace.Point.Y);
                    int endY = Math.Max(Point1.Value.CellFace.Point.Y, Point2.Value.CellFace.Point.Y);
                    int startZ = Math.Min(Point1.Value.CellFace.Point.Z, Point2.Value.CellFace.Point.Z);
                    int endZ = Math.Max(Point1.Value.CellFace.Point.Z, Point2.Value.CellFace.Point.Z);

                    startX += Point3.Value.CellFace.Point.X - Point1.Value.CellFace.Point.X;
                    startY += Point3.Value.CellFace.Point.Y - Point1.Value.CellFace.Point.Y;
                    startZ += Point3.Value.CellFace.Point.Z - Point1.Value.CellFace.Point.Z;
                    endX += Point3.Value.CellFace.Point.X - Point1.Value.CellFace.Point.X;
                    endY += Point3.Value.CellFace.Point.Y - Point1.Value.CellFace.Point.Y;
                    endZ += Point3.Value.CellFace.Point.Z - Point1.Value.CellFace.Point.Z;

                    PrimitivesRenderer3D primitivesRenderer3D = new PrimitivesRenderer3D();
                    Vector3 pointStart = new Vector3(startX, startY, startZ);
                    Vector3 pointEnd = new Vector3(endX + 1, endY + 1, endZ + 1);
                    BoundingBox boundingBox = new BoundingBox(pointStart, pointEnd);
                    primitivesRenderer3D.FlatBatch(-1, DepthStencilState.None, (RasterizerState)null, (BlendState)null).QueueBoundingBox(boundingBox, Color.Red);
                    primitivesRenderer3D.Flush(cam.ViewProjectionMatrix, true);
                }
            }
            /*
            PrimitivesRenderer3D.Flush(camera.ViewProjectionMatrix, true, int.MaxValue);
            PrimitivesRenderer2D.Flush(true);
            */


        }

        public T FindCamera<T>(bool throwOnError = true) where T : Camera
        {
            T obj = (T)m_cameras.FirstOrDefault(c => c is T);
            if ((object)obj != null || !throwOnError)
                return obj;
            throw new InvalidOperationException(string.Format("Camera with type \"{0}\" not found.", typeof(T).Name));
        }

        private static float ProcessInputValue(float value, float deadZone, float saturationZone)
        {
            return MathUtils.Sign(value) * MathUtils.Clamp((float)(((double)MathUtils.Abs(value) - (double)deadZone) / ((double)saturationZone - (double)deadZone)), 0.0f, 1f);
        }

        public void Update(float dt)
        {
            WE();

            if (Keyboard.IsKeyDownOnce(Engine.Input.Key.F11))
            {
                //API_WE.Round(true,25,13,true,ZuoBiaoType.Z,15,Point1);
                Select_mode(m_categories, names);



                // m_componentPlayer.View.ActiveCamera = FindCamera<DebugCamera>();


            }

            if (F12.IsChecked)
            {
                if (speed != 0)
                {

                    if (SettingsManager.MoveControlMode == MoveControlMode.Buttons)
                    {
                        MoveRoseWidget moveRoseWidget = m_componentPlayer.ComponentGui.MoveRoseWidget;
                        if (moveRoseWidget == null || (double)moveRoseWidget.Direction.Z <= 0.0)
                            return;
                        Vector3 viewDirection = m_componentPlayer.ComponentBody.Velocity;
                        m_componentPlayer.ComponentBody.Velocity = new Vector3(viewDirection.X * speed, viewDirection.Y * speed, viewDirection.Z * speed);
                    }

                    if (SettingsManager.MoveControlMode == MoveControlMode.Pad)
                    {
                        TouchInput? input = m_componentPlayer.ComponentGui.MoveWidget.TouchInput;
                        if (!input.HasValue || input.Value.InputType != TouchInputType.Move && input.Value.InputType != TouchInputType.Hold || (double)ProcessInputValue(-input.Value.TotalMoveLimited.Y, 0.2f * m_componentPlayer.ComponentGui.MoveWidget.Radius, m_componentPlayer.ComponentGui.MoveWidget.Radius) <= 0.0)
                            return;
                        Vector3 viewDirection = cam.ViewDirection;
                        m_componentPlayer.ComponentBody.Velocity = new Vector3(viewDirection.X * speed, viewDirection.Y * speed, viewDirection.Z * speed);
                    }

                    else
                    {

                        if (Keyboard.IsKeyDown(Key.W) || Keyboard.IsKeyDown(Key.A) || Keyboard.IsKeyDown(Key.S) || Keyboard.IsKeyDown(Key.D))
                        {

                            Vector3 viewDirection = cam.ViewDirection;
                            m_componentPlayer.ComponentBody.Velocity = new Vector3(viewDirection.X * speed, viewDirection.Y * speed, viewDirection.Z * speed);
                        }

                    }
                }
            }


            if (F12.IsClicked)
            {
                //  DialogsManager.ShowDialog(m_componentPlayer.View.GameWidget, new FastRun(m_componentPlayer));
            }

            if (F11.IsClicked)
            {
                Select_mode(m_categories, names);
            }


            if (Keyboard.IsKeyDownOnce(Engine.Input.Key.F12))
            {




                if (m_componentPlayer.View.ActiveCamera.GetType() == typeof(FppCamera))
                {
                    m_componentPlayer.View.ActiveCamera = FindCamera<DebugCamera>();

                }
                else
                {
                    m_componentPlayer.View.ActiveCamera = m_componentPlayer.View.FindCamera<FppCamera>();

                }
                //  DialogsManager.ShowDialog(m_componentPlayer.View.GameWidget, new FastRun(m_componentPlayer));

            }


            if ((Engine.Input.Keyboard.IsKeyDownOnce(Engine.Input.Key.F9)) || F9.IsClicked) // Копирование зоны в память
            {
                if (Point1 == null)
                {
                    m_componentPlayer.ComponentGui.DisplaySmallMessage("You have not selected point 1", false, false);
                }
                else if (Point2 == null)
                {
                    m_componentPlayer.ComponentGui.DisplaySmallMessage("You have not selected point 2", false, false);
                }
                else
                {
                    int startX = Math.Min(Point1.Value.CellFace.Point.X, Point2.Value.CellFace.Point.X);
                    int endX = Math.Max(Point1.Value.CellFace.Point.X, Point2.Value.CellFace.Point.X);
                    int startY = Math.Min(Point1.Value.CellFace.Point.Y, Point2.Value.CellFace.Point.Y);
                    int endY = Math.Max(Point1.Value.CellFace.Point.Y, Point2.Value.CellFace.Point.Y);
                    int startZ = Math.Min(Point1.Value.CellFace.Point.Z, Point2.Value.CellFace.Point.Z);
                    int endZ = Math.Max(Point1.Value.CellFace.Point.Z, Point2.Value.CellFace.Point.Z);

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
                                if (Point1.Value.CellFace.Point.X > Point2.Value.CellFace.Point.X)
                                {
                                    blmem.x = -x;
                                    X = Point1.Value.CellFace.Point.X - x;
                                }
                                else
                                {
                                    blmem.x = x;
                                    X = Point1.Value.CellFace.Point.X + x;
                                }

                                if (Point1.Value.CellFace.Point.Y > Point2.Value.CellFace.Point.Y)
                                {
                                    blmem.y = -y;
                                    Y = Point1.Value.CellFace.Point.Y - y;
                                }
                                else
                                {
                                    blmem.y = y;
                                    Y = Point1.Value.CellFace.Point.Y + y;
                                }

                                if (Point1.Value.CellFace.Point.Z > Point2.Value.CellFace.Point.Z)
                                {
                                    blmem.z = -z;
                                    Z = Point1.Value.CellFace.Point.Z - z;
                                }
                                else
                                {
                                    blmem.z = z;
                                    Z = Point1.Value.CellFace.Point.Z + z;
                                }

                                blmem.id = m_subsystemTerrain.Terrain.GetCellValue(X, Y, Z);
                                BlockList.Add(blmem);
                                blockCount++;
                            }
                        }
                    }
                }
                m_componentPlayer.ComponentGui.DisplaySmallMessage("Copied " + blockCount + " blocks", false, false);
                return;
            }

            if ((Engine.Input.Keyboard.IsKeyDownOnce(Engine.Input.Key.F10)) || F10.IsClicked)  // Вставка зоны из памяти
            {
                if (Point3 == null) m_componentPlayer.ComponentGui.DisplaySmallMessage("You have not selected point 3", false, false);
                else
                {
                    for (var i = 0; i < blockCount; i++)
                    {
                        var xPos = Point3.Value.CellFace.X + BlockList[i].x;
                        var yPos = Point3.Value.CellFace.Y + BlockList[i].y;
                        var zPos = Point3.Value.CellFace.Z + BlockList[i].z;

                        m_subsystemTerrain.ChangeCell(xPos, yPos, zPos, BlockList[i].id);
                    }
                    m_componentPlayer.ComponentGui.DisplaySmallMessage("Pasted " + blockCount + " blocks", false, false);
                }
            }














        }


        public void Select_mode(List<Category> m_categories, List<string> a)
        {

            if (m_componentPlayer != null)
                DialogsManager.ShowDialog(m_componentPlayer.View.GameWidget, new ListSelectionDialog(string.Empty, m_categories, 56f, c =>
                {
                    LabelWidget labelWidget = new LabelWidget();
                    labelWidget.Text = ((Category)c).Name;
                    labelWidget.Color = Color.White;
                    int num1 = 1;
                    labelWidget.HorizontalAlignment = (WidgetAlignment)num1;
                    int num2 = 1;
                    labelWidget.VerticalAlignment = (WidgetAlignment)num2;
                    return labelWidget;
                }, c =>
                {
                    if (c == null)
                        return;
                    int d = m_categories.IndexOf((Category)c);
                    string ds = a[d];

                    if (ds == "Image")
                    {


                        DialogsManager.ShowDialog(m_componentPlayer.View.GameWidget, new ImageDialog(m_componentPlayer, Point1, Point2, Point3, m_subsystemTerrain));
                        // API_WE.draw_img(new Point3(Point1.Value.CellFace.X, Point1.Value.CellFace.Y, Point1.Value.CellFace.Z), m_subsystemTerrain);


                    }

                    if (ds == "Round")
                    {
                        if (Point1 == null) m_componentPlayer.ComponentGui.DisplaySmallMessage("You have not selected point 1", false, false);
                        else
                        {
                            DialogsManager.ShowDialog(m_componentPlayer.View.GameWidget, new Round(m_componentPlayer, SelectedBlock, Point1, m_subsystemTerrain));
                        }

                    }

                    if (ds == "Sphere")
                    {
                        if (Point1 == null) m_componentPlayer.ComponentGui.DisplaySmallMessage("You have not selected point 1", false, false);
                        else
                        {
                            DialogsManager.ShowDialog(m_componentPlayer.View.GameWidget, new Sphere(m_componentPlayer, SelectedBlock, Point1, m_subsystemTerrain));
                        }

                    }
                    if (ds == "Prism")
                    {
                        if (Point1 == null) m_componentPlayer.ComponentGui.DisplaySmallMessage("You have not selected point 1", false, false);
                        else
                        {
                            DialogsManager.ShowDialog(m_componentPlayer.View.GameWidget, new Prism(m_componentPlayer, SelectedBlock, Point1, m_subsystemTerrain));
                        }

                    }

                    if (ds == "Square")
                    {
                        if (Point1 == null) m_componentPlayer.ComponentGui.DisplaySmallMessage("You have not selected point 1", false, false);
                        else
                        {
                            DialogsManager.ShowDialog(m_componentPlayer.View.GameWidget, new Square(m_componentPlayer, SelectedBlock, Point1, m_subsystemTerrain));
                        }

                    }

                    if (ds == "Frame or box")
                    {

                        if (Point1 == null && Point2 == null) m_componentPlayer.ComponentGui.DisplaySmallMessage("You have not selected points 1,2", false, false);
                        else
                        {
                            DialogsManager.ShowDialog(m_componentPlayer.View.GameWidget, new API_WE_Mod.Rectangle(m_componentPlayer, SelectedBlock, Point1, Point2, m_subsystemTerrain));
                        }




                    }

                    if (ds == "Maze")
                    {

                        Engine.Point3 PointStart;
                        Engine.Point3 PointEnd;

                        if (Point1 == null && Point2 == null)
                        {
                            m_componentPlayer.ComponentGui.DisplaySmallMessage("You have not selected points 1,2", false, false);
                        }

                        else
                        {

                            PointStart.X = Point1.Value.CellFace.X;
                            PointStart.Y = Point1.Value.CellFace.Y;
                            PointStart.Z = Point1.Value.CellFace.Z;

                            PointEnd.X = Point2.Value.CellFace.X;
                            PointEnd.Y = Point2.Value.CellFace.Y;
                            PointEnd.Z = Point2.Value.CellFace.Z;

                            API_WE.CreativeMaze(PointStart, PointEnd, SelectedBlock, m_subsystemTerrain, m_componentPlayer);
                        }








                    }

                    if (ds == "Mountain")
                    {
                        if (Point1 == null) m_componentPlayer.ComponentGui.DisplaySmallMessage("You have not selected point 1", false, false);
                        else
                        {
                            DialogsManager.ShowDialog(m_componentPlayer.View.GameWidget, new Mountain(m_componentPlayer, Point1, m_subsystemTerrain));
                        }


                    }

                    if (ds == "Coppy/Paste zone in file")
                    {
                        /*
                        Engine.Point3 PointStart;

                        PointStart.X = Point3.Value.CellFace.X;
                        PointStart.Y = Point3.Value.CellFace.Y;
                        PointStart.Z = Point3.Value.CellFace.Z;
                        */

                        //m_componentPlayer.ComponentGui.ModalPanelWidget = new ZoneWidget();


                        DialogsManager.ShowDialog(m_componentPlayer.View.GameWidget, new ZoneDialog(m_componentPlayer, Point1, Point2, Point3, m_subsystemTerrain));


                        //string o =  API_WE.s(names);




                    }


                    if (ds == "Line")
                    {
                        Engine.Point3 PointStart = new Engine.Point3();
                        Engine.Point3 PointEnd = new Engine.Point3();

                        if (Point1 == null && Point2 == null)
                        {
                            m_componentPlayer.ComponentGui.DisplaySmallMessage("You have not selected points 1,2", false, false);
                        }

                        else
                        {

                            PointStart.X = Point1.Value.CellFace.X;
                            PointStart.Y = Point1.Value.CellFace.Y;
                            PointStart.Z = Point1.Value.CellFace.Z;

                            PointEnd.X = Point2.Value.CellFace.X;
                            PointEnd.Y = Point2.Value.CellFace.Y;
                            PointEnd.Z = Point2.Value.CellFace.Z;

                            API_WE.LinePoint(PointStart, PointEnd, SelectedBlock, m_subsystemTerrain);
                        }


                    }

                    if (ds == "Fast Run")
                    {

                        DialogsManager.ShowDialog(m_componentPlayer.View.GameWidget, new FastRun(m_componentPlayer));

                    }








                }));
        }

        public void WE()
        {

            if (!(m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative))
            {
                WorldEditMenu.IsVisible = false;
            }
            else
            {
                WorldEditMenu.IsVisible = true;
            }

            WorldEditMenuContainerBottom.IsVisible = WorldEditMenu.IsChecked;
            WorldEditMenuContainerTop.IsVisible = WorldEditMenu.IsChecked;


            if (WorldEditMenu.IsClicked)
            {
                if (WorldEditMenu.IsChecked)
                {
                    OldLookControlMode = (int)SettingsManager.LookControlMode;
                    SettingsManager.LookControlMode = LookControlMode.SplitTouch;
                }
                else
                {
                    SettingsManager.LookControlMode = (LookControlMode)OldLookControlMode;
                }
            }


            //Vector3 viewDirection = cam.ViewDirection;

            //m_componentPlayer.ComponentBody.Velocity = new Vector3(viewDirection.X * 100f, viewDirection.Y * 100f, viewDirection.Z * 100f);

            if (Keyboard.IsKeyDownOnce(Engine.Input.Key.F1) || F1.IsClicked)
            {
                Point1 = m_componentPlayer.ComponentMiner.PickTerrainForDigging(this.cam.ViewPosition, this.cam.ViewDirection);
                if (Point1.HasValue)
                {
                    SelectedBlock = m_subsystemTerrain.Terrain.GetCellValue(Point1.Value.CellFace.X, Point1.Value.CellFace.Y, Point1.Value.CellFace.Z);
                    m_componentPlayer.ComponentGui.DisplaySmallMessage("Set position 1 on: " + (object)Point1.Value.CellFace.X + ", " + (object)Point1.Value.CellFace.Y + ", " + (object)Point1.Value.CellFace.Z + ",Block ID " + (object)SelectedBlock, false, false);
                }
            }
            if (Keyboard.IsKeyDown(Engine.Input.Key.F2) || F2.IsClicked)
            {
                Point2 = m_componentPlayer.ComponentMiner.PickTerrainForDigging(this.cam.ViewPosition, this.cam.ViewDirection);
                if (Point2.HasValue)
                {
                    ReplaceableBlock = m_subsystemTerrain.Terrain.GetCellValue(Point2.Value.CellFace.X, Point2.Value.CellFace.Y, Point2.Value.CellFace.Z);
                    m_componentPlayer.ComponentGui.DisplaySmallMessage("Set position 2 on: " + (object)Point2.Value.CellFace.X + ", " + (object)Point2.Value.CellFace.Y + ", " + (object)Point2.Value.CellFace.Z + ",Block ID " + (object)ReplaceableBlock, false, false);
                }
            }
            if (Keyboard.IsKeyDown(Engine.Input.Key.F3) || F3.IsClicked)
            {
                Point3 = m_componentPlayer.ComponentMiner.PickTerrainForDigging(this.cam.ViewPosition, this.cam.ViewDirection);
                if (Point3.HasValue)
                {
                    m_componentPlayer.ComponentGui.DisplaySmallMessage("Set position 3 on: " + (object)Point1.Value.CellFace.X + ", " + (object)Point1.Value.CellFace.Y + ", " + (object)Point1.Value.CellFace.Z, false, false);
                    return;
                }
            }


            if (Keyboard.IsKeyDownOnce(Engine.Input.Key.F5) || F5.IsClicked)
            {
                if (!Point1.HasValue)
                    m_componentPlayer.ComponentGui.DisplaySmallMessage("You have not selected point 1", false, false);
                else if (!Point2.HasValue)
                    m_componentPlayer.ComponentGui.DisplaySmallMessage("You have not selected point 2", false, false);
                else if (!Point3.HasValue)
                {
                    m_componentPlayer.ComponentGui.DisplaySmallMessage("You have not selected point 3", false, false);
                }
                else
                {
                    TerrainRaycastResult terrainRaycastResult = Point1.Value;
                    int x1 = terrainRaycastResult.CellFace.Point.X;
                    terrainRaycastResult = Point2.Value;
                    int x2 = terrainRaycastResult.CellFace.Point.X;
                    int num1 = Math.Min(x1, x2);
                    terrainRaycastResult = Point1.Value;
                    int x3 = terrainRaycastResult.CellFace.Point.X;
                    terrainRaycastResult = Point2.Value;
                    int x4 = terrainRaycastResult.CellFace.Point.X;
                    int num2 = Math.Max(x3, x4);
                    terrainRaycastResult = Point1.Value;
                    int y1 = terrainRaycastResult.CellFace.Point.Y;
                    terrainRaycastResult = Point2.Value;
                    int y2 = terrainRaycastResult.CellFace.Point.Y;
                    int num3 = Math.Min(y1, y2);
                    terrainRaycastResult = Point1.Value;
                    int y3 = terrainRaycastResult.CellFace.Point.Y;
                    terrainRaycastResult = Point2.Value;
                    int y4 = terrainRaycastResult.CellFace.Point.Y;
                    int num4 = Math.Max(y3, y4);
                    terrainRaycastResult = Point1.Value;
                    int z1 = terrainRaycastResult.CellFace.Point.Z;
                    terrainRaycastResult = Point2.Value;
                    int z2 = terrainRaycastResult.CellFace.Point.Z;
                    int num5 = Math.Min(z1, z2);
                    terrainRaycastResult = Point1.Value;
                    int z3 = terrainRaycastResult.CellFace.Point.Z;
                    terrainRaycastResult = Point2.Value;
                    int z4 = terrainRaycastResult.CellFace.Point.Z;
                    int num6 = Math.Max(z3, z4);
                    for (int index1 = 0; index1 <= num4 - num3; ++index1)
                    {
                        for (int index2 = 0; index2 <= num6 - num5; ++index2)
                        {
                            for (int index3 = 0; index3 <= num2 - num1; ++index3)
                            {
                                terrainRaycastResult = Point1.Value;
                                int x5 = terrainRaycastResult.CellFace.Point.X;
                                terrainRaycastResult = Point2.Value;
                                int x6 = terrainRaycastResult.CellFace.Point.X;
                                int x7;
                                int x8;
                                if (x5 > x6)
                                {
                                    terrainRaycastResult = Point1.Value;
                                    x7 = terrainRaycastResult.CellFace.Point.X - index3;
                                    terrainRaycastResult = Point3.Value;
                                    x8 = terrainRaycastResult.CellFace.Point.X - index3;
                                }
                                else
                                {
                                    terrainRaycastResult = Point1.Value;
                                    x7 = terrainRaycastResult.CellFace.Point.X + index3;
                                    terrainRaycastResult = Point3.Value;
                                    x8 = terrainRaycastResult.CellFace.Point.X + index3;
                                }
                                terrainRaycastResult = Point1.Value;
                                int y5 = terrainRaycastResult.CellFace.Point.Y;
                                terrainRaycastResult = Point2.Value;
                                int y6 = terrainRaycastResult.CellFace.Point.Y;
                                int y7;
                                int y8;
                                if (y5 > y6)
                                {
                                    terrainRaycastResult = Point1.Value;
                                    y7 = terrainRaycastResult.CellFace.Point.Y - index1;
                                    terrainRaycastResult = Point3.Value;
                                    y8 = terrainRaycastResult.CellFace.Point.Y - index1;
                                }
                                else
                                {
                                    terrainRaycastResult = Point1.Value;
                                    y7 = terrainRaycastResult.CellFace.Point.Y + index1;
                                    terrainRaycastResult = Point3.Value;
                                    y8 = terrainRaycastResult.CellFace.Point.Y + index1;
                                }
                                terrainRaycastResult = Point1.Value;
                                int z5 = terrainRaycastResult.CellFace.Point.Z;
                                terrainRaycastResult = Point2.Value;
                                int z6 = terrainRaycastResult.CellFace.Point.Z;
                                int z7;
                                int z8;
                                if (z5 > z6)
                                {
                                    terrainRaycastResult = Point1.Value;
                                    z7 = terrainRaycastResult.CellFace.Point.Z - index2;
                                    terrainRaycastResult = Point3.Value;
                                    z8 = terrainRaycastResult.CellFace.Point.Z - index2;
                                }
                                else
                                {
                                    terrainRaycastResult = Point1.Value;
                                    z7 = terrainRaycastResult.CellFace.Point.Z + index2;
                                    terrainRaycastResult = Point3.Value;
                                    z8 = terrainRaycastResult.CellFace.Point.Z + index2;
                                }
                                int cellValue = m_subsystemTerrain.Terrain.GetCellValue(x7, y7, z7);
                                if (cellValue != 15360)
                                    this.SetCell(x8, y8, z8, cellValue);
                            }
                        }
                    }
                }
            }
            else if (Keyboard.IsKeyDownOnce(Engine.Input.Key.F6) || F6.IsClicked)
            {
                if (!Point1.HasValue)
                    m_componentPlayer.ComponentGui.DisplaySmallMessage("You have not selected point 1", false, false);
                else if (!Point2.HasValue)
                {
                    m_componentPlayer.ComponentGui.DisplaySmallMessage("You have not selected point 2", false, false);
                }
                else
                {
                    TerrainRaycastResult terrainRaycastResult = Point1.Value;
                    int x1 = terrainRaycastResult.CellFace.Point.X;
                    terrainRaycastResult = Point2.Value;
                    int x2 = terrainRaycastResult.CellFace.Point.X;
                    int num1 = Math.Min(x1, x2);
                    terrainRaycastResult = Point1.Value;
                    int x3 = terrainRaycastResult.CellFace.Point.X;
                    terrainRaycastResult = Point2.Value;
                    int x4 = terrainRaycastResult.CellFace.Point.X;
                    int num2 = Math.Max(x3, x4);
                    terrainRaycastResult = Point1.Value;
                    int y1 = terrainRaycastResult.CellFace.Point.Y;
                    terrainRaycastResult = Point2.Value;
                    int y2 = terrainRaycastResult.CellFace.Point.Y;
                    int num3 = Math.Min(y1, y2);
                    terrainRaycastResult = Point1.Value;
                    int y3 = terrainRaycastResult.CellFace.Point.Y;
                    terrainRaycastResult = Point2.Value;
                    int y4 = terrainRaycastResult.CellFace.Point.Y;
                    int num4 = Math.Max(y3, y4);
                    terrainRaycastResult = Point1.Value;
                    int z1 = terrainRaycastResult.CellFace.Point.Z;
                    terrainRaycastResult = Point2.Value;
                    int z2 = terrainRaycastResult.CellFace.Point.Z;
                    int num5 = Math.Min(z1, z2);
                    terrainRaycastResult = Point1.Value;
                    int z3 = terrainRaycastResult.CellFace.Point.Z;
                    terrainRaycastResult = Point2.Value;
                    int z4 = terrainRaycastResult.CellFace.Point.Z;
                    int num6 = Math.Max(z3, z4);
                    for (int x5 = num1; x5 <= num2; ++x5)
                    {
                        for (int y5 = num3; y5 <= num4; ++y5)
                        {
                            for (int z5 = num5; z5 <= num6; ++z5)
                                this.SetCell(x5, y5, z5, SelectedBlock);
                        }
                    }
                }
            }
            else if (Keyboard.IsKeyDownOnce(Engine.Input.Key.F7) || F7.IsClicked)
            {
                if (!Point1.HasValue)
                    m_componentPlayer.ComponentGui.DisplaySmallMessage("You have not selected point 1", false, false);
                else if (!Point2.HasValue)
                {
                    m_componentPlayer.ComponentGui.DisplaySmallMessage("You have not selected point 2", false, false);
                }
                else
                {
                    TerrainRaycastResult terrainRaycastResult = Point1.Value;
                    int x1 = terrainRaycastResult.CellFace.Point.X;
                    terrainRaycastResult = Point2.Value;
                    int x2 = terrainRaycastResult.CellFace.Point.X;
                    int num1 = Math.Min(x1, x2);
                    terrainRaycastResult = Point1.Value;
                    int x3 = terrainRaycastResult.CellFace.Point.X;
                    terrainRaycastResult = Point2.Value;
                    int x4 = terrainRaycastResult.CellFace.Point.X;
                    int num2 = Math.Max(x3, x4);
                    terrainRaycastResult = Point1.Value;
                    int y1 = terrainRaycastResult.CellFace.Point.Y;
                    terrainRaycastResult = Point2.Value;
                    int y2 = terrainRaycastResult.CellFace.Point.Y;
                    int num3 = Math.Min(y1, y2);
                    terrainRaycastResult = Point1.Value;
                    int y3 = terrainRaycastResult.CellFace.Point.Y;
                    terrainRaycastResult = Point2.Value;
                    int y4 = terrainRaycastResult.CellFace.Point.Y;
                    int num4 = Math.Max(y3, y4);
                    terrainRaycastResult = Point1.Value;
                    int z1 = terrainRaycastResult.CellFace.Point.Z;
                    terrainRaycastResult = Point2.Value;
                    int z2 = terrainRaycastResult.CellFace.Point.Z;
                    int num5 = Math.Min(z1, z2);
                    terrainRaycastResult = Point1.Value;
                    int z3 = terrainRaycastResult.CellFace.Point.Z;
                    terrainRaycastResult = Point2.Value;
                    int z4 = terrainRaycastResult.CellFace.Point.Z;
                    int num6 = Math.Max(z3, z4);
                    for (int x5 = num1; x5 <= num2; ++x5)
                    {
                        for (int y5 = num3; y5 <= num4; ++y5)
                        {
                            for (int z5 = num5; z5 <= num6; ++z5)
                            {
                                if (m_subsystemTerrain.Terrain.GetCellValue(x5, y5, z5) == ReplaceableBlock)
                                    this.SetCell(x5, y5, z5, SelectedBlock);
                            }
                        }
                    }
                }
            }
            else
            {
                if (!Keyboard.IsKeyDownOnce(Engine.Input.Key.F8) && !F8.IsClicked)
                    return;
                if (!Point1.HasValue)
                    m_componentPlayer.ComponentGui.DisplaySmallMessage("You have not selected point 1", false, false);
                else if (!Point2.HasValue)
                {
                    m_componentPlayer.ComponentGui.DisplaySmallMessage("You have not selected point 2", false, false);
                }
                else
                {
                    TerrainRaycastResult terrainRaycastResult = Point1.Value;
                    int x1 = terrainRaycastResult.CellFace.Point.X;
                    terrainRaycastResult = Point2.Value;
                    int x2 = terrainRaycastResult.CellFace.Point.X;
                    int num1 = Math.Min(x1, x2);
                    terrainRaycastResult = Point1.Value;
                    int x3 = terrainRaycastResult.CellFace.Point.X;
                    terrainRaycastResult = Point2.Value;
                    int x4 = terrainRaycastResult.CellFace.Point.X;
                    int num2 = Math.Max(x3, x4);
                    terrainRaycastResult = Point1.Value;
                    int y1 = terrainRaycastResult.CellFace.Point.Y;
                    terrainRaycastResult = Point2.Value;
                    int y2 = terrainRaycastResult.CellFace.Point.Y;
                    int num3 = Math.Min(y1, y2);
                    terrainRaycastResult = Point1.Value;
                    int y3 = terrainRaycastResult.CellFace.Point.Y;
                    terrainRaycastResult = Point2.Value;
                    int y4 = terrainRaycastResult.CellFace.Point.Y;
                    int num4 = Math.Max(y3, y4);
                    terrainRaycastResult = Point1.Value;
                    int z1 = terrainRaycastResult.CellFace.Point.Z;
                    terrainRaycastResult = Point2.Value;
                    int z2 = terrainRaycastResult.CellFace.Point.Z;
                    int num5 = Math.Min(z1, z2);
                    terrainRaycastResult = Point1.Value;
                    int z3 = terrainRaycastResult.CellFace.Point.Z;
                    terrainRaycastResult = Point2.Value;
                    int z4 = terrainRaycastResult.CellFace.Point.Z;
                    int num6 = Math.Max(z3, z4);
                    for (int x5 = num1; x5 <= num2; ++x5)
                    {
                        for (int y5 = num3; y5 <= num4; ++y5)
                        {
                            for (int z5 = num5; z5 <= num6; ++z5)
                                this.SetCell(x5, y5, z5, 0);
                        }
                    }
                }
            }




        }




        public void lingTi(bool s, int f, int blockID, int r, TerrainRaycastResult? Point1)
        {


            int num = 0;
            for (int index1 = -r; index1 <= r; ++index1)
            {
                for (int index2 = -r; index2 <= r; ++index2)
                {
                    for (int index3 = -r; index3 <= r; ++index3)
                    {

                        if (f == 1)
                        {
                            if (Math.Abs(index1) + Math.Abs(index2) <= r && Math.Abs(index3) + Math.Abs(index2) <= r && (!s || Math.Abs(index1) + Math.Abs(index2) >= r || Math.Abs(index3) + Math.Abs(index2) >= r))
                            {
                                m_subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X + index1, Point1.Value.CellFace.Point.Y + index2, Point1.Value.CellFace.Point.Z + index3, blockID, true);
                                ++num;

                            }
                        }
                        else if (f == 2)
                        {
                            if (Math.Abs(index2) + Math.Abs(index1) <= r && Math.Abs(index3) + Math.Abs(index1) <= r && (!s || Math.Abs(index2) + Math.Abs(index1) >= r || Math.Abs(index3) + Math.Abs(index1) >= r))
                            {
                                m_subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X + index1, Point1.Value.CellFace.Point.Y + index2, Point1.Value.CellFace.Point.Z + index3, blockID, true);
                                ++num;

                            }
                        }
                        else if (Math.Abs(index2) + Math.Abs(index3) <= r && Math.Abs(index1) + Math.Abs(index3) <= r && (!s || Math.Abs(index2) + Math.Abs(index3) >= r || Math.Abs(index1) + Math.Abs(index3) >= r))
                        {
                            m_subsystemTerrain.ChangeCell(Point1.Value.CellFace.Point.X + index1, Point1.Value.CellFace.Point.Y + index2, Point1.Value.CellFace.Point.Z + index3, blockID, true);
                            ++num;

                        }
                    }
                }
            }
            m_componentPlayer.ComponentGui.DisplaySmallMessage(string.Format("7896868", (object)num), true, true);




        }

        private void LoadBTN()
        {
            GameWidget gameWidget = m_componentPlayer.View.GameWidget;
            Subtexture subtexture1 = new Subtexture(ContentManager.Get<Texture2D>("WE/Button1"), Vector2.Zero, Vector2.One);
            Subtexture subtexture2 = new Subtexture(ContentManager.Get<Texture2D>("WE/Button2"), Vector2.Zero, Vector2.One);
            Subtexture subtexture3 = new Subtexture(ContentManager.Get<Texture2D>("WE/Button3"), Vector2.Zero, Vector2.One);
            Subtexture subtexture4 = new Subtexture(ContentManager.Get<Texture2D>("WE/ButtonPaste"), Vector2.Zero, Vector2.One);
            Subtexture subtexture5 = new Subtexture(ContentManager.Get<Texture2D>("WE/ButtonFill"), Vector2.Zero, Vector2.One);
            Subtexture subtexture6 = new Subtexture(ContentManager.Get<Texture2D>("WE/ButtonReplace"), Vector2.Zero, Vector2.One);
            Subtexture subtexture7 = new Subtexture(ContentManager.Get<Texture2D>("WE/ButtonClear"), Vector2.Zero, Vector2.One);
            Subtexture subtexture8 = new Subtexture(ContentManager.Get<Texture2D>("WE/ButtonMemCopy"), Vector2.Zero, Vector2.One);
            Subtexture subtexture9 = new Subtexture(ContentManager.Get<Texture2D>("WE/ButtonMemPaste"), Vector2.Zero, Vector2.One);
            Subtexture subtexture10 = new Subtexture(ContentManager.Get<Texture2D>("WE/Button1_pressed"), Vector2.Zero, Vector2.One);
            Subtexture subtexture11 = new Subtexture(ContentManager.Get<Texture2D>("WE/Button2_pressed"), Vector2.Zero, Vector2.One);
            Subtexture subtexture12 = new Subtexture(ContentManager.Get<Texture2D>("WE/Button3_pressed"), Vector2.Zero, Vector2.One);
            Subtexture subtexture13 = new Subtexture(ContentManager.Get<Texture2D>("WE/ButtonPaste_pressed"), Vector2.Zero, Vector2.One);
            Subtexture subtexture14 = new Subtexture(ContentManager.Get<Texture2D>("WE/ButtonFill_pressed"), Vector2.Zero, Vector2.One);
            Subtexture subtexture15 = new Subtexture(ContentManager.Get<Texture2D>("WE/ButtonReplace_pressed"), Vector2.Zero, Vector2.One);
            Subtexture subtexture16 = new Subtexture(ContentManager.Get<Texture2D>("WE/ButtonClear_pressed"), Vector2.Zero, Vector2.One);
            Subtexture subtexture17 = new Subtexture(ContentManager.Get<Texture2D>("WE/ButtonMemCopy_pressed"), Vector2.Zero, Vector2.One);
            Subtexture subtexture18 = new Subtexture(ContentManager.Get<Texture2D>("WE/ButtonMemPaste_pressed"), Vector2.Zero, Vector2.One);

            Subtexture subtexture21 = new Subtexture(ContentManager.Get<Texture2D>("WE/Button_geo"), Vector2.Zero, Vector2.One);
            Subtexture subtexture22 = new Subtexture(ContentManager.Get<Texture2D>("WE/Button_geo_press"), Vector2.Zero, Vector2.One);

            Subtexture subtexture23 = new Subtexture(ContentManager.Get<Texture2D>("WE/Button_run"), Vector2.Zero, Vector2.One);
            Subtexture subtexture24 = new Subtexture(ContentManager.Get<Texture2D>("WE/Button_run_press"), Vector2.Zero, Vector2.One);


            Subtexture subtexture19 = new Subtexture(ContentManager.Get<Texture2D>("WE/WEBTN"), Vector2.Zero, Vector2.One);
            Subtexture subtexture20 = new Subtexture(ContentManager.Get<Texture2D>("WE/WEBTNP"), Vector2.Zero, Vector2.One);
            F1 = gameWidget.Children.Find<BitmapButtonWidget>("F1", true);
            F2 = gameWidget.Children.Find<BitmapButtonWidget>("F2", true);
            F3 = gameWidget.Children.Find<BitmapButtonWidget>("F3", true);
            F5 = gameWidget.Children.Find<BitmapButtonWidget>("F5", true);
            F6 = gameWidget.Children.Find<BitmapButtonWidget>("F6", true);
            F7 = gameWidget.Children.Find<BitmapButtonWidget>("F7", true);
            F8 = gameWidget.Children.Find<BitmapButtonWidget>("F8", true);
            F9 = gameWidget.Children.Find<BitmapButtonWidget>("F9", true);
            F10 = gameWidget.Children.Find<BitmapButtonWidget>("F10", true);

            F11 = gameWidget.Children.Find<BitmapButtonWidget>("F11", true);
            F12 = gameWidget.Children.Find<BitmapButtonWidget>("F12", true);

            WorldEditMenu = gameWidget.Children.Find<BitmapButtonWidget>("WorldEditMenu", true);
            WorldEditMenuContainerBottom = gameWidget.Children.Find<StackPanelWidget>("WorldEditMenuContainerBottom", true);
            WorldEditMenuContainerTop = gameWidget.Children.Find<StackPanelWidget>("WorldEditMenuContainerTop", true);
            WorldEditMenu.NormalSubtexture = subtexture19;
            WorldEditMenu.ClickedSubtexture = subtexture20;
            F1.NormalSubtexture = subtexture1;
            F2.NormalSubtexture = subtexture2;
            F3.NormalSubtexture = subtexture3;
            F5.NormalSubtexture = subtexture4;
            F6.NormalSubtexture = subtexture5;
            F7.NormalSubtexture = subtexture6;
            F8.NormalSubtexture = subtexture7;
            F9.NormalSubtexture = subtexture8;
            F10.NormalSubtexture = subtexture9;
            F1.ClickedSubtexture = subtexture10;
            F2.ClickedSubtexture = subtexture11;
            F3.ClickedSubtexture = subtexture12;
            F5.ClickedSubtexture = subtexture13;
            F6.ClickedSubtexture = subtexture14;
            F7.ClickedSubtexture = subtexture15;
            F8.ClickedSubtexture = subtexture16;
            F9.ClickedSubtexture = subtexture17;
            F10.ClickedSubtexture = subtexture18;

            F11.NormalSubtexture = subtexture21;
            F11.ClickedSubtexture = subtexture22;

            F12.NormalSubtexture = subtexture23;
            F12.ClickedSubtexture = subtexture24;



        }




        public int[] DrawOrders
        {
            get
            {
                return m_drawOrders;
            }
        }

        public int UpdateOrder
        {
            get
            {
                return 0;
            }
        }



    }
}
