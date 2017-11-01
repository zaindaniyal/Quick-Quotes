Public Class SplashScreen
    Dim TimeMe As Integer
    Private Sub SplashScreen2_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Timer1.Start()


    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        TimeMe += 1
        If TimeMe = 5 Then
            Me.Hide()
            MainMenu.Show()
            Timer1.Stop()
        End If
    End Sub
End Class