using Engine;
using Engine.Serialization;
using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;


namespace API_WE_Mod
{
   public class ZoneDialog : Dialog
    {

        ListPanelWidget list_build;

        private ButtonWidget AddButton;
        private ButtonWidget MoreButton;
        private ButtonWidget Cancel;
        
        private List<Category> m_categories = new List<Category>();
        private List<string> names_item = new List<string>();


        public static readonly string Path_mod = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/Survivalcraft/WorldEdit"; 


        string selected_item;

        string name_file;

        ComponentPlayer player;

        TerrainRaycastResult? Point1;
        TerrainRaycastResult? Point2;
        TerrainRaycastResult? Point3;
        SubsystemTerrain subsystemTerrain;

        public ZoneDialog(ComponentPlayer player, TerrainRaycastResult? Point1, TerrainRaycastResult? Point2, TerrainRaycastResult? Point3, SubsystemTerrain subsystemTerrain)
        {

            this.player = player;
            this.Point1 = Point1;
            this.Point2 = Point2;
            this.Point3 = Point3;


            this.subsystemTerrain = subsystemTerrain;
            WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("WE/DialogsWE/ZoneWidget"));


            
            list_build = Children.Find<ListPanelWidget>("ListView", true);

            AddButton = this.Children.Find<ButtonWidget>("AddButton", true);
            MoreButton = this.Children.Find<ButtonWidget>("MoreButton", true);
            Cancel = this.Children.Find<ButtonWidget>("Cancel", true);

            names_item.Add("Delete");
            //names_item.Add("Rename");
            names_item.Add("Create");
            
            foreach (string category in names_item)
                m_categories.Add(new Category()
                {
                    Name = category,

                });


            MoreButton.IsEnabled = false;


            update_builds();
         
        }

        void update_builds()
        {

            list_build.ClearItems();

            if (!Directory.Exists(Path_mod))
               Directory.CreateDirectory(Path_mod);
            foreach (string file in Directory.GetFiles(Path_mod))
            {
                if (Path.GetExtension (file) == ".scbuild")
                {
                    list_build.AddItem(Path.GetFileName(file));
                }
            }

          

        }

        public override void Update()
        {
            //list_build.ClearItems();

            
           // list_build.SelectionChanged += delegate
           // {

                selected_item = list_build.SelectedItem as string;
          //  };


            if (Cancel.IsClicked)
            {

                DialogsManager.HideDialog(this);
            }
            if (MoreButton.IsClicked)
            {
                if (ComponentWE5.m_componentPlayer != null)
                    DialogsManager.ShowDialog(ComponentWE5.m_componentPlayer.View.GameWidget, new ListSelectionDialog(string.Empty, m_categories, 56f, c =>
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
                        string ds = names_item[d];

                        if (ds == "Delete")
                        {
                            DialogsManager.ShowDialog((ContainerWidget)player.View.GameWidget, (Dialog)new MessageDialog("Warning", "Bild will deleted ", "Delete", "Cancel", (Action<MessageDialogButton>)(b =>
                            {
                                if (b != MessageDialogButton.Button1)
                                    return;
                               File.Delete(Path.Combine(Path_mod,selected_item));
                                update_builds();

                            })));

                        }

                        if (ds == "Rename")
                        {
                            this.Input.EnterText("Furniture Set Name", selected_item, 20, s =>
                            {
                                if (s == null)
                                    return;

                                //File.Replace();
                                //using (Stream fs2 = File.Open(Path.Combine(Path_mod, selected_item), FileMode.))
                               // {
                                   // rename(fs2, Path.Combine(Path_mod,s));
                                  //  fs2.Dispose();
                                //}
                                  

                                
                                    





                                //Storage.DeleteFile(Path.Combine(Path_mod, selected_item));

                            });

                          

                        }

                        if (ds == "Create")
                        {
                            if (Point3 == null)
                            {
                                DialogsManager.HideDialog(this);
                                player.ComponentGui.DisplaySmallMessage("You have not selected point 3", false, false);
                            
                            }

                            else
                            {

                                Engine.Point3 Start;

                                Start.X = Point3.Value.CellFace.X;
                                Start.Y = Point3.Value.CellFace.Y;
                                Start.Z = Point3.Value.CellFace.Z;



                                API_WE.Paste_zone(Start, subsystemTerrain, player, Path.Combine(Path_mod, selected_item));

                                DialogsManager.HideDialog(this);
                            }
                            //player.ComponentGui.DisplaySmallMessage(path, true, true);
                            //bool flag = true;

                            


                        }


                        //Mode.Text = ds;
                        // player.ComponentGui.DisplaySmallMessage(ds, false, true);



                    }));

            }

         

                



            if (list_build.SelectedItem != null)
            {
                MoreButton.IsEnabled = true;
            }
            else
            {
                MoreButton.IsEnabled = false;
            }


            if (AddButton.IsClicked)
            {

                //Storage.OpenFile(Path_mod + name_file + ".scbuild", OpenFileMode.Create);
                //string path = Path.Combine(Path_mod,name_file, ".scbuild");
                if (Point1 == null && Point2 == null)
                {
                    player.ComponentGui.DisplaySmallMessage("You have not selected points 1,2", false, false);
                    DialogsManager.HideDialog(this);
                }

                else
                {

                    this.Input.EnterText("Name build", "New build", 20, s =>
                {
                    if (s == null)
                        return;

                    name_file = s;

                  //  using (Stream stream = File.Open(Path.Combine(Path_mod, name_file + ".scbuild"), FileMode.Create))
                   // {
                   // }



                    Engine.Point3 PointStart;

                    PointStart.X = Point1.Value.CellFace.X;
                    PointStart.Y = Point1.Value.CellFace.Y;
                    PointStart.Z = Point1.Value.CellFace.Z;

                    Engine.Point3 PointEnd;

                    PointEnd.X = Point2.Value.CellFace.X;
                    PointEnd.Y = Point2.Value.CellFace.Y;
                    PointEnd.Z = Point2.Value.CellFace.Z;

                    API_WE.Coppy_zone(Path.Combine(Path_mod, name_file + ".scbuild"), PointStart, PointEnd, subsystemTerrain, player);
                    update_builds();

                });

                }

                

                
            }
           



        }

        



    }
}
