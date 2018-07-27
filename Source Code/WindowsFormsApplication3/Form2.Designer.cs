namespace AbstractGamesCreationKit
{
    partial class FormMultipleMoves
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
            this.groupBoxMovesList = new System.Windows.Forms.GroupBox();
            this.listBoxMovesList = new System.Windows.Forms.ListBox();
            this.buttonAddMove = new System.Windows.Forms.Button();
            this.groupBoxMoveSequence = new System.Windows.Forms.GroupBox();
            this.listBoxNewMultipleMove = new System.Windows.Forms.ListBox();
            this.buttonRemoveMove = new System.Windows.Forms.Button();
            this.buttonAddMultipleMove = new System.Windows.Forms.Button();
            this.radioButtonTypeSequence = new System.Windows.Forms.RadioButton();
            this.radioButtonTypeRepeat = new System.Windows.Forms.RadioButton();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxMultiple_Move_Name = new System.Windows.Forms.TextBox();
            this.groupBoxMultipleMoveName = new System.Windows.Forms.GroupBox();
            this.groupBoxMovesList.SuspendLayout();
            this.groupBoxMoveSequence.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxMultipleMoveName.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxMovesList
            // 
            this.groupBoxMovesList.Controls.Add(this.listBoxMovesList);
            this.groupBoxMovesList.Controls.Add(this.buttonAddMove);
            this.groupBoxMovesList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxMovesList.Location = new System.Drawing.Point(12, 77);
            this.groupBoxMovesList.Name = "groupBoxMovesList";
            this.groupBoxMovesList.Size = new System.Drawing.Size(133, 186);
            this.groupBoxMovesList.TabIndex = 2;
            this.groupBoxMovesList.TabStop = false;
            this.groupBoxMovesList.Text = "Moves List";
            // 
            // listBoxMovesList
            // 
            this.listBoxMovesList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxMovesList.FormattingEnabled = true;
            this.listBoxMovesList.Location = new System.Drawing.Point(6, 19);
            this.listBoxMovesList.Name = "listBoxMovesList";
            this.listBoxMovesList.Size = new System.Drawing.Size(121, 134);
            this.listBoxMovesList.TabIndex = 0;
            this.listBoxMovesList.SelectedIndexChanged += new System.EventHandler(this.ListBoxMovesListSelectedIndexChanged);
            this.listBoxMovesList.DoubleClick += new System.EventHandler(this.ButtonAddMoveClick);
            // 
            // buttonAddMove
            // 
            this.buttonAddMove.Enabled = false;
            this.buttonAddMove.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAddMove.Location = new System.Drawing.Point(30, 156);
            this.buttonAddMove.Name = "buttonAddMove";
            this.buttonAddMove.Size = new System.Drawing.Size(66, 23);
            this.buttonAddMove.TabIndex = 27;
            this.buttonAddMove.Text = "Add Move";
            this.buttonAddMove.UseVisualStyleBackColor = true;
            this.buttonAddMove.Click += new System.EventHandler(this.ButtonAddMoveClick);
            // 
            // groupBoxMoveSequence
            // 
            this.groupBoxMoveSequence.Controls.Add(this.listBoxNewMultipleMove);
            this.groupBoxMoveSequence.Controls.Add(this.buttonRemoveMove);
            this.groupBoxMoveSequence.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxMoveSequence.Location = new System.Drawing.Point(152, 77);
            this.groupBoxMoveSequence.Name = "groupBoxMoveSequence";
            this.groupBoxMoveSequence.Size = new System.Drawing.Size(133, 186);
            this.groupBoxMoveSequence.TabIndex = 3;
            this.groupBoxMoveSequence.TabStop = false;
            this.groupBoxMoveSequence.Text = "Move Sequence";
            // 
            // listBoxNewMultipleMove
            // 
            this.listBoxNewMultipleMove.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxNewMultipleMove.FormattingEnabled = true;
            this.listBoxNewMultipleMove.Location = new System.Drawing.Point(6, 19);
            this.listBoxNewMultipleMove.Name = "listBoxNewMultipleMove";
            this.listBoxNewMultipleMove.Size = new System.Drawing.Size(121, 134);
            this.listBoxNewMultipleMove.TabIndex = 0;
            // 
            // buttonRemoveMove
            // 
            this.buttonRemoveMove.Enabled = false;
            this.buttonRemoveMove.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRemoveMove.Location = new System.Drawing.Point(32, 156);
            this.buttonRemoveMove.Name = "buttonRemoveMove";
            this.buttonRemoveMove.Size = new System.Drawing.Size(66, 23);
            this.buttonRemoveMove.TabIndex = 28;
            this.buttonRemoveMove.Text = "Remove Move";
            this.buttonRemoveMove.UseVisualStyleBackColor = true;
            this.buttonRemoveMove.Click += new System.EventHandler(this.ButtonRemoveMoveClick);
            // 
            // buttonAddMultipleMove
            // 
            this.buttonAddMultipleMove.Enabled = false;
            this.buttonAddMultipleMove.Location = new System.Drawing.Point(158, 269);
            this.buttonAddMultipleMove.Name = "buttonAddMultipleMove";
            this.buttonAddMultipleMove.Size = new System.Drawing.Size(57, 23);
            this.buttonAddMultipleMove.TabIndex = 29;
            this.buttonAddMultipleMove.Text = "Add";
            this.buttonAddMultipleMove.UseVisualStyleBackColor = true;
            this.buttonAddMultipleMove.Click += new System.EventHandler(this.ButtonAddMultipleMoveClick);
            // 
            // radioButtonTypeSequence
            // 
            this.radioButtonTypeSequence.AutoSize = true;
            this.radioButtonTypeSequence.Location = new System.Drawing.Point(6, 19);
            this.radioButtonTypeSequence.Name = "radioButtonTypeSequence";
            this.radioButtonTypeSequence.Size = new System.Drawing.Size(121, 17);
            this.radioButtonTypeSequence.TabIndex = 30;
            this.radioButtonTypeSequence.TabStop = true;
            this.radioButtonTypeSequence.Text = "Sequence of Moves";
            this.radioButtonTypeSequence.UseVisualStyleBackColor = true;
            this.radioButtonTypeSequence.CheckedChanged += new System.EventHandler(this.RadioButtonTypeCheckedChanged);
            // 
            // radioButtonTypeRepeat
            // 
            this.radioButtonTypeRepeat.AutoSize = true;
            this.radioButtonTypeRepeat.Location = new System.Drawing.Point(6, 42);
            this.radioButtonTypeRepeat.Name = "radioButtonTypeRepeat";
            this.radioButtonTypeRepeat.Size = new System.Drawing.Size(135, 17);
            this.radioButtonTypeRepeat.TabIndex = 31;
            this.radioButtonTypeRepeat.TabStop = true;
            this.radioButtonTypeRepeat.Text = "Repeat Selected Move";
            this.radioButtonTypeRepeat.UseVisualStyleBackColor = true;
            this.radioButtonTypeRepeat.CheckedChanged += new System.EventHandler(this.RadioButtonTypeCheckedChanged);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(223, 269);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(57, 23);
            this.buttonCancel.TabIndex = 32;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancelClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonTypeRepeat);
            this.groupBox2.Controls.Add(this.radioButtonTypeSequence);
            this.groupBox2.Location = new System.Drawing.Point(139, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(146, 65);
            this.groupBox2.TabIndex = 33;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Type";
            // 
            // textBoxMultiple_Move_Name
            // 
            this.textBoxMultiple_Move_Name.Location = new System.Drawing.Point(12, 25);
            this.textBoxMultiple_Move_Name.Name = "textBoxMultiple_Move_Name";
            this.textBoxMultiple_Move_Name.Size = new System.Drawing.Size(100, 20);
            this.textBoxMultiple_Move_Name.TabIndex = 34;
            this.textBoxMultiple_Move_Name.Text = "insert name here";
            this.textBoxMultiple_Move_Name.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TextBoxMouseClick);
            this.textBoxMultiple_Move_Name.Leave += new System.EventHandler(this.TextBoxFocusLeave);
            // 
            // groupBoxMultipleMoveName
            // 
            this.groupBoxMultipleMoveName.Controls.Add(this.textBoxMultiple_Move_Name);
            this.groupBoxMultipleMoveName.Location = new System.Drawing.Point(11, 5);
            this.groupBoxMultipleMoveName.Name = "groupBoxMultipleMoveName";
            this.groupBoxMultipleMoveName.Size = new System.Drawing.Size(122, 64);
            this.groupBoxMultipleMoveName.TabIndex = 35;
            this.groupBoxMultipleMoveName.TabStop = false;
            this.groupBoxMultipleMoveName.Text = "Name";
            // 
            // FormMultipleMoves
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(293, 296);
            this.Controls.Add(this.groupBoxMultipleMoveName);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonAddMultipleMove);
            this.Controls.Add(this.groupBoxMoveSequence);
            this.Controls.Add(this.groupBoxMovesList);
            this.Name = "FormMultipleMoves";
            this.Text = "Multiple Moves";
            this.groupBoxMovesList.ResumeLayout(false);
            this.groupBoxMoveSequence.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBoxMultipleMoveName.ResumeLayout(false);
            this.groupBoxMultipleMoveName.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxMovesList;
        private System.Windows.Forms.ListBox listBoxMovesList;
        private System.Windows.Forms.GroupBox groupBoxMoveSequence;
        private System.Windows.Forms.ListBox listBoxNewMultipleMove;
        private System.Windows.Forms.Button buttonRemoveMove;
        private System.Windows.Forms.Button buttonAddMove;
        private System.Windows.Forms.Button buttonAddMultipleMove;
        private System.Windows.Forms.RadioButton radioButtonTypeSequence;
        private System.Windows.Forms.RadioButton radioButtonTypeRepeat;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxMultiple_Move_Name;
        private System.Windows.Forms.GroupBox groupBoxMultipleMoveName;
    }
}