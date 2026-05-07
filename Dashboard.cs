using System;

using System.Data;

using System.Drawing;

using System.Windows.Forms;

namespace JatayuAirlines

{

public partial class DashboardForm : Form

{

    public DashboardForm()

    {

        InitializeComponent();

        ApplyRoleBasedAccess();

        LoadDashboard();

    }



    /// <summary>

    /// Show/hide nav buttons depending on whether user is Admin or User.

    /// Admin: all buttons visible.

    /// User: only Search Flights and View My Bookings visible.

    /// </summary>

    private void ApplyRoleBasedAccess()

    {

        lblSubTitle.Text = "Welcome, " + SessionManager.FullName +

                           "  |  Role: " + SessionManager.Role;



        if (!SessionManager.IsAdmin)

        {

            // Hide admin-only nav buttons

            btnFlights.Visible = false;

            btnPassengers.Visible = false;

            btnUsers.Visible = false;



            // Reposition user buttons

            btnSearch.Location = new System.Drawing.Point(10, 55);

            btnBookings.Location = new System.Drawing.Point(10, 105);

            btnRefresh.Location = new System.Drawing.Point(10, 165);



            // Rename stat card labels to be user-relevant

            lblFlightsLbl.Text = "My Flights";

            lblPassengersLbl.Text = "My Profile";

            lblBookingsLbl.Text = "My Bookings";

            lblRevenueLbl.Text = "My Spending (INR)";



            lblRoleBadge.Text = "USER";

            lblRoleBadge.BackColor = System.Drawing.Color.FromArgb(46, 160, 80);

        }

        else

        {

            lblRoleBadge.Text = "ADMIN";

            lblRoleBadge.BackColor = System.Drawing.Color.FromArgb(180, 30, 80);

        }

    }



    private void LoadDashboard()

    {

        try

        {

            if (SessionManager.IsAdmin)

            {

                // Admin sees everything

                DataTable stats = DatabaseHelper.GetDashboardStats();

                if (stats.Rows.Count > 0)

                {

                    DataRow row = stats.Rows[0];

                    lblFlightsVal.Text = row["TotalFlights"].ToString();

                    lblPassengersVal.Text = row["TotalPassengers"].ToString();

                    lblBookingsVal.Text = row["ActiveBookings"].ToString();

                    double rev = Convert.ToDouble(row["TotalRevenue"]);

                    lblRevenueVal.Text = rev.ToString("N0");

                }

                dgvRecent.DataSource = DatabaseHelper.GetAllBookings();

            }

            else

            {

                // User sees only their own data

                DataTable myBookings = DatabaseHelper.GetBookingsByPassengerName(

                    SessionManager.FullName);



                // Count only confirmed bookings

                int activeCount = 0;

                double myRevenue = 0;

                foreach (DataRow r in myBookings.Rows)

                {

                    if (r["Status"].ToString() == "Confirmed")

                    {

                        activeCount++;

                        myRevenue += Convert.ToDouble(r["TotalFare"]);

                    }

                }



                lblFlightsVal.Text = myBookings.Rows.Count.ToString();

                lblPassengersVal.Text = "1";          // only themselves

                lblBookingsVal.Text = activeCount.ToString();

                lblRevenueVal.Text = myRevenue.ToString("N0");



                dgvRecent.DataSource = myBookings;

            }

        }

        catch (Exception ex)

        {

            MessageBox.Show("Error loading dashboard:\n" + ex.Message,

                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

    }



    private void OpenForm(Form form)

    {

        form.FormClosed += (s, e) => LoadDashboard();

        form.ShowDialog(this);

    }



    private void btnFlights_Click(object sender, EventArgs e)

    {

        OpenForm(new FlightsForm());

    }



    private void btnPassengers_Click(object sender, EventArgs e)

    {

        OpenForm(new PassengersForm());

    }



    private void btnBookings_Click(object sender, EventArgs e)

    {

        OpenForm(new BookingsForm());

    }



    private void btnSearch_Click(object sender, EventArgs e)

    {

        OpenForm(new SearchFlightsForm());

    }



    private void btnUsers_Click(object sender, EventArgs e)

    {

        OpenForm(new ManageUsersForm());

    }



    private void btnRefresh_Click(object sender, EventArgs e)

    {

        LoadDashboard();

    }



    private void btnLogout_Click(object sender, EventArgs e)

    {

        if (MessageBox.Show("Are you sure you want to logout?", "Logout",

            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)

        {

            SessionManager.Clear();

            // Show login again

            LoginForm login = new LoginForm();

            if (login.ShowDialog() == DialogResult.OK)

            {

                ApplyRoleBasedAccess();

                LoadDashboard();

            }

            else

            {

                Application.Exit();

            }

        }

    }

}

}       DashboardForm.cs  generate image
