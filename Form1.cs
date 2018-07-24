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

namespace wiki
{
    public partial class Form1 : Form
    {
    	string obrazekPomoc;
    	double zmPomocnicza, cenaPomocnicza, sumaZkupow;
    	double bank;
    	int przerwa, przerwaLimit;
    	
        public Form1()
        {
            InitializeComponent();
        }
		//dodac przez opcje add w programie
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Form2 autorzy = new Form2();
            autorzy.Show();
            
        }
		// po załadowaniu okna 
        void Form1Load(object sender, EventArgs e)
        {
            przerwa = 0;
            btOdczytaj.Enabled = true;
        	btSprzedaj.Enabled = true;
        	
        	try
			{
				FileStream fs2 = new FileStream("config.txt", FileMode.Open, FileAccess.Read);
						
				StreamReader sr2 = new StreamReader(fs2);

				bank = Convert.ToDouble(sr2.ReadLine());
				przerwaLimit = Convert.ToInt32(sr2.ReadLine());
				fs2.Close();
			}
			catch(Exception ex)
			{
				etUwagi.Text = " BŁĄD ODCZYTU BANKU Z PLIKU! ZGŁOŚ WUJKOWI MICHAŁOWI :-)";
				btSprzedaj.Enabled = btNowyKlient.Enabled = btOdczytaj.Enabled = false;
                return;
			}
        	
        	if(bank > 1000) {pictureNagroda1.Visible = true; groupNagroda1.Text = "   SPRZEDAWCZYNI   ";}
        	if(bank > 2500) {pictureNagroda2.Visible = true; groupNagroda2.Text = "   DOBRA SPRZEDAWCZYNI   ";}
        	if(bank > 4500) {pictureNagroda3.Visible = true; groupNagroda3.Text = "   SUPER EXTRA I NAJLEPSZA SPRZEDAWCZYNI W FILIPOWICACH :)   ";}
        }
  
        
////////////////////////////////////////////////////////////////////////////////////////////////////////////        
//dodawanie produktów. 
       
        void BtDodajClick(object sender, EventArgs e)
        {
        	
        	if(dataKodDodaj.Text == "")
			{
				etUwagiDodaj.Text = "PUSTE OKIENKA!";
			}

            try
            {
                zmPomocnicza = Convert.ToDouble(dataCenaDodaj.Text);
            }
            catch
            {
                etUwagiDodaj.Text = "ZŁY FORMAT CENY! (PAMIETAJ ŻE MA BYĆ PRZECINEK A NIE KROPKA :)";
                return;
            }

			if(File.Exists(dataKodDodaj.Text + ".txt"))
			{
				etUwagiDodaj.Text = "TAKI PRODUKT JEST JUŻ W SKLEPIE!";
				dataKodDodaj.Text = "";
			}
			
			else if(dataKodDodaj.Text!="" && dataNazwaDodaj.Text!="" && dataCenaDodaj.Text!="")
			{
				StreamWriter sw = File.CreateText(dataKodDodaj.Text + ".txt");
				sw.WriteLine(dataNazwaDodaj.Text);
				sw.WriteLine(dataCenaDodaj.Text);
				sw.WriteLine(dataObrazDodaj.Text + ".jpg");
				sw.Close();
				
				dataKodDodaj.Text = dataNazwaDodaj.Text = dataCenaDodaj.Text = dataObrazDodaj.Text = etUwagiDodaj.Text = "";	
			}
        }
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//odczytywanie kodów.

        void BtOdczytajClick(object sender, EventArgs e)
        {
        	grupObraz.Visible = true;
        	picBarbie.Visible = etZarobekBarbie.Visible = false;
        	btSprzedaj.Enabled = true; //uaktywnia guzik dodaj wyłączony po jego kliknieciu.
        	if(dataKod.Text == "")
			{
				etUwagi.Text = "PUSTE POLE KODU";
				btSprzedaj.Enabled = false;
				dataProdukt.Text = dataCena.Text = dataSztuki.Text = "";
				
			}
			else
			{
				if(File.Exists(dataKod.Text + ".txt"))
				{
					try
					{
						FileStream fs = new FileStream(dataKod.Text + ".txt", FileMode.Open, FileAccess.Read);
						
						StreamReader sr = new StreamReader(fs);
						
						while(!sr.EndOfStream)
						{
							dataProdukt.Text = sr.ReadLine();
							cenaPomocnicza = Convert.ToDouble(sr.ReadLine());
							obrazekPomoc = sr.ReadLine();
							dataSztuki.Text = "1";
						}
						
						dataCena.Text = cenaPomocnicza + " zł.";
						
						if(File.Exists(obrazekPomoc))
						{
							obrazek.Image = new System.Drawing.Bitmap(obrazekPomoc);
						}
						else obrazek.Image = new System.Drawing.Bitmap("empty.jpg");
						sr.Close();
					
					}
					catch(Exception ex)
					{
						etUwagi.Text = ex.Message + " BŁĄD ZAPISU! ZGŁOŚ PANU MICHAŁOWI :-)";
						dataProdukt.Text = dataCena.Text = dataKod.Text = "";
						btSprzedaj.Enabled = false;
					}
					dataKod.Text = etUwagi.Text = "";
				}
				else
				{	
					etUwagi.Text = "NIE MA TAKIEGO TOWARU!";
					dataProdukt.Text = dataCena.Text = dataKod.Text = dataSztuki.Text = "";
					obrazek.Image = new System.Drawing.Bitmap("empty.jpg");
					btSprzedaj.Enabled = false;
				}
			}
        }
        
