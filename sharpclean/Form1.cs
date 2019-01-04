using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ImageMagick;

namespace sharpclean
{
    public partial class Form1 : Form
    {
        fileOps mapCleanup = new fileOps();
        image img = new image();
        Image mapImage = null;
        toolbox tBox = null;
        string mapPath = "";
        string dirPath;
        string trajPath;
        string offsetPath;
        string tempPGMPath = "";
        string tempPNGPath = "";

        public Form1()
        {
            InitializeComponent();
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
        }

        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e) // Handle any unfinished things in the program
        {
            #region Try to delete the temporary files if they are in the directory
            try
            {
                File.Delete(mapCleanup.getTempPath());
            }
            catch (Exception ee)
            {
                Console.WriteLine("No file (temp.pgm) found to delete. Error: " + ee);
            }
            try
            {
                File.Delete(tempPGMPath);
            }
            catch (Exception ee)
            {
                Console.WriteLine("No file (temp2.pgm) found to delete. Error: " + ee);
            }
            try
            {
                pictureBox1.Image.Dispose();
                File.Delete(tempPNGPath);
            }
            catch (Exception ee)
            {
                Console.WriteLine("No file (temp.png) found to delete. Error: " + ee);
            }
            #endregion
        }

        // This is the button click to save the file paths and load the image into the picture box
        private void button1_Click(object sender, EventArgs e)
        {
            // Assign map path by bringing up file dialog
            this.mapPath = this.mapCleanup.getImage();

            // Only continue if a valid .png file was selected
            if (this.mapPath != "err::no_map_selected")
            {
                // Reset the progress bar
                progressBar1.Visible = false;
                label1.Visible = false;

                // Disable the Save Button
                button3.Enabled = false;

                #region Try to dispose of the previous map image and delete the temporary file(s)
                try
                {
                    File.Delete(mapCleanup.getTempPath());
                    this.mapImage.Dispose();
                }
                catch (Exception ee)
                {
                    Console.WriteLine("Exception thrown: " + ee);
                }
                try
                {
                    File.Delete(tempPGMPath);
                }
                catch (Exception ee)
                {
                    Console.WriteLine("No file (temp2.pgm) found to delete. Error: " + ee);
                }
                try
                {
                    pictureBox1.Image.Dispose();
                    File.Delete(tempPNGPath);
                }
                catch (Exception ee)
                {
                    Console.WriteLine("No file (temp.png) found to delete. Error: " + ee);
                }
                #endregion

                // Assign the image and clear the picturebox
                this.mapImage = Image.FromFile(this.mapPath);
                pictureBox1.Image = null;

                // Assign the image to the picture box
                pictureBox1.Image = this.mapImage;

                // Assign the directory path and load the trajectory and offset files
                this.dirPath = this.mapCleanup.getDir();

                // Assign the paths for the offset and trajectory files
                this.offsetPath = mapCleanup.getOffset();
                this.trajPath = mapCleanup.getTraj();

                // Generate a .pgm file 
                string pgmPath = mapCleanup.generatePGM();

                // Hide the generated .pgm file
                File.SetAttributes(pgmPath, FileAttributes.Hidden);

                // Make the store info headers visible
                label2.Visible = true;
                label3.Visible = true;

                // Populate the labels with the store name and the store number
                label4.Text = mapCleanup.getStoreInfo("name");
                label5.Text = mapCleanup.getStoreInfo("number");

                // Load the image
                if (img.load(pgmPath))
                {
                    tBox = new toolbox(img.getpixels(), img.getImageData().width, img.getImageData().totalpixels);
                }

                // Make the Clean Map button clickable
                button2.Enabled = true; // Clean Map Button

                // Make the progress bar and bar label visible to the user
                label1.Visible = true;
                progressBar1.Visible = true;
                progressBar1.Value = 0;
                progressBar1.Maximum = img.getImageData().totalpixels;
            }
        }

        private void button2_Click(object sender, EventArgs e) // Cleans the map
        {   
            if (tBox != null)
            {
                // Update the progress bar as the cleaning is performed
                tBox.clean(progressBar1);
                
                // Create a path to temporary cleaned .pgm and .png files
                this.tempPGMPath = mapCleanup.getDir() + "\\" + "temp2.pgm";
                this.tempPNGPath = mapCleanup.getDir() + "\\" + "temp.png";

                // Create a temporary cleaned .pgm file to hold the cleaned map
                img.write(tempPGMPath);

                // Create a new temporary cleaned .png file to hold the cleaned map to be used by the picturebox
                using (MagickImage newPNG = new MagickImage(this.tempPGMPath))
                {
                    newPNG.Format = MagickFormat.Png;
                    byte[] pngData = newPNG.ToByteArray();
                    File.WriteAllBytes(tempPNGPath, pngData);
                }

                // Hide the temporary .png and .pgm files so the user can't select or delete them accidentally
                File.SetAttributes(this.tempPGMPath, FileAttributes.Hidden);
                File.SetAttributes(this.tempPNGPath, FileAttributes.Hidden);

                // Set the picture box image to temp.png (This image is a cleaned version of the map, used for display purposes only)
                pictureBox1.Image.Dispose();
                pictureBox1.Image = Image.FromFile(this.tempPNGPath);

                // Display a message and enable saving upon success
                MessageBox.Show("Cleaning is done!", "Clean done", 0);
                button3.Enabled = true; // Save Map Button

                // Disable the Clean Map button
                button2.Enabled = false;
            }
            else
                MessageBox.Show("Toolbox was not loaded!", "Toolbox not loaded", 0);
        }

        private void button3_Click(object sender, EventArgs e) // Saves the file
        {
            string fileSaveName = mapCleanup.getSaveFile();

            try
            {
                img.write(fileSaveName);
                MessageBox.Show("File sucessfully saved!", "File Saved", 0);
            }
            catch
            {
                MessageBox.Show("File was not saved.", "File Not Saved", 0);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This saves the cleaning data to an excel file, replace this with actual code!");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Maybe we want to have a readme doc for FAQ and assistance on GITHUB and this links to it???

            // Display the Help Form
            helpForm newHelpForm = new helpForm();

            newHelpForm.Show();
            

            //MessageBox.Show("This displays the help feature, replace this with actual help instructions!");

        }
    }
}
