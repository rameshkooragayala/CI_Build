Public Class Form2

    Private Sub ButSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButSave.Click
        My.Forms.Form1.UserName = TxtUserName.Text
        My.Forms.Form1.Password = TxtPassword.Text
        Me.Close()
    End Sub

End Class