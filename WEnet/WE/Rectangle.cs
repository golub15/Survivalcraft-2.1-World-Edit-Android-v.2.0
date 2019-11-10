using Engine;
using Game;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Collections;

namespace API_WE_Mod
{
    public class Rectangle : Dialog
    {

        private ComponentPlayer player;
        // BlockIconWidget s;

        private LabelWidget m_title;


    

        private ButtonWidget m_okButton;
        private ButtonWidget m_cancelButton;

        private List<string> names = new List<string>();
        private List<Category> m_categories = new List<Category>();

        private List<string> names_pos = new List<string>();
        private List<Category> m_categories_pos = new List<Category>();

        private ButtonWidget mselect_pos;
        private ButtonWidget Icon_select;

        private LabelWidget mPosition;

        private BlockIconWidget m_blockIconWidget;

   

        int pos1 = 0;


        int id1;
        TerrainRaycastResult? Point;
        TerrainRaycastResult? Point2;
        SubsystemTerrain m_subsystemTerrain;


        public Rectangle(ComponentPlayer player, int id, TerrainRaycastResult? Point1, TerrainRaycastResult? Point2 ,SubsystemTerrain subsystemTerrain)
        {
            m_subsystemTerrain = subsystemTerrain;
            Point = Point1;
            this.Point2 = Point2;
            id1 = id;
            this.player = player;
            WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("WE/DialogsWE/PillarDialog"));

            m_title = this.Children.Find<LabelWidget>("Pillar_Dialog.Title", true);

            mPosition = this.Children.Find<LabelWidget>("Pillar_Dialog.Pos", true);

            Icon_select = this.Children.Find<ButtonWidget>("Pillar_Dialog.Icon_select", true);

            mselect_pos = this.Children.Find<ButtonWidget>("Pillar_Dialog.Select_pos", true);

           

            m_okButton = this.Children.Find<ButtonWidget>("Pillar_Dialog.OK", true);
            m_cancelButton = this.Children.Find<ButtonWidget>("Pillar_Dialog.Cancel", true);

            this.m_blockIconWidget = this.Children.Find<BlockIconWidget>("Pillar_Dialog.Icon", true);


            m_title.Text = "Frame or box";

            //switch_mode.Text = "Holow";

            //m_blockIconWidget.Value

            mPosition.Text = "Frame";


          

           

            m_blockIconWidget.Value = id;

            names.Add("Soild");
            names.Add("Hollow");

            names_pos.Add("Frame");
            names_pos.Add("Hollow Box");
            names_pos.Add("Soild Box");



            foreach (string category in names)
                m_categories.Add(new Category()
                {
                    Name = category,

                });

            foreach (string category in names_pos)
                m_categories_pos.Add(new Category()
                {
                    Name = category,

                });

        }

        public override void Update()
        {







            m_blockIconWidget.Value = id1;


            if (mselect_pos.IsClicked)
            {
                Select_pos(m_categories_pos, names_pos);
            }

            if (Icon_select.IsClicked)
            {
                DialogsManager.ShowDialog(null, new ListSelectionDialog("Select Block", new int[21]
           {
          18,
          92,
          8,
          2,
          7,
          3,
          67,
          66,
          4,
          5,
          26,
          73,
          21,
          46,
          47,
          15,
          62,
          68,
          126,
          71,
          1
           }, 72f, index =>
           {
               ContainerWidget containerWidget = (ContainerWidget)WidgetsManager.LoadWidget((object)null, ContentManager.Get<XElement>("Widgets/SelectBlockItem"), null);
               containerWidget.Children.Find<BlockIconWidget>("SelectBlockItem.Block", true).Contents = (int)index;
               containerWidget.Children.Find<LabelWidget>("SelectBlockItem.Text", true).Text = BlocksManager.Blocks[(int)index].GetDisplayName(null, Terrain.MakeBlockValue((int)index));

               return containerWidget;
           }, index => id1 = (int)index));
                ;
            }






            if (this.m_okButton.IsClicked)
            {

           

                

                

                if (mPosition.Text == "Frame")
                {
                    pos1 = 2;
                }
                if (mPosition.Text == "Hollow Box")
                {
                    pos1 = 1;
                }

                if (mPosition.Text == "Soild Box")
                {
                    pos1 = 0;
                }

                Engine.Point3 PointStart;

                PointStart.X = Point.Value.CellFace.X;
                PointStart.Y = Point.Value.CellFace.Y;
                PointStart.Z = Point.Value.CellFace.Z;

                Engine.Point3 PointEnd;

                PointEnd.X = Point2.Value.CellFace.X;
                PointEnd.Y = Point2.Value.CellFace.Y;
                PointEnd.Z = Point2.Value.CellFace.Z;

                // API_WE.Pillar(s, lenght + 1, radius + 1, (Position)pos1, id1, PointStart, m_subsystemTerrain, player);
                API_WE.Rectangle(pos1,id1,PointStart,PointEnd,player,m_subsystemTerrain);
                DialogsManager.HideDialog(this);
                //player.ComponentGui.DisplaySmallMessage(Convert.ToString(radius),false,true);
            }

            if (this.Input.Cancel || this.m_cancelButton.IsClicked)
            {
                DialogsManager.HideDialog(this);
            }

            UpdateControls();

        }

        private void UpdateControls()
        {
            //this.m_slider1.Value = (float)(this.m_maxExtension + 1);
          


        }





       

        public void Select_pos(List<Category> m_categories, List<string> a)
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
                    string ds = a[d];
                    mPosition.Text = ds;
                    // player.ComponentGui.DisplaySmallMessage(ds, false, true);



                }));
        }

    }
}
