using Engine;
using Engine.Serialization;
using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace API_WE_Mod
{
    public class ImageDialog : Dialog
    {

        ListPanelWidget list_build;

        private ButtonWidget AddButton;
        private ButtonWidget MoreButton;
        private ButtonWidget Cancel;
        private LabelWidget Title;

        private List<Category> m_categories = new List<Category>();
        private List<string> names_item = new List<string>();


        public static readonly string Path_img = AppDomain.CurrentDomain.BaseDirectory + @"\img\";


        string selected_item;



        ComponentPlayer player;

        TerrainRaycastResult? Point1;
        TerrainRaycastResult? Point2;
        TerrainRaycastResult? Point3;
        SubsystemTerrain subsystemTerrain;

        public ImageDialog(ComponentPlayer player, TerrainRaycastResult? Point1, TerrainRaycastResult? Point2, TerrainRaycastResult? Point3, SubsystemTerrain subsystemTerrain)
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
            Title = this.Children.Find<LabelWidget>("Dialog.Title", true);


            Title.Text = "Image in SC";

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

            if (!Directory.Exists(Path_img))
                Directory.CreateDirectory(Path_img);
            foreach (string file in Directory.GetFiles(Path_img))
            {
                if (Path.GetExtension(file) == ".jpg" || Path.GetExtension(file) == ".png")
                {
                    list_build.AddItem(Path.GetFileName(file));
                }
            }



        }

        public override void Update()
        {


            selected_item = list_build.SelectedItem as string;



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
                            DialogsManager.ShowDialog((ContainerWidget)player.View.GameWidget, (Dialog)new MessageDialog("Warning", "Image will deleted ", "Delete", "Cancel", (Action<MessageDialogButton>)(b =>
                            {
                                if (b != MessageDialogButton.Button1)
                                    return;
                                File.Delete(Path.Combine(Path_img, selected_item));
                                update_builds();

                            })));

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

                                SettingsManager.GuiSize = GuiSize.Smallest;

                                DialogsManager.ShowDialog(player.View.GameWidget, new Img(player, Path.Combine(Path_img, selected_item), Point3, subsystemTerrain));



                                DialogsManager.HideDialog(this);
                            }




                        }




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



                this.Input.EnterText("URL image", " ", 100, s =>
                {
                    if (s == null)
                        return;

                   
                        WEB_manager.Dowland(Path_img, s, c =>
                        {
                            DialogsManager.ShowDialog((ContainerWidget)player.View.GameWidget, (Dialog)new MessageDialog("Error download image", c, null, "OK", null));

                        }, null);




                    

                    update_builds();

                });






            }




        }





    }
}
