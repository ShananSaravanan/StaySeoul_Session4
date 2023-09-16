using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace StaySeoul_Session4
{
    public partial class Form1 : Form
    {
        SS_4Entities ent = new SS_4Entities();
        string searchType;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd/MM/yyyy";
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "dd/MM/yyyy";
            dateTimePicker3.Format = DateTimePickerFormat.Custom;
            dateTimePicker3.CustomFormat = "dd/MM/yyyy";
            dateTimePicker1.MinDate = DateTime.Today;
            dateTimePicker2.MinDate = DateTime.Today;
            dateTimePicker3.MinDate = DateTime.Today.AddDays(1);
            listView1.Visible = false;
            listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            dataGridView1.Visible = true;
            dataGridView2.Visible = false;
            toolStripStatusLabel2.Text = "Displaying " + (dataGridView1.Rows.Count).ToString() + " different option(s)";
        }
        public void loadAdvancedData()
        {
            areaCmbBox.Items.Insert(0, "");
            areaCmbBox.SelectedIndex = 0;
            var areas = ent.Areas.ToList();
            foreach (var a in areas)
            {
                areaCmbBox.Items.Add(a.Name);
            }
            var amenities = ent.Amenities.ToList();
            foreach (var a in amenities)
            {
                amenity1.Items.Add(a.Name);
                amenity2.Items.Add(a.Name);
                amenity3.Items.Add(a.Name);
            }
            var itemTypes = ent.ItemTypes;
            pTypeCmbBox.Items.Insert(0, "");
            pTypeCmbBox.SelectedIndex = 0;
            foreach (var i in itemTypes)
            {
                pTypeCmbBox.Items.Add(i.Name);
            }
        }

        private void advancedBtn1_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            dataGridView2.Visible = true;
            groupBox1.Visible = false;
            groupBox3.Visible = true;
            groupBox2.Location = new System.Drawing.Point(0, 250);
            this.Height = 613;
            loadAdvancedData();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker3.MinDate = dateTimePicker2.Value.AddDays(1);
        }

        private void simpleSearchBtn_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = true;
            dataGridView2.Visible = false;
            groupBox3.Visible = false;
            groupBox1.Visible = true;
            groupBox2.Location = new System.Drawing.Point(0, 130);
            this.Height = 480;
        }

        private void searchTxtBox_TextChanged(object sender, EventArgs e)
        {
            if (searchTxtBox.Text.Length > 2)
            {
                listView1.Visible = true;
                var items = ent.Items.Where(x => x.Title.Contains(searchTxtBox.Text));
                var areas = ent.Areas.Where(x => x.Name.Contains(searchTxtBox.Text));
                var attractions = ent.Attractions.Where(x => x.Name.Contains(searchTxtBox.Text));
                var amenities = ent.Amenities.Where(x => x.Name.Contains(searchTxtBox.Text));
                var types = ent.ItemTypes.Where(x => x.Name.Contains(searchTxtBox.Text));
                if (items != null || areas != null || attractions != null || amenities != null || types != null)
                {
                    listView1.Items.Clear();
                    foreach (var item in items)
                    {

                        listView1.Items.Add(item.Title).SubItems.Add("Listing");
                    }
                    foreach (var area in areas)
                    {
                        listView1.Items.Add(area.Name).SubItems.Add("Area");
                    }
                    foreach (var attraction in attractions)
                    {
                        listView1.Items.Add(attraction.Name).SubItems.Add("Attraction");
                    }
                    foreach (var amenity in amenities)
                    {
                        listView1.Items.Add(amenity.Name).SubItems.Add("Amenity");
                    }
                    foreach (var type in types)
                    {
                        listView1.Items.Add(type.Name).SubItems.Add("Property Type");
                    }

                }
            }
            else
            {
                listView1.Visible = false;
            }
        }


        private void listView1_DoubleClick(object sender, EventArgs e)
        {

            searchType = listView1.SelectedItems[0].SubItems[1].Text;
            searchTxtBox.Text = listView1.SelectedItems[0].Text;
            listView1.Visible = false;
        }
        public void loadData(List<ItemPrice> items, DataGridView dg)
        {
            DateTime startdate = dateTimePicker1.Value;
            dg.Rows.Clear();
            List<ItemPrice> sysChosenDates = new List<ItemPrice>();
            sysChosenDates.Clear();
            var selectDates = items.Where(x => x.Date >= startdate).ToList();
            int nights = (int)nights1.Value;
            if (dg == dataGridView2)
            {
                startdate = dateTimePicker2.Value;
                DateTime enddate = dateTimePicker3.Value;
                selectDates = items.Where(x => x.Date >= startdate && x.Date <= enddate).ToList();
                nights = (int)nights2.Value;
            }
            
            if (selectDates.Any())
            {


                for (int j = 0; j < selectDates.Count; j++)
                {
                    int count = 0;
                    int k = 0;
                    for (int i = j; i < nights + j; i++)
                    {
                        if ((nights + j) > selectDates.Count)
                        {
                            break;
                        }
                        else if (selectDates[i].Date == selectDates[j].Date.AddDays(k))
                        {

                            count++;
                        }
                        k++;
                    }
                    if (count >= nights)
                    {
                        sysChosenDates.Add(selectDates[j]);
                    }

                }
                if (sysChosenDates.Any())
                {
                    if (dg == dataGridView2)
                    {
                        foreach(var i in sysChosenDates)
                        {
                            var itemID = i.ItemID;
                            var scores = ent.ItemScores.Where(x => x.ItemID == itemID).Average(x => x.Value);
                            var reservations = ent.BookingDetails.Count(x => x.ItemPrice.ItemID == itemID);
                            dataGridView2.Rows.Add(i.Item.Title, i.Item.Area.Name, Math.Round(scores, 1), reservations, i.Price,i.Date.ToShortDateString());
                        }
                    }
                    var ID = sysChosenDates.ToLookup(x => x.ItemID);
                    foreach (var i in ID)
                    {
                        var itemID = i.FirstOrDefault().ItemID;
                        decimal price = 0;
                        var scores = ent.ItemScores.Where(x => x.ItemID == itemID).Average(x => x.Value);
                        var reservations = ent.BookingDetails.Count(x => x.ItemPrice.ItemID == itemID);
                        var payable = sysChosenDates.Where(x => x.ItemID == itemID).OrderByDescending(x => x.Price).Take(nights);
                        foreach (var p in payable)
                        {
                            price += p.Price;
                        }

                        dataGridView1.Rows.Add(i.FirstOrDefault().Item.Title, i.FirstOrDefault().Item.Area.Name, Math.Round(scores, 1), reservations, price);
                        
                    }

                }
                else
                {
                    MessageBox.Show("No rooms are available for selected number of nights");
                }

            }
            else
            {
                MessageBox.Show("No rooms available from selected date");
            }
            if (dg == dataGridView1)
            {
                toolStripStatusLabel2.Text = "Displaying " + dataGridView1.Rows.Count + " different option(s)";
            }
            if(dg == dataGridView2)
            {
                toolStripStatusLabel2.Text = "Displaying " + dataGridView2.Rows.Count + " different option(s) from " + sysChosenDates.ToLookup(x => x.ItemID).Count + " properties";
            }
            


        }
        private void searchBtn_Click(object sender, EventArgs e)
        {

            var searchedItems = ent.ItemPrices.Where(x => x.Item.Title == searchTxtBox.Text);
            if (string.IsNullOrEmpty(searchTxtBox.Text) == false)
            {
                if (searchType == "Listing")
                {
                    searchedItems = ent.ItemPrices.Where(x => x.Item.Title == searchTxtBox.Text && x.Item.Capacity >= (int)nights1.Value);
                }
                if (searchType == "Area")
                {
                    searchedItems = ent.ItemPrices.Where(x => x.Item.Area.Name == searchTxtBox.Text && x.Item.Capacity >= (int)nights1.Value);

                }
                if (searchType == "Attraction")
                {
                    searchedItems = ent.ItemPrices.Where(x => x.Item.ItemAttractions.Any(y => y.Attraction.Name == searchTxtBox.Text) && x.Item.Capacity >= (int)nights1.Value);

                }
                if (searchType == "Property Type")
                {
                    searchedItems = ent.ItemPrices.Where(x => x.Item.ItemType.Name == searchTxtBox.Text && x.Item.Capacity >= (int)nights1.Value);

                }
                if (searchType == "Amenity")
                {
                    searchedItems = ent.ItemPrices.Where(x => x.Item.ItemAmenities.Any(y => y.Amenity.Name == searchTxtBox.Text) && x.Item.Capacity >= (int)nights1.Value);

                }
                if (searchedItems.Any())
                {
                    var availables = searchedItems.Select(x => x.ID).Except(ent.BookingDetails.Select(y => y.ItemPriceID));
                    if (availables.Any())
                    {
                        List<ItemPrice> items = new List<ItemPrice>();
                        items.Clear();
                        foreach (var a in availables)
                        {
                            var details = ent.ItemPrices.Where(x => x.ID == a).First();
                            items.Add(details);
                        }
                        loadData(items, dataGridView1);
                    }
                    else
                    {
                        dataGridView1.Rows.Clear();
                        MessageBox.Show("Room(s) are not available on selected dates");
                    }
                }
                else
                {
                    dataGridView1.Rows.Clear();
                    MessageBox.Show("Me No rooms are available");
                }


            }

            if (string.IsNullOrEmpty(searchTxtBox.Text) == true)
            {

                var allItems = ent.ItemPrices;
                if (allItems.Any())
                {
                    var availables = allItems.Select(x => x.ID).Except(ent.BookingDetails.Select(y => y.ItemPriceID));
                    if (availables.Any())
                    {
                        List<ItemPrice> items = new List<ItemPrice>();
                        items.Clear();
                        foreach (var a in availables)
                        {
                            var details = ent.ItemPrices.Where(x => x.ID == a).First();
                            items.Add(details);
                        }
                        loadData(items, dataGridView1);
                    }
                    else
                    {
                        dataGridView1.Rows.Clear();
                        MessageBox.Show("Room(s) are not available on selected dates");
                    }
                }
                else
                {
                    dataGridView1.Rows.Clear();
                    MessageBox.Show("No rooms are available");
                }
            }
            
        }

        private void Form1_Activated(object sender, EventArgs e)
        {



        }

        private void areaCmbBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            attractionCmbBox.Items.Clear();
            attractionCmbBox.Items.Insert(0, "");
            attractionCmbBox.SelectedIndex = 0;
            string areaName = areaCmbBox.SelectedItem.ToString();
            var attractions = ent.Attractions.Where(x => x.Area.Name == areaName);
            foreach (var a in attractions)
            {
                attractionCmbBox.Items.Add(a.Name);
            }
            pTitleCmbBox.Items.Clear();
            pTitleCmbBox.Items.Insert(0, "");
            pTitleCmbBox.SelectedIndex = 0;
            var pTitle = ent.Items.Where(x => x.Area.Name == areaName);
            foreach (var p in pTitle)
            {
                pTitleCmbBox.Items.Add(p.Title);
            }

        }

        public void amenityData(int index)
        {
            ComboBox[] cmbs = new ComboBox[3];
            cmbs[0] = amenity1;
            cmbs[1] = amenity2;
            cmbs[2] = amenity3;
            var amenities = ent.Amenities.ToList();
            for (int i = 0; i < cmbs.Count(); i++)
            {
                bool selectedisNull = string.IsNullOrEmpty(cmbs[i].Text);
                if (selectedisNull == false)
                {
                    var value = cmbs[i].Text;
                    var amenity = ent.Amenities.Where(x => x.Name == value).FirstOrDefault();
                    if (amenities.Contains(amenity))
                    {
                        amenities.Remove(amenity);
                    }
                }
            }
            for (int i = 0; i < cmbs.Count(); i++)
            {
                if (i != index)
                {
                    bool selectedisNull = string.IsNullOrEmpty(cmbs[i].Text);
                    if (selectedisNull == false)
                    {
                        var value = cmbs[index].Text;
                        var amenity = ent.Amenities.Where(x => x.Name == value).FirstOrDefault();
                        var amenitiesOLD = ent.Amenities.ToList();
                        if (amenitiesOLD.Contains(amenity))
                        {
                            amenitiesOLD.Remove(amenity);
                        }
                        cmbs[i].Items.Clear();
                        foreach (var a in amenities)
                        {
                            cmbs[i].Items.Add(a.Name);
                        }
                    }
                    else
                    {
                        cmbs[i].Items.Clear();
                        foreach (var a in amenities)
                        {
                            cmbs[i].Items.Add(a.Name);
                        }
                    }
                }
            }

        }
        private void amenity1_SelectedIndexChanged(object sender, EventArgs e)
        {
            amenityData(0);
        }

        private void amenity2_SelectedIndexChanged(object sender, EventArgs e)
        {
            amenityData(1);
        }

        private void amenity3_SelectedIndexChanged(object sender, EventArgs e)
        {
            amenityData(2);
        }

        private void search2Btn_Click(object sender, EventArgs e)
        {
            string areaName = areaCmbBox.Text;
            MessageBox.Show(areaName);
            string attractionName = attractionCmbBox.Text;
            string propertyTitle = pTitleCmbBox.Text;
            string propertyType = pTypeCmbBox.Text;
            string firstAmenity = amenity1.Text;
            string secondAmenity = amenity2.Text;
            string thirdAmenity = amenity3.Text;
            decimal SpriceRange = numericUpDown1.Value;
            decimal EpriceRange = numericUpDown2.Value;
            int capacity = (int)people2.Value;
            var items = ent.ItemPrices.ToList();
            if (string.IsNullOrEmpty(areaName) == false)
            {
                items = items.Where(x => x.Item.Area.Name == areaName).ToList();
            }
            if (string.IsNullOrEmpty(attractionName) == false)
            {
                items = items.Where(x => x.Item.ItemAttractions.Any(y => y.Attraction.Name == attractionName)).ToList();
            }
            if (string.IsNullOrEmpty(propertyTitle) == false)
            {
                items = items.Where(x => x.Item.Title == propertyTitle).ToList();
            }
            if (string.IsNullOrEmpty(propertyType) == false)
            {
                items = items.Where(x => x.Item.ItemType.Name == propertyType).ToList();
            }
            if (string.IsNullOrEmpty(firstAmenity) == false)
            {
                items = items.Where(x => x.Item.ItemAmenities.Any(y => y.Amenity.Name == firstAmenity)).ToList();
            }
            if (string.IsNullOrEmpty(secondAmenity) == false)
            {
                items = items.Where(x => x.Item.ItemAmenities.Any(y => y.Amenity.Name == secondAmenity)).ToList();
            }
            if (string.IsNullOrEmpty(thirdAmenity) == false)
            {
                items = items.Where(x => x.Item.ItemAmenities.Any(y => y.Amenity.Name == thirdAmenity)).ToList();
            }
            if (string.IsNullOrEmpty(capacity.ToString()) == false)
            {
                items = items.Where(x => x.Item.Capacity >= capacity).ToList();
            }
            if (EpriceRange!=0)
            {
                items = items.Where(x => x.Price>=SpriceRange && x.Price <= EpriceRange).ToList();
            }
            dataGridView2.Rows.Clear();
            if (items.Any())
            {
                var availables = items.Select(x => x.ID).Except(ent.BookingDetails.Select(y => y.ItemPriceID));
                if (availables.Any())
                {
                    List<ItemPrice> itemsList = new List<ItemPrice>();
                    itemsList.Clear();
                    foreach (var a in availables)
                    {
                        var details = ent.ItemPrices.Where(x => x.ID == a).First();
                        itemsList.Add(details);
                    }
                    loadData(itemsList, dataGridView2);
                }
                else
                {
                    dataGridView2.Rows.Clear();
                    MessageBox.Show("Room(s) are not available on selected dates");
                }

            }
            else
            {
                MessageBox.Show("No rooms are available");
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown2.Minimum = numericUpDown1.Value;
            
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown1.Maximum = numericUpDown2.Value;
            
        }
    }
}
    
