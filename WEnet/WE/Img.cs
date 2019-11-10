using Engine;
using Game;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Collections;

namespace API_WE_Mod
{
    public class Img : Dialog
    {

        private ComponentPlayer player;


        private ButtonWidget m_okButton;
        private ButtonWidget m_cancelButton;


        private ButtonWidget m_type_creatingButton;
        private LabelWidget m_type_creatingLabel;

        private SliderWidget m_resizeSlider;
        private SliderWidget m_furniture_resolutionSlider;
        private SliderWidget m_deep_colorSlider;
        private SliderWidget m_color_ofsetSlider;

        private ButtonWidget m_posButton;
        private ButtonWidget m_rotButton;

        private LabelWidget m_posLabel;
        private LabelWidget m_rotLabel;

        private CheckboxWidget m_colors_useBox;
        private CheckboxWidget m_color_saveBox;
        private CheckboxWidget m_mirrorBox;

        Point3 Point;
        SubsystemTerrain m_subsystemTerrain;



        public Img(ComponentPlayer player, string path, TerrainRaycastResult? Point1, SubsystemTerrain subsystemTerrain)
        {

            path_img = path;



            Point = new Point3(Point1.Value.CellFace.Point.X, Point1.Value.CellFace.Point.Y, Point1.Value.CellFace.Point.Z);
            m_subsystemTerrain = subsystemTerrain;


            this.player = player;
            WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("WE/DialogsWE/ImgDialog"));
            // WidgetsManager.LoadWidgetContents(this, this, XElement.Load(@"D:\Games\WSC\Content\WE\DialogsWE\ImgDialog.xml"));

            m_okButton = this.Children.Find<ButtonWidget>("Img_Dialog.OK", true);
            m_cancelButton = this.Children.Find<ButtonWidget>("Img_Dialog.Cancel", true);


            m_type_creatingButton = this.Children.Find<ButtonWidget>("Img_Dialog.type_cr_btn", true);
            m_type_creatingLabel = this.Children.Find<LabelWidget>("Img_Dialog.type_cr_text", true);

            m_resizeSlider = this.Children.Find<SliderWidget>("Img_Dialog.resize_sl", true);
            m_furniture_resolutionSlider = this.Children.Find<SliderWidget>("Img_Dialog.furn_res_sl", true);
            m_deep_colorSlider = this.Children.Find<SliderWidget>("Img_Dialog.Slider_deep", true);
            m_color_ofsetSlider = this.Children.Find<SliderWidget>("Img_Dialog.ofst_sl", true);

            m_posButton = this.Children.Find<ButtonWidget>("Img_Dialog.pos_sel", true);
            m_rotButton = this.Children.Find<ButtonWidget>("Img_Dialog.rot_sel", true);

            m_posLabel = this.Children.Find<LabelWidget>("Img_Dialog.pos_txt", true);
            m_rotLabel = this.Children.Find<LabelWidget>("Img_Dialog.rot_txt", true);


            m_colors_useBox = this.Children.Find<CheckboxWidget>("Img_Dialog.Line0", true);
            m_color_saveBox = this.Children.Find<CheckboxWidget>("Img_Dialog.Line1", true);
            m_mirrorBox = this.Children.Find<CheckboxWidget>("Img_Dialog.mirror", true);

            m_resizeSlider.MinValue = 1f;
            m_resizeSlider.MaxValue = 8f;

            m_furniture_resolutionSlider.MinValue = 2f;
            m_furniture_resolutionSlider.MaxValue = 16f;


            m_color_ofsetSlider.MinValue = 0f;
            m_color_ofsetSlider.MaxValue = 8f;


            m_deep_colorSlider.MinValue = 2f;
            m_deep_colorSlider.MaxValue = 16f;



            t_c_txt = "Furniture";


            m_colors_useBox.IsChecked = false;
            m_color_saveBox.IsChecked = false;

            m_resizeSlider.Value = 2;
            m_furniture_resolutionSlider.Value = 16;
            m_deep_colorSlider.Value = 16;



            pos_txt = "Horizontally";
            rot_txt = "Front";



        }


        string pos_txt;
        string rot_txt;
        string t_c_txt;

