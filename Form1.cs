using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ServerStartv2
{
    public partial class Form1 : Form
    {
        Process process;
        public Form1()
        {
            InitializeComponent();
            label1.Text = "Выбранный файл: Отcутствует";
            label1.BackColor = Color.Red;
            label1.ForeColor = Color.White;
            label2.Text = "Статус: Не выбран файл";
            label2.BackColor = Color.Gray;
            label2.ForeColor = Color.White;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "JavaScript files (*.js)|*.js|All files (*.*)|*.*";
            openFileDialog1.Multiselect = false;
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.DefaultExt = "js";
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                label1.Text = "Выбранный файл: " + openFileDialog1.FileName;
                label1.BackColor = Color.White;
                label1.ForeColor = Color.Green;
                label2.Text = "Статус: Оффлайн";
                label2.BackColor = Color.DarkRed;
                label2.ForeColor = Color.White;
            }
        }

        // Потокобезопасный вывод в RichTextBox
        private void AppendToRichTextBox(RichTextBox box, string text, Color? color = null)
        {
            if(box.InvokeRequired)
            {
                box.Invoke((MethodInvoker)delegate
                { AppendToRichTextBox(box, text, color); });
                return;
            }

            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            if(color.HasValue)
            {
                box.SelectionColor = color.Value;
            }

            box.AppendText(text);
            box.ForeColor = Color.White;
            box.ScrollToCaret();
        }

        private async Task RunConsoleOutput(RichTextBox box, ProcessStartInfo startInfo) 
        {
            try
            {
                using (process)
                {
                    process.StartInfo = startInfo;
                    process.OutputDataReceived += (send, data) =>
                    {
                        if (!string.IsNullOrEmpty(data.Data)) AppendToRichTextBox(richTextBox1, data.Data, Color.Green);
                    };
                    process.ErrorDataReceived += (send, err) =>
                    {
                        if (!string.IsNullOrEmpty(err.Data)) AppendToRichTextBox(richTextBox1, err.Data, Color.DarkRed);
                    };

                    process.Start();
                    label2.Text = "Статус: Онлайн";
                    label2.ForeColor = Color.Green;
                    label2.BackColor = Color.White;
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    await Task.Run(() => process.WaitForExit());
                }
            }
            catch (Exception ex)
            {
                AppendToRichTextBox(richTextBox1, $"Ошибка: {ex.Message}\n", Color.DarkRed);
                label2.Text = "Статус: Оффлайн";
                label2.ForeColor = Color.White;
                label2.BackColor = Color.DarkRed;
            }
        }
        
        private async void button2_Click(object sender, EventArgs e)
        {
            var ProcessInfo = new ProcessStartInfo
            {
                FileName = @"C:\Program Files\nodejs\node.exe",
                Arguments = $"\"{openFileDialog1.FileName}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                StandardErrorEncoding = System.Text.Encoding.UTF8
            };
            if (openFileDialog1.FileName != null)
            {
                process = new Process();
                await RunConsoleOutput(richTextBox1, ProcessInfo);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            try
            {
                if (process.HasExited != false)
                {
                    process.Close();
                    label2.Text = "Статус: Оффлайн";
                    label2.ForeColor = Color.White;
                    label2.BackColor = Color.DarkRed;
                }
            }
            catch
            {
                MessageBox.Show("Этот процесс уже завершён!");
                label2.Text = "Статус: Оффлайн";
                label2.ForeColor = Color.White;
                label2.BackColor = Color.DarkRed;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
        }
    }
}
