using System.Diagnostics;
namespace Crypto_Currency_Simulator
{
    public partial class CryptoCurrencySimulator : Form
    {
        private double[] price = new double[4];
        private double[] h1 = new double[4];
        private double[] h24 = new double[4];
        private double balance;
        private double[] bought = new double[4];
        private double[] last1h = new double[4];
        private double[] last24h = new double[4];
        private Task[] tasks;
        private Random rand;

        private delegate void UpdateBalanceDelegate();
        private delegate void UpdatePricesDelegate(int index);
        private delegate void UpdatePercentagesDelegate(int index);
        private delegate void UpdateInvestmentsDelegate(int index);

        private CancellationTokenSource cancellationTokenSource;

        //Simulation speed in milliseconds (time that a thread should sleep before refreshing values)
        private int simSpeed;
        private int normalSpeed = 15000;
        private int fastSpeed = 100;

        public CryptoCurrencySimulator()
        {
            //initialize all prices and indices
            for(short i = 0; i < 4; i++)
            {
                price[i] = 50.0f;
                h1[i] = 0.0f;
                h24[i] = 0.0f;
                last1h[i] = 50.0f;
                last24h[i] = 50.0f;
                balance = 100f;
                bought[i] = 0.0f;
            }

            //initialize components
            InitializeComponent();

            cancellationTokenSource= new CancellationTokenSource();
            rand = new Random();
            simSpeed = normalSpeed;

            //initialize labels
            lbl_balance.Text = "Your balance: $" + balance.ToString();
            lbl_price_1.Text = "$" + price[0].ToString();
            lbl_price_2.Text = "$" + price[1].ToString();
            lbl_price_3.Text = "$" + price[2].ToString();
            lbl_price_4.Text = "$" + price[3].ToString();
        }


        private async Task adjust_Speed()
        {
            while (true)
            {
                if (btn_time.Checked)
                    simSpeed = fastSpeed;
                else
                    simSpeed = normalSpeed;

                // Restart tasks with the new simulation speed
                restartTasks();

                await Task.Delay(10);
            }
        }

        private void refresh_Investments(int index)
        {
            if(textBox1.InvokeRequired)
            {
                // Use Invoke to marshal the call to the UI thread
                textBox1.Invoke(new UpdateInvestmentsDelegate(refresh_Investments));
            }
            else
            {
                // Update the label directly on the UI thread
                switch (index)
                {
                    case 0:
                        {
                            if (textBox1.Text != "" && button1.Text == "Buy Out")
                            {
                                textBox1.Text = Math.Round(bought[0] * price[0], 2).ToString();
                            }
                            break;
                        }
                    case 1:
                        {
                            if (textBox2.Text != "" && button2.Text == "Buy Out")
                            {
                                textBox2.Text = Math.Round(bought[1] * price[1], 2).ToString();
                            }
                            break;
                        }
                    case 2:
                        {
                            if (textBox3.Text != "" && button3.Text == "Buy Out")
                            {
                                textBox3.Text = Math.Round(bought[2] * price[2], 2).ToString();
                            }
                            break;
                        }
                    case 3:
                        {
                            if (textBox4.Text != "" && button4.Text == "Buy Out")
                            {
                                textBox4.Text = Math.Round(bought[3] * price[3], 2).ToString();
                            }
                            break;
                        }
                }
                
            }
        }

        private void refresh_Balance()
        {
            // Check if InvokeRequired is true, meaning the call is coming from a different thread
            if (lbl_balance.InvokeRequired)
            {
                // Use Invoke to marshal the call to the UI thread
                lbl_balance.Invoke(new UpdateBalanceDelegate(refresh_Balance));
            }
            else
            {
                // Update the label directly on the UI thread
                lbl_balance.Text = "Your balance: " + Math.Round(balance, 2).ToString();
            }
        }

