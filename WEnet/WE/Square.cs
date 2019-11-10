using Engine;
using Game;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Collections;

namespace API_WE_Mod
{
    public class Square : Dialog
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

        private List<string> names = new List<string>();
        private List<Category> m_categories = new List<Category>();

        private List<string> names_pos = new List<string>();
        private List<Category> m_categories_pos = new List<Category>();

        private ButtonWidget mSelect_mode;
        private ButtonWidget mselect_pos;
        private ButtonWidget Icon_select;

        private LabelWidget mPosition;
        private LabelWidget Mode;

        private BlockIconWidget m_blockIconWidget;

        private int radius;
        private int lenght;


        int id1;
        TerrainRaycastResult? Point;
        SubsystemTerrain m_subsystemTerrain;


        public Square(ComponentPlayer player, int id, TerrainRaycastResult? Point1, SubsystemTerrain subsystemTerrain)
        {
            m_subsystemTerrain = subsystemTerrain;
            Point = Point1;
            id1 = id;
            this.player = player;
            WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("WE/DialogsWE/SquareDialog"));

            m_title = this.Children.Find<LabelWidget>("Square_Dialog.Title", true);

            Mode = this.Children.Find<LabelWidget>("Square_Dialog.Mode", true);
            mPosition = this.Children.Find<LabelWidget>("Square_Dialog.Pos", true);

            Icon_select = this.Children.Find<ButtonWidget>("Square_Dialog.Icon_select", true);

            mselect_pos = this.Children.Find<ButtonWidget>("Square_Dialog.Select_pos", true);

            m_radius = this.Children.Find<SliderWidget>("Square_Dialog.Slider1", true);
            m_lenght = this.Children.Find<SliderWidget>("Square_Dialog.Slider2", true);

            plusButton = this.Children.Find<ButtonWidget>("Square_Dialog.Button4", true);
            minusButton = this.Children.Find<ButtonWidget>("Square_Dialog.Button3", true);

            lenght_plusButton = this.Children.Find<ButtonWidget>("Square_Dialog.Button2", true);
            lenght_minusButton = this.Children.Find<ButtonWidget>("Square_Dialog.Button1", true);



            mSelect_mode = this.Children.Find<ButtonWidget>("Square_Dialog.Select_mode", true);

            m_okButton = this.Children.Find<ButtonWidget>("Square_Dialog.OK", true);
            m_cancelButton = this.Children.Find<ButtonWidget>("Square_Dialog.Cancel", true);

            this.m_blockIconWidget = this.Children.Find<BlockIconWidget>("Square_Dialog.Icon", true);


            m_title.Text = "Square";

            //switch_mode.Text = "Holow";

            //m_blockIconWidget.Value

            mPosition.Text = "Flat";
            Mode.Text = "Hollow";


            m_radius.MinValue = 1f;
            m_radius.MaxValue = 100;
            m_radius.Value = 1f;

            m_lenght.MinValue = 1f;
            m_lenght.MaxValue = 100;
            m_lenght.Value = 1f;

            m_blockIconWidget.Value = id;

            names.Add("Soild");
            names.Add("Hollow");

            names_pos.Add("Flat");
            names_pos.Add("Pos_X");
            names_pos.Add("Pos_Y");


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

            radius = (int)m_radius.Value - 1;
            lenght = (int)m_lenght.Value - 1;

            m_blockIconWidget.Value = id1;


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



            if (mSelect_mode.IsClicked)
            {
                Select_mode(m_categories, names);

            }

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
                int pos1 = 1;

                if (mPosition.Text == "Flat")
                {
                    pos1 = 1;
                }
                if (mPosition.Text == "Pos_X")
                {
                    pos1 = 0;
                }
                if(mPosition.Text == "Pos_Y")
                {
                    pos1 = 3;
                }
            



                API_WE.Square(s, radius + 1, lenght + 1, (Position)pos1, id1, Point, m_subsystemTerrain);

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
