'Creator: Tachyon edited/rework by LNGCGENETIC 
'Date: 26 July 2018
'Release Date: 27 October 2015 rework[TBD]
'License: GNU GPL v2

Imports DotRas
Public Class Form1
    Dim RasCon As RasConnection

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If My.Settings.Username.Equals("") Then 'Checks to see if there is a username stored in the settings
        Else
            txtUsername.Text = My.Settings.Username 'If there is, fill the inforamtion into the textbox 
            chkRemember.Checked = True 'And check the checkbox to show that the remember function was enabled
        End If
    End Sub
    Private Sub dialer_StateChanged(sender As Object, e As StateChangedEventArgs) Handles dialer.StateChanged
        Dim li As ListViewItem 'Creates a new list view item to add items to the listview
        li = ListView1.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")) 'Adds the current date and time to the listview for the logs
        li.SubItems.Add(e.State.ToString) 'Adds the current state of the connection to the listview for log purposes
    End Sub

    Private Sub chkRemember_CheckedChanged(sender As Object, e As EventArgs) Handles chkRemember.CheckedChanged
        'This Sub basically checks if the checkbox for storing the users' username is checked, if it is store it, if not, dont
        If chkRemember.Checked Then
            My.Settings.Username = txtUsername.Text
        Else
            My.Settings.Username = ""
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ListView1.Items.Clear()
        If Locations.SelectedItem = "Los Angeles" Then
            txtIP.Text = "104.207.152.120"
        ElseIf Locations.SelectedItem = "Melbourne" Then
            txtIP.Text = " "
        ElseIf Locations.SelectedItem = "Sydney" Then
            txtIP.Text = " "
        End If 'Finished Up Our ComboBox Config'
        Dim creds As Net.NetworkCredential 'Creates the new NetworkCredential required to authenticate to the VPN
        Dim l2tpEntry As RasEntry 'Creates an empty object of type RasEntry to be modified later for L2TP use

        Try 'Start of Try Catch statement
            RasPhoneBook1.Open(True) 'Opens the users' default phonebook to access VPN connection information

            If txtUsername.Text = "" Then 'Makes sure that the user inputted a username
                MessageBox.Show("Please enter a username.", "No username", MessageBoxButtons.OK, MessageBoxIcon.Error) 'If there was no username entered display an error message
            Else 'If there is  username entered carry on
                If txtPassword.Text = "" Then 'Makes sure that the user inputted a password 
                    MessageBox.Show("Please enter a password.", "No password", MessageBoxButtons.OK, MessageBoxIcon.Error) 'If there was no password entered display an error
                Else 'If there is a username and password entered carry on
                    creds = New Net.NetworkCredential(txtUsername.Text, txtPassword.Text) 'Asigns the login information to the user credential object

                    For Each RasEntry In RasPhoneBook1.Entries.ToList 'Checks to see whether the VPN Connection already exists within the VPN Connection book
                        If RasEntry.Name = "PacketGG" Then 'If it finds a entry do the following:
                            RasEntry.Remove()
                            'Dial the exisitng VPN with the user credentials the user provided
                            'Exits the Sub as the connection has been started
                        End If 'End of the check of existing VPN Connections
                    Next 'Moves on to next RasEntry in the users' VPN Phonebook

                    l2tpEntry = RasEntry.CreateVpnEntry("PacketGG", txtIP.Text, DotRas.RasVpnStrategy.L2tpOnly, RasDevice.GetDeviceByName("(L2TP)", RasDeviceType.Vpn, False))
                    ' The above line creates a new VPN entry for the VPN connection as no VPN connections matching the criteria have been found
                    l2tpEntry.Options.UsePreSharedKey = True 'This tells the VPN Entry that this connection will require a Pre-Shared Key, obviously using the L2TP protocol

                    RasPhoneBook1.Entries.Add(l2tpEntry) 'Adds this new VPN Connection into the phonebook

                    l2tpEntry.UpdateCredentials(RasPreSharedKey.Client, "123") 'Updates the Pre-Shared key of the newly created VPN Connection

                    dialVPN(txtIP.Text, RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User), creds) 'Starts the VPN connection to the newly created entry
                End If 'End of the If statement to see which protocol is required
            End If 'End of the If statement to check the password field
        Catch ex As Exception 'Catches any Exceptions
            MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error) 'Creates a MessageBox with the error that has occured
        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim li As ListViewItem 'Creates a new listview item to add information to for use late on

        RasCon = RasConnection.GetActiveConnectionByName("PacketGG", RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User)) 'Gets the current VPN connection
        RasCon.HangUp() 'Ends the current VPN connection

        ListView1.Items.Clear() 'Clears the listbox of all it's information

        li = ListView1.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")) 'Gets the current date/time and adds it to the listbox item
        li.SubItems.Add("VPN Disconnected") 'Adds the information regarding what happened to the connection to the item and adds it to the listbox
    End Sub

    Public Sub dialVPN(ByVal entryName As String, ByVal bookPath As String, ByVal credentials As Net.NetworkCredential)
        'Method to actually connect the the VPN that has been created/called
        dialer.EntryName = "PacketGG" 'Sets the dialer to the name of the connection you are trying to use
        dialer.PhoneBookPath = bookPath 'Sets the book path of where the connection information is actually stored
        dialer.Credentials = credentials 'Sets the credentials required to login the the VPN service

        dialer.DialAsync() 'Finally, it dials the VPN and connects to it (hopefully)
    End Sub

    Private Sub radPPTP_CheckedChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub txtPSK_TextChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub txtIP_TextChanged(sender As Object, e As EventArgs) Handles txtIP.TextChanged

    End Sub

    Private Sub Locations_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Locations.SelectedIndexChanged

        If Locations.SelectedItem = "Los Angeles" Then
            Dim ping As New System.Net.NetworkInformation.Ping
            Dim ms = ping.Send("104.207.152.120").RoundtripTime()

            Label5.Text = (ms)
        ElseIf Locations.SelectedItem = "Melbourne" Then
            Dim ping As New System.Net.NetworkInformation.Ping
            Dim ms = ping.Send("45.121.209.16").RoundtripTime()

            Label5.Text = (ms)
        ElseIf Locations.SelectedItem = "Sydney" Then
            Dim ping As New System.Net.NetworkInformation.Ping
            Dim ms = ping.Send("45.121.210.27").RoundtripTime()

            Label5.Text = (ms)
        End If 'Finished Up Our PingTest'

    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

    End Sub

    Private Sub ToolStripStatusLabel1_Click(sender As Object, e As EventArgs) Handles ToolStripStatusLabel1.Click

    End Sub

    Private Sub ToolStripStatusLabel2_Click(sender As Object, e As EventArgs) 

    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub

    Private Sub GroupBox2_Enter(sender As Object, e As EventArgs) Handles GroupBox2.Enter

    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click

    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label5.Click

    End Sub
End Class