        private void refresh_Prices(int index)
        {
            // Check if InvokeRequired is true, meaning the call is coming from a different thread
            if (lbl_price_1.InvokeRequired)
            {
                // Use Invoke to marshal the call to the UI thread
                lbl_price_1.Invoke(new UpdatePricesDelegate(refresh_Prices));
            }
            else
            {
                // Update the label directly on the UI thread
                switch (index)
                {
                    case 0:
                        {
                            lbl_price_1.Text = "$" + Math.Round(price[0], 4).ToString();
                            break;
                        }
                    case 1:
                        {
                            lbl_price_2.Text = "$" + Math.Round(price[1], 4).ToString();
                            break;
                        }
                    case 2:
                        {
                            lbl_price_3.Text = "$" + Math.Round(price[2], 4).ToString();
                            break;
                        }
                    case 3:
                        {
                            lbl_price_4.Text = "$" + Math.Round(price[3], 4).ToString();
                            break;
                        }
                }
                
            }
            refresh_Investments(index);
        }

        private void refresh_Percentages(int index)
        {
            // Check if InvokeRequired is true, meaning the call is coming from a different thread
            if (lbl_1h_1.InvokeRequired)
            {
                // Use Invoke to marshal the call to the UI thread
                lbl_1h_1.Invoke(new UpdatePercentagesDelegate(refresh_Percentages));
            }
            else
            {
                double[] values = new double[8];
                values[index * 2] = calculate_Percentage1h(index);
                values[index * 2 + 1] = calculate_Percentage24h(index);

                // Update the label directly on the UI thread
                switch (index)
                {
                    case 0:
                        {
                            lbl_1h_1.Text = values[0].ToString();
                            lbl_24h_1.Text = values[1].ToString();
                            if (values[0] < 0)
                            {
                                lbl_1h_1.ForeColor = Color.Red;
                            }
                            else
                            {
                                lbl_1h_1.ForeColor = Color.Green;
                            }
                            if (values[1] < 0)
                            {
                                lbl_24h_1.ForeColor = Color.Red;
                            }
                            else
                            {
                                lbl_24h_1.ForeColor = Color.Green;
                            }
                            break;
                        }
                    case 1:
                        {
                            lbl_1h_2.Text = values[2].ToString();
                            lbl_24h_2.Text = values[3].ToString();
                            if (values[2] < 0)
                            {
                                lbl_1h_2.ForeColor = Color.Red;
                            }
                            else
                            {
                                lbl_1h_2.ForeColor = Color.Green;
                            }
                            if (values[3] < 0)
                            {
                                lbl_24h_2.ForeColor = Color.Red;
                            }
                            else
                            {
                                lbl_24h_2.ForeColor = Color.Green;
                            }
                            break;
                        }
                    case 2:
                        {
                            lbl_1h_3.Text = values[4].ToString();
                            lbl_24h_3.Text = values[5].ToString();
                            if (values[4] < 0)
                            {
                                lbl_1h_3.ForeColor = Color.Red;
                            }
                            else
                            {
                                lbl_1h_3.ForeColor = Color.Green;
                            }
                            if (values[5] < 0)
                            {
                                lbl_24h_3.ForeColor = Color.Red;
                            }
                            else
                            {
                                lbl_24h_3.ForeColor = Color.Green;
                            }
                            break;
                        }
                    case 3:
                        {
                            lbl_1h_4.Text = values[6].ToString();
                            lbl_24h_4.Text = values[7].ToString();
                            if (values[6] < 0)
                            {
                                lbl_1h_4.ForeColor = Color.Red;
                            }
                            else
                            {
                                lbl_1h_4.ForeColor = Color.Green;
                            }

                            if (values[7] < 0)
                            {
                                lbl_24h_4.ForeColor = Color.Red;
                            }
                            else
                            {
                                lbl_24h_4.ForeColor = Color.Green;
                            }
                            break;
                        }
                }
            }
        }

        private double calculate_Percentage1h(int index)
        {
            double num;
            num = Math.Round((price[index] - last1h[index]) / last1h[index] * 100, 2);
            return num;
        }