        string path_img;
        int resize;
        int furnit_resol;

        int deep_color;
        int ofst_color;

        bool type_cr;
        bool pos;
        bool rot;



        public override void Update()
        {

            resize = (int)m_resizeSlider.Value;
            furnit_resol = (int)m_furniture_resolutionSlider.Value;
            deep_color = (int)m_deep_colorSlider.Value;
            ofst_color = (int)m_color_ofsetSlider.Value;

   
            if (m_type_creatingButton.IsClicked)
            {
                Select_type_creating();
            }

            if (m_posButton.IsClicked)
            {
                Select_pos();
            }
            if (m_rotButton.IsClicked)
            {
                Select_rot();
            }


            if (m_okButton.IsClicked)
            {

                if (t_c_txt == "Furniture")
                {
                    type_cr = true;
                }
                else
                {
                    type_cr = false;
                }


                if (pos_txt == "Vertical")
                {
                    pos = false;
                }
                else
                {
                    pos = true;
                }

                if (rot_txt == "Front")
                {
                    rot = true;
                }
                else
                {
                    rot = false;
                }
                //  player.ComponentGui.DisplaySmallMessage(string.Format(type_cr.ToString()), true, true);
                API_WE.draw_img(m_mirrorBox.IsChecked, ofst_color, m_color_saveBox.IsChecked, m_colors_useBox.IsChecked, deep_color, type_cr, resize, furnit_resol, pos, rot, path_img, Point, m_subsystemTerrain, player);
                DialogsManager.HideDialog((Dialog)this);
            }



            if (this.Input.Cancel || this.m_cancelButton.IsClicked)
            {
                DialogsManager.HideDialog((Dialog)this);
            }

            UpdateControls();

        }

        private void UpdateControls()
        {

            m_resizeSlider.Text = ((int)m_resizeSlider.Value).ToString();
            m_furniture_resolutionSlider.Text = ((int)m_furniture_resolutionSlider.Value).ToString();
            m_deep_colorSlider.Text = ((int)m_deep_colorSlider.Value).ToString();
            m_color_ofsetSlider.Text = ((int)m_color_ofsetSlider.Value).ToString();


            m_type_creatingLabel.Text = t_c_txt;
            m_posLabel.Text = pos_txt;
            m_rotLabel.Text = rot_txt;


        }



        public void Select_type_creating()
        {

            List<string> names = new List<string>();

            names.Add("Furniture");
            names.Add("Blocks");

            if (ComponentWE5.m_componentPlayer != null)
                DialogsManager.ShowDialog(ComponentWE5.m_componentPlayer.View.GameWidget, new ListSelectionDialog(string.Empty, names, 56f, c =>
                {
                    LabelWidget labelWidget = new LabelWidget();
                    labelWidget.Text = c.ToString();
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
                    int d = names.IndexOf(c.ToString());
                    string ds = names[d];

                    t_c_txt = ds;

                }));
        }


        public void Select_pos()
        {

            List<string> names = new List<string>();

            names.Add("Vertical");
            names.Add("Horizontally ");

            if (ComponentWE5.m_componentPlayer != null)
                DialogsManager.ShowDialog(ComponentWE5.m_componentPlayer.View.GameWidget, new ListSelectionDialog(string.Empty, names, 56f, c =>
                {
                    LabelWidget labelWidget = new LabelWidget();
                    labelWidget.Text = c.ToString();
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
                    int d = names.IndexOf(c.ToString());
                    string ds = names[d];

                    pos_txt = ds;


                }));
        }

        public void Select_rot()
        {

            List<string> names = new List<string>();

            names.Add("Front");
            names.Add("Side");

            if (ComponentWE5.m_componentPlayer != null)
                DialogsManager.ShowDialog(ComponentWE5.m_componentPlayer.View.GameWidget, new ListSelectionDialog(string.Empty, names, 56f, c =>
                {
                    LabelWidget labelWidget = new LabelWidget();
                    labelWidget.Text = c.ToString();
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
                    int d = names.IndexOf(c.ToString());
                    string ds = names[d];

                    rot_txt = ds;


                }));
        }



    }
}