        void BtSprzedajClick(object sender, EventArgs e)
        {
        	if(dataCena.Text == "")
            {
                etUwagi.Text = "PUSTE POLE KODU!";
                dataProdukt.Text = dataCena.Text = dataKod.Text = dataSztuki.Text = "";
                obrazek.Image = new System.Drawing.Bitmap("empty.jpg");
                return;
                
            }
            
            if(przerwa == przerwaLimit)
        	{
        		etUwagi.Text = "CZAS NA 10 MINUTEK PRZERWY! :)";
        		btOdczytaj.Enabled = false;
        		btSprzedaj.Enabled = false;
        		dataProdukt.Text = dataCena.Text = dataKod.Text = dataSztuki.Text = "";
				obrazek.Image = new System.Drawing.Bitmap("empty.jpg");
        		return;
        			
        	}
        	
        	przerwa++;
        	if(Convert.ToInt32(dataSztuki.Text) < 4)
        	{
        		sumaZkupow += Convert.ToDouble(cenaPomocnicza * (Convert.ToDouble(dataSztuki.Text)));
				dataZaplata.Text = Convert.ToString(sumaZkupow) + " zł.";
				
//w celu niemożności dodania kolejnych sztuk tego produktu po zatwierdzeniu sprzedarzy.
// mozna znow to robić po ponownym zeskanowaniu kodu.
				btSprzedaj.Enabled = false;
				dataProdukt.Text = dataCena.Text = dataKod.Text = dataSztuki.Text = etUwagi.Text = "";
				obrazek.Image = new System.Drawing.Bitmap("empty.jpg");
        	}
        	
        	else
        	{
        		etUwagi.Text = "ZA DUŻO SZTUK CHCESZ SPRZEDAĆ! (MAX. 3)";
        		dataSztuki.Clear();
        	}
        }
        
        void BtNowyKlientClick(object sender, EventArgs e)
        {
        	
        	bank += sumaZkupow;
        	sumaZkupow = 0;

        	etZarobekBarbie.Text = dataZaplata.Text; // barbie mówi ile zarobił klient.

	        dataProdukt.Text = dataCena.Text = dataKod.Text = dataSztuki.Text = dataZaplata.Text = etUwagi.Text = "";
			obrazek.Image = new System.Drawing.Bitmap("empty.jpg");
			grupObraz.Visible = false;
			btSprzedaj.Enabled = false;
			picBarbie.Visible = etZarobekBarbie.Visible = true;	
        }
        
        void BtPokazBankClick(object sender, EventArgs e)
        {
        	dataBank.Text = Convert.ToString(bank) + " zł.";
        	if(bank > 1000 && pictureNagroda1.Visible == false) {pictureNagroda1.Visible = true; groupNagroda1.Text = "   SPRZEDAWCZYNI   ";}
        	if(bank > 2500 && pictureNagroda2.Visible == false) {pictureNagroda2.Visible = true; groupNagroda2.Text = "   DOBRA SPRZEDAWCZYNI   ";}
        	if(bank > 4500 && pictureNagroda3.Visible == false) {pictureNagroda3.Visible = true; groupNagroda3.Text = "   SUPER EXTRA I NAJLEPSZA SPRZEDAWCZYNI W FILIPOWICACH :)   ";}
        }
        
        void TabZamykamyClick(object sender, EventArgs e)
        {
        	dataBank.Text = "";
        }
        
        void Button1Click(object sender, EventArgs e)
        { 
            FileStream fs3 = new FileStream("config.txt", FileMode.OpenOrCreate, FileAccess.Write);
        	
        	StreamWriter sw3 = new StreamWriter(fs3);
        	sw3.WriteLine(bank); // zapis do pliku aktualnego stanu banku.
            sw3.WriteLine("5");
        	sw3.Close();
        	
        	Application.Exit();
        }
}}

// wyłaczyć iknone programu i guziki minimalizacji max i close.