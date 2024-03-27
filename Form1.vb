

Imports System.Web.Services.Protocols
Imports System.Configuration
Imports System.EnterpriseServices
Imports System.Reflection
Imports System.Xml
Imports System.Xml.Serialization
Imports TestAdcs.WebReference
Imports System.CodeDom.Compiler

Public Class Form1
    Public listlabel As New List(Of Label)
    Public k As Integer
    Public listnodes As New List(Of XmlNode)
    Dim ws As New ADCS
    Sub s()
        Dim xmldoc As New XmlDocument()
        xmldoc.LoadXml("<ADCS><Header UseCaseCode='HELLO'></Header></ADCS>")
        ws.Credentials = New Net.NetworkCredential("admin", "!1q2w3e", "bg.local")
        Dim str As String = "<ADCS><Header UseCaseCode='HELLO'></Header></ADCS>"
        initcontrols()
        ws.ProcessDocument(str)
        xmldoc.LoadXml(str)
        parsexml(xmldoc)
        Me.Update()
        TextBox1.Text = str
    End Sub
    Sub d(val As String, ind As String) 'card
        Dim xmldoc As New XmlDocument()
        xmldoc.LoadXml("<ADCS><Header UseCaseCode='HELLO'></Header></ADCS>")
        ws.Credentials = New Net.NetworkCredential("admin", "!1q2w3e", "bg.local")
        Dim str As String = TextBox1.Text
        Dim xmldoc2 As New XmlDocument
        xmldoc.LoadXml(str)
        Dim xmlel As XmlNode = xmldoc.CreateNode(XmlNodeType.Element, "Input", "")
        xmlel.InnerText = val
        Dim xmlatr As XmlAttribute = xmldoc.CreateAttribute("FieldID")
        xmlatr.Value = ind
        xmlel.Attributes.Append(xmlatr)
        Dim xmln As XmlNode = xmldoc.SelectSingleNode("//ADCS/Header")
        xmln.AppendChild(xmlel)

        'xmldoc.InsertAfter(xmlel, xmln)
        str = xmldoc.OuterXml
        Try
            ws.ProcessDocument(str)
        Catch es As SoapException
            MessageBox.Show($"NAV ERROR: {es.Message.ToString}")
            Exit Sub
        Catch ex As Exception
            MessageBox.Show($"HttpError: {ex.Message.ToString}")


        End Try

        xmldoc.LoadXml(str)
        parsexml(xmldoc)
        Me.Update()
        TextBox1.Text = str
    End Sub
    Sub l(ind As Integer)
        Dim str As String = TextBox1.Text
        Dim xmldoc As New XmlDocument
        xmldoc.LoadXml(str)

        Dim xmlel As XmlNode = xmldoc.CreateNode(XmlNodeType.Element, "Input", "")
        xmlel.InnerText = listnodes(ind).InnerText
        Dim xmlatr As XmlAttribute = xmldoc.CreateAttribute("TableNo")
        xmlatr.Value = listnodes(ind).Attributes.GetNamedItem("TableNo").Value
        xmlel.Attributes.Append(xmlatr)
        Dim xmlatr2 As XmlAttribute = xmldoc.CreateAttribute("RecordID")
        xmlatr2.Value = listnodes(ind).Attributes.GetNamedItem("RecordID").Value
        xmlel.Attributes.Append(xmlatr2)
        Dim xmln As XmlNode = xmldoc.SelectSingleNode("//ADCS/Header")
        xmln.AppendChild(xmlel)

        'xmldoc.InsertAfter(xmlel, xmln)
        str = xmldoc.OuterXml
        Try
            ws.ProcessDocument(str)
            xmldoc.LoadXml(str)
            parsexml(xmldoc)
            Me.Update()
            TextBox1.Text = str
        Catch es As SoapException
            MessageBox.Show($"NAV ERROR: {es.Message.ToString}")
            Exit Sub
        Catch ex As Exception
            MessageBox.Show($"HttpError: {ex.Message.ToString}")


        End Try
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        s()

    End Sub
    Sub parsexml(xml As XmlDocument)

        If xml.SelectSingleNode("//ADCS/Header").Attributes.GetNamedItem("FormTypeOpt").Value = "Card" Then
            parsecard(xml)
        ElseIf xml.SelectSingleNode("//ADCS/Header").Attributes.GetNamedItem("FormTypeOpt").Value = "Selection List" Then
            parsesellist(xml)
        End If
    End Sub
    Sub initcontrols()

        For Each lbl In listlabel
            Me.Controls.Remove(lbl)
        Next
        For Each tb In Me.Controls.OfType(Of TextBox)
            If tb.Name.Contains("Input") = True Then
                Me.Controls.Remove(tb)
            End If
        Next
        For Each lb In Me.Controls.OfType(Of ListBox)

            Me.Controls.Remove(lb)

        Next
        k = 0
        listlabel.Clear()
        listnodes.Clear()

        Me.Controls.RemoveByKey("inputbox")

    End Sub
    Sub parsesellist(xml As XmlDocument)
        Dim xmlnodesource As XmlNode = xml.SelectSingleNode("//ADCS/Lines")
        initcontrols()
        Me.Update()
        Dim lb As New ListBox
        lb.Name = "listbox1"
        lb.Location = New Point(50, 10)
        lb.Size = New Size(300, 300)
        lb.Parent = Me
        lb.Visible = True
        AddHandler lb.Click, AddressOf listboxselect
        Dim i = 0
        k = 0
        Try

            Dim xmlnodebody As XmlNode = xml.SelectSingleNode("//ADCS/Lines/Body")

            For Each listnode As XmlNode In xmlnodebody
                listnodes.Add(listnode)
                k += 1
            Next


        Catch ex As Exception

        End Try
        For Each node As XmlNode In listnodes
            Dim childnode As XmlNode = node.ChildNodes.Item(0)
            lb.Items.Add(childnode.InnerText)
        Next

        Me.Update()
    End Sub
    Sub parsecard(xml As XmlDocument)
        initcontrols()
        Dim i As Integer
        For i = 1 To xml.SelectSingleNode("//ADCS/Header").Attributes.GetNamedItem("NoOfLines").Value
            listlabel.Add(New Label())
        Next
        If xml.SelectSingleNode("//ADCS/Lines").HasChildNodes = True Then
            Dim xmlnode As XmlNode = xml.SelectSingleNode("//ADCS/Lines")
            parsexmlnode(xmlnode)
        End If


        Me.Update()
    End Sub
    Sub parsexmlnode(xml As XmlNode)
        If xml.HasChildNodes = True And xml.ChildNodes.Count > 1 Then
            For i = 0 To xml.ChildNodes.Count - 1
                parsexmlnode(xml.ChildNodes.Item(i))
            Next
        Else
            Try
                If IsNothing(xml.Attributes.GetNamedItem("Type")) = False Then


                    If xml.Attributes.GetNamedItem("Type").Value = "Text" Then

                        Lbl.SetLabel(listlabel(k), k, xml.InnerText)



                    ElseIf xml.Attributes.GetNamedItem("Type").Value = "Input" Then
                        Dim bt As New TextBox
                        bt.Name = $"InputBox{k}"
                        bt.Tag = xml.Attributes.GetNamedItem("FieldID").Value
                        bt.Location = New Point(50, 20 * k + 10)
                        bt.Size = New Size(300, 20)
                        bt.Parent = Me
                        bt.Visible = True
                        If IsNothing(xml.Attributes.GetNamedItem("Descrip")) = False Then
                            'bt.Text = xml.Attributes.GetNamedItem("descrip").Value
                            bt.SelectedText = xml.Attributes.GetNamedItem("Descrip").Value
                            bt.Focus()
                        End If

                        AddHandler bt.KeyPress, AddressOf F_KeyPress

                    Else Lbl.SetLabel(listlabel(k), k, xml.Attributes.GetNamedItem("Descrip").Value)
                    End If
                End If

            Catch

            End Try
            k += 1
        End If
    End Sub

    Private Sub F_KeyPress(sender As Object, e As KeyPressEventArgs) Handles MyBase.KeyPress
        If e.KeyChar = Convert.ToChar(Keys.Return) Then
            For Each tb In Me.Controls.OfType(Of TextBox)
                If tb.Name.Contains("Input") Then
                    If tb.Text.Length > 0 Then
                        d(tb.Text, tb.Tag)
                    End If
                End If
            Next
        End If


    End Sub
    Private Sub listboxselect(sender As Object, e As MouseEventArgs)
        Try

            Dim lb As ListBox = sender
            'MessageBox.Show(lb.SelectedItem.ToString())
            l(lb.SelectedIndex)
        Catch ex As Exception
            Exit Sub
        End Try

    End Sub
End Class
Public Class Lbl
    Inherits Label
    Shared Function NewLabel(name As String, location As Point, size As Size, text As String, parent As Object, index As Integer) As Lbl
        Dim lb As New Label

        Return lb
    End Function
    Shared Sub SetLabel(ByRef lbl As Label, index As Integer, text As String) ', name As String, location As Point, size As Size, text As String, parent As Object, index As Integer)

        lbl.Name = $"label{index}"
        lbl.Text = text
        lbl.Location = New Point(50, 20 * index + 10)
        lbl.Size = New Size(300, 20)
        lbl.Parent = Form1
        lbl.Visible = True
        lbl.BorderStyle = BorderStyle.FixedSingle

    End Sub

End Class
