using Engine;
using Game;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Collections;

namespace API_WE_Mod
{
    public class Mountain : Dialog
    {

        private ComponentPlayer player;
        // BlockIconWidget s;

        private LabelWidget m_title;
        private SliderWidget m_radius;
        private SliderWidget m_lenght;


        private ButtonWidget plusButton;
        private ButtonWidget minusButton;

        private ButtonWidget lenght_plusButton;
        private ButtonWidget lenght_minusButton;

        private ButtonWidget m_okButton;
        private ButtonWidget m_cancelButton;

        private ButtonWidget Icon_select;
        private ButtonWidget Icon_select1;
        private ButtonWidget Icon_select2;


        private BlockIconWidget m_blockIconWidget;
        private BlockIconWidget m_blockIconWidget1;
        private BlockIconWidget m_blockIconWidget2;

        private int radius;
        private int lenght;


        int id1;
        int id2;
        int id3;
        TerrainRaycastResult? Point;
        SubsystemTerrain m_subsystemTerrain;


        public Mountain(ComponentPlayer player,TerrainRaycastResult? Point1, SubsystemTerrain subsystemTerrain)
        {
            m_subsystemTerrain = subsystemTerrain;
            Point = Point1;

            id1 = 3;
            id2 = 2;
            id3 = 8;

            this.player = player;
            WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("WE/DialogsWE/MountainDialog"));

            m_title = this.Children.Find<LabelWidget>("Mountain_Dialog.Title", true);

            Icon_select = this.Children.Find<ButtonWidget>("Mountain_Dialog.Icon_select", true);
            Icon_select1 = this.Children.Find<ButtonWidget>("Mountain_Dialog.Icon_select1", true);
            Icon_select2 = this.Children.Find<ButtonWidget>("Mountain_Dialog.Icon_select2", true);

            m_radius = this.Children.Find<SliderWidget>("Mountain_Dialog.Slider1", true);
            m_lenght = this.Children.Find<SliderWidget>("Mountain_Dialog.Slider2", true);

            plusButton = this.Children.Find<ButtonWidget>("Mountain_Dialog.Button4", true);
            minusButton = this.Children.Find<ButtonWidget>("Mountain_Dialog.Button3", true);

            lenght_plusButton = this.Children.Find<ButtonWidget>("Mountain_Dialog.Button2", true);
            lenght_minusButton = this.Children.Find<ButtonWidget>("Mountain_Dialog.Button1", true);


            m_okButton = this.Children.Find<ButtonWidget>("Mountain_Dialog.OK", true);
            m_cancelButton = this.Children.Find<ButtonWidget>("Mountain_Dialog.Cancel", true);

            m_blockIconWidget = this.Children.Find<BlockIconWidget>("Mountain_Dialog.Icon", true);
            m_blockIconWidget1 = this.Children.Find<BlockIconWidget>("Mountain_Dialog.Icon1", true);
            m_blockIconWidget2 = this.Children.Find<BlockIconWidget>("Mountain_Dialog.Icon2", true);


            m_title.Text = "Mountain";

            //switch_mode.Text = "Holow";

            //m_blockIconWidget.Value

          

            m_radius.MinValue = 1f;
            m_radius.MaxValue = 100;
            m_radius.Value = 1f;

            m_lenght.MinValue = 1f;
            m_lenght.MaxValue = 100;
            m_lenght.Value = 1f;

            





        }

        public override void Update()
        {

            radius = (int)m_radius.Value - 1;
            lenght = (int)m_lenght.Value - 1;

            m_blockIconWidget.Value = id1;
            m_blockIconWidget1.Value = id2;
            m_blockIconWidget2.Value = id3;

            if (plusButton.IsClicked)
            {
                m_radius.Value = MathUtils.Min(m_radius.Value + 1f, (int)m_radius.MaxValue);
            }

            if (minusButton.IsClicked)
            {
                m_radius.Value = MathUtils.Max(m_radius.Value - 1f, (int)m_radius.MinValue);
            }



            if (lenght_plusButton.IsClicked)
            {
                m_lenght.Value = MathUtils.Min(m_lenght.Value + 1f, (int)m_lenght.MaxValue);
            }

            if (lenght_minusButton.IsClicked)
            {
                m_lenght.Value = MathUtils.Max(m_lenght.Value - 1f, (int)m_lenght.MinValue);
            }



        

            if (Icon_select.IsClicked)
            {
                DialogsManager.ShowDialog(null, new ListSelectionDialog("Select Block", new int[3]
           {
          8,
          2,
          7
          
          
           }, 72f, index =>
           {
               ContainerWidget containerWidget = (ContainerWidget)WidgetsManager.LoadWidget((object)null, ContentManager.Get<XElement>("Widgets/SelectBlockItem"), null);
               containerWidget.Children.Find<BlockIconWidget>("SelectBlockItem.Block", true).Contents = (int)index;
               containerWidget.Children.Find<LabelWidget>("SelectBlockItem.Text", true).Text = BlocksManager.Blocks[(int)index].GetDisplayName(null, Terrain.MakeBlockValue((int)index));

               return containerWidget;
           }, index => id1 = (int)index));
            }

            if (Icon_select1.IsClicked)
            {
                DialogsManager.ShowDialog(null, new ListSelectionDialog("Select Block", new int[3]
           {
          8,
          2,
          7


           }, 72f, index =>
           {
               ContainerWidget containerWidget = (ContainerWidget)WidgetsManager.LoadWidget((object)null, ContentManager.Get<XElement>("Widgets/SelectBlockItem"), null);
               containerWidget.Children.Find<BlockIconWidget>("SelectBlockItem.Block", true).Contents = (int)index;
               containerWidget.Children.Find<LabelWidget>("SelectBlockItem.Text", true).Text = BlocksManager.Blocks[(int)index].GetDisplayName(null, Terrain.MakeBlockValue((int)index));

               return containerWidget;
           }, index => id2 = (int)index));
            }

            if (Icon_select2.IsClicked)
            {
                DialogsManager.ShowDialog(null, new ListSelectionDialog("Select Block", new int[3]
           {
          8,
          2,
          7


           }, 72f, index =>
           {
               ContainerWidget containerWidget = (ContainerWidget)WidgetsManager.LoadWidget((object)null, ContentManager.Get<XElement>("Widgets/SelectBlockItem"), null);
               containerWidget.Children.Find<BlockIconWidget>("SelectBlockItem.Block", true).Contents = (int)index;
               containerWidget.Children.Find<LabelWidget>("SelectBlockItem.Text", true).Text = BlocksManager.Blocks[(int)index].GetDisplayName(null, Terrain.MakeBlockValue((int)index));

               return containerWidget;
           }, index => id3 = (int)index));
            }





            if (this.m_okButton.IsClicked)
            {

                Engine.Point3 PointStart;

                PointStart.X = Point.Value.CellFace.X;
                PointStart.Y = Point.Value.CellFace.Y;
                PointStart.Z = Point.Value.CellFace.Z;



                //API_WE.Round(s, radius + 1, lenght + 1, (Position)pos1, id1, Point, m_subsystemTerrain);
                API_WE.Mountain(PointStart,radius,lenght,m_subsystemTerrain,id1,id2,id3,player);
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
            this.m_radius.Text = string.Format("{0} blocks", radius + 1);
            this.m_lenght.Text = string.Format("{0} blocks", lenght + 1);


        }





       

    }
}
