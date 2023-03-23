using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
namespace EMS_Editor
{
    public partial class Form1 : Form
    {
        OpenFileDialog dialog = new OpenFileDialog();
        EMS.EMS_Struct emsStruct = new EMS.EMS_Struct();
        EMS ems;

        // Global variables
        string filepath = "";
        public Form1()
        {
            InitializeComponent();
        }
        public void CreateTable()
        {
            mainTable.Columns.Clear();
            mainTable.Columns.Add("active", "Active"); // 0
            mainTable.Columns.Add("index", "Index"); // 1
            mainTable.Columns.Add("animation", "Animation"); // 2
            mainTable.Columns.Add("member", "Member"); // 3
            mainTable.Columns.Add("family", "Family"); // 4
            mainTable.Columns.Add("weapon", "Weapon"); // 5
            mainTable.Columns.Add("variation", "Variation"); // 6
            mainTable.Columns.Add("unk1", "Unk 1"); // 7
            mainTable.Columns.Add("action", "Action"); // 8
            mainTable.Columns.Add("posX", "Position X"); // 9
            mainTable.Columns.Add("posY", "Position Y"); // 10
            mainTable.Columns.Add("posZ", "Position Z"); // 11
            mainTable.Columns.Add("rotY", "Rotation Y"); // 12
        }
        public void FormatTable()
        {
            int rowCount = mainTable.Rows.Count;
            int columnCount = mainTable.Columns.Count;

            // Convert fields to Hexadecimal
            for (int column = 0; column < 9; column++)
            {
                mainTable.Columns[column].Width = 62;
                mainTable.Columns[column].DefaultCellStyle.Format = "X";
            }

            // Sets alignment
            for (int i = 0; i < columnCount; i++)
            {
                mainTable.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                mainTable.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            mainTable.Columns[7].Visible = false;
            mainTable.Columns[9].DefaultCellStyle.Format = "0.000";
            mainTable.Columns[10].DefaultCellStyle.Format = "0.000";
            mainTable.Columns[11].DefaultCellStyle.Format = "0.000";
            mainTable.Columns[12].DefaultCellStyle.Format = "0.000";
        }
        public uint[] GetDatOffsets()
        {
            BinaryReader br = new BinaryReader(File.Open(filepath, FileMode.Open));
            uint filecount = br.ReadUInt32();
            uint[] offsets = new uint[filecount];

            // Stores all offsets from .dat file
            for (int file = 0; file < filecount; file++)
            {
                offsets[file] = br.ReadUInt32();
            }
            br.Close();

            return offsets;
        }
        public uint FindEmsOffset()
        {
            BinaryReader br = new BinaryReader(File.Open(filepath, FileMode.Open));

            uint[] offsets = GetDatOffsets();
            uint emsStartOffset = 0;

            // Reads extensions and find which index is EMS, then grab the offset value
            for (int ext = 0; ext < offsets.Length; ext++)
            {
                if (br.ReadUInt32() == 0x00534D45)
                {
                    emsStartOffset = offsets[ext];
                    break;
                }
                if (ext == offsets.Length)
                {
                    MessageBox.Show("EMS file note found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            br.Close();
            return emsStartOffset;
        }
        public void UpdateDatOffsets(string operation)
        {
            if (Path.GetExtension(filepath) == ".dat")
            {
                BinaryReader br = new BinaryReader(File.Open(filepath, FileMode.Open));

                uint filecount = br.ReadUInt32();
                uint[] offsets = new uint[filecount];
                int emsFileIndex = 0;

                // Stores all offsets from .dat file
                for (int file = 0; file < filecount; file++)
                {
                    offsets[file] = br.ReadUInt32();
                }

                // Reads extensions and find which index is EMS, then grab the offset value
                for (int ext = 0; ext < filecount; ext++)
                {
                    if (br.ReadUInt32() == 0x00534D45)
                    {
                        emsFileIndex = ext;
                        break;
                    }
                }
                br.Close();

                // Updates .dat offsets if new enemy row was added
                BinaryWriter bw = new BinaryWriter(File.Open(filepath, FileMode.Open));
                bw.BaseStream.Position = 0x08 + (0x04 * emsFileIndex);

                if (operation == "add")
                {
                    for (int i = emsFileIndex + 1; i < offsets.Length; i++)
                    {
                        bw.Write((uint)(offsets[i] + 0x40));
                    }
                }
                else if (operation == "remove")
                {
                    for (int i = emsFileIndex + 1; i < offsets.Length; i++)
                    {
                        bw.Write((uint)(offsets[i] - 0x40));
                    }
                }
                bw.Close();

            }
        }
        public void CreateBackup()
        {
            try
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\backup");
                File.Copy(filepath, Directory.GetCurrentDirectory() + $"\\backup\\{Path.GetFileName(filepath)}", true);
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }
        public void LoadEnemyData()
        {
            CreateTable();

            // Creates backup if checked
            if (autoBackupToolStripMenuItem.Checked)
            {
                CreateBackup();
            }

            // Check if file is .dat format
            uint emsStartOffset = 0;
            if (Path.GetExtension(filepath) == ".dat")
            {
                emsStartOffset = FindEmsOffset();
            }

            BinaryReader br = new BinaryReader(File.Open(filepath, FileMode.Open));
            br.BaseStream.Position = emsStartOffset;

            // Gets magic and verifies if it's EMS
            emsStruct.Magic = br.ReadUInt32();
            if (emsStruct.Magic != 0x00534D45)
            {
                MessageBox.Show("File does not contain .EMS, please choose a valid .EMS or .DAT file.", "Error reading file structure",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                mainTable.Rows.Clear();
                return;
            }

            emsStruct.EnemyCount = br.ReadUInt32();
            emsStruct.Unk1 = br.ReadUInt32();
            lblEnemyCount.Text = emsStruct.EnemyCount.ToString();

            for (int i = 0; i < emsStruct.EnemyCount; i++)
            {
                // Getting data to struct
                emsStruct.PosX = br.ReadSingle();
                emsStruct.PosY = br.ReadSingle();
                emsStruct.PosZ = br.ReadSingle();
                emsStruct.Padding1 = br.ReadSingle();
                emsStruct.RotX = br.ReadSingle();
                emsStruct.RotY = br.ReadSingle();
                emsStruct.RotZ = br.ReadSingle();
                emsStruct.Padding2 = br.ReadSingle();
                emsStruct.Weapon = br.ReadByte();
                emsStruct.Unk2 = br.ReadByte();
                emsStruct.Action = br.ReadByte();
                emsStruct.isSpawned = br.ReadByte();
                emsStruct.Variation = br.ReadUInt32();
                emsStruct.Animation = br.ReadByte();
                emsStruct.EnemySubtype = br.ReadByte();
                emsStruct.EnemyType = br.ReadByte();
                emsStruct.Index = br.ReadByte();
                emsStruct.Unk3 = br.ReadUInt32();
                emsStruct.Unk4 = br.ReadUInt32();
                emsStruct.Unk5 = br.ReadUInt32();
                emsStruct.Unk6 = br.ReadUInt32();
                emsStruct.Unk7 = br.ReadUInt32();

                // Create rows
                var index = mainTable.Rows.Add();
                mainTable.Rows[index].Cells[0].Value = emsStruct.isSpawned.ToString("X");
                mainTable.Rows[index].Cells[1].Value = emsStruct.Index.ToString("X");
                mainTable.Rows[index].Cells[2].Value = emsStruct.Animation.ToString("X");
                mainTable.Rows[index].Cells[3].Value = emsStruct.EnemySubtype.ToString("X");
                mainTable.Rows[index].Cells[4].Value = emsStruct.EnemyType.ToString("X");
                mainTable.Rows[index].Cells[5].Value = emsStruct.Weapon.ToString("X");
                mainTable.Rows[index].Cells[6].Value = emsStruct.Variation.ToString("X");
                mainTable.Rows[index].Cells[7].Value = emsStruct.Unk2.ToString("X");
                mainTable.Rows[index].Cells[8].Value = emsStruct.Action.ToString("X");
                mainTable.Rows[index].Cells[9].Value = emsStruct.PosX;
                mainTable.Rows[index].Cells[10].Value = emsStruct.PosY;
                mainTable.Rows[index].Cells[11].Value = emsStruct.PosZ;
                mainTable.Rows[index].Cells[12].Value = emsStruct.RotY;
            }
            FormatTable();
            br.Close();
        }
        public void SaveEnemyData()
        {
            // Check if file is .dat format
            uint emsStartOffset = 0;
            if (Path.GetExtension(filepath) == ".dat")
            {
                emsStartOffset = FindEmsOffset();
            }

            BinaryWriter bw = new BinaryWriter(File.Open(filepath, FileMode.Open));

            // Go to EMS start offset and updates enemy index
            bw.BaseStream.Position = emsStartOffset + 0x04;
            bw.Write(Convert.ToInt32(lblEnemyCount.Text));
            bw.BaseStream.Position += 0x04;

            // Loop through each row and writes values
            for (int row = 0; row < Convert.ToInt32(lblEnemyCount.Text); row++)
            {
                byte weapon = Byte.Parse(mainTable.Rows[row].Cells[5].Value.ToString(), NumberStyles.HexNumber);
                byte unk1 = Byte.Parse(mainTable.Rows[row].Cells[7].Value.ToString(), NumberStyles.HexNumber);
                byte action = Byte.Parse(mainTable.Rows[row].Cells[8].Value.ToString(), NumberStyles.HexNumber);
                byte isSpawned = Byte.Parse(mainTable.Rows[row].Cells[0].Value.ToString(), NumberStyles.HexNumber);
                uint variation = UInt32.Parse(mainTable.Rows[row].Cells[6].Value.ToString(), NumberStyles.HexNumber);
                byte animation = Byte.Parse(mainTable.Rows[row].Cells[2].Value.ToString(), NumberStyles.HexNumber);
                byte member = Byte.Parse(mainTable.Rows[row].Cells[3].Value.ToString(), NumberStyles.HexNumber);
                byte family = Byte.Parse(mainTable.Rows[row].Cells[4].Value.ToString(), NumberStyles.HexNumber);
                byte index = Byte.Parse(mainTable.Rows[row].Cells[1].Value.ToString(), NumberStyles.HexNumber);

                bw.Write(Convert.ToSingle(mainTable.Rows[row].Cells[9].Value)); // Position X
                bw.Write(Convert.ToSingle(mainTable.Rows[row].Cells[10].Value)); // Position Y
                bw.Write(Convert.ToSingle(mainTable.Rows[row].Cells[11].Value)); // Position Z
                bw.BaseStream.Position += 0x08;
                bw.Write(Convert.ToSingle(mainTable.Rows[row].Cells[12].Value)); // Rotation Y
                bw.BaseStream.Position += 0x08;
                bw.Write(weapon); // Weapon
                bw.Write(unk1); // Unknown 1
                bw.Write(action); // Action
                bw.Write(isSpawned); // Is Spawned
                bw.Write(variation); // Variation
                bw.Write(animation); // Animation
                bw.Write(member); // Member
                bw.Write(family); // Family
                bw.Write(index); // Index
                bw.BaseStream.Position += 0x14;
            }
            bw.Close();
            MessageBox.Show("File saved successfully!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void AddNewEnemy()
        {
            if (Path.GetExtension(filepath) == ".dat")
            {
                BinaryReader br = new BinaryReader(File.Open(filepath, FileMode.Open));
                uint filecount = br.ReadUInt32();
                uint[] offsets = new uint[filecount];
                uint splitOffset = 0;
                uint emsStartOffset = 0;

                // Stores all offsets from .dat file
                for (int file = 0; file < filecount; file++)
                {
                    offsets[file] = br.ReadUInt32();
                }

                // Reads extensions and find which index is EMS, then grab the offset value
                for (int ext = 0; ext < filecount; ext++)
                {
                    if (br.ReadUInt32() == 0x00534D45)
                    {
                        splitOffset = offsets[ext + 1];
                        emsStartOffset = offsets[ext];
                        break;
                    }
                    if (ext == filecount)
                    {
                        MessageBox.Show("Cannot find EMS offset to add new enemy.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }

                // Split the file, so we can append new data
                br.BaseStream.Position = 0;
                byte[] topPart = br.ReadBytes((int)splitOffset);
                byte[] bottomPart = br.ReadBytes((int)(br.BaseStream.Length - (int)splitOffset));
                br.Close();

                // Writes the new file and appends new 0x40 bytes for new enemy chunk
                BinaryWriter bw = new BinaryWriter(File.Open(filepath, FileMode.Create));
                bw.Write(topPart);
                bw.Write(new byte[0x40]);
                bw.Write((bottomPart));

                // Updates enemy count
                bw.BaseStream.Position = emsStartOffset + 0x04;
                bw.Write(Convert.ToUInt32(lblEnemyCount.Text) + 1);
                bw.Close();
            }
            else
            {
                // Append new 0x40 to the end of the file
                BinaryWriter bwAppend = new BinaryWriter(File.Open(filepath, FileMode.Append));
                bwAppend.Write(new byte[0x40]);
                bwAppend.Close();

                // Updates enemy count
                BinaryWriter bw = new BinaryWriter(File.Open(filepath, FileMode.Open));
                bw.BaseStream.Position = 0x04;
                bw.Write(Convert.ToUInt32(lblEnemyCount.Text) + 1);
                bw.Close();
            }

            // Updates index label
            uint tempCount = Convert.ToUInt32(lblEnemyCount.Text) + 1;
            lblEnemyCount.Text = tempCount.ToString();

            // If it's a .dat file, updates offsets
            if (Path.GetExtension(filepath) == ".dat")
            {
                UpdateDatOffsets("add");
            }

            // Creating new row
            var index = mainTable.Rows.Add();
            mainTable.Rows[index].Cells[0].Value = 0;
            mainTable.Rows[index].Cells[1].Value = 0;
            mainTable.Rows[index].Cells[2].Value = 0;
            mainTable.Rows[index].Cells[3].Value = 0;
            mainTable.Rows[index].Cells[4].Value = 0;
            mainTable.Rows[index].Cells[5].Value = 0;
            mainTable.Rows[index].Cells[6].Value = 0;
            mainTable.Rows[index].Cells[7].Value = 0;
            mainTable.Rows[index].Cells[8].Value = 0;
            mainTable.Rows[index].Cells[9].Value = 0;
            mainTable.Rows[index].Cells[10].Value = 0;
            mainTable.Rows[index].Cells[11].Value = 0;
            mainTable.Rows[index].Cells[12].Value = 0;

            FormatTable();
        }
        public void RemoveEnemy()
        {
            if (Path.GetExtension(filepath) == ".dat")
            {
                uint emsStartOffset = FindEmsOffset();

                for (int i = 0; i < mainTable.SelectedRows.Count; i++)
                {
                    BinaryReader br = new BinaryReader(File.Open(filepath, FileMode.Open));
                    br.BaseStream.Position = 0;

                    // Splits the file
                    byte[] topPart = br.ReadBytes((int)(emsStartOffset + (mainTable.SelectedRows[i].Index * 0x40) + 0x0C));
                    br.BaseStream.Position += 0x40;
                    byte[] bottomPart = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
                    br.Close();

                    // Writes the new file and appends new 0x40 bytes for new enemy chunk
                    BinaryWriter bw = new BinaryWriter(File.Open(filepath, FileMode.Create));
                    bw.Write(topPart);
                    bw.Write(bottomPart);

                    // Updates index
                    bw.BaseStream.Position = emsStartOffset + 0x04;
                    bw.Write(Convert.ToUInt32(lblEnemyCount.Text) - 1);
                    bw.Close();
                    mainTable.Rows.Remove(mainTable.SelectedRows[i]);

                    // Updates index label
                    uint tempCount = Convert.ToUInt32(lblEnemyCount.Text) - 1;
                    lblEnemyCount.Text = tempCount.ToString();

                    // If it's a .dat file, updates offsets
                    UpdateDatOffsets("remove");
                }
            }
            else
            {
                for (int i = 0; i < mainTable.SelectedRows.Count; i++)
                {
                    BinaryReader br = new BinaryReader(File.Open(filepath, FileMode.Open));
                    br.BaseStream.Position = 0;

                    // Splits the file
                    byte[] topPart = br.ReadBytes(mainTable.SelectedRows[i].Index * 0x40 + 0x0C);
                    br.BaseStream.Position += 0x40;
                    byte[] bottomPart = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
                    br.Close();

                    // Writes the new file and appends new 0x40 bytes for new enemy chunk
                    BinaryWriter bw = new BinaryWriter(File.Open(filepath, FileMode.Create));
                    bw.Write(topPart);
                    bw.Write(bottomPart);

                    // Updates index
                    bw.BaseStream.Position = 0x04;
                    bw.Write(Convert.ToUInt32(lblEnemyCount.Text) - 1);
                    bw.Close();

                    mainTable.Rows.Remove(mainTable.SelectedRows[i]);

                    // Updates index label
                    uint tempCount = Convert.ToUInt32(lblEnemyCount.Text) - 1;
                    lblEnemyCount.Text = tempCount.ToString();
                }
            }
        }

        // MENU BUTTONS
        private void openemsFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dialog.Filter = "God Hand Files(*.EMS,*.DAT)|*.EMS;*.DAT";
            dialog.ShowDialog();
            filepath = dialog.FileName;
            this.Text = "God Hand Enemy Editor " + filepath;
            LoadEnemyData();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            AddNewEnemy();
        }
        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveEnemyData();
        }
        private void btnRemoveEnemy_Click(object sender, EventArgs e)
        {
            RemoveEnemy();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void mainTable_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveEnemy();
            }
        }

        private void creditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var creditsForm = new Form2();
            creditsForm.Show();
        }
    }
}