        private double calculate_Percentage24h(int index)
        {
            double num;
            num = Math.Round((price[index] - last24h[index]) / last24h[index] * 100, 2);
            return num;
        }

        private void filter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '.')
            {
                // If not a digit, control key, or decimal point, suppress the key press
                e.Handled = true;
            }

            // Allow only one decimal point
            if (e.KeyChar == '.' && ((TextBox)sender).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void configure_ClickInvest(object sender, EventArgs e, TextBox txt, int index)
        {

            if (((Button)sender).Text == "Invest")
            {
                if (txt.Text != "" && Double.Parse(txt.Text) <= balance)
                {
                    ((Button)sender).Text = "Buy Out";
                    balance -= Double.Parse(txt.Text);
                    Invoke(new Action(() => refresh_Balance()));
                    bought[index] = Double.Parse(txt.Text) / price[index];
                }
                else
                {
                    MessageBox.Show("Insufficient funds!");
                }
            }
            else
            {
                ((Button)sender).Text = "Invest";
                balance += Double.Parse(txt.Text);
                txt.Text = "";
                Invoke(new Action(() => refresh_Balance()));
                bought[index] = 0;
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != "")
            {
                configure_ClickInvest(sender, e, textBox1, 0);
            }
            else
            {
                Random rand = new Random();
                textBox1.Text = (rand.NextDouble() * 5).ToString();
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                configure_ClickInvest(sender, e, textBox2, 1);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox3.Text != "")
            {
                configure_ClickInvest(sender, e, textBox3, 2);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox4.Text != "")
            {
                configure_ClickInvest(sender, e, textBox4, 3);
            }
        }

        private void Simulate(CancellationToken token, ref int speed, int index)
        {
            int minuteInHour = 0;
            int hoursInDay = 0;
            double generatedValue;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if(minuteInHour + 1 == 60)
                    {
                        last1h[index] = price[index];
                        minuteInHour = 0;
                        if(hoursInDay + 1 == 12)
                        {
                            last24h[index] = price[index];
                            hoursInDay = 0;
                        }
                        else
                        {
                            hoursInDay++;
                        }
                        
                    }
                    else
                    {
                        minuteInHour++;
                    }
                    
                    Debug.WriteLine(Thread.CurrentThread.Name);
                    generatedValue = rand.NextDouble() * 5 - 2.5;
                    if (price[index] - generatedValue < 0)
                    {
                        price[index] /= generatedValue;
                    }
                    else {
                        price[index] += generatedValue;
                    }
                    Thread.Sleep(speed);

                    // Update UI on the main thread
                    Invoke(new Action(() =>
                    {
                        refresh_Prices(index);
                        refresh_Percentages(index);
                    }));
                    
                }
            }

            catch (OperationCanceledException)
            {
                Console.WriteLine("Thread canceled");
            }

        }

        private void CryptoCurrencySimulator_Load(object sender, EventArgs e)
        {
            startTasks();
        }

        private void btn_time_CheckedChanged(object sender, EventArgs e)
        {
            if (btn_time.Checked)
            {
                simSpeed = fastSpeed;
            }
            else
            {
                simSpeed = normalSpeed;
            }
        }

        private void startTasks()
        {
            cancellationTokenSource = new CancellationTokenSource();

            tasks = new Task[4];
            for (int i = 0; i < tasks.Length; i++)
            {
                int index = i;
                tasks[i] = Task.Run(() => Simulate(cancellationTokenSource.Token, ref simSpeed, index));
            }
        }

        private void stopTasks()
        {
            cancellationTokenSource?.Cancel();
            Task.WaitAll(tasks);
            cancellationTokenSource?.Dispose();
        }

        private void restartTasks()
        {
            stopTasks();
            startTasks();
        }

        private void CryptoCurrencySimulator_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }
    }
}
/*TODO:
 * 1) Fix last 1h and 24h values
 * 2) Try and fix the restarting threads
*/