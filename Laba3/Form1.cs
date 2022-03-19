using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using System.IO;
using System.Diagnostics;

namespace Laba3
{
    public partial class Form1 : Form
    {
        const string originalImagePath = "Original.png";
        const string quantImagePath = "QuantImage.png";
        Image image;
        Image quantImage;
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            //Выбор изображения
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Файлы изображений|*.bmp;*.png;*.jpg";
            if (openDialog.ShowDialog() != DialogResult.OK)
                return;
            image = Image.FromFile(openDialog.FileName);
            image.Save(originalImagePath);
            image.Dispose();
            QuantHist();
            panel2.Invalidate();
            panel1.Invalidate();
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (image == null)
                return;
            using (image = Image.FromFile(originalImagePath))
            {
                int s1 = image.Height * 700 / image.Width;
                e.Graphics.DrawImage(
                    image,
                    panel1.AutoScrollPosition.X,
                    panel1.AutoScrollPosition.Y,
                    700, s1);
            }
        } // Панели для отрисовки изображений
        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            if (quantImage == null)
                return;
            using (quantImage = Image.FromFile(quantImagePath))
            {
                int s1 = quantImage.Height * 700 / quantImage.Width;
                e.Graphics.DrawImage(
                    quantImage,
                    panel2.AutoScrollPosition.X,
                    panel2.AutoScrollPosition.Y,
                    700, s1);
            }
        }
        private void QuantHist()
        {
            if (image == null)
                return;
            //Создание квантованного изображения
            using (Mat quant = new Mat())
            {
                Kmeans(Cv2.ImRead(originalImagePath), quant, Convert.ToInt32(comboBox1.SelectedItem), comboBox2.SelectedIndex);
                Cv2.ImWrite(quantImagePath, quant);
                quantImage = Image.FromFile(quantImagePath);
                quantImage.Dispose();
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            QuantHist();
            panel2.Invalidate();
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            QuantHist();
            panel2.Invalidate();
        }
        public static void Kmeans(Mat input, Mat result, int k, int cb2) // Метод для квантования изображение
        {
            if (cb2 == 1)
            {
                Cv2.CvtColor(input, input, ColorConversionCodes.BGR2HSV);
            }
            using (Mat points = new Mat())
            {
                using (Mat labels = new Mat())
                {
                    using (Mat centers = new Mat())
                    {
                        int width = input.Cols;
                        int height = input.Rows;

                        points.Create(width * height, 1, MatType.CV_32FC3);
                        centers.Create(k, 1, points.Type());
                        result.Create(height, width, input.Type());

                        int i = 0;
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++, i++)
                            {
                                Vec3f vec3f = new Vec3f
                                {
                                    Item0 = input.At<Vec3b>(y, x).Item0,
                                    Item1 = input.At<Vec3b>(y, x).Item1,
                                    Item2 = input.At<Vec3b>(y, x).Item2
                                };

                                points.Set<Vec3f>(i, vec3f);
                            }
                        }

                        Cv2.Kmeans(
                            points,
                            k, labels,
                            new TermCriteria(CriteriaTypes.Eps |
                            CriteriaTypes.MaxIter, 10, 1.0),
                            3, KMeansFlags.PpCenters, centers);

                        i = 0;
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++, i++)
                            {
                                int idx = labels.Get<int>(i);

                                Vec3b vec3b = new Vec3b();
                                int tmp = 0;
                                if (cb2 == 1) //Квантование по яркости
                                {
                                    vec3b.Item0 = input.At<Vec3b>(y, x).Item0;
                                    vec3b.Item1 = input.At<Vec3b>(y, x).Item1;
                                }
                                else //Квантование по цвету
                                {
                                    tmp = Convert.ToInt32(Math.Round(centers.At<Vec3f>(idx).Item0));
                                    tmp = tmp > 255 ? 255 : tmp < 0 ? 0 : tmp;
                                    vec3b.Item0 = Convert.ToByte(tmp);

                                    tmp = Convert.ToInt32(Math.Round(centers.At<Vec3f>(idx).Item1));
                                    tmp = tmp > 255 ? 255 : tmp < 0 ? 0 : tmp;
                                    vec3b.Item1 = Convert.ToByte(tmp);
                                }

                                tmp = Convert.ToInt32(Math.Round(centers.At<Vec3f>(idx).Item2));
                                tmp = tmp > 255 ? 255 : tmp < 0 ? 0 : tmp;
                                vec3b.Item2 = Convert.ToByte(tmp);

                                result.Set<Vec3b>(y, x, vec3b);

                            }
                        }

                    }
                }
            }
            if (cb2 == 1)
            {
                Cv2.CvtColor(result, result, ColorConversionCodes.HSV2BGR);
            }
        }
    }
}
