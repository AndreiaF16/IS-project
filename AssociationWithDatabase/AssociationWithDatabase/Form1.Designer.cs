namespace AssociationWithDatabase
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelTemperatura = new System.Windows.Forms.Label();
            this.labelHumidade = new System.Windows.Forms.Label();
            this.labelBateria = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelTemperatura
            // 
            this.labelTemperatura.AutoSize = true;
            this.labelTemperatura.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTemperatura.Location = new System.Drawing.Point(12, 50);
            this.labelTemperatura.Name = "labelTemperatura";
            this.labelTemperatura.Size = new System.Drawing.Size(443, 37);
            this.labelTemperatura.TabIndex = 0;
            this.labelTemperatura.Text = "Average of Temperature: 0 ºC";
            this.labelTemperatura.Click += new System.EventHandler(this.labelTemperatura_Click);
            // 
            // labelHumidade
            // 
            this.labelHumidade.AutoSize = true;
            this.labelHumidade.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHumidade.Location = new System.Drawing.Point(12, 120);
            this.labelHumidade.Name = "labelHumidade";
            this.labelHumidade.Size = new System.Drawing.Size(377, 37);
            this.labelHumidade.TabIndex = 1;
            this.labelHumidade.Text = "Average of Humidity: 0 %";
            this.labelHumidade.Click += new System.EventHandler(this.labelHumidade_Click);
            // 
            // labelBateria
            // 
            this.labelBateria.AutoSize = true;
            this.labelBateria.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBateria.Location = new System.Drawing.Point(12, 192);
            this.labelBateria.Name = "labelBateria";
            this.labelBateria.Size = new System.Drawing.Size(353, 37);
            this.labelBateria.TabIndex = 2;
            this.labelBateria.Text = "Average of Battery: 0 %";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 299);
            this.Controls.Add(this.labelBateria);
            this.Controls.Add(this.labelHumidade);
            this.Controls.Add(this.labelTemperatura);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Association With Database";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing_1);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTemperatura;
        private System.Windows.Forms.Label labelHumidade;
        private System.Windows.Forms.Label labelBateria;
    }
}

