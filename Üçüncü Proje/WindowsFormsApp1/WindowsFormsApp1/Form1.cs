using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using AForge;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using Point = System.Drawing.Point;
using System.IO.Ports;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection kamera;
        private VideoCaptureDevice Finalvideo;
        public Form1()
        {
            InitializeComponent();
        }
        int R,G,B;
          
        private void Form1_Load(object sender, EventArgs e)
        {
            kamera = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo goruntu in kamera)
            {
                comboBox1.Items.Add(goruntu.Name);
            }
            comboBox1.SelectedIndex = 0;

            LoadPorts(); // Portları ekleme
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Finalvideo = new VideoCaptureDevice(kamera[comboBox1.SelectedIndex].MonikerString);
            Finalvideo.NewFrame += new NewFrameEventHandler(Finalvideo_NewFrame);
            Finalvideo.DesiredFrameRate = 30;//saniyede kaç görüntü alsın istiyorsanız. FPS
            Finalvideo.DesiredFrameSize = new Size(500, 300);//görüntü boyutları
            Finalvideo.Start();
        }
        void Finalvideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            Bitmap image1 = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = image;
                        
              
             
            
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                filter.CenterColor = new RGB(Color.FromArgb(R, G, B));
                filter.Radius = 100;
                filter.ApplyInPlace(image1);
                nesnebul(image1);
             
        }
        public void nesnebul(Bitmap image)
        {
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.MinWidth = 10;
            blobCounter.MinHeight = 10;
            blobCounter.FilterBlobs = true; // Parazitleri atar
            blobCounter.ObjectsOrder = ObjectsOrder.Size;
            BitmapData objectsData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            Grayscale grayscaleFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            UnmanagedImage grayImage = grayscaleFilter.Apply(new UnmanagedImage(objectsData));
            image.UnlockBits(objectsData);
            blobCounter.ProcessImage(image);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            Blob[] blobs = blobCounter.GetObjectsInformation();
            pictureBox2.Image = image;
            if (radioButton1.Checked)
            {
                for (int i = 0; rects.Length > i; i++)
                {
                    Rectangle objectRect = rects[i];
                    Graphics g = pictureBox1.CreateGraphics();
                    if (objectRect.Width * objectRect.Height < 6500 && objectRect.Width * objectRect.Height > 3250)
                    {
                        using (Pen pen = new Pen(Color.FromArgb(252, 3, 26), 2))
                        {
                            g.DrawRectangle(pen, objectRect);
                            g.DrawString((i + 1).ToString(), new Font("Arial", 12), Brushes.Red, objectRect);
                        }

                        int objectX = objectRect.X + (objectRect.Width / 2);
                        int objectY = objectRect.Y + (objectRect.Height / 2);

                        if (objectY < 100)
                        {
                            if (objectX < 250)
                                WriteToPort("a");
                            else
                                WriteToPort("d");
                        }
                        else if(objectY > 200)
                        {
                            if (objectX > 250)
                                WriteToPort("c");
                            else
                                WriteToPort("f");
                        }
                        else
                        {
                            if (objectX < 250)
                                WriteToPort("b");
                            else
                                WriteToPort("e");
                        }
                    }
                    g.Dispose();
                }
            }
        }
        public void WriteToPort(string text)
        {
            byte[] bytes = serialPort1.Encoding.GetBytes(text);
            serialPort1.Write(bytes , 0, bytes.Length);
        }
        public void LoadPorts()
        {
            for (int i = 0; i < 30; i++)
            {
                try
                {
                    serialPort1.PortName = "COM" + i.ToString();
                    serialPort1.Open();
                    cbPort.Items.Add(serialPort1.PortName);
                    serialPort1.Close();
                }
                catch (Exception)
                { continue; }
            }
        }
       
        private void button2_Click(object sender, EventArgs e)
        {
            if (Finalvideo.IsRunning)
            {
                Finalvideo.Stop();              
            }
        }     
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            R = trackBar1.Value;
            label1.Text = trackBar1.Value.ToString();
        }
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            G = trackBar2.Value;
            label2.Text = trackBar2.Value.ToString();
        }
        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            B = trackBar3.Value;
            label6.Text = trackBar3.Value.ToString();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (Finalvideo.IsRunning)
            {
                Finalvideo.Stop();
            }
            Application.Exit();
        }

        private void cbPort_SelectedValueChanged(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
                serialPort1.Close();
           
            serialPort1.PortName = cbPort.Text;
            serialPort1.Open();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPort1.IsOpen)
                serialPort1.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }


    }
}