'within vb there has been some codes saved which makes coding easier by eliminiating most commonly used words.
'this line does something so that you dont have to type 'oledb.' everytime you use oledb properties.
Imports System.Data.OleDb
Imports System.Console

Public Class MainMenu
    'this lines reserves a space in memory for a connection to the database and makes it exist. It also provides the path to the database.
    Dim cn As OleDbConnection = New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Customers.mdb;")
    'this reserves a space in memory for the command builder.
    'Dim cn As OleDbConnection = New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Customers.accdb;")
    Dim cmd As OleDbCommandBuilder
    'this reserves a space in memory for the data reader.
    Dim dr As OleDbDataReader
    'this reserves a space for the data table.
    Dim myTable As New DataTable
    'this revserves a space for the adapter.
    Dim myAdapter As OleDbDataAdapter
    'this declares a space in memory for customer input. its a way of varifying whatever the user searches in one textbox.
    Dim customersInput As Boolean
    'This declares space for 5 different numbers that will be generated when calculation happens.
    'paymentCalc is a number which appears when the calculation has happened.
    'TotalCal is a number which is paymentclac x number of months.
    'InterestRate is a number which takes the value from the txtInterest and puts it into this varible to be used in the calculation.
    'Time is the number of months, put into this varible by the textbox txtmonthlyPeriod to be used in the calculations
    'price is price of the car take away the deposit.
    Dim paymentCalc, TotalCal, InterestRate, Time, price As Double
    'w is jus decalred to reserve and use a space in memory for the number 16
    Dim w As Integer = 16
    'q is another number reserved in memory that will be used up later.
    Dim q As Integer = 534
    'this is a number reserved in memory for later use.
    Dim PageNumber As Integer = 1
    'the code in the next sub runs when a key is pressed on the keyboard
    Private Sub CustomerSearch_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtSearchCustomers.KeyPress
        'this if statement means if a number added is not a number then dont let it happen or dont enter the number.
        If (Not (IsNumeric(e.KeyChar))) Then
            e.Handled = True
        End If
        'this if statement makes sure that no punctuation marks have been editted.
        If Char.IsPunctuation(e.KeyChar) Then
            e.Handled = True
        End If
        'this bit of code makes sure that backspace, tab and other keys that have control work.
        If Char.IsControl(e.KeyChar) Then
            e.Handled = False
        End If
    End Sub
    'The code in this sub runs when text in the customer search box changes.
    Private Sub CustomerSearch_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtSearchCustomers.TextChanged
        'this if statement makes makes sure tht when 11 charcters have been added, 
        'customersInput becomes true. nd if the length is not 11, den it becomes false.
        If txtSearchCustomers.Text.Length = 11 Then
            customersInput = True
            txtSearchCustomers.BackColor = Color.LightGreen
        Else
            customersInput = False
            txtSearchCustomers.BackColor = Color.White
        End If
        'this if statement makes sure tht when customerInput boolean is true then the button 'cmdSearchCustomers' becomes enabled.
        If customersInput = True Then
            cmdSearchCustomers.Enabled = True
        Else
            cmdSearchCustomers.Enabled = False
        End If
    End Sub
    'the code in this sub runs when the form loads.
    Private Sub MainMenu_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'the next 2 lines of code makes the page number 1
        PageNumber = 1
        refreshInterface()
        sliderDeposit.Maximum = 99999999
        sliderDeposit.Minimum = 0
        sliderDeposit.TickFrequency = Val(txtPrice.Text) / 100
        cmdSaveCars.Enabled = False
    End Sub
    Private Sub CarSearch_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtSearchCars.KeyPress
        If Char.IsPunctuation(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub cmdSearchCustomers_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSearchCustomers.Click
        'this sub is called wehn search button is clicked.
        runSQLRetriveDataFromTheDatabase()
    End Sub
    'this sub runs all the things tht r below.
    Public Sub runSQLRetriveDataFromTheDatabase()
        'try catch code basically tries a piece of code and if the code throws an exception, it catches it,
        'and it executes watever is written below exception, normally its a message box appearing which carries
        'the execption, to explain the problem.
        Try
            'this opens the connection between the program and the database.
            cn.Open()
            'this line carries the message to the database via the adapter. This line also includes the sql statemnet. 
            'this sql statement selects everything from the row where mobile number is what is typded into the cuxtomer search textbox.
            myAdapter = New OleDbDataAdapter("Select * from Customer where MobileNumber ='" & txtSearchCustomers.Text & "'", cn)
            'the table is cleared, jus in case if there is data in it.
            myTable.Clear()
            'the culumns in the table r clear, just in case if data is stilll in there.
            myTable.Columns.Clear()
            'now table will fill with the nes inforation tht it has jus retrived from database.
            myAdapter.Fill(myTable)
            'this clears the customer search textbox.
            txtSearchCustomers.Clear()
            'this makes the second page appear.
            lblBoxID.Text = 2
            refreshInterface()
            'this makes the delete button enabled.
            cmdDeleteCustomer.Enabled = True
            'this makes the edit button enabled.
            cmdEditCustomers.Enabled = True
        Catch ex As OleDbException
            MsgBox("Please enter a mobile number. " + ex.Message)
        Catch ex As Exception
            MsgBox("Cannot retrive data from the database. " + ex.Message)
            lblBoxID.Text = 1
            refreshInterface()
        Finally
            'this closes the connection.
            cn.Close()
            'this calls a sub which filles the data into the textboxes.
            InsertDataIntoTextBoxes()
        End Try


        'this calls another sub which links cars to customers as cars and customers r in a different table.
        LinkCarToCustomer()
        cmdAddNewCustomer.Enabled = True
        UnablingTextboxes()
        cmdGotoLoan.Enabled = True
    End Sub

    Private Sub InsertDataIntoTextBoxes()
        Try
            'this is basically inserting values from mytable into the textboxes.
            txtFullName.Text = myTable.Rows(0)(0)
            txtMobileNumber.Text = myTable.Rows(0)(3)
            txtEmailAddress.Text = myTable.Rows(0)(4)
            txtPostCode.Text = myTable.Rows(0)(2)
            txtFirstName.Text = myTable.Rows(0)(1)
            txtNotes.Text = myTable.Rows(0)(5)
            lblCustomerNumber.Text = myTable.Rows(0)(6)
            txtPrice.Text = myTable.Rows(0)(7)
            txtDeposit.Text = myTable.Rows(0)(8)
            txtMonthlyPeriod.Text = myTable.Rows(0)(9)
            txtInterest.Text = myTable.Rows(0)(10)

        Catch ex As Exception
            'if the code throws an exception, message box will display 'Customer not Found.'
            MsgBox("Customer not Found or one or more details are missing from the database.")
            lblBoxID.Text = 1
            refreshInterface()
            If txtFullName.Text = "" Or txtMobileNumber.Text = "" Or txtEmailAddress.Text = "" Or txtPostCode.Text = "" _
                                       Or txtFirstName.Text = "" Or txtNotes.Text = "" Or lblLoadCarID.Text = 0 Then
                lblBoxID.Text = 1
                refreshInterface()
            Else
                lblBoxID.Text = 2
                refreshInterface()
            End If
            Exit Sub
        End Try

        Try
            lblLoadCarID.Text = myTable.Rows(0)(11)
        Catch ex As Exception
            MsgBox("Customer has not selected a car.")
        End Try

    End Sub

    Private Sub cmdEditCustomers_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdEditCustomers.Click
        'this calls another sub which enables the textboxes in order to get them ready to edit.
        EnableCustomersTextBoxes()
    End Sub

    Private Sub EnableCustomersTextBoxes()
        'all this codes enables textboxes.
        txtPostCode.Enabled = True
        txtMobileNumber.Enabled = True
        txtFullName.Enabled = True
        txtFirstName.Enabled = True
        txtNotes.Enabled = True
        txtEmailAddress.Enabled = True
        cmdSaveCustomerDetails.Enabled = True
    End Sub

    Private Sub cmdSaveCustomerDetails_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSaveCustomerDetails.Click

        'this if statement basically means if the new button is disabled then add a new customer.
        ' and if it is enabled then update the existing values.
        If cmdAddNewCustomer.Enabled = False Then
            Try
                'this is the string that send to database through myadapter to add a new customer.
                Dim sqlStringAddingCustomers As String = "Insert into Customer values('" & txtFullName.Text & "','" & txtMobileNumber.Text & "','" _
                                                         & txtEmailAddress.Text & "','" & txtFirstName.Text & "','" & txtPostCode.Text & "','" _
                                                         & txtNotes.Text & "'," & lblCustomerNumber.Text & "," & txtPrice.Text & "," & txtDeposit.Text & "," _
                                                         & txtMonthlyPeriod.Text & "," & txtInterest.Text & "," & lblCarID.Text & ")"
                'this opens the connection.
                cn.Open()
                'the adapter carries the the string to the database.
                myAdapter = New OleDbDataAdapter(sqlStringAddingCustomers, cn)
                'the command builder has the adapter.
                cmd = New OleDbCommandBuilder(myAdapter)
                'this clears the table.
                myTable.Clear()
                'this clears the columns.
                myTable.Columns.Clear()
                'this fills the table with the new data.
                myAdapter.Fill(myTable)
                'this catches any oldeb exception if the above code throws an oledb exception.
            Catch ex As OleDbException
                'this code is run if an oledb exception is thrown
                MsgBox("Cannot access the database. " + ex.Message)
                'this catches any other exception if thrown.
            Catch ex As Exception
                'this code is run if an exception is thrown.
                MsgBox("Cannot Add a new Customer. " + ex.Message)
                'when the above code has happened wheather an exception or just executed, this bit of code runs.
            Finally
                'this calls a sub which unables textboxes.
                UnablingTextboxes()
                'this closes the connection.
                cn.Close()
            End Try
            'if the new button is enabled, this next bit of code runs.
        Else
            Try
                'this string is decalred so that it can be used later, this way i dont have to write it so many times.
                Dim sqlString As String = "Update Customer set FullName = '" & txtFullName.Text & "', FirstLine = '" & txtMobileNumber.Text & _
                                                        "', PostCode = '" & txtEmailAddress.Text & "', MobileNumber = '" & txtFirstName.Text & "', EmailAddress = '" & _
                                                       txtPostCode.Text & "', Notes = '" & txtNotes.Text & "' where ID = " & lblCustomerNumber.Text
                'connection is opened.
                cn.Open()
                'my adapter contains the string.
                myAdapter = New OleDbDataAdapter(sqlString, cn)
                'command builder contians the the adapter.
                cmd = New OleDb.OleDbCommandBuilder(myAdapter)
                'this clears the table.
                myTable.Clear()
                'this clearnsthe columns.
                myTable.Columns.Clear()
                'this inserts new data into my table, if there is new data.
                'this is written because VB.NET likes it that way, otherwise it gives errors.
                myAdapter.Fill(myTable)
                'this message will display if no exception is thrown.
                MsgBox("Details have been saved.")
                'this catches any oledb exception
            Catch ex As OleDbException
                'when an oledb exception is thrown, this code is executed.
                MsgBox("Cannot access the Database." + ex.Message)
                'this catches any other exception.
            Catch ex As Exception
                'when an exception is thrown, this code is then run.
                MsgBox("Cannot save the information. " + ex.Message)
                'after an exception has been throw or not, this code is run.
            Finally
                'this calls another sub which unables textboxes.
                UnablingTextboxes()
                'this closes the connection.
                cn.Close()
            End Try
        End If
    End Sub
    'the code in this sub is run when cmdLoadsCustomerSearch is pressed.
    Private Sub cmdLoadsCustomerSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdLoadsCustomerSearch.Click
        'this makes the search box visible.
        BoxSearchCustomers.Visible = True
    End Sub

    'the code in this sub is run when sliderDeposit is scrolled.
    Private Sub sliderDeposit_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles sliderDeposit.Scroll
        sliderDeposit.Maximum = Val(txtPrice.Text)
        'this code changes the text of the deposit label as the price and the trackbar value changes. It also makes it in the format "0.00" and put a % at the end.
        lblDepositValue.Text = Format(sliderDeposit.Value / Val(txtPrice.Text) * 100, "0.00") & "%"
        'this bit of code makes sure tht the deposit text box has the value of the deposit track bar.
        txtDeposit.Text = Format(Val(sliderDeposit.Value), "0.00")
        'this makes sure tht the balance text box has the value of the price taken away from the deposit.
        lblBalance.Text = Format((Val(txtPrice.Text) - (Val(txtDeposit.Text))), "0.00")
    End Sub
    'the code in this sub runs when cmdAddNew Customer is Clicked.
    Private Sub cmdAddNewCustomer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddNewCustomer.Click
        'this prepares the group box for adding a new customer.
        NewFormCustomers()
    End Sub
    'this sub basically empytes the textboxes.
    Private Sub EmptyTextBoxes()
        txtMobileNumber.Clear()
        txtFullName.Clear()
        txtFirstName.Clear()
        txtNotes.Clear()
        txtEmailAddress.Clear()
        txtPostCode.Clear()
        txtSearchCustomers.Clear()
    End Sub
    'the code in this sub prepares for adding a new customer.
    Private Sub NewFormCustomers()
        'this is the sub called to empty the textboxes.
        EmptyTextBoxes()
        'this is the sub called to enable all the textboxes.
        EnableCustomersTextBoxes()
        'this disables the edit and delete button.
        cmdEditCustomers.Enabled = False
        cmdDeleteCustomer.Enabled = False
        'this disables the gotoloan button
        cmdGotoLoan.Enabled = False
        'this disables the cmdAddNewCustomer button.
        cmdAddNewCustomer.Enabled = False
        Try
            'connection opens.
            cn.Open()
            'the adapter carries the sql statement. ie. take everything from the customer table and put it in the table in the program.
            myAdapter = New OleDbDataAdapter("Select * from customer", cn)
            'clears the table
            myTable.Clear()
            'clears the columns.
            myTable.Columns.Clear()
            'fills the table with new data.
            myAdapter.Fill(myTable)
            'catches an oledb exception
        Catch ex As OleDbException
            'executes this code if oledb exception is thrown.
            MsgBox("Cannot connect to the the Database.  " + ex.Message)
            'catches any other exception.
        Catch ex As Exception
            MsgBox("Cannot retrive data from the database.  " + ex.Message)
            'runs the code when finished trying.
        Finally
            'closes the connection.
            cn.Close()
        End Try
        'space is reserved in memory for a number, name of the space is max.
        Dim max As Integer = 0
        'space is reserved in memory for a number, name of the space is x.
        Dim x As Integer
        'for next loop is used.
        'this loop runs from 1 upto the number of rows in my table. 
        For x = 1 To myTable.Rows.Count
            'this if statement is saying that max is 0 to begin with, if the number that we are looking at is > max then
            'max is that number, if not, then max stays as it is.
            If myTable.Rows(x - 1)(6) > max Then
                max = myTable.Rows(x - 1)(6)
            End If
        Next
        'this code is saying what max was add one to it.
        max = max + 1
        'this code is saying the value of max should display on the customer Number label.
        lblCustomerNumber.Text = max
    End Sub

    'this sub is run when cmdRegisterCustomer is clicked.
    Private Sub cmdRegisterCustomers_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdRegisterCustomers.Click
        'this takes to the 2nd page.
        lblBoxID.Text = 2
        refreshInterface()
        'this prepares the group box to add a new customer.
        NewFormCustomers()
    End Sub

    'this sub runs when about button is clicked.
    Private Sub cmdAbout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAbout.Click
        'the about form is a seperate form.
        'this code makes the about form visible.
        About.Show()
    End Sub
    'the sub is run when cmdDeleteCustomer is clicked.
    Private Sub cmdDeleteCustomer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDeleteCustomer.Click
        'his makes the delete window appear.
        boxDelete.Visible = True
        'this tells it what position it should be in.
        'this tells its property from left.
        boxDelete.Left = 63
        'this tells its top property.
        boxDelete.Top = 52
        'this tells the label of the delete group box to be: Are you sure you want to delete this Customer?
        lblWarning.Text = "Are you sure you want to delete this Customer?"
        'the delete window is brought to front, so that it is on the top.
        boxDelete.BringToFront()
        'the back and next buttons are disabled
        cmdTakesTONextPage.Enabled = False
        cmdTakesToPreviousPage.Enabled = False
    End Sub
    'this sub is called from other places' its job is to disable the prefered textboxes.
    Private Sub UnablingTextboxes()
        txtMobileNumber.Enabled = False
        txtFullName.Enabled = False
        txtEmailAddress.Enabled = False
        txtFirstName.Enabled = False
        txtPostCode.Enabled = False
        txtNotes.Enabled = False
        cmdSaveCustomerDetails.Enabled = False
    End Sub
    'this sub is run when cancel button is pressed.
    Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
        'this means make the delete window disappear.
        boxDelete.Visible = False
        'the back and next buttons r disabled.
        cmdTakesTONextPage.Enabled = True
        cmdTakesToPreviousPage.Enabled = True
    End Sub
    'this sub is run when yes button on the delete window is pressed.
    Private Sub cmdYes_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdYes.Click
        'because I'm using the same delete window for the customers and for the cars, I've used a way of determinning which one I'm deleting.
        'if I'm on second page, its going to be customer being deleting, if its not second page, it has to be the car.
        If lblBoxID.Text = 2 Then
            Try
                'connection opens
                cn.Open()
                'the adapter carried the sql string as always. This sql delete the whole row where customer number is the reference number.
                myAdapter = New OleDbDataAdapter("Delete from Customer where ID =" & lblCustomerNumber.Text, cn)
                'clears the table, if data was in it,
                myTable.Clear()
                'clears the columns.
                myTable.Columns.Clear()
                'fills the table with new data, if there is any.
                myAdapter.Fill(myTable)
                'if an oledb exception is thrown, the next bit of code will catch it,
            Catch ex As OleDbException
                'this code is run when oledb exception is thrown
                MsgBox("Cannot access the database. " + ex.Message)
                'if any other exception is thrown, this runs.
            Catch ex As Exception
                'this code executes if any exception is thrown.
                MsgBox("Cannot delete this Customer. " + ex.Message)
                'this code runs when the sub has finished trying the code.
            Finally
                'this is calling the sub which empties desired textboxes.
                EmptyTextBoxes()
                'closes the connection.
                cn.Close()
            End Try
            'if what we want to delete is not the customer, 'else' makes sure it will be the car.
        Else
            Try
                cn.Open()
                myAdapter = New OleDbDataAdapter("Delete from Car where ID =" & lblCarID.Text, cn)
                myTable.Clear()
                myTable.Columns.Clear()
                myAdapter.Fill(myTable)
            Catch ex As OleDbException
                MsgBox("Cannot access the database. " + ex.Message)
            Catch ex As Exception
                MsgBox("Cannot delete this Car. " + ex.Message)
            Finally
                'the next 6 lines simply clear the textboxes. to show that the car has been deleted. and it clears the list View.
                ClearCarTextBoxes()
                LsvCars.Items.Clear()
                'this calls for another sub that will load the cars again, this time, it will obviously not contain the deleted car.
                LoadCars()
                cn.Close()
            End Try
        End If
        'delete window disappears.
        boxDelete.Visible = False
        'the back and the next button r enabled.
        cmdTakesTONextPage.Enabled = True
        cmdTakesToPreviousPage.Enabled = True
    End Sub

    'this sub is called when the montlly period track bar is scrolled.
    Private Sub SliderMonthlyPeriod_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SliderMonthlyPeriod.Scroll
        'this says tht the monthly period text box must have the track bar value, whatever it maybe.
        txtMonthlyPeriod.Text = Val(SliderMonthlyPeriod.Value)
        'this is a sub called by the scroll monthly period.
        'this contains the most important bit of the program.
        Calculation()
    End Sub

    'this sub is called when interest track bar is scrolled.
    Private Sub sliderInterest_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles sliderInterest.Scroll
        'this says that interest text box must contain the value that the trackbar is on.
        txtInterest.Text = Val(sliderInterest.Value)
        'this sub is called by interest track bar as well.
        Calculation()
        'this is saying that the track bar must have the value which is in the interest text box
        sliderInterest.Value = Val(txtInterest.Text)
    End Sub

    'this sub does all the calculation of how much the customer needs to pay per month and their total payment,
    'total payment without deposit and with deposit, depending on the price, deposit, interest, monthly period.
    Private Sub Calculation()
        'this is basically declaring that time equals monthly period, price equals the value of price take away the value of deposit,
        'interestRate equals the value of interest 
        Time = Val(txtMonthlyPeriod.Text)
        price = (Val(txtPrice.Text) - (Val(txtDeposit.Text)))
        InterestRate = Val(txtInterest.Text)
        'if price and time do not equa zero then the calculation begins.
        If price <> 0 And Time <> 0 Then
            'this is saying that paymentCalc equals the calculation.
            paymentCalc = price * (1 + InterestRate / 1200) ^ Time / ((1 - (1 + InterestRate / 1200) ^ Time) / (1 - (1 + InterestRate / 1200)))
            'this rounds off the paymentCalc value.
            paymentCalc = Int(paymentCalc * 100 + 0.5) / 100
            'this says that TotalCal equals paymentCalc times by time.
            TotalCal = paymentCalc * Time
            'this is saying that monthly payment label contains the paymentCal value in the format "0.00"
            lblMonthlyPayment.Text = Format(paymentCalc, "0.00")
            'this line is saying that total payment label should read the totalCal and the value of deposit in the format "0.00"
            lblTotalPayment.Text = Format(TotalCal + Val(txtDeposit.Text), "0.00")
            'this line is saying that the total payment without the deposit should read the total Cal in the format "0.00"
            lblTotalPaymentNoDeposit.Text = Format((TotalCal), "0.00")
        End If
        'if the balance label is zero, that is to say: if the whole amount has been paid in deposit.
        If lblBalance.Text = 0 Then
            'then monthly payment label should read 0
            lblMonthlyPayment.Text = "0.00"
            'total payment label should read what the deposit is.
            lblTotalPayment.Text = txtDeposit.Text
            'total payment with no deposit should read 0
            lblTotalPaymentNoDeposit.Text = "0.00"
        End If
    End Sub


    'when text is changed in the balance label, this sub runs.
    Private Sub lblBalance_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lblBalance.TextChanged
        'calculation sub is called from here as well.
        Calculation()
    End Sub

    Private Sub txtPrice_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtPrice.KeyPress
        'this makes sure that controling keys r allowed.
        If Char.IsControl(e.KeyChar) Then
            e.Handled = False
        ElseIf (Not (IsNumeric(e.KeyChar))) Then
            e.Handled = True
        End If
    End Sub

    'when the vale of price textbox is changed, this sub is called.
    Private Sub txtPrice_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtPrice.TextChanged
        'this line translates to: the balance label equals the value of price textbox take away the value of deposit textbox in the format "0.00"
        lblBalance.Text = Format((Val(txtPrice.Text) - (Val(txtDeposit.Text))), "0.00")
    End Sub
    'this sub is called when Show Loan is clicked
    Private Sub cmdGotoLoan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdGotoLoan.Click
        'this is saying that show the loan group box.
        lblBoxID.Text = 4
        refreshInterface()
    End Sub

    'this sub is called when the search button on the second page is pressed, this is basically a second way of searching for customers.
    Private Sub cmdSearchesCustomerInsideCustomerDetails_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSearchesCustomerInsideCustomerDetails.Click
        'this sub is called to retrieve the data that is wanted.
        runSQLRetriveDataFromTheDatabase()
        'this clears the search textbox
        txtSearchCustomers.Clear()
        'this clears the textbox which was being typed into.
        txtSearchCustomersSecondOption.Clear()

    End Sub

    Private Sub txtSearchCustomersSecondOption_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtSearchCustomersSecondOption.KeyPress
        'this if statement means if a number added is not a number then dont let it happen or dont enter the number.
        If (Not (IsNumeric(e.KeyChar))) Then
            e.Handled = True
        End If
        'this if statement makes sure that no punctuation marks have been editted.
        If Char.IsPunctuation(e.KeyChar) Then
            e.Handled = True
        End If
        'this bit of code makes sure that backspace, tab and other keys that have control work.
        If Char.IsControl(e.KeyChar) Then
            e.Handled = False
        End If
    End Sub
    'this sub is called when the search textbox's text was changed.
    Private Sub txtSearchCustomersSecondOption_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtSearchCustomersSecondOption.TextChanged
        txtSearchCustomers.Text = txtSearchCustomersSecondOption.Text

        If txtSearchCustomers.Text.Length = 11 Then
            customersInput = True
            txtSearchCustomersSecondOption.BackColor = Color.LightGreen
        Else
            customersInput = False
            txtSearchCustomersSecondOption.BackColor = Color.White
        End If
        'this if statement makes sure tht when customerInput boolean is true then the button 'cmdSearchCustomers' becomes enabled.
        If customersInput = True Then
            cmdSearchesCustomerInsideCustomerDetails.Enabled = True
        Else
            cmdSearchesCustomerInsideCustomerDetails.Enabled = False
        End If

    End Sub
    'this sub is manually created so that it can be called from other places.
    'its job is to load cars and insert them into the listview just in case if the customer doesn't remember the registration.
    Private Sub LoadCars()
        'clears the list view first
        LsvCars.Items.Clear()

        Try
            cn.Open()
            'this time the sql statemnet is to select everything from the Car table and put it into my Table.
            myAdapter = New OleDbDataAdapter("Select * from Car", cn)
            myTable.Clear()
            myTable.Columns.Clear()
            myAdapter.Fill(myTable)
        Catch ex As OleDbException
            MsgBox("Cannot connect to the the database.  " + ex.Message)
        Catch ex As Exception
            MsgBox("Cannot retrive data from the database.  " + ex.Message)
        Finally
            cn.Close()
        End Try
        'this declares a new line in list view, reserves a space in memory for a list view line.
        Dim newLine As New ListViewItem
        'this reserves a space in memory for a number, its called x
        Dim x As Integer
        'for next loop has been used.
        'it says from 0 to the number of rows in my table take away 1.
        For x = 0 To myTable.Rows.Count - 1
            'this inserts a new line and put the value whatever is in my table.
            newLine = LsvCars.Items.Add(myTable.Rows(x)(1))
            'this inserts any sub item into the same item.
            newLine.SubItems.Add(myTable.Rows(x)(2))
            newLine.SubItems.Add(myTable.Rows(x)(3))
            newLine.SubItems.Add(myTable.Rows(x)(4))
            newLine.SubItems.Add(myTable.Rows(x)(5))
            newLine.SubItems.Add(myTable.Rows(x)(0))
        Next
        'a space is reserved in memory for a number, its called i
        Dim i As Integer
        'for next loop has been used
        'this is saying  that from 0 to the number of items in the list view.
        For i = 0 To LsvCars.Items.Count - 1
            'the property of each item is unlocked. this property simply unlocks alot of controls that have been locked by 
            'Mircrosoft by default for unknown reasons.
            LsvCars.Items(i).UseItemStyleForSubItems = False
            'the property is changed here, its setting the colour of the text or forground colour of a sub item not a whole item to white.
            'this simpply makes it invisible.
            LsvCars.Items(i).SubItems(5).ForeColor = Color.White
        Next
    End Sub

    'this sub is called to link cars to customers, since they aren't in the same database.
    Private Sub LinkCarToCustomer()
        'this is saying that if the value of load car label is more than 0 then fine, begin the process of linking the car.
        If lblLoadCarID.Text > 0 Then
            Try
                cn.Open()
                myAdapter = New OleDbDataAdapter("Select * from Car where ID=" & lblLoadCarID.Text & "", cn)
                myTable.Clear()
                myTable.Columns.Clear()
                myAdapter.Fill(myTable)
            Catch ex As OleDbException
                MsgBox("Cannot connect to the the database.  " + ex.Message)
            Catch ex As Exception
                MsgBox("Cannot retrive data from the database.  " + ex.Message)
            Finally
                cn.Close()
            End Try
            'this is basically putting values into the textboxes and labels from my table
            lblCarID.Text = myTable.Rows(0)(0)
            txtReg.Text = myTable.Rows(0)(1)
            txtMake.Text = myTable.Rows(0)(2)
            txtModel.Text = myTable.Rows(0)(3)
            txtYear.Text = myTable.Rows(0)(4)
            txtPriceDatabase.Text = myTable.Rows(0)(5)
        Else
            'it says tht if the value of load cars equals to 0 then dont do anything.
            Exit Sub
        End If
    End Sub
    'this is saying that when search cars button is pressed, call this sub.
    Private Sub cmdSearchCars_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSearchCars.Click
        'if the search cars text bos is empty then simply call loadCar()
        If txtSearchCars.Text = "" Then
            LoadCars()
            'however if it is not empty then execute the following code
        Else
            Try
                cn.Open()
                myAdapter = New OleDbDataAdapter("Select * from Car where RegNumber='" & txtSearchCars.Text & "'", cn)
                myTable.Clear()
                myTable.Columns.Clear()
                myAdapter.Fill(myTable)
            Catch ex As OleDbException
                MsgBox("Cannot connect to the the database.  " + ex.Message)
            Catch ex As Exception
                MsgBox("Cannot retrive data from the database.  " + ex.Message)
            Finally
                cn.Close()
            End Try
        End If
        Try
            'this says that insert the car details into the textboxes.
            lblCarID.Text = myTable.Rows(0)(0)
            txtReg.Text = myTable.Rows(0)(1)
            txtMake.Text = myTable(0)(2)
            txtModel.Text = myTable(0)(3)
            txtYear.Text = myTable(0)(4)
            txtPriceDatabase.Text = myTable(0)(5)
        Catch ex As Exception
            MsgBox("Car is not in the database. " + ex.Message)
        End Try
        'this says tht show the cars page or go to the cars window.
        lblBoxID.Text = 3
        refreshInterface()
        cmdLaodCarsInside.Enabled = False
    End Sub
    'this is called when the save button in the 4th page or in the loan section is pressed.
    Private Sub cmdSaveLoan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSaveLoan.Click
        'this updates the loan information, four values of the loan r changed or updated.
        Try
            Dim sqlStringSaveQuote As String = "Update Customer set Price = " & txtPrice.Text & ", Deposit = " & txtDeposit.Text & _
                                                        ", MonthlyRepaymentPeriod = " & txtMonthlyPeriod.Text & ", InterestRate = " & txtInterest.Text _
                                                        & ", CarID =" & lblLoadCarID.Text & " where ID = " & lblCustomerNumber.Text
            cn.Open()
            myAdapter = New OleDbDataAdapter(sqlStringSaveQuote, cn)
            cmd = New OleDb.OleDbCommandBuilder(myAdapter)
            myTable.Clear()
            myTable.Columns.Clear()
            myAdapter.Fill(myTable)
            MsgBox("Quote has been saved")
        Catch ex As OleDbException
            MsgBox("Could not connect to the database. " + ex.Message)
        Catch ex As Exception
            MsgBox("Could not save the quote. " + ex.Message)
        Finally
            cn.Close()
        End Try
    End Sub
    'this sub is called when one item of the list view is clicked.
    Private Sub LsvCars_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LsvCars.Click
        'these lines are saying that whatever is foucsed or highlighted by clicking it, the information of that listview should appear in the textboxes.
        txtMake.Text = LsvCars.FocusedItem.SubItems(1).Text()
        txtReg.Text = LsvCars.FocusedItem.SubItems(0).Text()
        txtModel.Text = LsvCars.FocusedItem.SubItems(2).Text()
        txtYear.Text = LsvCars.FocusedItem.SubItems(3).Text()
        txtPriceDatabase.Text = LsvCars.FocusedItem.SubItems(4).Text()
        lblCarID.Text = LsvCars.FocusedItem.SubItems(5).Text()
    End Sub
    'this sub is called when register Cars is pressed.
    Private Sub cmdRegisterCars_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdRegisterCars.Click
        'the next 2 lines translate to: go t cars page or go to Cars window.
        lblBoxID.Text = 3
        refreshInterface()
        'this sub is called to disable buttons that will cause unnecessary errors if pressed
        DisableCarButtons()
        'this sub is called to prepare the inserting of cars.
        PrepareInsertCars()

    End Sub

    'this sub is called to clear the textboxes that are in the car window.
    Private Sub ClearCarTextBoxes()
        txtReg.Clear()
        txtMake.Clear()
        txtModel.Clear()
        txtYear.Clear()
        txtPriceDatabase.Clear()
    End Sub

    'this is the sub that prepares the groupbox to get ready to insert a new customer into the database.
    Private Sub PrepareInsertCars()
        'text boxes are enabled.
        txtReg.Enabled = True
        txtMake.Enabled = True
        txtModel.Enabled = True
        txtYear.Enabled = True
        txtPriceDatabase.Enabled = True
        'a sub is called to clear the textboxes.
        ClearCarTextBoxes()
        'the save button is enabled.
        cmdSaveCars.Enabled = True
        Try
            cn.Open()
            myAdapter = New OleDbDataAdapter("Select * from Car", cn)
            myTable.Clear()
            myTable.Columns.Clear()
            myAdapter.Fill(myTable)
        Catch ex As OleDbException
            MsgBox("Cannot connect to the the Database.  " + ex.Message)
        Catch ex As Exception
            MsgBox("Cannot retrive data from the database.  " + ex.Message)
        Finally
            cn.Close()
        End Try
        'a space for an integer is reserved in memory and is called max.
        Dim max As Integer = 0
        'a space is reserved in meory for an integer and is called x.
        Dim x As Integer
        'for next loop is used,
        For x = 1 To myTable.Rows.Count
            'this if statement is saying that max is 0 to begin with, if the number that we are looking at is > max then
            'max is that number, if not, then max stays as it is.
            If myTable.Rows(x - 1)(0) > max Then
                max = myTable.Rows(x - 1)(0)
            End If
        Next
        'thsis says that watever max is, add 1 to it.
        max = max + 1
        'this says the value of max should appear in car id label.
        lblCarID.Text = max
        LsvCars.Items.Clear()
        cmdLaodCarsInside.Enabled = False
    End Sub
    'this sub is called when save button is pressed.
    Private Sub cmdSaveCars_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSaveCars.Click
        Try
            'this statement says that insert  the values of the label and the textboxes into the car table in the database
            Dim sqlAddCarString As String = "Insert into Car values(" & lblCarID.Text & ",'" & txtReg.Text & "','" _
                                                     & txtMake.Text & "','" & txtModel.Text & "','" & txtYear.Text & "','" _
                                                     & txtPriceDatabase.Text & "')"
            cn.Open()
            myAdapter = New OleDbDataAdapter(sqlAddCarString, cn)
            cmd = New OleDbCommandBuilder(myAdapter)
            myTable.Clear()
            myTable.Columns.Clear()
            myAdapter.Fill(myTable)
        Catch ex As OleDbException
            MsgBox("Cannot access the database. " + ex.Message)
        Catch ex As Exception
            MsgBox("Cannot Save the new Customer. " + ex.Message)
        Finally
            MsgBox("Car has been saved")
            cn.Close()
        End Try
    End Sub
    'this sub is called when new button is pressed in the Car window.
    Private Sub cmdNewCar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdNewCar.Click
        'this calls the sub that disables prefered buttons
        DisableCarButtons()
        'this sub is called to perepare to add a new car.
        PrepareInsertCars()
        'this clears the list view.
        LsvCars.Items.Clear()
    End Sub
    'this sub is used to disable the pefered buttons
    Private Sub DisableCarButtons()
        cmdNewCar.Enabled = False
        cmdLoan.Enabled = False
        cmdDeleteCar.Enabled = False
    End Sub
    'this sub is called when search is cancelled
    Private Sub cmdCancelSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancelSearch.Click
        'this makes the search window disappear.
        BoxSearchCustomers.Visible = False
    End Sub
    'this sub unables the save button in loan window, this is required because i have 2 ways of doing one thing, one way is just giving a quote,
    'the other way its giving a quote while saving it.
    Private Sub UnableSaveButton()
        'this is saving if the customer number is more than 0 then you can save the data.
        If lblCustomerNumber.Text > 0 Then
            cmdSaveLoan.Enabled = True
        Else
            'otherwise u cant save data.
            'if the customer is loaded, the calue of customer number will be more than 0, if it isn't loaded, the value would be 0.
            cmdSaveLoan.Enabled = False
        End If
    End Sub
    'this sub is needed when the loan button is pressed.
    Private Sub cmdLoan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdLoan.Click
        'this basically says that go to loan window
        lblBoxID.Text = 4
        refreshInterface()
        'this says that teh price appear in the Cars window should appear in the loan window
        txtPrice.Text = txtPriceDatabase.Text
        'this sub is called to disable the save button to prevent unneccessary errors.
        UnableSaveButton()
    End Sub

    'this sub calls the print document
    Private Sub cmdPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPrint.Click
        'this code says that the print document should now be send to a printer to print it.
        PrintPage.Print()
    End Sub

    'this sub is called when the value of deposit text box is changed.
    Private Sub txtDeposit_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtDeposit.TextChanged
        'this says that the value of deposit trackbar should equal the value of deposit textbox.
        sliderDeposit.Value = Val(txtDeposit.Text)

    End Sub
    'this sub is called whenever we need to navigate across the program.
    Private Sub refreshInterface()
        'this says that the value that appears in the Group box ID equals to the pageNumber, which was reserved at the beginning of the program
        PageNumber = lblBoxID.Text
        'select case is used because there would be too many if statements if we did use if statements.
        Select Case PageNumber
            'the first case says that if case one is chosen then main menu should appear. And the text of the form will chnage.
            Case Is = 1
                boxMainMenu.Left = w
                boxCustomers.Left = q
                BoxLoan.Left = q
                boxCars.Left = q
                Me.Text = "Main Menu - Quick Quotes"
                'in the second case the customers window will appear. and the text of the form will change.
            Case Is = 2
                boxCustomers.Left = w
                boxCars.Left = q
                BoxLoan.Left = q
                boxMainMenu.Left = q
                Me.Text = "Customers - Quick Quotes"
                'the third case will take you to the third window which is Cars. and the text of the form will change.
            Case Is = 3
                boxCars.Left = w
                boxCustomers.Left = q
                boxMainMenu.Left = q
                BoxLoan.Left = q
                Me.Text = "Cars - Quick Quotes"
                'the 4th case will take you to the last window which is of loan. and the text of the form will change.
            Case Is = 4
                BoxLoan.Left = w
                boxCars.Left = q
                boxCustomers.Left = q
                boxMainMenu.Left = q
                Me.Text = "Loans - Quick Quotes"
        End Select
        'this says that when ever the program tries to go below one then it will still stay one.
        If PageNumber < 1 Then
            PageNumber = 1
        End If
        'this says that whenever the program tries to go above 4, it will still stay at 4.
        If PageNumber > 4 Then
            PageNumber = 4
        End If
        'this says that when the group box ID is 1 the back button disables.
        If lblBoxID.Text = 1 Then
            cmdTakesToPreviousPage.Enabled = False
        Else
            cmdTakesToPreviousPage.Enabled = True
        End If
        'this says that whenever you reach 4 the next button will disable.
        If lblBoxID.Text = 4 Then
            cmdTakesTONextPage.Enabled = False
        Else
            cmdTakesTONextPage.Enabled = True
        End If
    End Sub
    'this  sub is called when the next button is pressed.
    Private Sub cmdTakesTONextPage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdTakesTONextPage.Click
        'this says that whatever was in group box ID label, add 1 to it.
        lblBoxID.Text = lblBoxID.Text + 1
        'this calls for the sub that does the navigation.
        refreshInterface()
        'this unables the save button.
        UnableSaveButton()
    End Sub
    'this sub is called when back button is pressed.
    Private Sub cmdTakesToPreviousPage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdTakesToPreviousPage.Click
        'this says that whatever the value of group box ID label is, add 1 to it.
        lblBoxID.Text = lblBoxID.Text - 1
        'this calls for the navigation to refresh.
        refreshInterface()
    End Sub
    'this sub is called when the new button is pressed on the last and 4th window.
    Private Sub cmdNewLoan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdNewLoan.Click
        'basically this resets the whole loan values
        'this says that the value of price text box is 50
        txtPrice.Text = "50.00"
        'the value of interest textbox is 1
        txtInterest.Text = 1
        'the value of monthly period textbox is 6
        txtMonthlyPeriod.Text = 6
        txtDeposit.Text = "0.00"
        lblBalance.Text = "50.00"
        lblDepositValue.Text = "0.00 %"
    End Sub
    'this sub is called when the user presses the simple quote button
    Private Sub cmdSimpleQuote_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSimpleQuote.Click
        'this takes to the loan page for a simple and easy quote.
        lblBoxID.Text = 4
        refreshInterface()
        'this sub checks if the customer is loaded or not.
        UnableSaveButton()
    End Sub
    'this sub is called when key is presses in the registration text box.
    Private Sub txtReg_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtReg.KeyPress
        'this says that controlling keys should be allowed.
        If Char.IsControl(e.KeyChar) Then
            'this says that if a controlling key is pressed, then allow it
            e.Handled = False
        End If
        'this says that no punctuation mark should be allowed in.
        If Char.IsPunctuation(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub
    'this sub is called when key is pressed in the make text box
    Private Sub txtMake_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtMake.KeyPress
        'this says that controlling keys should be allowed.
        If Char.IsControl(e.KeyChar) Then
            'this says that if a controlling key is pressed, then allow it
            e.Handled = False
        End If
        'this says that no punctuation mark should be allowed in.
        If Char.IsPunctuation(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub
    'this sub is called when a key is pressed in the model text box
    Private Sub txtModel_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtModel.KeyPress
        'this says that controlling keys should be allowed.
        If Char.IsControl(e.KeyChar) Then
            'this says that if a controlling key is pressed, then allow it
            e.Handled = False
        End If
        'this says that no punctuation mark should be allowed in.
        If Char.IsPunctuation(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub
    'this sub is called when key is pressed in the year text box.
    Private Sub txtYear_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtYear.KeyPress
        'this says tht contolling keys r allowed and only numbers are allowed.
        If Char.IsControl(e.KeyChar) Then
            e.Handled = False
        ElseIf (Not (IsNumeric(e.KeyChar))) Then
            e.Handled = True
        End If
    End Sub
    'this sub is called when key is pressed in the price text box.
    Private Sub txtPriceDatabase_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtPriceDatabase.KeyPress
        'this makes sure that controling keys r allowed.
       If Char.IsControl(e.KeyChar) Then
            e.Handled = False
        ElseIf (Not (IsNumeric(e.KeyChar))) Then
            e.Handled = True
        End If
    End Sub
    'this sub is called when load cars is pressed in the the Cars window.
    Private Sub cmdLaodCarsInside_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdLaodCarsInside.Click
        'another sub is called to load cars and put the values in the list view.
        LoadCars()
        'this disables the load cars button, so that it can't be pressed again.
        cmdLaodCarsInside.Enabled = False
    End Sub
    'this sub runs when delete button is pressed in the Cars window.
    Private Sub cmdDeleteCar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDeleteCar.Click
        'this makes the delete window appear and it tells it the position to be in and tells it to come to the top of everything.
        boxDelete.Visible = True
        boxDelete.Left = 63
        boxDelete.Top = 52
        boxDelete.BringToFront()
        'this changes the label inside the delete window, since 1 delete window is being used in 2 places
        lblWarning.Text = "Are you sure you want to delete this car?"
        'this sets the next and the back buttons to disable
        cmdTakesTONextPage.Enabled = False
        cmdTakesToPreviousPage.Enabled = False
    End Sub
    'this sub runs when a key is pressed in the email address text box. 
    Private Sub txtEmailAddress_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtEmailAddress.KeyPress
        'this statemnt doesn't allow any aphostrophe or quotation marks to be entered.
        'this is done to avoid a big error.
        If e.KeyChar = """" Or e.KeyChar = "'" Then
            e.Handled = True
        End If
    End Sub
    'this next bit of code is for printing. it is called from another sub which sends all the information taken from this
    'sub and translate it into a drawing that is on A4page., so that can print.
    Private Sub PrintPage_PrintPage(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PrintPage.PrintPage
        'a space is reserved in memory for a number, its named as c and its value is 100.
        Dim c As Integer = 100
        'a space is reserved in memory for a number, its named as v and its value is 400.
        Dim v As Integer = 400
        'this says tht put the picture 2 in a certain place, the co-ordinates r given.
        e.Graphics.DrawImage(PictureBox2.Image, 50, 50)
        'this says draw a straight line, startinng from a point, ending to a point.
        e.Graphics.DrawLine(Pens.Black, 50, 132, 600, 132)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(Label3.Text, Label3.Font, Brushes.Black, 172, 50)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(lblCustomerDetails.Text, lblCustomerDetails.Font, Brushes.Black, 50, 172)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(lblFullName.Text, lblFullName.Font, Brushes.Black, c, 212)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(lblEmailAddress.Text, lblEmailAddress.Font, Brushes.Black, c, 252)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(lblAddress2.Text, lblAddress2.Font, Brushes.Black, c, 292)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(lblMobileNumber.Text, lblMobileNumber.Font, Brushes.Black, c, 332)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(lblCarDetails.Text, lblCarDetails.Font, Brushes.Black, 50, 372)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(lblReg.Text, lblReg.Font, Brushes.Black, c, 412)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(lblMake.Text, lblMake.Font, Brushes.Black, c, 452)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(lblModel.Text, lblModel.Font, Brushes.Black, c, 492)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(lblYear.Text, lblYear.Font, Brushes.Black, c, 532)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(lblPriceDatabase.Text, lblPriceDatabase.Font, Brushes.Black, c, 572)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(lblLoanDetails.Text, lblLoanDetails.Font, Brushes.Black, 50, 612)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(lblPrice.Text, lblPrice.Font, Brushes.Black, c, 652)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(lblDeposit.Text, lblDeposit.Font, Brushes.Black, c, 692)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(lblMonthlyRepaymentPeriod.Text, lblMonthlyRepaymentPeriod.Font, Brushes.Black, c, 732)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(lblInterest.Text, lblInterest.Font, Brushes.Black, c, 772)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(lblTotalPayND.Text, lblTotalPayND.Font, Brushes.Black, c, 852)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(lblMonth.Text, lblMonth.Font, Brushes.Black, c, 812)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(lblTotalPay.Text, lblTotalPay.Font, Brushes.Black, c, 912)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(lblNotes.Text, lblNotes.Font, Brushes.Black, c, 972)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.

        'this marks the beginning of the actual textboxes. whicb contain different values all the time.
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(txtFullName.Text, txtFullName.Font, Brushes.Black, v, 212)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(txtPostCode.Text, txtPostCode.Font, Brushes.Black, v, 252)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(txtMobileNumber.Text & ",   " & txtEmailAddress.Text, txtMobileNumber.Font, Brushes.Black, v, 292)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(txtFirstName.Text, txtFirstName.Font, Brushes.Black, v, 332)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(txtReg.Text, txtReg.Font, Brushes.Black, v, 412)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(txtMake.Text, txtMake.Font, Brushes.Black, v, 452)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(txtModel.Text, txtModel.Font, Brushes.Black, v, 492)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(txtYear.Text, txtYear.Font, Brushes.Black, v, 532)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString("£ " & txtPriceDatabase.Text, txtPriceDatabase.Font, Brushes.Black, v, 572)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString("£ " & txtPrice.Text, txtPrice.Font, Brushes.Black, v, 652)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString("£ " & txtDeposit.Text, txtDeposit.Font, Brushes.Black, v, 692)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(txtMonthlyPeriod.Text & " months", txtMonthlyPeriod.Font, Brushes.Black, v, 732)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(txtInterest.Text & " %", txtInterest.Font, Brushes.Black, v, 772)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString("£ " & lblMonthlyPayment.Text, lblMonthlyPayment.Font, Brushes.Black, v, 812)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString("£ " & lblTotalPaymentNoDeposit.Text, lblTotalPaymentNoDeposit.Font, Brushes.Black, v, 852)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString("£ " & lblTotalPayment.Text, lblTotalPayment.Font, Brushes.Black, v, 912)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
        e.Graphics.DrawString(txtNotes.Text, txtNotes.Font, Brushes.Black, v, 972)
        'this says draw the following string, the string being the name of the label or textbox, followed by the font and the co-ordinates at the end.
    End Sub

    Private Sub txtMonthlyPeriod_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtMonthlyPeriod.TextChanged
        SliderMonthlyPeriod.Value = Val(txtMonthlyPeriod.Text)
    End Sub
    Private Sub txtInterest_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtInterest.TextChanged
        sliderInterest.Value = Val(txtInterest.Text)
    End Sub

    Private Sub txtFullName_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtFullName.KeyPress
        'this says that controlling keys should be allowed.
        If Char.IsControl(e.KeyChar) Then
            'this says that if a controlling key is pressed, then allow it
            e.Handled = False
        End If
        'this says that no punctuation mark should be allowed in.
        If Char.IsPunctuation(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub
    Private Sub txtPostCode_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtPostCode.KeyPress
        'this says that controlling keys should be allowed.
        If Char.IsControl(e.KeyChar) Then
            'this says that if a controlling key is pressed, then allow it
            e.Handled = False
        End If
        'this says that no punctuation mark should be allowed in.
        If Char.IsPunctuation(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub txtFirstName_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtFirstName.KeyPress
        'this says that controlling keys should be allowed.
        If Char.IsControl(e.KeyChar) Then
            'this says that if a controlling key is pressed, then allow it
            e.Handled = False
        End If
        'this says that no punctuation mark should be allowed in.
        If Char.IsPunctuation(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub
    Private Sub txtMobileNumber_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtMobileNumber.KeyPress
        'this says that controlling keys should be allowed.
        If Char.IsControl(e.KeyChar) Then
            'this says that if a controlling key is pressed, then allow it
            e.Handled = False
        End If
        'this says that no punctuation mark should be allowed in.
        If Char.IsPunctuation(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub
End Class
