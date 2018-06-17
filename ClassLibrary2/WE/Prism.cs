using Engine;
using Game;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Collections;

namespace API_WE_Mod
{
    public class Prism : Dialog
    {

        private ComponentPlayer player;
        // BlockIconWidget s;

        private LabelWidget m_title;
        private SliderWidget m_radius;



        private ButtonWidget plusButton;
        private ButtonWidget minusButton;



        private ButtonWidget m_okButton;
        private ButtonWidget m_cancelButton;

        private List<string> names = new List<string>();
        private List<Category> m_categories = new List<Category>();



        private ButtonWidget mSelect_mode;
        private ButtonWidget Icon_select;

        private LabelWidget Mode;

        private BlockIconWidget m_blockIconWidget;

        private int radius;



        int id1;
        TerrainRaycastResult? Point;
        SubsystemTerrain m_subsystemTerrain;



        public Prism(ComponentPlayer player, int id, TerrainRaycastResult? Point1, SubsystemTerrain subsystemTerrain)
        {
            Point = null;
            Point = Point1;
            m_subsystemTerrain = subsystemTerrain;

            id1 = id;
            this.player = player;
            WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("WE/DialogsWE/PrismDialog"));
            m_title = this.Children.Find<LabelWidget>("Prism_Dialog.Title", true);

            Mode = this.Children.Find<LabelWidget>("Prism_Dialog.Mode", true);

            Icon_select = this.Children.Find<ButtonWidget>("Prism_Dialog.Icon_select", true);

            m_radius = this.Children.Find<SliderWidget>("Prism_Dialog.Slider1", true);

            plusButton = this.Children.Find<ButtonWidget>("Prism_Dialog.Button4", true);
            minusButton = this.Children.Find<ButtonWidget>("Prism_Dialog.Button3", true);





            mSelect_mode = this.Children.Find<ButtonWidget>("Prism_Dialog.Select_mode", true);

            m_okButton = this.Children.Find<ButtonWidget>("Prism_Dialog.OK", true);
            m_cancelButton = this.Children.Find<ButtonWidget>("Prism_Dialog.Cancel", true);

            this.m_blockIconWidget = this.Children.Find<BlockIconWidget>("Prism_Dialog.Icon", true);


            m_title.Text = "Prism";

            //switch_mode.Text = "Holow";

            //m_blockIconWidget.Value


            Mode.Text = "Hollow";


            m_radius.MinValue = 1f;
            m_radius.MaxValue = 100;
            m_radius.Value = 1f;




            names.Add("Soild");
            names.Add("Hollow");


            foreach (string category in names)
                m_categories.Add(new Category()
                {
                    Name = category,

                });


        }

        public override void Update()
        {

            radius = (int)m_radius.Value - 1;

            m_blockIconWidget.Value = id1;


            if (plusButton.IsClicked)
            {
                m_radius.Value = MathUtils.Min(m_radius.Value + 1f, (int)m_radius.MaxValue);
            }

            if (minusButton.IsClicked)
            {
                m_radius.Value = MathUtils.Max(m_radius.Value - 1f, (int)m_radius.MinValue);
            }







            if (mSelect_mode.IsClicked)
            {
                Select_mode(m_categories, names);

            }



            if (Icon_select.IsClicked)
            {
                DialogsManager.ShowDialog(null, new ListSelectionDialog("Select Block", new int[20]
           {
          18,
          92,
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
            }






            if (this.m_okButton.IsClicked)
            {

                bool s;

                if (Mode.Text == "Hollow")
                {
                    s = true;
                }
                else
                {
                    s = false;
                }



                API_WE.Prism(s, radius, id1, Point, m_subsystemTerrain);
                
                DialogsManager.HideDialog(this);
            }

            if (this.Input.Cancel || this.m_cancelButton.IsClicked)
            {
                DialogsManager.HideDialog((Dialog)this);
            }

            UpdateControls();

        }

        private void UpdateControls()
        {
            //this.m_slider1.Value = (float)(this.m_maxExtension + 1);
            this.m_radius.Text = string.Format("{0} blocks", radius + 1);



        }





        public void Select_mode(List<Category> m_categories, List<string> a)
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
                    Mode.Text = ds;
                    // player.ComponentGui.DisplaySmallMessage(ds, false, true);



                }));
        }



    }
}